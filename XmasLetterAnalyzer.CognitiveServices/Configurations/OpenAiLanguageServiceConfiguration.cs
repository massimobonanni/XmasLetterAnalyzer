using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmasLetterAnalyzer.CognitiveServices.Configurations
{
    internal class OpenAILanguageServiceConfiguration
    {
        const string ConfigRootName = "OpenAIService";
        public string Key { get; set; }
        public string Endpoint { get; set; }
        public string ModelName { get; set; }

        public static OpenAILanguageServiceConfiguration Load(IConfiguration config)
        {
            var retVal = new OpenAILanguageServiceConfiguration();
            retVal.Key = config[$"{ConfigRootName}:Key"];
            retVal.Endpoint = config[$"{ConfigRootName}:Endpoint"];
            retVal.ModelName = config[$"{ConfigRootName}:ModelName"];
            return retVal;
        }

    }
}
