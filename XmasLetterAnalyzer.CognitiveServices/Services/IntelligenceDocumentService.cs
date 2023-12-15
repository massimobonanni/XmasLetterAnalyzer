using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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

            DocumentAnalysisClient client = new DocumentAnalysisClient(new Uri(this.config.Endpoint), 
                new AzureKeyCredential(this.config.Key));

            ReadOperationResult results;

            AnalyzeDocumentOperation operation = await client.AnalyzeDocumentAsync(WaitUntil.Started, 
                "prebuilt-read", stream);

            do
            {
                await Task.Delay(125);
                await operation.UpdateStatusAsync( cancellationToken: token);
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
