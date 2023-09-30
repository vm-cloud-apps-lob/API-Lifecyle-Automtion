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
if (-not $?) {
    Write-Error "Azure authentication failed."
    exit 1
}

# Create the API Management context
$apimContext = New-AzApiManagementContext -ResourceGroupName $resourceGroupName -ServiceName $apimName

# Function to parse YAML content and extract the version and title
function Get-YamlInfo($yamlContent) {
    $yamlData = $yamlContent | ConvertFrom-Yaml
    $info = $yamlData.info
    return @{
        Version = $info.version
        Title = $info.title
    }
}

# Get the version and title from the OAS file (assuming it's in YAML format)
$oasContent = Get-Content -Path $oasFilePath -Raw
$oasInfo = Get-YamlInfo -yamlContent $oasContent

# Check if the version and title follow the expected pattern
if ($oasInfo.Version -match '^\d+\.\d+\.\d+$') {
    $majorVersion = [int]($oasInfo.Version.Split('.')[0])
    $minorVersion = [int]($oasInfo.Version.Split('.')[1])

    if ($minorVersion -eq 0) {
        # If minor version is 0, it's a major version change, create a new API
        $apiVersion = "v$majorVersion"
        $apiName = "$oasInfo.Title $apiVersion"
        $apiId = "$apiName" -replace ' ', '-' # Replace spaces with hyphens
        Write-Output "Creating a new API for version $($oasInfo.Version)"
        $api = Import-AzApiManagementApi -Context $apimContext -ApiId $apiId -Path "/$apiId" -SpecificationPath $oasFilePath -SpecificationFormat OpenApiJson
    } else {
        # If minor version is greater than 0, it's a revision
        $apiRevision = $oasInfo.Version -replace '\.', '-'
        $api = New-AzApiManagementApiRevision -Context $apimContext -ApiId $apiId -ApiRevision $apiRevision
        Write-Output "Creating a revision for API version $($oasInfo.Version)"
    }
} else {
    Write-Error "Invalid version format: $($oasInfo.Version)"
    exit 1
}

# Set policies using Set-AzApiManagementPolicy
$apiPolicies = Get-Content -Path $apiPolicyConfigFilePath -Raw
Set-AzApiManagementPolicy -Context $apimContext -ApiId $apiName -Policy $apiPolicies

Write-Output "Script execution completed."
