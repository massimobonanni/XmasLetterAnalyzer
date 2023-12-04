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

            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(this.config.Key))
              { Endpoint = this.config.Endpoint };

            ReadOperationResult results;

            ReadInStreamHeaders? textHeaders = await client.ReadInStreamAsync(stream, cancellationToken: token);
            var operationId = textHeaders.GetOperationId();

            do
            {
                await Task.Delay(125);
                results = await client.GetReadResultAsync(operationId, cancellationToken: token);
            }
            while (results.Status == OperationStatusCodes.Running ||
                results.Status == OperationStatusCodes.NotStarted);

            if (results.Status == OperationStatusCodes.Succeeded)
            {
                StringBuilder respnseText = new StringBuilder();
                foreach (ReadResult page in results.AnalyzeResult.ReadResults)
                {
                    foreach (Line line in page.Lines)
                    {
                        respnseText.Append($"{line.Text} ");
                    }
                }
                returnValue.Data = respnseText.ToString();
            }
            return returnValue;
        }


    }
}
