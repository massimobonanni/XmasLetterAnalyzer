using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmasLetterAnalyzer.CognitiveServices.Configurations
{
    internal class VisionOcrServiceConfiguration
    {
        const string ConfigRootName = "VisionService";
        public string Key { get; set; }
        public string Endpoint { get; set; }

        public static VisionOcrServiceConfiguration Load(IConfiguration config)
        {
            var retVal = new VisionOcrServiceConfiguration();
            retVal.Key = config[$"{ConfigRootName}:Key"];
            retVal.Endpoint = config[$"{ConfigRootName}:Endpoint"];
            return retVal;
        }

    }
}
