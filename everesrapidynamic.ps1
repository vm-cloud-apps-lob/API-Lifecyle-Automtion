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

# Remove dots from the version string to simplify the versioning (e.g., 2.0.0 becomes v2)
$simplifiedVersion = $oasVersion -replace '\.', ''

# Check if the version follows the pattern of x.y.z (e.g., 1.0.0, 2.0.0, 1.0.1, etc.)
if ($oasVersion -match '^\d+\.\d+\.\d+$') {
    $simplifiedVersion = "v$($oasVersion -replace '\.', '')"  # Remove dots from the version
    
    # Modify the API name to include the version
    $newApiName = "${apiName}-${simplifiedVersion}"
    
    # Check if an API with the same name already exists
    $existingApi = Get-AzApiManagementApi -Context $apimContext -ApiId $newApiName -ResourceGroupName $resourceGroupName
    
    if ($existingApi -eq $null) {
        # If the API doesn't exist, create it
        Write-Output "Creating a new API for version $simplifiedVersion with name $newApiName"
        $api = Import-AzApiManagementApi -Context $apimContext -ApiId $newApiName -Path "/$newApiName" -SpecificationPath $oasFilePath -SpecificationFormat OpenApiJson
    } else {
        # If the API already exists, consider handling this case, e.g., creating a new revision
        Write-Output "API with name $newApiName already exists. You may want to create a new revision or handle this case accordingly."
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
