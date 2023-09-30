# Define the path to your configuration file
$configFile = "$env:GITHUB_WORKSPACE\config.txt"

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

# Get the version from the OAS file (assuming it's in YAML format)
$oasContent = Get-Content -Path $oasFilePath -Raw
$oasVersion = Get-YamlVersion -yamlContent $oasContent

# Replace dots with hyphens in the version for the API revision
$apiRevision = $oasVersion -replace '\.', '-'

# Construct the API path with a valid identifier
$apiPath = "/$apiName-v$majorVersion-$minorVersion"  # Adjust the naming convention as needed

# Remove any invalid characters from the API identifier
$apiPath = $apiPath -replace '[^a-zA-Z0-9-]', ''

# Import API using the local file path and specify the -ApiRevision parameter
$api = Import-AzApiManagementApi -Context $apimContext -ApiId $apiPath -Path $apiPath -SpecificationPath $oasFilePath -SpecificationFormat OpenApiJson -ApiRevision $apiRevision

# Check the result of API import
if ($?) {
    Write-Output "API import successful. Detected API revision: $($api.ApiRevision)"
} else {
    Write-Error "API import failed."
    exit 1
}

# Function to check if two versions are the same major version
function IsSameMajorVersion($version1, $version2) {
    $major1, $minor1, $patch1 = $version1.Split('.')
    $major2, $minor2, $patch2 = $version2.Split('.')
    return $major1 -eq $major2
}

# Check if the version follows the pattern of x.y.z (e.g., 1.0.0, 2.0.0, 1.0.1, etc.)
if ($oasVersion -match '^\d+\.\d+\.\d+$') {
    $majorVersion = [int]($oasVersion.Split('.')[0])

    # Get the latest version of the API in APIM
    $latestApiVersion = Get-AzApiManagementApiVersion -Context $apimContext -ApiId $apiId | Sort-Object -Property Version -Descending | Select-Object -First 1

    if ($latestApiVersion) {
        $latestVersion = $latestApiVersion.Version
    } else {
        $latestVersion = "0.0.0"
    }

    # Check if it's a major version change
    if (!IsSameMajorVersion $oasVersion $latestVersion) {
        Write-Output "Creating a new API for version $oasVersion"
        $api = Import-AzApiManagementApi -Context $apimContext -ApiId "$apiName-v$majorVersion" -Path "/$apiName-v$majorVersion" -SpecificationPath $oasFilePath -SpecificationFormat OpenApiJson
    }
    # Check if it's a minor version change or patch update
    elseif ($oasVersion -gt $latestVersion) {
        Write-Output "Creating a revision for API version $oasVersion"
        $apiRevision = $oasVersion -replace '\.', '-'
        $api = New-AzApiManagementApiRevision -Context $apimContext -ApiId $apiId -ApiRevision $apiRevision
    }
    else {
        Write-Error "Invalid version format: $oasVersion"
        exit 1
    }
} else {
    Write-Error "Invalid version format: $oasVersion"
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

# Set policies using Set-AzApiManagementPolicy
Set-AzApiManagementPolicy -Context $apimContext -ApiId $apiId -Policy $apiPolicies

# Associate the API with the existing product "Unlimited"
Add-AzApiManagementApiToProduct -Context $apimContext -ApiId $apiId -ProductId "Unlimited"

Write-Output "Script execution completed."
