PdfFormOptions formOptions = htmlToPdfConverter.PdfDocumentOptions.PdfFormOptions;
formOptions.FieldNamingMode = PdfFieldNamingMode.PreserveHtmlNamesWhenSafe;
formOptions.ApplyRequiredAsPdfRequiredFlag = true;
formOptions.SubmitBehavior = PdfSubmitBehavior.NativePdfAction;
formOptions.SubmitFormat = PdfSubmitFormat.Html;
formOptions.ValidationBehavior = PdfValidationBehavior.AcrobatJavaScriptBasic;
formOptions.FormFieldFont = PdfFontManager.CreateBaseFont(fontFilePath);
