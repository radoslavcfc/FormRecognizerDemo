using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace FormRecognizerDemo.Models
{
    public class DocumentAttributeResponse
    {
        public string Value { get; set; }
        public float? ConfidenceLevel { get; set; }
    }
    public class LineItemResponse
    {
        public KeyValuePair<string, float?> Description { get; set; }
        public KeyValuePair<string, float?> Quantity { get; set; }
        public KeyValuePair<string, float?> Amount { get; set; }
    }
    public class InvoiceResponse
    {
        public KeyValuePair<string, float?> VendorName { get; set; }
        public KeyValuePair<string, float?> CustomerName { get; set; }
        public KeyValuePair<string, float?> SubTotal { get; set; }
        public KeyValuePair<string, float?>  TotalTax { get; set; }
        public KeyValuePair<string, float?> InvoiceTotal { get; set; }
        public KeyValuePair<string, float?> BillingAddress { get; private set; }
        public KeyValuePair<string, float?> VendorAddress { get; private set; }
        public KeyValuePair<string, float?> InvoiceDate { get; private set; }
        public IEnumerable<LineItemResponse> LineItems { get; set; }

        public static IEnumerable<InvoiceResponse> MapToDto (AnalyzeResult result)
        {
            var invoiceResponseCollection = new List<InvoiceResponse>();

            for (int i = 0; i < result.Documents.Count; i++)
            {    
                AnalyzedDocument document = result.Documents[i];
                var invoiceResponse = new InvoiceResponse();
                invoiceResponse.VendorName = GetField(nameof(VendorName), document);
                invoiceResponse.CustomerName = GetField(nameof(CustomerName), document);
                invoiceResponse.SubTotal = GetField(nameof(SubTotal), document);
                invoiceResponse.CustomerName = GetField(nameof(CustomerName), document);
                invoiceResponse.SubTotal = GetField(nameof(SubTotal), document);
                invoiceResponse.TotalTax = GetField(nameof(TotalTax), document);
                invoiceResponse.InvoiceTotal = GetField(nameof(InvoiceTotal), document);
                invoiceResponse.BillingAddress = GetField(nameof(BillingAddress), document);
                invoiceResponse.VendorAddress = GetField(nameof(VendorAddress), document);
                invoiceResponse.InvoiceDate = GetField(nameof(InvoiceDate), document);
                invoiceResponse.LineItems = GetLineItems(document);
                invoiceResponseCollection.Add(invoiceResponse);                
            }
            return invoiceResponseCollection;
        }

        private static KeyValuePair<string, float?> GetField(string fieldName, AnalyzedDocument document)
        {
            
            if (document.Fields.TryGetValue(fieldName, out DocumentField? docField))
            {
                if (docField.FieldType == DocumentFieldType.String)
                {
                    string stringValue = docField.Value.AsString();
                    return new KeyValuePair<string, float?>(stringValue, docField.Confidence);
                   
                }
                if (docField.FieldType == DocumentFieldType.Currency)
                {
                    CurrencyValue currencyValue = docField.Value.AsCurrency();
                    return new KeyValuePair<string, float?>
                        ($"{currencyValue.Symbol}{currencyValue.Amount}", docField.Confidence);                    
                }

                if (docField.FieldType == DocumentFieldType.Date)
                {
                    DateTimeOffset dateValue = docField.Value.AsDate();
                    return new KeyValuePair<string, float?>
                        ($"{dateValue.ToString()}", docField.Confidence);
                }

                if (docField.FieldType == DocumentFieldType.Address)
                {
                    AddressValue addressValue = docField.Value.AsAddress();
                    return new KeyValuePair<string, float?>
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
                    foreach (DocumentField itemField in itemsField.Value.AsList())
                    {   
                        var currentLine = new LineItemResponse();

                        if (itemField.FieldType == DocumentFieldType.Dictionary)
                        {
                            IReadOnlyDictionary<string, DocumentField> itemFields = itemField.Value.AsDictionary();

                            if (itemFields.TryGetValue("Description", out DocumentField? itemDescriptionField))
                            {
                                if (itemDescriptionField.FieldType == DocumentFieldType.String)
                                {
                                    string itemDescription = itemDescriptionField.Value.AsString();

                                    Console.WriteLine($"  Description: '{itemDescription}', with confidence {itemDescriptionField.Confidence}");
                                }
                            }

                            if (itemFields.TryGetValue("Amount", out DocumentField? itemAmountField))
                            {
                                if (itemAmountField.FieldType == DocumentFieldType.Currency)
                                {
                                    CurrencyValue itemAmount = itemAmountField.Value.AsCurrency();

                                    Console.WriteLine($"  Amount: '{itemAmount.Symbol}{itemAmount.Amount}', with confidence {itemAmountField.Confidence}");
                                }
                            }
                        }
                    }
                }
            }
            return lineItems;

        }
    }
}
