using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmasLetterAnalyzer.Core.Entities;
using XmasLetterAnalyzer.Core.Responses;

namespace XmasLetterAnalyzer.Core.Interfaces
{
    public interface ILanguageService
    {
        Task<LanguageServiceResponse<ChildData>> AnalyzeTextAsync(string text, CancellationToken token);
    }
}
