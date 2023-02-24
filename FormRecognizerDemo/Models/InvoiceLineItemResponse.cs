namespace FormRecognizerDemo.Models
{
    public class InvoiceLineItemResponse
    {
        public DocumentAttributeResponse Description { get; set; }
        public DocumentAttributeResponse Quantity { get; set; }
        public DocumentAttributeResponse Amount { get; set; }
    }
}
