using Azure.AI.FormRecognizer.DocumentAnalysis;
using FormRecognizerDemo.Models;

namespace FormRecognizerDemo
{
    public static class DocumentFieldMapper
    {
        private static DocumentAttributeResponse GetValueByType(DocumentField docField)
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

            if (docField.FieldType == DocumentFieldType.Double)
            {
                double doubleValue = docField.Value.AsDouble();
                return new DocumentAttributeResponse
                    ($"{doubleValue}", docField.Confidence);
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
                    $"{addressValue.State}, {addressValue.PostalCode}, {addressValue.CountryRegion}",
                    docField.Confidence);
            }
            return default;
        }
        public static DocumentAttributeResponse GetField(string fieldName, IReadOnlyDictionary<string, DocumentField> itemFields)
        {

            if (itemFields.TryGetValue(fieldName, out DocumentField? docField))
            {
                return GetValueByType(docField);
            }
            return default;
        }

        public static IEnumerable<InvoiceLineItemResponse> GetInvoiceLineItems(AnalyzedDocument document)
        {
            var lineItems = new List<InvoiceLineItemResponse>();

            if (document.Fields.TryGetValue("Items", out DocumentField? itemsField))
            {
                if (itemsField.FieldType == DocumentFieldType.List)
                {                    
                    foreach (DocumentField itemField in itemsField.Value.AsList())
                    {
                        var currentLine = new InvoiceLineItemResponse();
                        if (itemField.FieldType == DocumentFieldType.Dictionary)
                        {
                            IReadOnlyDictionary<string, DocumentField> itemFields = itemField.Value.AsDictionary();
                            currentLine.Description = GetField((nameof(currentLine.Description)), itemFields);
                            currentLine.Quantity = GetField((nameof(currentLine.Quantity)), itemFields);
                            currentLine.Amount = GetField((nameof(currentLine.Amount)), itemFields);
                        }
                        lineItems.Add(currentLine);
                    }                   
                }
            }
            return lineItems;
        }

        public static IEnumerable<ReceiptLineItemResponse> GetReceiptLineItems(AnalyzedDocument document)
        {
            var lineItems = new List<ReceiptLineItemResponse>();

            if (document.Fields.TryGetValue("Items", out DocumentField? itemsField))
            {
                if (itemsField.FieldType == DocumentFieldType.List)
                {
                    foreach (DocumentField itemField in itemsField.Value.AsList())
                    {
                        var currentLine = new ReceiptLineItemResponse();
                        if (itemField.FieldType == DocumentFieldType.Dictionary)
                        {
                            IReadOnlyDictionary<string, DocumentField> itemFields = itemField.Value.AsDictionary();
                            currentLine.Description = GetField((nameof(currentLine.Description)), itemFields);
                            currentLine.Quantity = GetField((nameof(currentLine.Quantity)), itemFields);
                            currentLine.TotalPrice = GetField((nameof(currentLine.TotalPrice)), itemFields);
                        }
                        lineItems.Add(currentLine);
                    }
                }
            }
            return lineItems;
        }

        public static IEnumerable<DocumentAttributeResponse> GetMultipleFields(string fieldName, IReadOnlyDictionary<string, DocumentField> itemFields)
        {
            if (itemFields.TryGetValue(fieldName, out DocumentField jobTitlesFields))
            {
                if (jobTitlesFields.FieldType == DocumentFieldType.List)
                {
                    var collection = new List<DocumentAttributeResponse>();

                    foreach (DocumentField jobTitleField in jobTitlesFields.Value.AsList())
                    {
                        collection.Add(GetValueByType(jobTitleField));
                    }
                    return collection;
                }
            }
            return default;
        }        
    }
}
