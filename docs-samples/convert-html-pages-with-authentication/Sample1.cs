// Create the HTML to PDF converter
HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();
// Set authentication options
htmlToPdfConverter.AuthenticationOptions.Username = username;
htmlToPdfConverter.AuthenticationOptions.Password = password;            

// Create the HTML to Image converter
HtmlToImageConverter htmlToImageConverter = new HtmlToImageConverter();
// Set authentication options
htmlToImageConverter.AuthenticationOptions.Username = username;
htmlToImageConverter.AuthenticationOptions.Password = password;
