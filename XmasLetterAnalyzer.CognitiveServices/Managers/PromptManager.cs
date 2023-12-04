using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmasLetterAnalyzer.Core.Utilities;

namespace XmasLetterAnalyzer.CognitiveServices.Managers
{
    internal class PromptManager
    {
        private static bool firstTimeExecution = true;
        private static string systemPrompt = string.Empty;
        private static string userPrompt = string.Empty;

        public async Task Load(string promptBlobSAS)
        {
            if (!string.IsNullOrEmpty(promptBlobSAS))
            {
                if (firstTimeExecution)
                {
                    var promptText = await StorageAccountUtility.DownloadText(promptBlobSAS);
                    var prompts = promptText.Split("###---###", StringSplitOptions.RemoveEmptyEntries);
                    systemPrompt = prompts[0];
                    userPrompt = prompts[1];
                    firstTimeExecution = false;
                }
            }
            else
            {
                systemPrompt = Properties.Resources.DefaultSystemPrompt;
                userPrompt = Properties.Resources.DefaultUserPrompt;
            }
        }

        public string GenerateSystemPrompt(string letterText)
        {
            return systemPrompt.Replace("{letterText}", letterText);
        }

        public string GenerateUserPrompt(string letterText)
        {
            return userPrompt.Replace("{letterText}", letterText);
        }
    }
}
