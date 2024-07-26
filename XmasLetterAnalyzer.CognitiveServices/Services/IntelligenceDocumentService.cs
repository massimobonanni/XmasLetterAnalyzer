using Azure;
using Azure.AI.DocumentIntelligence;
using Microsoft.Extensions.Configuration;
using XmasLetterAnalyzer.CognitiveServices.Configurations;
using XmasLetterAnalyzer.Core.Interfaces;
using XmasLetterAnalyzer.Core.Responses;

namespace XmasLetterAnalyzer.CognitiveServices.Services
{
    public class IntelligenceDocumentService : IOcrService
    {
        private readonly IntelligenceDocumentServiceConfiguration config;

        public IntelligenceDocumentService(IConfiguration configuration)
        {
            this.config = IntelligenceDocumentServiceConfiguration.Load(configuration);
        }

        public async Task<OcrServiceResponse<string>> ExtractAsync(Stream stream, CancellationToken token)
        {
            var returnValue = new OcrServiceResponse<string>();

            AzureKeyCredential credential = new AzureKeyCredential(this.config.Key);
            DocumentIntelligenceClient client = new DocumentIntelligenceClient(new Uri(this.config.Endpoint), credential);

            var options = new List<DocumentAnalysisFeature>() {
                    DocumentAnalysisFeature.OcrHighResolution,
                    DocumentAnalysisFeature.Languages
                };

            var request = new AnalyzeDocumentContent();
            request.Base64Source = BinaryData.FromStream(stream);

            var operation = await client.AnalyzeDocumentAsync(WaitUntil.Started,
                       "prebuilt-read", request, features: options, cancellationToken: token);
            do
            {
                await Task.Delay(125);
                await operation.UpdateStatusAsync(cancellationToken: token);
            } while (!operation.HasCompleted);

            if (operation.HasValue)
            {
                AnalyzeResult result = operation.Value;
                returnValue.Data = result.Content;
            }

            return returnValue;
        }


    }
}
