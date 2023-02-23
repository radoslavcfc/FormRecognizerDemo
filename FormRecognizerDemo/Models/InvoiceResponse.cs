using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace FormRecognizerDemo.Models
{
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
                invoiceResponse.VendorName = DocumentFieldMapper.GetField(nameof(VendorName), fields);
                invoiceResponse.CustomerName = DocumentFieldMapper.GetField(nameof(CustomerName), fields);
                invoiceResponse.SubTotal = DocumentFieldMapper.GetField(nameof(SubTotal), fields);
                invoiceResponse.CustomerName = DocumentFieldMapper.GetField(nameof(CustomerName), fields);
                invoiceResponse.SubTotal = DocumentFieldMapper.GetField(nameof(SubTotal), fields);
                invoiceResponse.TotalTax = DocumentFieldMapper.GetField(nameof(TotalTax), fields);
                invoiceResponse.InvoiceTotal = DocumentFieldMapper.GetField(nameof(InvoiceTotal), fields);
                invoiceResponse.BillingAddress = DocumentFieldMapper.GetField(nameof(BillingAddress), fields);
                invoiceResponse.VendorAddress = DocumentFieldMapper.GetField(nameof(VendorAddress), fields);
                invoiceResponse.InvoiceDate = DocumentFieldMapper.GetField(nameof(InvoiceDate), fields);
                invoiceResponse.LineItems = DocumentFieldMapper.GetLineItems(document);
                invoiceResponseCollection.Add(invoiceResponse);                
            }
            return invoiceResponseCollection;
        }
    }
}
