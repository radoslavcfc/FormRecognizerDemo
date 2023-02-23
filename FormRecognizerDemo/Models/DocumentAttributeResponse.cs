namespace FormRecognizerDemo.Models
{
    public class DocumentAttributeResponse
    {
        public DocumentAttributeResponse(string value, float? confidenceLevel)
        {
            Value = value;
            ConfidenceLevel = confidenceLevel * 100;
        }

        public string Value { get; set; }
        public float? ConfidenceLevel { get; set; }
    }
}
