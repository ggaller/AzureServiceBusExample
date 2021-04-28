# Setup
1. Install main Azure resources from `MainArmTemplate\azuredeploy.json` ARM template (using `MainArmTemplate\Deploy-AzureResourceGroup.ps1` or via GUI);
2. Configure installed resources using `MainArmTemplate\PostConfigure.ps1`

# Run locally
1. Create file `FunctionApp\local.settings.json`;
2. Fill with it with propper connections strings:
    ``` json
    {
        "IsEncrypted": false,
        "Values": {
            "FUNCTIONS_WORKER_RUNTIME": "dotnet",
            "AzureWebJobsStorage": "Your storage account connection string",
            "ServiceBusConnectionString": "Your namespace shared access policies send-all connection string"
        }
    }
    ```
3. Run Debug

# Run in Cloud
1. Deploy function `FunctionApp\SendMessage.cs` to preconfigured FunctionApp;
2. Configure function endpoint in `MessageSender\appsettings.json`
3. Build and Run MessageSender project

# Preconfigured rules

1. `ru` -> `de,nl`
2. `de` -> `gb`
3. `nl` -> `null`