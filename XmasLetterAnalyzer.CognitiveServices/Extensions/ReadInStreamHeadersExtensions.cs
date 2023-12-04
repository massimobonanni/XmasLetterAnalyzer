
namespace Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models
{
    internal static class ReadInStreamHeadersExtensions
    {
        const int numberOfCharsInOperationId = 36;
        public static Guid GetOperationId(this ReadInStreamHeaders streamHeaders)
        {
            string operationLocation = streamHeaders.OperationLocation;
            string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);
            return Guid.Parse(operationId);
        }
    }
}
