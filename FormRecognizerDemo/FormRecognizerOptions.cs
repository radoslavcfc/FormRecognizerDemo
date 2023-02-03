public class FormRecognizerOptions
{
    public string Endpoint { get; set; }
    public string ApiKey { get; set; }
    public static string GetName ()
        => nameof(FormRecognizerOptions).Replace("Options", string.Empty);
}