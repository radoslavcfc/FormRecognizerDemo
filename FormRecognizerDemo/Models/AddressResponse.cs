namespace FormRecognizerDemo.Models
{
    public class AddressResponse
    {
        public string HouseNumber { get; internal set; }
        public string Road { get; internal set; }
        public string City { get; internal set; }
        public string State { get; internal set; }
        public string PostalCode { get; internal set; }
        public string CountryRegion { get; internal set; }
        public float? ConfidenceLevel { get; internal set; }
    }
}
