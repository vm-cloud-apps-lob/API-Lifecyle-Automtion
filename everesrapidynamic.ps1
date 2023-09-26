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

# Access the OAS file path from the configuration object
$oasFilePath = $config | Where-Object { $_.Key -eq "OasFilePath" } | Select-Object -ExpandProperty Value

# Authenticate with your Azure account and add debugging output
Write-Output "Authenticating with Azure..."
az login --use-device-code

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
az apim api import --path "/$apiName" --resource-group $resourceGroupName --service-name $apimName --specification-format OpenApiJson --specification-path $oasFilePath --api-id $apiId

# Check the result of API import
if ($?) {
    Write-Output "API import successful."
} else {
    Write-Error "API import failed."
    exit 1
}

# Step 2: Azure API Management Setup
# If APIM instance does not exist, create it
$existingApim = az apim show --name $apimName --resource-group $resourceGroupName --query "name" -o tsv
if (-not $existingApim) {
    az apim create --name $apimName --resource-group $resourceGroupName --publisher-email "your_publisher_email" --publisher-name "your_publisher_name"
}

# Step 3: Policies Configuration
# Apply policies to the created API using your policy config file
az apim api update --resource-group $resourceGroupName --service-name $apimName --api-id $apiId --set "policies=@$apiPolicyConfigFilePath"

# Step 4: API Publishing and Visibility
# Publish the API and set visibility
az apim api update --resource-group $resourceGroupName --service-name $apimName --api-id $apiName

# Associate the API with the existing product "Unlimited"
az apim product api add --resource-group $resourceGroupName --service-name $apimName --product-id "Unlimited" --api-id $apiId

Write-Output "Script execution completed."
