using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Microsoft.Extensions.Configuration;
using XmasLetterAnalyzer.CognitiveServices.Configurations;
using XmasLetterAnalyzer.Core.Interfaces;
using XmasLetterAnalyzer.Core.Responses;

namespace XmasLetterAnalyzer.CognitiveServices.Services
{
    public class VisionOcrService : IOcrService
    {
        private readonly VisionOcrServiceConfiguration config;

        public VisionOcrService(IConfiguration configuration)
        {
            this.config = VisionOcrServiceConfiguration.Load(configuration);
        }

        public async Task<OcrServiceResponse<string>> ExtractAsync(Stream stream, CancellationToken token)
        {
            var returnValue = new OcrServiceResponse<string>();

            ImageAnalysisClient client =
              new ImageAnalysisClient(new Uri(this.config.Endpoint),
                new AzureKeyCredential(this.config.Key));

            var imageData = BinaryData.FromStream(stream);
            var visualFeature = VisualFeatures.Read;

            var visionResponse = await client.AnalyzeAsync(imageData, visualFeature, cancellationToken: token);

            if (!visionResponse.GetRawResponse().IsError && visionResponse.Value != null 
                && visionResponse.Value.Read != null)
            {
                returnValue.Data = visionResponse.Value.Read.Blocks
                    .SelectMany(block => block.Lines)
                    .Select(line => line.Text)
                    .Aggregate((a,b)=>a+" "+b);
            }
            
            return returnValue;
        }


    }
}
