using System;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.Excel_to_PDF;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.Excel_to_PDF
{
    public class Excel_to_PDFController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Excel_to_PDFController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConvertExcelToPdf(Excel_to_PDF_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the library in demo mode
            Licensing.LicenseKey = "3FJDU0ZDU0NTQkddQ1NAQl1CQV1KSkpKU0M=";

            // Create an Excel to PDF converter object with default settings
            ExcelToPdfConverter excelToPdfConverter = new ExcelToPdfConverter();

            // Set whether to keep the original Excel column widths percentage in the generated PDF document
            excelToPdfConverter.KeepColumnWidths = model.KeepColumnWidths;

            // Set whether to keep the original Excel row heights in the generated PDF document
            excelToPdfConverter.KeepRowHeights = model.KeepRowHeights;

            // Set whether to use page settings (size, margins) from the Excel document or the custom settings
            excelToPdfConverter.PdfDocumentOptions.UsePageSettingsFromExcel = model.PdfPageSettingsMode == "FromExcelDocument";

            // Set whether page settings from the first worksheet or from active worksheet are used for all worksheets
            // Has no effect when UsePageSettingsFromExcel is false
            excelToPdfConverter.PdfDocumentOptions.UseFirstWorksheetPageSettings = model.UseFirstWorksheetPageSettings;

            // Set whether to convert only the first worksheet of the workbook or all worksheets
            excelToPdfConverter.PdfDocumentOptions.ConvertOnlyFirstWorksheet = model.ConvertOnlyFirstWorksheet;

            // Set whether each worksheet starts on a new PDF page
            excelToPdfConverter.PdfDocumentOptions.PageBreakBetweenWorksheets = model.PageBreakBetweenWorksheets;

            // Set whether the worksheet title is displayed at the top of each worksheet in the PDF document
            excelToPdfConverter.PdfDocumentOptions.DisplayWorksheetTitle = model.DisplayWorksheetTitle;

            if (!excelToPdfConverter.PdfDocumentOptions.UsePageSettingsFromExcel)
            {
                // Set PDF page size which can be a predefined size like A4 or a custom size in points 
                // Leave it not set to have a default A4 PDF page
                excelToPdfConverter.PdfDocumentOptions.PdfPageSize = SelectedPdfPageSize(model.PdfPageSize);

                // Set PDF page orientation to Portrait or Landscape
                // Leave it not set to have a default Portrait orientation for PDF page
                excelToPdfConverter.PdfDocumentOptions.PdfPageOrientation = SelectedPdfPageOrientation(model.PdfPageOrientation);

                // Set PDF page margins in points or leave them not set to have a PDF page without margins
                excelToPdfConverter.PdfDocumentOptions.LeftMargin = model.LeftMargin;
                excelToPdfConverter.PdfDocumentOptions.RightMargin = model.RightMargin;
                excelToPdfConverter.PdfDocumentOptions.TopMargin = model.TopMargin;
                excelToPdfConverter.PdfDocumentOptions.BottomMargin = model.BottomMargin;

                // Set the excel viewer zoom percentage
                excelToPdfConverter.PdfDocumentOptions.Zoom = model.ExcelViewerZoom;
            }

            // Set PDF header and footer
            SetHeader(excelToPdfConverter, model);
            SetFooter(excelToPdfConverter, model);

            byte[] inputExcelBytes = null;

            // If an uploaded file exists, use it with priority
            if (model.ExcelFile != null && model.ExcelFile.Length > 0)
            {
                try
                {
                    using var ms = new MemoryStream();
                    await model.ExcelFile.CopyToAsync(ms);
                    inputExcelBytes = ms.ToArray();
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to read the uploaded Excel file", ex);
                }
            }
            else
            {
                // Otherwise, fall back to the URL
                string excelUrl = model.ExcelFileUrl?.Trim();
                if (string.IsNullOrWhiteSpace(excelUrl))
                    throw new Exception("No Excel file provided: upload a file or specify a URL");

                try
                {
                    if (excelUrl.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                    {
                        string localPath = new Uri(excelUrl).LocalPath;
                        inputExcelBytes = await System.IO.File.ReadAllBytesAsync(localPath);
                    }
                    else
                    {
                        using var httpClient = new System.Net.Http.HttpClient();
                        inputExcelBytes = await httpClient.GetByteArrayAsync(excelUrl);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Could not download the Excel file from URL", ex);
                }
            }

            // The buffer to receive the generated PDF document
            byte[] outPdfBuffer = excelToPdfConverter.ConvertToPdf(inputExcelBytes);

            // Send the PDF file to browser
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            // send as attachment
            fileResult.FileDownloadName = "Excel_to_Pdf.pdf";

            return fileResult;
        }

        private void SetHeader(ExcelToPdfConverter excelToPdfConverter, Excel_to_PDF_ViewModel model)
        {
            bool headerEnabled = model.HeaderEnabled;
            if (!headerEnabled)
                return;

            // Set the header HTML from a URL or from an HTML string
            bool headerHtmlFromUrl = model.HeaderHtmlSource == "Url";
            if (headerHtmlFromUrl)
            {
                string headerUrl = model.HeaderUrlTextBox;

                excelToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.HtmlSourceUrl = headerUrl;
            }
            else
            {
                string headerHtml = model.HeaderHtmlTextBox;
                string headerHtmlBaseUrl = model.HeaderHtmlBaseUrlTextBox;

                excelToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.Html = headerHtml;
                excelToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.HtmlBaseUrl = headerHtmlBaseUrl;
            }

            // Enable automatic height adjustment based on header HTML content
            bool autoSizeHeaderContentHeight = model.AutoSizeHeaderContentHeight;
            excelToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.AutoSizeContentHeight = autoSizeHeaderContentHeight;

            // Set the minimum and maximum content height used when AutoSizeContentHeight is enabled
            excelToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.MinContentHeight = model.HeaderMinContentHeight;
            excelToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.MaxContentHeight = model.HeaderMaxContentHeight;

            // Set a fixed height for the header if AutoResizeHeight is disabled
            if (model.HeaderHeight.HasValue)
            {
                int headerHeight = model.HeaderHeight.Value;
                excelToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.Height = headerHeight;
            }

            // If AutoResizeHeight is enabled and both Height and FitHeight are set,
            // the content may be scaled down to fit the specified height
            bool fitHeaderHeight = model.FitHeaderHeight;
            excelToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.FitHeight = fitHeaderHeight;

            // Enable automatic top margin adjustment in the PDF based on the header
            bool autoResizeTopMargin = model.AutoResizeTopMargin;
            excelToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.AutoResizePdfMargins = autoResizeTopMargin;

            // Set header visibility on specific PDF pages: first page, odd-numbered pages and even-numbered pages
            excelToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.ShowInFirstPage = model.ShowHeaderInFirstPage;
            excelToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.ShowInOddPages = model.ShowHeaderInOddPages;
            excelToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.ShowInEvenPages = model.ShowHeaderInEvenPages;

            // Reserve space for the header on all pages, regardless of visibility
            // If false, the document will be rendered using print styles instead of screen styles
            excelToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.ReserveSpaceAlways = model.ReserveHeaderSpace;

            // Optimize the header rendering time by providing a hint if the HTML template contains variables such as { page_number} or { total_pages}
            excelToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.SkipVariablesParsing = model.SkipHeaderVariablesParsing;

            // Optionally set additional time to wait for the asynchronous header HTML content before rendering
            if (model.HeaderConversionDelay.HasValue)
                excelToPdfConverter.PdfDocumentOptions.PdfHtmlHeader.ConversionDelay = model.HeaderConversionDelay.Value;
        }

        private void SetFooter(ExcelToPdfConverter excelToPdfConverter, Excel_to_PDF_ViewModel model)
        {
            bool footerEnabled = model.FooterEnabled;
            if (footerEnabled)
            {
                // Set the footer HTML from a URL or from an HTML string
                bool footerHtmlFromUrl = model.FooterHtmlSource == "Url";
                if (footerHtmlFromUrl)
                {
                    string footerUrl = model.FooterUrlTextBox;

                    excelToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.HtmlSourceUrl = footerUrl;
                }
                else
                {
                    string footerHtml = model.FooterHtmlTextBox;
                    string footerHtmlBaseUrl = model.FooterHtmlBaseUrlTextBox;

                    excelToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.Html = footerHtml;
                    excelToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.HtmlBaseUrl = footerHtmlBaseUrl;
                }

                // Enable automatic height adjustment based on footer HTML content
                bool autoSizeFooterContentHeight = model.AutoSizeFooterContentHeight;
                excelToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.AutoSizeContentHeight = autoSizeFooterContentHeight;

                // Set the minimum and maximum content height used when AutoSizeContentHeight is enabled
                excelToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.MinContentHeight = model.FooterMinContentHeight;
                excelToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.MaxContentHeight = model.FooterMaxContentHeight;

                // Set a fixed height for the footer if AutoResizeHeight is disabled
                if (model.FooterHeight.HasValue)
                {
                    int footerHeight = model.FooterHeight.Value;
                    excelToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.Height = footerHeight;
                }

                // If AutoResizeHeight is enabled and both Height and FitHeight are set,
                // the content may be scaled down to fit the specified height
                bool fitFooterHeight = model.FitFooterHeight;
                excelToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.FitHeight = fitFooterHeight;

                // Enable automatic bottom margin adjustment in the PDF based on the footer
                bool autoResizeBottomMargin = model.AutoResizeBottomMargin;
                excelToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.AutoResizePdfMargins = autoResizeBottomMargin;

                // Set footer visibility on specific PDF pages: first page, odd-numbered pages and even-numbered pages
                excelToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.ShowInFirstPage = model.ShowFooterInFirstPage;
                excelToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.ShowInOddPages = model.ShowFooterInOddPages;
                excelToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.ShowInEvenPages = model.ShowFooterInEvenPages;

                // Reserve space for the footer on all pages, regardless of visibility
                // If false, the document will be rendered using print styles instead of screen styles
                excelToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.ReserveSpaceAlways = model.ReserveFooterSpace;

                // Optimize the footer rendering time by providing a hint if the HTML template contains variables such as { page_number} or { total_pages}
                excelToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.SkipVariablesParsing = model.SkipFooterVariablesParsing;

                // Optionally set additional time to wait for the asynchronous footer HTML content before rendering
                if (model.FooterConversionDelay.HasValue)
                    excelToPdfConverter.PdfDocumentOptions.PdfHtmlFooter.ConversionDelay = model.FooterConversionDelay.Value;
            }
        }

        private PdfPageSize SelectedPdfPageSize(string selectedValue)
        {
            switch (selectedValue)
            {
                case "A0":
                    return PdfPageSize.A0;
                case "A1":
                    return PdfPageSize.A1;
                case "A10":
                    return PdfPageSize.A10;
                case "A2":
                    return PdfPageSize.A2;
                case "A3":
                    return PdfPageSize.A3;
                case "A4":
                    return PdfPageSize.A4;
                case "A5":
                    return PdfPageSize.A5;
                case "A6":
                    return PdfPageSize.A6;
                case "A7":
                    return PdfPageSize.A7;
                case "A8":
                    return PdfPageSize.A8;
                case "A9":
                    return PdfPageSize.A9;
                case "ArchA":
                    return PdfPageSize.ArchA;
                case "ArchB":
                    return PdfPageSize.ArchB;
                case "ArchC":
                    return PdfPageSize.ArchC;
                case "ArchD":
                    return PdfPageSize.ArchD;
                case "ArchE":
                    return PdfPageSize.ArchE;
                case "B0":
                    return PdfPageSize.B0;
                case "B1":
                    return PdfPageSize.B1;
                case "B2":
                    return PdfPageSize.B2;
                case "B3":
                    return PdfPageSize.B3;
                case "B4":
                    return PdfPageSize.B4;
                case "B5":
                    return PdfPageSize.B5;
                case "Flsa":
                    return PdfPageSize.Flsa;
                case "HalfLetter":
                    return PdfPageSize.HalfLetter;
                case "Ledger":
                    return PdfPageSize.Ledger;
                case "Legal":
                    return PdfPageSize.Legal;
                case "Letter":
                    return PdfPageSize.Letter;
                case "Letter11x17":
                    return PdfPageSize.Letter11x17;
                case "Note":
                    return PdfPageSize.Note;
                default:
                    return PdfPageSize.A4;
            }
        }

        private PdfPageOrientation SelectedPdfPageOrientation(string selectedValue)
        {
            return selectedValue == "Portrait" ? PdfPageOrientation.Portrait : PdfPageOrientation.Landscape;
        }

        private Excel_to_PDF_ViewModel SetViewModel()
        {
            var model = new Excel_to_PDF_ViewModel();

            var contentRootPath = System.IO.Path.Combine(m_hostingEnvironment.ContentRootPath, "wwwroot");

            HttpRequest request = ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            if (request.Host.Port != null)
                uriBuilder.Port = (int)request.Host.Port;
            uriBuilder.Path = request.PathBase.ToString() + request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();

            string currentPageUrl = uriBuilder.Uri.AbsoluteUri;
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Excel_To_PDF".Length);

            model.ExcelFileUrl = rootUrl + "/DemoAppFiles/Input/Excel_Files/Excel_Document.xlsx";
            model.HeaderHtmlTextBox = System.IO.File.ReadAllText(System.IO.Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/Header_HTML.html"));
            model.FooterHtmlTextBox = System.IO.File.ReadAllText(System.IO.Path.Combine(contentRootPath, "DemoAppFiles/Input/HTML_Files/Footer_HTML.html"));
            model.HeaderHtmlBaseUrlTextBox = rootUrl + "DemoAppFiles/Input/HTML_Files/";
            model.HeaderUrlTextBox = rootUrl + "DemoAppFiles/Input/HTML_Files/Header_HTML.html";
            model.FooterHtmlBaseUrlTextBox = rootUrl + "DemoAppFiles/Input/HTML_Files/";
            model.FooterUrlTextBox = rootUrl + "DemoAppFiles/Input/HTML_Files/Footer_HTML.html";

            return model;
        }
    }
}
