using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace FormRecognizerDemo.Models
{
    public class ReceiptResponse
    {
        public DocumentAttributeResponse MerchantName { get; set; }
        public AddressResponse MerchantAddress { get; set; }
        public DocumentAttributeResponse MerchantPhoneNumber { get; set; }
        public DocumentAttributeResponse Subtotal { get; set; }
        public DocumentAttributeResponse Total { get; set; }
        public DocumentAttributeResponse TotalTax { get; private set; }
        public DocumentAttributeResponse TransactionDate { get; private set; }
        public DocumentAttributeResponse TransactionTime { get; private set; }
        public IEnumerable<ReceiptLineItemResponse> LineItems { get; set; }

        public static IEnumerable<ReceiptResponse> MapToDto(AnalyzeResult result)
        {
            var invoiceResponseCollection = new List<ReceiptResponse>();

            for (int i = 0; i < result.Documents.Count; i++)
            {
                AnalyzedDocument document = result.Documents[i];
                var fields = document.Fields;
                var receiptResponse = new ReceiptResponse();
                receiptResponse.MerchantName = DocumentFieldMapper.GetField(nameof(MerchantName), fields);
                receiptResponse.MerchantAddress = DocumentFieldMapper.GetAddress(nameof(MerchantAddress), fields);
                receiptResponse.MerchantPhoneNumber = DocumentFieldMapper.GetField(nameof(MerchantPhoneNumber), fields);
                receiptResponse.Subtotal = DocumentFieldMapper.GetField(nameof(Subtotal), fields);
                receiptResponse.Total = DocumentFieldMapper.GetField(nameof(Total), fields);
                receiptResponse.TotalTax = DocumentFieldMapper.GetField(nameof(TotalTax), fields);
                receiptResponse.TotalTax = DocumentFieldMapper.GetField(nameof(TotalTax), fields);
                receiptResponse.TransactionDate = DocumentFieldMapper.GetField(nameof(TransactionDate), fields);
                receiptResponse.TransactionTime = DocumentFieldMapper.GetField(nameof(TransactionTime), fields);
                receiptResponse.LineItems = DocumentFieldMapper.GetReceiptLineItems(document);
                invoiceResponseCollection.Add(receiptResponse);
            }
            return invoiceResponseCollection;
        }
    }
}
