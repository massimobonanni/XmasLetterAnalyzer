using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmasLetterAnalyzer.Core.Responses;

namespace XmasLetterAnalyzer.Core.Interfaces
{
    public interface IOcrService
    {
        Task<OcrServiceResponse<string>> ExtractAsync(Stream stream, CancellationToken token);
    }
}
