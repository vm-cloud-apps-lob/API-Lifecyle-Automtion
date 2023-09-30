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

# Function to parse YAML content and extract the version
function Get-YamlVersion($yamlContent) {
    $yamlData = $yamlContent | ConvertFrom-Yaml
    $version = $yamlData.info.version
    return $version
}

# Get the version from the OAS file (assuming it's in YAML format)
$oasContent = Get-Content -Path $oasFilePath -Raw
$oasVersion = Get-YamlVersion -yamlContent $oasContent

# Check if the version follows the pattern of x.y.z or x.y.z.z (e.g., 1.0.0, 2.0.0, 1.0.1, 1.0.5)
if ($oasVersion -match '^\d+\.\d+(\.\d+)+$') {
    # Split the version into its components
    $versionComponents = $oasVersion.Split('.')
    $majorVersion = [int]($versionComponents[0])
    $minorVersion = [int]($versionComponents[1])

    # Check if it's a major version change (minor version is zero)
    if ($minorVersion -eq 0) {
        Write-Output "Creating a new API for version $oasVersion"
        $api = Import-AzApiManagementApi -Context $apimContext -ApiId $apiName -Path "/$apiName-v$majorVersion" -SpecificationPath $oasFilePath -SpecificationFormat OpenApiJson
    }
    # Check if it's a revision (minor version is greater than zero)
    else {
        Write-Output "Creating a revision for API version $oasVersion"
        $apiRevision = $oasVersion -replace '\.', '-'
        $api = New-AzApiManagementApiRevision -Context $apimContext -ApiId $apiName -ApiRevision $apiRevision
    }
} else {
    Write-Error "Invalid version format: $oasVersion"
    exit 1
}

# Set policies using Set-AzApiManagementPolicy
$apiPolicies = Get-Content -Path $apiPolicyConfigFilePath -Raw
Set-AzApiManagementPolicy -Context $apimContext -ApiId $api.ApiId -Policy $apiPolicies

Write-Output "Script execution completed."
