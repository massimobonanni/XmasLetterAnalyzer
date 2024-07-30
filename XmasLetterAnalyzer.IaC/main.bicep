targetScope = 'subscription'
@description('The name of the resource group that contains all the resources')
param resourceGroupName string = 'XmasLetterAnalyzer-rg'

@description('The name of the environment. It will be used to create the name of the resources in the resource group.')
@maxLength(5)
@minLength(3)
param environmentName string = 'xla'

@description('The location of the resource group and resources')
@allowed([
  'westeurope'
  'eastus'
  'westus2'
])
param location string = 'westeurope'

param currentDate string = utcNow()

resource resourceGroup 'Microsoft.Resources/resourceGroups@2021-01-01' = {
  name: resourceGroupName
  location: location
}

module aiServicesModule 'aiServices.bicep' = {
  scope: resourceGroup
  name: 'aiServices-${currentDate}'
  params: {
    location: location
    environmentName: environmentName
  }
}

module frontEndModule 'frontEnd.bicep' = {
  scope: resourceGroup
  name: 'frontEnd-${currentDate}'
  params: {
    location: location
    environmentName: environmentName
    oaiServiceName: aiServicesModule.outputs.oaiServiceName
    aiServiceName: aiServicesModule.outputs.aiServiceName
  }
}

module storageModule 'storage.bicep' = {
  scope: resourceGroup
  name: 'storage-${currentDate}'
  params: {
    location: location
    environmentName: environmentName
  }
}
