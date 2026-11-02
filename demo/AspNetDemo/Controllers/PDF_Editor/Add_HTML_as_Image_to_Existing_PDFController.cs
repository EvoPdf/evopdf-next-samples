using System;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.PDF_Editor;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.PDF_Editor
{
    public class Add_HTML_as_Image_to_Existing_PDFController : Controller
    {
        // The area of the letterhead pages left free by their header and footer
        private const int leftMargin = 36;
        private const int topMargin = 90;
        private const int bottomMargin = 70;
        // The content area of the pages inside the demo margins, from the size of the first page of the loaded document
        private int contentWidth;

        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Add_HTML_as_Image_to_Existing_PDFController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditPdf(Add_HTML_as_Image_to_Existing_PDF_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            byte[] inputPdfBytes = null;

            // If an uploaded file exists, use it with priority
            if (model.PdfFile != null && model.PdfFile.Length > 0)
            {
                try
                {
                    using var ms = new MemoryStream();
                    await model.PdfFile.CopyToAsync(ms);
                    inputPdfBytes = ms.ToArray();
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to read the uploaded PDF file", ex);
                }
            }
            else
            {
                string pdfUrl = model.PdfFileUrl?.Trim();
                if (string.IsNullOrWhiteSpace(pdfUrl))
                    throw new Exception("No PDF file provided: upload a file or specify a URL");

                try
                {
                    if (pdfUrl.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                    {
                        string localPath = new Uri(pdfUrl).LocalPath;
                        inputPdfBytes = await System.IO.File.ReadAllBytesAsync(localPath);
                    }
                    else
                    {
                        using var httpClient = new System.Net.Http.HttpClient();
                        inputPdfBytes = await httpClient.GetByteArrayAsync(pdfUrl);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Could not download the PDF file from URL", ex);
                }
            }

            string password = string.IsNullOrEmpty(model.OwnerPassword) ? model.UserPassword : model.OwnerPassword;
            using PdfEditor pdfEditor = new PdfEditor(inputPdfBytes, password);

            PdfPageInfo firstPageInfo = pdfEditor.GetPdfPageInfo(1);
            // A rotation of 90 or 270 degrees swaps the width and the height of the page as it is displayed
            bool firstPageTurned = firstPageInfo.Rotation % 180 != 0;
            contentWidth = (int)(firstPageTurned ? firstPageInfo.PageSize.Height : firstPageInfo.PageSize.Width) - 2 * leftMargin;

            string fontFilePath = Path.Combine(GetDemoFontsPath(), "DejaVuSerif.ttf");
            PdfBaseFont baseFont = PdfFontManager.CreateBaseFont(fontFilePath);
            PdfFont titleFont = PdfFontManager.CreateFont(baseFont, 16f, PdfFontStyle.Bold, PdfColor.DarkBlue);
            PdfFont smallFont = PdfFontManager.CreateFont(baseFont, 8f, PdfFontStyle.Normal, PdfColor.DarkGray);

            // The title and the HTML image go to the first page of the document, below its header
            int currentPage = 1;
            int crtYPos = topMargin;

            PdfTextElement titleElement = new PdfTextElement("HTML Rendered as an Image", titleFont)
            {
                X = leftMargin,
                Y = crtYPos,
                Alignment = PdfTextAlignment.Center,
                Width = contentWidth
            };
            titleElement.Accessibility.StructureType = PdfStructureType.Heading1;
            var titleInfo = pdfEditor.AddText(currentPage, titleElement);
            currentPage = titleInfo.LastPageRectangle.PageNumber;
            crtYPos = (int)titleInfo.LastPageRectangle.Bounds.Bottom + 15;

            // A page added after the pages of the document: the image continues over it and over the pages before it,
            // or the pages where the image continues are inserted before them and they keep their content, as the
            // option asks
            int pageAfterImage = pdfEditor.AddPage();
            pdfEditor.AddText(pageAfterImage, new PdfTextElement("The page that followed the HTML image", smallFont)
            {
                X = leftMargin,
                Y = topMargin
            });

            // The HTML content rendered as an image, from a URL or from a HTML string with a base URL. The pages
            // of an edited document have no margins, so the continuation margins give the area of the following
            // pages where the image continues
            PdfHtmlImageElement htmlImage = model.HtmlPageSource == "Url"
                ? new PdfHtmlImageElement(model.Url)
                : new PdfHtmlImageElement(model.HtmlStringTextBox ?? string.Empty, model.BaseUrlTextBox);
            htmlImage.X = leftMargin + model.ElementX;
            htmlImage.Y = crtYPos + model.ElementY;
            htmlImage.Width = model.ElementWidth;
            htmlImage.Height = model.ElementHeight;
            htmlImage.Resolution = model.Resolution;
            htmlImage.ContinueOnNextPage = model.ContinueOnNextPage;
            // The pages where the image continues are inserted after the current one, so the pages that follow in
            // the document keep their content; when false, the image continues on the following pages of the document
            htmlImage.InsertContinuationPages = model.InsertContinuationPages;
            htmlImage.ContinuationTopMargin = topMargin;
            htmlImage.ContinuationBottomMargin = bottomMargin;
            htmlImage.Converter.HtmlViewerWidth = model.HtmlViewerWidth;
            htmlImage.Accessibility.AlternateText = "HTML content rendered as an image";

            PdfImageRenderInfo htmlImageInfo = pdfEditor.AddHtmlImage(currentPage, htmlImage);

            var partsText = new System.Text.StringBuilder(
                "The image was drawn on " + htmlImageInfo.RenderedRectangles.Count + " page(s):");
            foreach (PdfRenderedRectangle part in htmlImageInfo.RenderedRectangles)
            {
                partsText.AppendFormat(" page {0} ({1:F0}, {2:F0}, {3:F0}x{4:F0});", part.PageNumber,
                    part.Bounds.X, part.Bounds.Y, part.Bounds.Width, part.Bounds.Height);
            }

            currentPage = htmlImageInfo.LastPage;
            // The part of the image drawn on its last page: the whole image, or the part that fits when it is cut
            PdfRectangleF lastPart = htmlImageInfo.LastPageRectangle.VisibleBounds ?? htmlImageInfo.LastPageRectangle.Bounds;
            PdfTextElement partsLabel = new PdfTextElement(partsText.ToString(), smallFont)
            {
                X = leftMargin,
                Y = (int)lastPart.Bottom + 10,
                Width = contentWidth
            };
            partsLabel.Accessibility.StructureType = PdfStructureType.Artifact;
            pdfEditor.AddText(currentPage, partsLabel);

            byte[] outPdfBuffer = pdfEditor.Save();
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "HTML_as_Image_Edit_Demo.pdf";
            return fileResult;
        }

        private Add_HTML_as_Image_to_Existing_PDF_ViewModel SetViewModel()
        {
            var model = new Add_HTML_as_Image_to_Existing_PDF_ViewModel();

            HttpRequest request = ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder
            {
                Scheme = request.Scheme,
                Host = request.Host.Host,
                Path = request.PathBase.ToString() + request.Path.ToString(),
                Query = request.QueryString.ToString()
            };
            if (request.Host.Port != null)
                uriBuilder.Port = (int)request.Host.Port;

            string currentPageUrl = uriBuilder.Uri.AbsoluteUri;
            string rootUrl = currentPageUrl.Substring(
                0, currentPageUrl.Length - "Add_HTML_as_Image_to_Existing_PDF".Length);

            // A one-page document with a letterhead; the image goes between its header and footer
            model.PdfFileUrl = rootUrl + "DemoAppFiles/Input/PDF_Files/Letterhead.pdf";
            model.HtmlStringTextBox = GetSampleHtml();
            return model;
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
