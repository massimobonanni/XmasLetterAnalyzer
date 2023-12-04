using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using XmasLetterAnalyzer.Core.Entities;

namespace XmasLetterAnalyzer.Core.Utilities
{
    public static class JsonUtility
    {
        static private JsonSerializerOptions options = new JsonSerializerOptions
        {
             PropertyNameCaseInsensitive = true,
              WriteIndented=true
        };

        static private JsonSerializerSettings settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
        };

        public static ChildData Deserialize(string jsonString)
        {
            return JsonConvert.DeserializeObject<ChildData>(jsonString, settings);
        }
    }
}
