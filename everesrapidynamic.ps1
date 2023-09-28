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

# Authenticate with Azure using Azure CLI (already authenticated in GitHub Actions)
az login --use-device-code

# Synchronize Azure CLI context with Azure PowerShell
Connect-AzAccount -UseDeviceAuthentication

# Verify if authentication was successful
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

# Import API using the local file path and specify the -ApiRevision parameter
$api = Import-AzApiManagementApi -Context $apimContext -ApiId $apiId -Path "/$apiName" -SpecificationPath $oasFilePath -SpecificationFormat OpenApiJson -ApiRevision $apiRevision

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

# Set policies using Set-AzApiManagementPolicy
Set-AzApiManagementPolicy -Context $apimContext -ApiId $apiId -Policy $apiPolicies

# Step 5: Create or Update a Container App
Write-Output "Creating or Updating a Container App..."

# Define the name and other parameters for the Container App
$containerAppName = "everest-backoffice"
$containerAppDescription = "The container is created for PA Submission"
$containerAppRevision = "1"  # Specify the desired revision

# Check if the Container App already exists
$existingContainerApp = az apim api list --resource-group $resourceGroupName --service-name $apimName --query "[?name=='$containerAppName']" --output tsv

if ($existingContainerApp) {
    # The Container App already exists, update it with the new API information
    Write-Output "Updating existing Container App..."
    
    az apim api update --resource-group $resourceGroupName --service-name $apimName --api-id $existingContainerApp --set "displayName=$containerAppName" "description=$containerAppDescription" "revision=$containerAppRevision"

    # Check the result of Container App update
    if ($?) {
        Write-Output "Container App update successful."
    } else {
        Write-Error "Container App update failed."
        exit 1
    }
} else {
    # The Container App does not exist, create it using Azure CLI
    Write-Output "Creating a new Container App..."
    
    az apim api create --resource-group $resourceGroupName --service-name $apimName --api-id $containerAppName --path "/$containerAppName" --display-name "$containerAppName"

    # Check the result of Container App creation
    if ($?) {
        Write-Output "Container App creation successful."
    } else {
        Write-Error "Container App creation failed."
        exit 1
    }
}

Write-Output "Script execution completed."
