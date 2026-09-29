HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

// Add the authentication cookie to request
htmlToPdfConverter.HttpRequestCookies.Add(AuthCookieName, AuthCookieValue);

htmlToPdfConverter.ConvertUrl(urlToConvert);
