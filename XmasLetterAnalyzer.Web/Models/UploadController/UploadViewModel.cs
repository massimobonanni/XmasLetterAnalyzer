using XmasLetterAnalyzer.Core.Entities;
using XmasLetterAnalyzer.Core.Responses;

namespace XmasLetterAnalyzer.Web.Models.UploadController
{
    public class UploadViewModel
    {
        public LetterAnalyzerServiceResponse<LetterData> LetterData { get; set; }
        public byte[] ImageData { get; set; }

        public UploadViewModel(LetterAnalyzerServiceResponse<LetterData> letterServiceResponse)
        {
             this.LetterData = letterServiceResponse;
        }
    }
}
