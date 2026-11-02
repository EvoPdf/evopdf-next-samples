using System;
using System.IO;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.HTML_to_PDF
{
    public class Retrieve_Conversion_InfoController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;
        public Retrieve_Conversion_InfoController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public ActionResult Index()
        {
            var model = SetViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult ConvertHtmlToPdf(Retrieve_Conversion_Info_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            // Create a HTML to PDF converter object with default settings
            HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

            // The pages of the table of contents are counted separately in the conversion information
            htmlToPdfConverter.PdfDocumentOptions.GenerateTableOfContents = model.GenerateTableOfContents;

            if (model.AddStartPdf)
            {
                // The pages of a PDF document added before the HTML content
                htmlToPdfConverter.PdfDocumentOptions.AddStartPdf(Path.Combine(GetDemoFilesPath(), "PDF_Files", "Letterhead.pdf"));
            }

            if (model.AddEndPdf)
            {
                // The pages of a PDF document added after the HTML content
                htmlToPdfConverter.PdfDocumentOptions.AddEndPdf(Path.Combine(GetDemoFilesPath(), "PDF_Files", "PDF_Form.pdf"));
            }

            // The buffer to receive the generated PDF document
            byte[] outPdfBuffer = null;

            if (model.HtmlPageSource == "Url")
            {
                string url = model.Url;

                // Convert the HTML page given by an URL to a PDF document in a memory buffer
                outPdfBuffer = htmlToPdfConverter.ConvertUrl(url);
            }
            else
            {
                string htmlString = model.HtmlStringTextBox;
                string baseUrl = model.BaseUrlTextBox;

                // Convert a HTML string with a base URL to a PDF document in a memory buffer
                outPdfBuffer = htmlToPdfConverter.ConvertHtml(htmlString, baseUrl);
            }

            // The information is available after the conversion; it is written in a page of the generated document
            HtmlToPdfConversionInfo conversionInfo = htmlToPdfConverter.ConversionInfo;

            StringBuilder infoText = new StringBuilder();
            infoText.Append("Pages generated from the HTML content: ").Append(conversionInfo.PagesFromHtml).Append('\n');
            infoText.Append("Pages of the table of contents inserted before the HTML content: ").Append(conversionInfo.TocPagesInsertedBefore).Append('\n');
            infoText.Append("Pages of external PDF documents inserted before the HTML content: ").Append(conversionInfo.ExternalPagesInsertedBefore).Append('\n');
            infoText.Append("Total pages inserted before the HTML content: ").Append(conversionInfo.TotalPagesInsertedBefore).Append('\n');
            infoText.Append("Pages inserted after the HTML content: ").Append(conversionInfo.ExternalPagesInsertedAfter).Append('\n');
            infoText.Append("Total pages of the generated document: ").Append(conversionInfo.TotalPages).Append('\n');
            infoText.Append("Zoom at which the HTML content was drawn: ").Append(conversionInfo.PrintZoom.ToString("0.##")).Append('%').Append('\n');

            if (model.InfoPagePosition != "None")
            {
                using PdfEditor pdfEditor = new PdfEditor(outPdfBuffer, null);

                int infoPageNumber = model.InfoPagePosition == "Start"
                    ? pdfEditor.InsertPage(1, PdfPageSize.A4, PdfPageOrientation.Portrait)
                    : pdfEditor.AddPage(PdfPageSize.A4, PdfPageOrientation.Portrait);

                PdfBaseFont baseFont = PdfFontManager.CreateBaseFont(Path.Combine(GetDemoFontsPath(), "DejaVuSerif.ttf"));
                PdfFont titleFont = PdfFontManager.CreateFont(baseFont, 14f, PdfFontStyle.Bold, PdfColor.DarkBlue);
                PdfFont textFont = PdfFontManager.CreateFont(baseFont, 10f, PdfFontStyle.Normal, PdfColor.Black);

                pdfEditor.AddText(infoPageNumber, new PdfTextElement("HTML to PDF Conversion Information", titleFont) { X = 40, Y = 40 });
                pdfEditor.AddText(infoPageNumber, new PdfTextElement(infoText.ToString(), textFont) { X = 40, Y = 70, Width = 515 });
                pdfEditor.AddText(infoPageNumber, new PdfTextElement("This page was added to the generated document after the conversion and is not counted above.", textFont)
                {
                    X = 40,
                    Y = 200,
                    Width = 515
                });

                outPdfBuffer = pdfEditor.Save();
            }

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "Retrieve_Conversion_Info.pdf";

            return fileResult;
        }

        private Retrieve_Conversion_Info_ViewModel SetViewModel()
        {
            var model = new Retrieve_Conversion_Info_ViewModel();

            var contentRootPath = Path.Combine(m_hostingEnvironment.ContentRootPath, "wwwroot");

            HttpRequest request = ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            if (request.Host.Port != null)
                uriBuilder.Port = (int)request.Host.Port;
            uriBuilder.Path = request.PathBase.ToString() + request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();

            string currentPageUrl = uriBuilder.Uri.AbsoluteUri;
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Retrieve_Conversion_Info".Length);

            model.HtmlStringTextBox = System.IO.File.ReadAllText(Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/Table_of_Contents.html"));
            model.BaseUrlTextBox = rootUrl + "DemoAppFiles/Input/HTML_Files/";

            return model;
        }

        private string GetDemoFilesPath() => m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/";
        private string GetDemoFontsPath() => Path.Combine(GetDemoFilesPath(), "Font_Files");
    }
}
