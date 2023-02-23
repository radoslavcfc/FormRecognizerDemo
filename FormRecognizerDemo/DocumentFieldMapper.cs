using Azure.AI.FormRecognizer.DocumentAnalysis;
using FormRecognizerDemo.Models;

namespace FormRecognizerDemo
{
    public static class DocumentFieldMapper
    {
        public static DocumentAttributeResponse GetField(string fieldName, IReadOnlyDictionary<string, DocumentField> itemFields)
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

        public static IEnumerable<LineItemResponse> GetLineItems(AnalyzedDocument document)
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
