using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using EvoPdf.Next;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace PdfFunction;

public class Function
{
    public APIGatewayHttpApiV2ProxyResponse FunctionHandler(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        string url = null;
        request.QueryStringParameters?.TryGetValue("url", out url);

        HtmlToPdfConverter converter = new HtmlToPdfConverter();

        byte[] pdf = string.IsNullOrEmpty(url)
            ? converter.ConvertHtml("<b>Hello World</b> from EvoPdf Next!", null)
            : converter.ConvertUrl(url);

        return new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = 200,
            Body = Convert.ToBase64String(pdf),
            IsBase64Encoded = true,
            Headers = new Dictionary<string, string> { ["Content-Type"] = "application/pdf" }
        };
    }
}
