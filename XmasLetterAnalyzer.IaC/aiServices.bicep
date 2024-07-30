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

var oaiRegion = location == 'westeurope' ? 'swedencentral' : 'canadaeast'

var resourceNamePrefix= '${environmentName}${uniqueString(subscription().id, resourceGroup().name)}'

var aiServiceName = '${resourceNamePrefix}-ai'
var oaiServiceName = '${resourceNamePrefix}-oai'

resource aiService 'Microsoft.CognitiveServices/accounts@2024-04-01-preview' = {
  name: aiServiceName
  location: location
  sku: {
    name: 'S0'
  }
  kind: 'CognitiveServices'
  properties: {
    apiProperties: {
      statisticsEnabled: false
    }
    customSubDomainName: toLower(aiServiceName)
    publicNetworkAccess: 'Enabled'
  }
}

resource oaiService 'Microsoft.CognitiveServices/accounts@2024-04-01-preview' = {
  name: oaiServiceName
  location: oaiRegion
  sku: {
    name: 'S0'
  }
  kind: 'OpenAI'
  properties: {
    customSubDomainName: toLower(oaiServiceName)
    networkAcls: {
      defaultAction: 'Allow'
      virtualNetworkRules: []
      ipRules: []
    }
    publicNetworkAccess: 'Enabled'
  }
}

resource oaiModel 'Microsoft.CognitiveServices/accounts/deployments@2024-04-01-preview' = {
  parent: oaiService
  name: 'letterAnalyzerModel'
  sku: {
    name: 'Standard'
    capacity: 10
  }
  properties: {
    model: {
      format: 'OpenAI'
      name: 'gpt-4-32k'
      version: '0613'
    }
    versionUpgradeOption: 'NoAutoUpgrade'
    raiPolicyName:'Microsoft.Default'
  }
}

output oaiServiceName string = oaiService.name
output aiServiceName string = aiService.name
