# Define the path to your configuration file
$configFile = Join-Path $env:GITHUB_WORKSPACE ".api\config.properties"

# Read values from the configuration file
$config = Get-Content -Path $configFile | ForEach-Object {
    $key, $value = $_ -split "="
    [PSCustomObject]@{
        Key = $key.Trim()
        Value = $value.Trim()
    }
}

# Access values from the configuration object
$subscriptionId = $config | Where-Object { $_.Key -eq "SubscriptionId" } | Select-Object -ExpandProperty Value
$resourceGroupName = $config | Where-Object { $_.Key -eq "ResourceGroupName" } | Select-Object -ExpandProperty Value
$apiName = $config | Where-Object { $_.Key -eq "ApiName" } | Select-Object -ExpandProperty Value
$apiId = $config | Where-Object { $_.Key -eq "ApiId" } | Select-Object -ExpandProperty Value
$apimName = $config | Where-Object { $_.Key -eq "ApimName" } | Select-Object -ExpandProperty Value
$apiPolicyConfigFilePath = $config | Where-Object { $_.Key -eq "ApiPolicyConfigFilePath" } | Select-Object -ExpandProperty Value
$apiVisibility = $config | Where-Object { $_.Key -eq "ApiVisibility" } | Select-Object -ExpandProperty Value
$postmanCollectionFilePath = $config | Where-Object { $_.Key -eq "PostmanCollectionFilePath" } | Select-Object -ExpandProperty Value

# Specify the path to your OAS file in the repository
$oasFilePath = "$env:GITHUB_WORKSPACE\openapi.yaml"

# Authenticate with Azure using Azure PowerShell
Connect-AzAccount -UseDeviceAuthentication

# Check if authentication was successful
if ($?) {
    Write-Output "Azure authentication successful."
} else {
    Write-Error "Azure authentication failed."
    exit 1
}

# Step 1: API Creation and Validation
# Create API in APIM using the OAS file path from your configuration
Write-Output "Importing API from OAS file..."
# Create the API Management context
$apimContext = New-AzApiManagementContext -ResourceGroupName $resourceGroupName -ServiceName $apimName

# Function to parse YAML content and extract the version
function Get-YamlVersion($yamlContent) {
    $yamlData = $yamlContent | ConvertFrom-Yaml
    $version = $yamlData.info.version
    return $version
}


# Function to check if it's a major version change
function Is-MajorVersionChange($currentVersion, $newVersion) {
    $currentVersionParts = $currentVersion -split '\.'
    $newVersionParts = $newVersion -split '\.'

    $currentMajor = [int]$currentVersionParts[0]
    $newMajor = [int]$newVersionParts[0]

    return $newMajor -gt $currentMajor
}

# Function to create a new API or revision based on version change
function Create-ApiOrRevision($context, $apiId, $oasFilePath, $oasVersion) {
    $apiRevision = $oasVersion -replace '\.', '-'

    if (Is-MajorVersionChange -currentVersion $oasVersion -newVersion $apiRevision) {
        # If it's a major version change, create a new API
        $apiName = "$($oasTitle)_v$oasVersion" -replace '\s+', '_'  # Ensure no spaces in the identifier
        Write-Output "Creating a new API for version $oasVersion with name: $apiName"
        Import-AzApiManagementApi -Context $context -ApiId $apiName -Path "/$apiName" -SpecificationPath $oasFilePath -SpecificationFormat OpenApiJson
    } else {
        # If it's a minor version change, create a revision
        Write-Output "Creating a revision for API version $oasVersion"
        New-AzApiManagementApiRevision -Context $context -ApiId $apiId -ApiRevision $apiRevision
    }
}

# Get the version from the OAS file (assuming it's in YAML format)
$oasContent = Get-Content -Path $oasFilePath -Raw
$oasVersion = Get-YamlVersion -yamlContent $oasContent

# Import API using the local file path and specify the -ApiRevision parameter
$api = Create-ApiOrRevision -context $apimContext -apiId $apiId -oasFilePath $oasFilePath -oasVersion $oasVersion

# Check the result of API import
if ($?) {
    Write-Output "API import successful. Detected API revision: $($api.ApiRevision)"
} else {
    Write-Error "API import failed."
    exit 1
}

# Step 2: Azure API Management Setup
# If APIM instance does not exist, create it
$existingApim = Get-AzApiManagement -ResourceGroupName $resourceGroupName -Name $apimName -ErrorAction SilentlyContinue
if ($null -eq $existingApim) {
    New-AzApiManagement -ResourceGroupName $resourceGroupName -Name $apimName -PublisherEmail "vamsi.sapireddy@valuemomentum.com" -PublisherName "Vamsi"
}

# Step 3: Set API Management context
$apimContext = New-AzApiManagementContext -ResourceGroupName $resourceGroupName -ServiceName $apimName

# Read the policies content from your policy config file
$apiPolicies = Get-Content -Path $apiPolicyConfigFilePath -Raw

# Associate the API with the existing product "Unlimited"
Add-AzApiManagementApiToProduct -Context $apimContext -ApiId $apiId -ProductId "Unlimited"

# Set the desired backend URL
$backendUrl = "https://pa-submission-workflow-api.graycoast-ea31beda.eastus.azurecontainerapps.io"

# Set the API context
$apiContext = New-AzApiManagementContext -ResourceGroupName $resourceGroupName -ServiceName $apimName

# Update the backend URL for the API
Set-AzApiManagementApi -Context $apiContext -ApiId $apiId -ServiceUrl $backendUrl

# Create a new API release
New-AzApiManagementApiRelease -Context $apiContext -ApiId $apiId -ApiRevision $apiRevision -Note "Releasing version $apiRevision"

# Explicitly set the backend URL to ensure consistency
Set-AzApiManagementApi -Context $apiContext -ApiId $apiId -ServiceUrl $backendUrl

# Set policies using Set-AzApiManagementPolicy
Set-AzApiManagementPolicy -Context $apimContext -ApiId $apiId -Policy $apiPolicies

Write-Output "Script execution completed."
