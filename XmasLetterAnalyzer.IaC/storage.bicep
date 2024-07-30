@description('The location wher you want to create the resources.')
@allowed([
  'westeurope'
  'eastus'
  'westus2'
])
param location string = 'westeurope'

@description('The name of the environment. It will be used to create the name of the resources in the resource group.')
@maxLength(5)
@minLength(3)
param environmentName string = 'xla'

var resourceNamePrefix= '${environmentName}${uniqueString(subscription().id, resourceGroup().name)}'

var promptsStorageName = '${resourceNamePrefix}store'

resource promptsStorage 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: promptsStorageName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

resource promptsBlobService 'Microsoft.Storage/storageAccounts/blobServices@2023-05-01' = {
  parent: promptsStorage
  name: 'default'
}

resource promptsContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-05-01' = {
  parent: promptsBlobService
  name: 'prompts'
  properties: {
    publicAccess: 'None'
  }
}
