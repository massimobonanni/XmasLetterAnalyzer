using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmasLetterAnalyzer.Core.Responses
{
    public abstract class ServiceResponseBase<TResponse>
    {
        public string Error { get; set; }
        public TResponse Data { get; set; }

        public bool HasError => !string.IsNullOrEmpty(this.Error);
    }
}
