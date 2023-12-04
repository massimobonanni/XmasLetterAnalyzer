using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmasLetterAnalyzer.Core.Entities;
using XmasLetterAnalyzer.Core.Responses;

namespace XmasLetterAnalyzer.Core.Interfaces
{
    public interface ILetterAnalyzer
    {
        Task<LetterAnalyzerServiceResponse<LetterData>> AnalyzeAsync(Stream sourceStream, CancellationToken token = default);
    }
}
