using Azure.AI.FormRecognizer.DocumentAnalysis;

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
    public class LineItemResponse
    {
        public DocumentAttributeResponse Description { get; set; }
        public DocumentAttributeResponse Quantity { get; set; }
        public DocumentAttributeResponse Amount { get; set; }
    }
    public class InvoiceResponse
    {
        public DocumentAttributeResponse VendorName { get; set; }
        public DocumentAttributeResponse CustomerName { get; set; }
        public DocumentAttributeResponse SubTotal { get; set; }
        public DocumentAttributeResponse TotalTax { get; set; }
        public DocumentAttributeResponse InvoiceTotal { get; set; }
        public DocumentAttributeResponse BillingAddress { get; private set; }
        public DocumentAttributeResponse VendorAddress { get; private set; }
        public DocumentAttributeResponse InvoiceDate { get; private set; }
        public IEnumerable<LineItemResponse> LineItems { get; set; }

        public static IEnumerable<InvoiceResponse> MapToDto (AnalyzeResult result)
        {
            var invoiceResponseCollection = new List<InvoiceResponse>();

            for (int i = 0; i < result.Documents.Count; i++)
            {    
                AnalyzedDocument document = result.Documents[i];
                var fields = document.Fields;
                var invoiceResponse = new InvoiceResponse();
                invoiceResponse.VendorName = GetField(nameof(VendorName), fields);
                invoiceResponse.CustomerName = GetField(nameof(CustomerName), fields);
                invoiceResponse.SubTotal = GetField(nameof(SubTotal), fields);
                invoiceResponse.CustomerName = GetField(nameof(CustomerName), fields);
                invoiceResponse.SubTotal = GetField(nameof(SubTotal), fields);
                invoiceResponse.TotalTax = GetField(nameof(TotalTax), fields);
                invoiceResponse.InvoiceTotal = GetField(nameof(InvoiceTotal), fields);
                invoiceResponse.BillingAddress = GetField(nameof(BillingAddress), fields);
                invoiceResponse.VendorAddress = GetField(nameof(VendorAddress), fields);
                invoiceResponse.InvoiceDate = GetField(nameof(InvoiceDate), fields);
                invoiceResponse.LineItems = GetLineItems(document);
                invoiceResponseCollection.Add(invoiceResponse);                
            }
            return invoiceResponseCollection;
        }

        private static DocumentAttributeResponse GetField(string fieldName, IReadOnlyDictionary<string, DocumentField> itemFields)
        {
            
            if (itemFields.TryGetValue(fieldName, out DocumentField? docField))
            {
                if (docField.FieldType == DocumentFieldType.String)
                {
                    string stringValue = docField.Value.AsString();
                    return new DocumentAttributeResponse(stringValue, docField.Confidence);
                   
                }
                if (docField.FieldType == DocumentFieldType.Currency)
                {
                    CurrencyValue currencyValue = docField.Value.AsCurrency();
                    return new DocumentAttributeResponse
                        ($"{currencyValue.Symbol}{currencyValue.Amount}", docField.Confidence);                    
                }

                if (docField.FieldType == DocumentFieldType.Date)
                {
                    DateTimeOffset dateValue = docField.Value.AsDate();
                    return new DocumentAttributeResponse
                        ($"{dateValue.ToString("d")}", docField.Confidence);
                }

                if (docField.FieldType == DocumentFieldType.Address)
                {
                    AddressValue addressValue = docField.Value.AsAddress();
                    return new DocumentAttributeResponse
                        ($"{addressValue.HouseNumber}, {addressValue.Road}, {addressValue.City}, " +
                        $"{addressValue.State}, {addressValue.PoBox}, {addressValue.PostalCode}, {addressValue.CountryRegion}",
                        docField.Confidence);
                }
            }
            return default;
        }

        private static IEnumerable<LineItemResponse> GetLineItems(AnalyzedDocument document)
        {
            var lineItems = new List<LineItemResponse>();

            if (document.Fields.TryGetValue("Items", out DocumentField? itemsField))
            {
                if (itemsField.FieldType == DocumentFieldType.List)
                {
                    var currentLine = new LineItemResponse();
                    foreach (DocumentField itemField in itemsField.Value.AsList())
                    {   
                        if (itemField.FieldType == DocumentFieldType.Dictionary)
                        {
                            IReadOnlyDictionary<string, DocumentField> itemFields = itemField.Value.AsDictionary();
                            currentLine.Description = GetField((nameof(currentLine.Description)), itemFields);
                            currentLine.Quantity = GetField((nameof(currentLine.Quantity)), itemFields);
                            currentLine.Amount = GetField((nameof(currentLine.Amount)), itemFields);
                        }
                    }
                    lineItems.Add(currentLine);
                }
            }
            return lineItems;

        }
    }
}
