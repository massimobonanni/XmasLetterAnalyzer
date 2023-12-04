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

namespace XmasLetterAnalyzer.CognitiveServices.Services
{
    public class OpenAILanguageService : ILanguageService
    {
        private readonly OpenAILanguageServiceConfiguration config;
        private readonly ILogger<OpenAILanguageService> logger;

        public OpenAILanguageService(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<OpenAILanguageService>();
            this.config = OpenAILanguageServiceConfiguration.Load(configuration);
        }

        private static readonly string systemPrompt =
            "You are a Sant Claus assistant that helps Santa Claus to read the children letters looking for their wishes for xmas gifts.";

        private string GenerateUserPrompt(string letterText)
        {
            string prompt = $@"Analyze the following letter: ""{letterText}"".
                Extract which gifts the child wants in the following JSON format:
                {{
                    ""childName"" : ""name of the child"",
                    ""gifts"" : [{{
                        ""type"" : ""type of gift"",
                        ""brand"": ""brand of gift"",
                        ""material"" : ""material of gift"",
                        ""model"": ""model of gift""
                    }}]
                }}
                return only the JSON.
                If a field is not present in the letter, it must not be present in the JSON.
                If you cannot extract data, return an empty string.

                EXAMPLE

                ""Dear Santa Claus, I'm Massimo and I want a football ball of Nike.""

                {{
                    ""childName"" : ""Massimo"",
                    ""gifts"" : [{{
                        ""type"" : ""footbal ball"",
                        ""brand"": ""Nike""
                    }}]
                }}";
            return prompt;
        }

        public async Task<LanguageServiceResponse<ChildData>> AnalyzeTextAsync(string text,
            CancellationToken token)
        {
            logger.LogInformation($"Analyzing text\n{text}");
            var returnValue = new LanguageServiceResponse<ChildData>();

            OpenAIClient client = new OpenAIClient(new Uri(this.config.Endpoint),
                new AzureKeyCredential(this.config.Key));

            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                Messages =
                    {
                         new ChatMessage(ChatRole.System, systemPrompt),
                         new ChatMessage(ChatRole.User, GenerateUserPrompt(text))
                     },
                Temperature = 1.0f,
                MaxTokens = 800,
                DeploymentName = config.ModelName
            };

            Response<ChatCompletions> response = await client.GetChatCompletionsAsync(chatCompletionsOptions);
            ExtractResult(returnValue, response);

            return returnValue;
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
                        var responseText = completions.Choices[0].Message.Content;
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
    }
}
