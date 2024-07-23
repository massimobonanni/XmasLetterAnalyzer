using Azure.AI.OpenAI;
using Azure;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmasLetterAnalyzer.CognitiveServices.Configurations;
using XmasLetterAnalyzer.Core.Entities;
using XmasLetterAnalyzer.Core.Interfaces;
using XmasLetterAnalyzer.Core.Utilities;
using XmasLetterAnalyzer.Core.Responses;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using XmasLetterAnalyzer.CognitiveServices.Managers;

namespace XmasLetterAnalyzer.CognitiveServices.Services
{
    public class OpenAILanguageService : ILanguageService
    {
        private readonly OpenAILanguageServiceConfiguration config;
        private readonly ILogger<OpenAILanguageService> logger;
        private readonly PromptManager promptManager = new PromptManager();

        public OpenAILanguageService(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<OpenAILanguageService>();
            this.config = OpenAILanguageServiceConfiguration.Load(configuration);
        }     

        private void ExtractResult(LanguageServiceResponse<ChildData> returnValue, Response<ChatCompletions> response)
        {
            if (!response.GetRawResponse().IsError)
            {
                ChatCompletions completions = response.Value;
                logger.LogInformation($"OpenAI usage\n{completions.Usage}");

                var mainChoice = completions.Choices.FirstOrDefault();

                if (mainChoice != null)
                {
                    if (mainChoice.ContentFilterResults.Error == null)
                    {
                        var responseText = mainChoice.Message.Content;
                        if (!string.IsNullOrWhiteSpace(responseText))
                        {
                            returnValue.Data = JsonUtility.Deserialize(responseText);
                        }
                        else
                        {
                            returnValue.Error = "No child information from letter";
                        }
                    }
                    else
                    {
                        returnValue.Error = mainChoice.ContentFilterResults.Error.Message;
                    }
                }
                else
                {
                    returnValue.Error = "No child information from letter";
                }
            }
            else
            {
                returnValue.Error = response.GetRawResponse().ReasonPhrase;
            }
        }

        public async Task<LanguageServiceResponse<ChildData>> AnalyzeTextAsync(string text,
            CancellationToken token)
        {
            await promptManager.Load(config.PromptTextSAS);

            logger.LogInformation($"Analyzing text\n{text}");
            var returnValue = new LanguageServiceResponse<ChildData>();

            OpenAIClient client = new OpenAIClient(new Uri(this.config.Endpoint),
                new AzureKeyCredential(this.config.Key));

            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                Messages =
                    {
                         new ChatMessage(ChatRole.System, promptManager.GenerateSystemPrompt(text)),
                         new ChatMessage(ChatRole.User, promptManager.GenerateUserPrompt(text))
                     },
                Temperature = 0.0f,
                MaxTokens = 100,
                ChoiceCount = 1,
                DeploymentName = config.ModelName
            };

            Response<ChatCompletions> response = await client.GetChatCompletionsAsync(chatCompletionsOptions);
            ExtractResult(returnValue, response);

            return returnValue;
        }

        
    }
}
