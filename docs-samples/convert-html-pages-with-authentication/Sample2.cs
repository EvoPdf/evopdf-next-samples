// Source: https://www.evopdf.com/help/evopdf-next-dotnet/html/convert-html-pages-with-authentication.htm
// Documentation page: Convert HTML Pages with Authentication

HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

// Add the authentication cookie to request
htmlToPdfConverter.HttpRequestCookies.Add(AuthCookieName, AuthCookieValue);

htmlToPdfConverter.ConvertUrl(urlToConvert);
