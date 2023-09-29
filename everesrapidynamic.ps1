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
$apimName = $config | Where-Object { $_.Key -eq "ApimName" } | Select-Object -ExpandProperty Value
$apiPolicyConfigFilePath = $config | Where-Object { $_.Key -eq "ApiPolicyConfigFilePath" } | Select-Object -ExpandProperty Value

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

# Check if the API with the same name exists
$existingApi = Get-AzApiManagementApi -Context $apimContext -ApiId $apiName

if ($existingApi) {
    Write-Output "API with name $apiName already exists. Checking version..."

    # Get the existing API's version
    $existingVersion = $existingApi.ApiVersion

    # Compare the existing version with the OAS version
    if ($existingVersion -eq $oasVersion) {
        Write-Output "API version $oasVersion already exists. Creating a revision..."
        $apiRevision = $oasVersion -replace '\.', '-'
        $api = New-AzApiManagementApiRevision -Context $apimContext -ApiId $apiName -ApiRevision $apiRevision
    } else {
        Write-Output "Creating a new API version $oasVersion..."
        # Specify the version set ID here if you have one, otherwise, leave it empty
        $versionSetId = "" 
        $api = Import-AzApiManagementApi -Context $apimContext -ApiId $apiName -Path "/$apiName" -SpecificationPath $oasFilePath -SpecificationFormat OpenApiJson -ApiVersion $oasVersion -ApiVersionSetId $versionSetId
    }
} else {
    Write-Output "Creating a new API version $oasVersion..."
    # Specify the version set ID here if you have one, otherwise, leave it empty
    $versionSetId = "" 
    $api = Import-AzApiManagementApi -Context $apimContext -ApiId $apiName -Path "/$apiName" -SpecificationPath $oasFilePath -SpecificationFormat OpenApiJson -ApiVersion $oasVersion -ApiVersionSetId $versionSetId
}

# Check the result of API import
if ($?) {
    Write-Output "API import successful. Detected API version: $($api.ApiVersion)"
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

# Set policies using Set-AzApiManagementPolicy
Set-AzApiManagementPolicy -Context $apimContext -ApiId $apiName -Policy $apiPolicies

Write-Output "Script execution completed."
