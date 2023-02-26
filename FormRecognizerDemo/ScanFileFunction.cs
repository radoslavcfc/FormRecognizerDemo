using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using FormRecognizerDemo.Core;
using FormRecognizerDemo.Enums;
using FormRecognizerDemo.Models;
using HttpMultipartParser;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Runtime.CompilerServices;

namespace FormRecognizerDemo
{
    public class ScanFileFunction
    {
        private readonly ILogger _logger;
        private readonly FormRecognizerOptions _recognizerOptions;

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
            AzureKeyCredential credential = new AzureKeyCredential(_recognizerOptions.ApiKey);
            DocumentAnalysisClient client =
                new DocumentAnalysisClient(new Uri(_recognizerOptions.Endpoint), credential);

            //Setting up the document type
            var documentType = PrebuiltModelType.Receipt;

            var camelCasedDocType = Helper.FirstCharToLowerCase(documentType.ToString());
            var modelId = $"prebuilt-{camelCasedDocType}";

            var documnetSource = req.Headers.First(name => name.Key == "source").Value.First();

            AnalyzeDocumentOperation operation = default;

            if (documnetSource == SourceType.Url.ToString())
            {
                var documentUri = new Uri(req.Headers.First(name => name.Key == "fileurl").Value.First());
               
                operation = await client
                    .AnalyzeDocumentFromUriAsync(WaitUntil.Completed, modelId, documentUri);
            }

            if (documnetSource == SourceType.File.ToString())
            {
                var parsedFormBody = MultipartFormDataParser.ParseAsync(req.Body);
                var file = parsedFormBody.Result.Files[0].Data;
                operation = await client
                   .AnalyzeDocumentAsync(WaitUntil.Completed, modelId, file);
            }

            //Analyze the results:
            var response = req.CreateResponse(HttpStatusCode.OK);
            AnalyzeResult result = operation.Value;

            var mappedResult = documentType == PrebuiltModelType.Invoice ?
                InvoiceResponse.MapToDto(result) :
                (documentType == PrebuiltModelType.Receipt ?
                    ReceiptResponse.MapToDto(result) :
                    documentType == PrebuiltModelType.BusinessCard ?
                    BusinessCardResponse.MapToDto(result) : new Object());

            await response.WriteAsJsonAsync(mappedResult);
            return response;
        }
    }
}
