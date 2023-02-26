namespace FormRecognizerDemo.Models
{
    public class ReceiptLineItemResponse
    {      
        public DocumentAttributeResponse Description { get; set; }
        public DocumentAttributeResponse Quantity { get; set; }
        public DocumentAttributeResponse TotalPrice { get; set; }
    }
}
