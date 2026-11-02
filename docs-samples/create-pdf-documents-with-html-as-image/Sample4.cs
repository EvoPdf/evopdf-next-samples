using System.IO;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.PDF_Creator;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.PDF_Creator
{
    public class Create_PDF_Documents_with_HTML_as_ImageController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Create_PDF_Documents_with_HTML_as_ImageController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = new Create_PDF_Documents_with_HTML_as_Image_ViewModel();
            model.HtmlStringTextBox = GetSampleHtml();
            return View(model);
        }

        [HttpPost]
        public ActionResult CreatePdf(Create_PDF_Documents_with_HTML_as_Image_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            PdfDocumentCreateSettings pdfCreateSettings = new PdfDocumentCreateSettings()
            {
                PageSize = PdfPageSize.A4,
                PageOrientation = PdfPageOrientation.Portrait,
                Margins = new PdfMargins(36, 36, 36, 36),
                PdfStandard = model.PdfStandard,
                Language = "en-US"
            };

            using PdfDocument pdfDocument = new PdfDocument(pdfCreateSettings);
            pdfDocument.PdfDocumentInfo.Title = "HTML Rendered as an Image in a PDF Document";

            string fontFilePath = Path.Combine(GetDemoFontsPath(), "DejaVuSerif.ttf");
            PdfBaseFont baseFont = PdfFontManager.CreateBaseFont(fontFilePath);
            PdfFont titleFont = PdfFontManager.CreateFont(baseFont, 16f, PdfFontStyle.Bold, PdfColor.DarkBlue);
            PdfFont textFont = PdfFontManager.CreateFont(baseFont, 10f, PdfFontStyle.Normal, PdfColor.Black);
            PdfFont smallFont = PdfFontManager.CreateFont(baseFont, 8f, PdfFontStyle.Normal, PdfColor.DarkGray);

            PdfTextElement titleElement = new PdfTextElement("HTML Rendered as an Image in a PDF Document", titleFont)
            {
                X = 0,
                Y = 0,
                Width = pdfDocument.ContentWidth,
                Alignment = PdfTextAlignment.Center
            };
            titleElement.Accessibility.StructureType = PdfStructureType.Heading1;
            float crtYPos = pdfDocument.AddText(titleElement).LastPageRectangle.Bounds.Bottom + 10;

            PdfTextElement introElement = new PdfTextElement(
                "The HTML content below is rendered as an image at " + model.Resolution + " DPI, at the size the content has on " +
                "screen or at the size given to the element, and continues on the next pages when it is taller than the space left on the page.",
                textFont)
            {
                X = 0,
                Y = crtYPos,
                Width = pdfDocument.ContentWidth
            };
            crtYPos = pdfDocument.AddText(introElement).LastPageRectangle.Bounds.Bottom + 15;

            // The HTML content rendered as an image, from a URL or from a HTML string with a base URL. The image
            // starts at the given position in the current page, inside the margins, and the part that does not fit
            // above the bottom margin continues on the next pages from the top margin
            PdfHtmlImageElement htmlImage = model.HtmlPageSource == "Url"
                ? new PdfHtmlImageElement(model.Url)
                : new PdfHtmlImageElement(model.HtmlStringTextBox ?? string.Empty, model.BaseUrlTextBox);
            htmlImage.X = model.ElementX;
            // The image starts at the given distance below the text above it
            htmlImage.Y = crtYPos + model.ElementY;
            // 0 draws the image at the size of the content on screen, scaled down to the right margin when it is wider
            htmlImage.Width = model.ElementWidth;
            // 0 gives no height limit; a taller image is scaled down to this height, keeping its aspect ratio
            htmlImage.Height = model.ElementHeight;
            // The resolution of the image: the size in the page is the same at any resolution, a higher one gives a
            // sharper image
            htmlImage.Resolution = model.Resolution;
            // When false, the part of the image outside the page is cut
            htmlImage.ContinueOnNextPage = model.ContinueOnNextPage;
            // The width of the browser window where the HTML is laid out; the image has the width of the content
            htmlImage.Converter.HtmlViewerWidth = model.HtmlViewerWidth;
            htmlImage.Accessibility.AlternateText = "HTML content rendered as an image";

            PdfImageRenderInfo htmlImageInfo = pdfDocument.AddHtmlImage(htmlImage);

            // The rectangle of each part of the image, one for each page; the elements added after the image go
            // below the part drawn on its last page
            var partsText = new System.Text.StringBuilder(
                "The image was drawn on " + htmlImageInfo.RenderedRectangles.Count + " page(s):");
            foreach (PdfRenderedRectangle part in htmlImageInfo.RenderedRectangles)
            {
                partsText.AppendFormat(" page {0} ({1:F0}, {2:F0}, {3:F0}x{4:F0});", part.PageNumber,
                    part.Bounds.X, part.Bounds.Y, part.Bounds.Width, part.Bounds.Height);
            }

            // The part of the image drawn on its last page: the whole image, or the part that fits when it is cut
            PdfRectangleF lastPart = htmlImageInfo.LastPageRectangle.VisibleBounds ?? htmlImageInfo.LastPageRectangle.Bounds;
            PdfTextElement partsElement = new PdfTextElement(partsText.ToString(), smallFont)
            {
                X = 0,
                Y = lastPart.Bottom + 10,
                Width = pdfDocument.ContentWidth
            };
            pdfDocument.AddText(partsElement);

            byte[] outPdfBuffer = pdfDocument.Save();

            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "HTML_as_Image_in_Pdf_Document.pdf";
            return fileResult;
        }

        // A styled table of 40 rows, taller than a page at the size it has on screen
        private static string GetSampleHtml()
        {
            var html = new System.Text.StringBuilder();
            html.Append("<html>\n<head>\n<style>\n" +
                "body { margin: 0; font-family: Arial, sans-serif; font-size: 15px; color: #1f2937; }\n" +
                ".sheet { width: 660px; padding: 14px; background: linear-gradient(180deg, #f8fafc, #ffffff); " +
                "border: 1px solid #cbd5e1; border-radius: 8px; }\n" +
                "h2 { margin: 0 0 10px 0; font-size: 22px; color: #1e3a8a; }\n" +
                "table { width: 100%; border-collapse: collapse; }\n" +
                "th { background: #1e3a8a; color: #ffffff; padding: 7px 10px; text-align: left; }\n" +
                "td { padding: 6px 10px; border-bottom: 1px solid #e2e8f0; }\n" +
                "tr:nth-child(even) td { background: #eff6ff; }\n" +
                ".num { text-align: right; font-variant-numeric: tabular-nums; }\n" +
                "</style>\n</head>\n<body>\n<div class=\"sheet\">\n<h2>Monthly Sales by Region</h2>\n<table>\n" +
                "<tr><th>Month</th><th>Region</th><th class=\"num\">Units</th><th class=\"num\">Revenue</th></tr>\n");

            string[] months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            string[] regions = { "North", "South", "East", "West" };
            int row = 0;
            foreach (string month in months)
            {
                foreach (string region in regions)
                {
                    int units = 120 + (row * 37) % 95;
                    html.AppendFormat("<tr><td>{0}</td><td>{1}</td><td class=\"num\">{2}</td><td class=\"num\">{3:N2}</td></tr>\n",
                        month, region, units, units * 24.5);
                    row++;
                }
            }

            html.Append("</table>\n</div>\n</body>\n</html>");
            return html.ToString();
        }

        private string GetDemoFilesPath() => m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/";
        private string GetDemoFontsPath() => Path.Combine(GetDemoFilesPath(), "Font_Files");
    }
}
