using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace FormRecognizerDemo.Models
{
    public class BusinessCardResponse
    {
        public IEnumerable<DocumentAttributeResponse> CompanyNames { get; set; }
        public IEnumerable<DocumentAttributeResponse> ContactNames { get; set; }
        public IEnumerable<DocumentAttributeResponse> Emails { get; set; }
        public IEnumerable<DocumentAttributeResponse> Websites { get; set; }
        public IEnumerable<DocumentAttributeResponse> WorkPhones { get; set; }

        public static IEnumerable<BusinessCardResponse> MapToDto(AnalyzeResult result)
        {
            var invoiceResponseCollection = new List<BusinessCardResponse>();

            for (int i = 0; i < result.Documents.Count; i++)
            {
                AnalyzedDocument document = result.Documents[i];
                var fields = document.Fields;
                var invoiceResponse = new BusinessCardResponse();
                invoiceResponse.CompanyNames = DocumentFieldMapper.GetMultipleFields(nameof(CompanyNames), fields);
                invoiceResponse.ContactNames = DocumentFieldMapper.GetMultipleFields(nameof(ContactNames), fields);
                invoiceResponse.Emails = DocumentFieldMapper.GetMultipleFields(nameof(Emails), fields);
                invoiceResponse.Websites = DocumentFieldMapper.GetMultipleFields(nameof(Websites), fields);
                invoiceResponse.WorkPhones = DocumentFieldMapper.GetMultipleFields(nameof(WorkPhones), fields);
                
                invoiceResponseCollection.Add(invoiceResponse);
            }
            return invoiceResponseCollection;
        }
    }

}