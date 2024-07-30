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

param oaiServiceName string
param aiServiceName string

var resourceNamePrefix= '${environmentName}${uniqueString(subscription().id, resourceGroup().name)}'

var appServiceName = '${resourceNamePrefix}-app'
var appServicePlanName = '${resourceNamePrefix}-plan'
var appInsightsName = '${resourceNamePrefix}-appinsight'

resource aiService 'Microsoft.CognitiveServices/accounts@2024-04-01-preview' existing = {
  name: aiServiceName
}
resource oaiService 'Microsoft.CognitiveServices/accounts@2024-04-01-preview' existing = {
  name: oaiServiceName
}

resource appService 'Microsoft.Web/sites@2023-12-01' = {
  name: appServiceName
  location: location
  kind: 'app'
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      netFrameworkVersion: 'v6.0'
      managedPipelineMode: 'Integrated'
    }
  }
}

resource scmBasicCredentials 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2022-09-01' = {
  name: 'scm'
  parent: appService
  properties: {
    allow: true
  }
}

resource appServiceSettings 'Microsoft.Web/sites/config@2022-03-01' = {
  name: 'appsettings'
  parent: appService
  properties: {
    APPINSIGHTS_INSTRUMENTATIONKEY: appInsights.properties.InstrumentationKey
    'IntelligenceDocumentService:Endpoint':aiService.properties.endpoint
    'IntelligenceDocumentService:Key':aiService.listKeys().key1
    'OpenAIService:Endpoint':oaiService.properties.endpoint
    'OpenAIService:Key':oaiService.listKeys().key1
    'OpenAIService:Modelname':'letterAnalyzerModel'
    'OpenAIService:PromptTextSAS':''
    'VisionService:Endpoint':aiService.properties.endpoint
    'VisionService:Key':aiService.listKeys().key1
  }
}


resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01'={
  name: appServicePlanName
  location: location
  sku: {
    name: 'F1'
  }
}

resource appInsights 'Microsoft.Insights/components@2015-05-01' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Request_Source: 'rest'
  }
}
