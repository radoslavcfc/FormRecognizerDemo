using System.Net;
using System.Text.Json;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure.Core;
using FormRecognizerDemo.Models;
using HttpMultipartParser;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FormRecognizerDemo
{
    public class ScanFileFunction
    {
        private readonly ILogger _logger;
        private readonly FormRecognizerOptions _recognizerOptions;
        private Stream fileUri;

        public ScanFileFunction(ILoggerFactory loggerFactory, IOptions<FormRecognizerOptions> recognizerOptions)
        {
            _logger = loggerFactory.CreateLogger<ScanFileFunction>();
            _recognizerOptions = recognizerOptions.Value;
        }

        [Function(nameof(ScanFileFunction))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            var parsedFormBody = MultipartFormDataParser.ParseAsync(req.Body);
            var file = parsedFormBody.Result.Files[0].Data;

            AzureKeyCredential credential = new AzureKeyCredential(_recognizerOptions.ApiKey);
            DocumentAnalysisClient client =                 
                new DocumentAnalysisClient(new Uri(_recognizerOptions.Endpoint), credential);

            var documentType = ("prebuilt-" + PrebuiltModelType.Invoice).ToLower();
            AnalyzeDocumentOperation operation = await client
               .AnalyzeDocumentAsync(WaitUntil.Completed, documentType, file);

            //AnalyzeDocumentOperation operation = await client
            //    .AnalyzeDocumentFromUriAsync(WaitUntil.Completed, "prebuilt-document", fileUri);

            AnalyzeResult result = operation.Value;
            var mappedResult = InvoiceResponse.MapToDto(result);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(mappedResult);
            return response;
        }
    }
}
