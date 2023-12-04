using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XmasLetterAnalyzer.Core.Entities;
using XmasLetterAnalyzer.Core.Interfaces;
using XmasLetterAnalyzer.Core.Responses;

namespace XmasLetterAnalyzer.CognitiveServices.Services
{
    public class CognitiveLetterAnalyzer : ILetterAnalyzer
    {
        private readonly IOcrService ocrService;
        private readonly ILanguageService languageService;
        private readonly IConfiguration configuration;
        private readonly ILogger<CognitiveLetterAnalyzer> logger;

        public CognitiveLetterAnalyzer(IOcrService ocrService, ILanguageService languageService,
            IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this.ocrService = ocrService;
            this.languageService = languageService;
            this.configuration = configuration;
            this.logger = loggerFactory.CreateLogger<CognitiveLetterAnalyzer>();
        }

        public async Task<LetterAnalyzerServiceResponse<LetterData>> AnalyzeAsync(Stream sourceStream, CancellationToken token = default)
        {

            var result = new LetterAnalyzerServiceResponse<LetterData>();
            try
            {
                var ocrResult = await this.ocrService.ExtractAsync(sourceStream, token);
                if (!ocrResult.HasError)
                {
                    var languageResult = await this.languageService.AnalyzeTextAsync(ocrResult.Data, token);
                    if (!languageResult.HasError)
                    {
                        result.Data = new LetterData()
                        {
                            Child = languageResult.Data,
                            Text = ocrResult.Data
                        };

                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while analyzing letter");
                result.Error = ex.Message;
            }
            return result;
        }

    }
}
