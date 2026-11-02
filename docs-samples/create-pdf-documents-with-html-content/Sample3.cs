pdfDocument.TableOfContents.Enabled = true;
pdfDocument.TableOfContents.Title = "Contents";
pdfDocument.AddHtml(new PdfHtmlElement(firstHtml, null));
pdfDocument.AddPage();
pdfDocument.AddHtml(new PdfHtmlElement(secondHtml, null));
byte[] pdf = pdfDocument.Save();
