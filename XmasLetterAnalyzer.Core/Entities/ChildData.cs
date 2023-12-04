namespace XmasLetterAnalyzer.Core.Entities
{
    public class ChildData
    {
        public string ChildName { get; set; }

        public IEnumerable<GiftData> Gifts { get; set; }
    }
}