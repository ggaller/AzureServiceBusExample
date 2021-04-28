#Requires -Version 3.0
Param(
    [string] [Parameter(Mandatory=$true)] $SubscriptionName,
    [string] $ResourceGroupName = 'AbcApp',
    [string] $StorageAccountName = 'abcappstorage',
    [string] $FunctionApp = 'AbcFunctionApp',
    [string] $Namespace = 'abcnamespace'
    )   

# Log on to Azure and set the active subscription
Connect-AzAccount
Select-AzSubscription -SubscriptionName $SubscriptionName

# Get the storage key for the storage account
$storageAccountKey = (Get-AzStorageAccountKey -ResourceGroupName $ResourceGroupName -Name $StorageAccountName).Value[0]

# Get a storage context
$storageContext = New-AzStorageContext -StorageAccountName $StorageAccountName -StorageAccountKey $storageAccountKey

# Get a reference to the table
$table = Get-AzStorageTable -Name "inboxrules" -Context $storageContext 

$partitionKey = "InboundRules"
[Microsoft.WindowsAzure.Storage.Table.TableBatchOperation]$batchOperation = New-Object -TypeName Microsoft.WindowsAzure.Storage.Table.TableBatchOperation

# Inset rules
$entity = New-Object -TypeName Microsoft.WindowsAzure.Storage.Table.DynamicTableEntity -ArgumentList $partitionKey, "ru"
$entity.Properties.Add("Queues", "de,nl")
$batchOperation.InsertOrReplace($entity)

$entity = New-Object -TypeName Microsoft.WindowsAzure.Storage.Table.DynamicTableEntity -ArgumentList $partitionKey, "de"
$entity.Properties.Add("Queues", "gb")
$batchOperation.InsertOrReplace($entity)

$entity = New-Object -TypeName Microsoft.WindowsAzure.Storage.Table.DynamicTableEntity -ArgumentList $partitionKey, "nl"
$batchOperation.InsertOrReplace($entity)

#$table.CloudTable.ExecuteBatch($batchOperation)

# Configure FunctionApp ConnectionStrings
$namespaceConnections = Get-AzServiceBusKey -ResourceGroup $ResourceGroupName -Namespace $Namespace -Name "send-all"
$appSettings = @{"AzureWebJobsStorage" = $storageContext.ConnectionString; "ServiceBusConnectionString" = $namespaceConnections.PrimaryConnectionString }
Update-AzFunctionAppSetting -Name $FunctionApp -ResourceGroupName $ResourceGroupName -AppSetting $appSettings -Force