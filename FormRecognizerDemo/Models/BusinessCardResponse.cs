using Azure.AI.FormRecognizer.DocumentAnalysis;
using FormRecognizerDemo.Core;

namespace FormRecognizerDemo.Models
{
    public class BusinessCardResponse
    {
        public IEnumerable<DocumentAttributeResponse> CompanyNames { get; set; }
        public IEnumerable<DocumentAttributeResponse> ContactNames { get; set; }
        public IEnumerable<DocumentAttributeResponse> Emails { get; set; }
        public IEnumerable<DocumentAttributeResponse> Websites { get; set; }
        public IEnumerable<DocumentAttributeResponse> WorkPhones { get; set; }
        public IEnumerable<AddressResponse> Addresses { get; set; }
        public IEnumerable<AddressResponse> MobilePhones { get; set; }
        public static IEnumerable<BusinessCardResponse> MapToDto(AnalyzeResult result)
        {
            var invoiceResponseCollection = new List<BusinessCardResponse>();

            for (int i = 0; i < result.Documents.Count; i++)
            {
                AnalyzedDocument document = result.Documents[i];
                var fields = document.Fields;
                var businessCardResponse = new BusinessCardResponse();
                businessCardResponse.CompanyNames = DocumentFieldMapper.GetMultipleFields(nameof(CompanyNames), fields);
                businessCardResponse.ContactNames = DocumentFieldMapper.GetMultipleFields(nameof(ContactNames), fields);
                businessCardResponse.Emails = DocumentFieldMapper.GetMultipleFields(nameof(Emails), fields);
                businessCardResponse.Websites = DocumentFieldMapper.GetMultipleFields(nameof(Websites), fields);
                businessCardResponse.WorkPhones = DocumentFieldMapper.GetMultipleFields(nameof(WorkPhones), fields);
                businessCardResponse.Addresses = DocumentFieldMapper.GetMultipleAddresses(nameof(Addresses), fields);
                businessCardResponse.MobilePhones = DocumentFieldMapper.GetMultipleAddresses(nameof(MobilePhones), fields);

                invoiceResponseCollection.Add(businessCardResponse);
            }
            return invoiceResponseCollection;
        }
    }

}