using System;
using System.IO;
using System.Threading.Tasks;
using System.Text;
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
    public class Add_File_Attachments_to_Existing_PDFController : Controller
    {
        private const int leftMargin = 36;
        private const int topMargin = 36;
        private const int contentWidth = 595 - 72;
        private const int contentHeight = 842 - 72;

        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Add_File_Attachments_to_Existing_PDFController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditPdf(Add_File_Attachments_to_Existing_PDF_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the library in demo mode
            Licensing.LicenseKey = "3FJDU0ZDU0NTQkddQ1NAQl1CQV1KSkpKU0M=";

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
                // Otherwise, fall back to the URL
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

            // Open the loaded PDF for editing.  PdfEditor inherits the standard (PDF/A, PDF/UA etc.)
            string password = string.IsNullOrEmpty(model.OwnerPassword) ? model.UserPassword : model.OwnerPassword;
            using PdfEditor pdfEditor = new PdfEditor(inputPdfBytes, password);
            pdfEditor.PdfDocumentInfo.Title = "PDF File Attachments Demo";

            // Open the Attachments panel on document load
            pdfEditor.PdfViewerPreferences.PageMode = ViewerPageMode.UseAttachments;
            // Display the document title as required by PDF/UA-1 and PDF/UA-2
            pdfEditor.PdfViewerPreferences.DisplayDocTitle = true;

            string fontsPath = GetDemoFontsPath();
            string fontFilePath = Path.Combine(fontsPath, "DejaVuSerif.ttf");
            PdfBaseFont baseFont = PdfFontManager.CreateBaseFont(fontFilePath);

            PdfFont titleFont = PdfFontManager.CreateFont(baseFont, 18f,
                PdfFontStyle.Bold | PdfFontStyle.Underline, PdfColor.Black);
            PdfFont sectionFont = PdfFontManager.CreateFont(baseFont, 14f,
                PdfFontStyle.Bold, PdfColor.DarkBlue);
            PdfFont bodyFont = PdfFontManager.CreateFont(baseFont, 11f,
                PdfFontStyle.Normal, PdfColor.Black);
            PdfFont labelFont = PdfFontManager.CreateFont(baseFont, 9f,
                PdfFontStyle.Normal, PdfColor.DarkGray);
            PdfFont iconCaptionFont = PdfFontManager.CreateFont(baseFont, 10f,
                PdfFontStyle.Bold, PdfColor.DarkSlateGray);
            PdfFont codeFont = PdfFontManager.CreateFont(baseFont, 10f,
                PdfFontStyle.Bold, PdfColor.DarkSlateGray);

            const int xLeft = leftMargin;
            const int ySeparator = 10;
            const int pageNumber = 1;
            int currentPage = 1;
            int crtYPos = topMargin;

            // ===== Title =====
            PdfTextElement titleElement = new PdfTextElement(
                "PDF File Attachments Demo", titleFont)
            {
                X = xLeft,
                Y = crtYPos,
                Alignment = PdfTextAlignment.Center,
                Width = contentWidth
            };
            titleElement.Accessibility.StructureType = PdfStructureType.Heading1;
            var titleElementInfo = pdfEditor.AddText(currentPage, titleElement);
            currentPage = titleElementInfo.LastPageRectangle.PageNumber;
            crtYPos = (int)titleElementInfo.LastPageRectangle.Bounds.Bottom + ySeparator * 2;

            // ===== Section 1: Document-level attachments (PdfFileAttachment) =====
            crtYPos = AddSectionLabel(pdfEditor, ref currentPage, sectionFont,
                "1. Document-level attachments (PdfFileAttachment)",
                xLeft, crtYPos, ySeparator);

            crtYPos = AddCaption(pdfEditor, ref currentPage, bodyFont,
                "Document-level attachments appear in the viewer's Attachments panel " +
                "(opened with the paperclip icon in Acrobat's left sidebar). They have " +
                "no visible marker on any page. Two factory methods cover the common cases",
                xLeft, crtYPos, contentWidth) + ySeparator;

            // FromBytes - embed an in-memory XML invoice
            byte[] invoiceXmlBytes = Encoding.UTF8.GetBytes(BuildSampleInvoiceXml());
            var xmlAttachment = PdfFileAttachment.FromBytes(invoiceXmlBytes, "invoice.xml");
            xmlAttachment.MimeType = "application/xml";
            xmlAttachment.Description = "Source XML invoice data";
            xmlAttachment.Relationship = PdfAttachmentRelationship.Source;
            pdfEditor.AddFileAttachment(xmlAttachment);

            crtYPos = AddBulletItem(pdfEditor, ref currentPage, codeFont, bodyFont,
                code: "PdfFileAttachment.FromBytes(invoiceXmlBytes, \"invoice.xml\")",
                description: "Embeds an in-memory byte buffer as an attached file. " +
                             "The PDF remains portable — the data travels with the document",
                x: xLeft, y: crtYPos, width: 540);
            crtYPos += ySeparator;

            // FromFile - read Alphabet.txt from the demo input folder and embed it
            string alphabetFilePath = Path.Combine(GetDemoTextsPath(), "Alphabet.txt");
            var textAttachment = PdfFileAttachment.FromFile(alphabetFilePath);
            textAttachment.MimeType = "text/plain";
            textAttachment.Description = "Sample alphabet text";
            pdfEditor.AddFileAttachment(textAttachment);

            crtYPos = AddBulletItem(pdfEditor, ref currentPage, codeFont, bodyFont,
                code: "PdfFileAttachment.FromFile(\"Alphabet.txt\")",
                description: "Reads a file from disk and embeds its content. " +
                             "The file name shown in the Attachments panel is taken from the path",
                x: xLeft, y: crtYPos, width: 540);
            crtYPos += ySeparator * 2;

            // ===== Section 2: Page-level icon annotations (PdfFileAttachmentAnnotation) =====
            EnsureSpaceOnPage(ref crtYPos, ref currentPage, 180, pdfEditor, contentHeight, topMargin);
            crtYPos = AddSectionLabel(pdfEditor, ref currentPage, sectionFont,
                "2. Page-level icon annotations (PdfFileAttachmentAnnotation)",
                xLeft, crtYPos, ySeparator);

            crtYPos = AddCaption(pdfEditor, ref currentPage, bodyFont,
                "Page-level annotations show a clickable icon on the page and add the file " +
                "to the Attachments panel. Click any icon below to download or open the " +
                "attached file. The four standard PdfAttachmentIcon values are shown side-by-side",
                xLeft, crtYPos, contentWidth) + ySeparator;

            const int iconSize = 24;
            int[] iconXs = { leftMargin + 30, leftMargin + 160, leftMargin + 290, leftMargin + 420 };

            // Paperclip (default) -- attaches a CSV inventory snippet
            byte[] csvBytes = Encoding.UTF8.GetBytes(BuildSampleCsv());
            AddIconSample(pdfEditor, ref currentPage, iconCaptionFont,
                data: csvBytes, fileName: "inventory.csv", mimeType: "text/csv",
                tooltip: "Click to open inventory.csv",
                description: "Inline CSV inventory data",
                icon: PdfAttachmentIcon.Paperclip,
                caption: "Paperclip (default)",
                x: iconXs[0], y: crtYPos, pageNumber: pageNumber, iconSize: iconSize);

            // PushPin -- attaches a small JSON config snippet
            byte[] jsonBytes = Encoding.UTF8.GetBytes(BuildSampleConfigJson());
            AddIconSample(pdfEditor, ref currentPage, iconCaptionFont,
                data: jsonBytes, fileName: "config.json", mimeType: "application/json",
                tooltip: "Click to open config.json",
                description: "Sample configuration values",
                icon: PdfAttachmentIcon.PushPin,
                caption: "PushPin",
                x: iconXs[1], y: crtYPos, pageNumber: pageNumber, iconSize: iconSize);

            // Graph -- attaches a CSV with numeric measurements
            byte[] measurementsBytes = Encoding.UTF8.GetBytes(BuildSampleMeasurementsCsv());
            AddIconSample(pdfEditor, ref currentPage, iconCaptionFont,
                data: measurementsBytes, fileName: "measurements.csv", mimeType: "text/csv",
                tooltip: "Click to open measurements.csv",
                description: "Sample numeric measurements",
                icon: PdfAttachmentIcon.Graph,
                caption: "Graph",
                x: iconXs[2], y: crtYPos, pageNumber: pageNumber, iconSize: iconSize);

            // Tag -- attaches a key/value metadata snippet
            byte[] tagsBytes = Encoding.UTF8.GetBytes(BuildSampleTagsText());
            AddIconSample(pdfEditor, ref currentPage, iconCaptionFont,
                data: tagsBytes, fileName: "tags.txt", mimeType: "text/plain",
                tooltip: "Click to open tags.txt",
                description: "Document tags and metadata",
                icon: PdfAttachmentIcon.Tag,
                caption: "Tag",
                x: iconXs[3], y: crtYPos, pageNumber: pageNumber, iconSize: iconSize);

            crtYPos += iconSize + 25 + ySeparator;

            // ===== Section 3: Embedded file from disk on a page =====
            EnsureSpaceOnPage(ref crtYPos, ref currentPage, 110, pdfEditor, contentHeight, topMargin);
            crtYPos = AddSectionLabel(pdfEditor, ref currentPage, sectionFont,
                "3. Embedded file from disk with PdfFileAttachmentAnnotation.FromFile",
                xLeft, crtYPos, ySeparator);

            crtYPos = AddCaption(pdfEditor, ref currentPage, bodyFont,
                "The FromFile factory reads a file from disk at AddFileAttachmentAnnotation " +
                "time, embeds its content in the PDF and places a clickable icon on the " +
                "specified page",
                xLeft, crtYPos, contentWidth) + ySeparator;

            // Anchor a Paperclip icon next to a body paragraph describing
            // the file.  Clicking the icon opens the embedded Alphabet.txt
            var fileAnnotation = PdfFileAttachmentAnnotation.FromFile(
                alphabetFilePath,
                pageNumber: pageNumber, x: leftMargin + 0, y: crtYPos);
            fileAnnotation.MimeType = "text/plain";
            fileAnnotation.Description = "Embedded alphabet text from disk";
            fileAnnotation.TooltipText = "Click to open Alphabet.txt";
            fileAnnotation.Icon = PdfAttachmentIcon.Paperclip;
            pdfEditor.AddFileAttachmentAnnotation(fileAnnotation);

            // Caption next to the icon explains what the file is.
            PdfTextElement fileLabel = new PdfTextElement(
                "Alphabet.txt — read from disk and embedded in the PDF. " +
                "The file travels with the document, so the receiver does not " +
                "need access to the original location",
                bodyFont)
            {
                X = leftMargin + 35, Y = crtYPos, Width = 500
            };
            var fileLabelInfo = pdfEditor.AddText(currentPage, fileLabel);
            currentPage = fileLabelInfo.LastPageRectangle.PageNumber;
            crtYPos = (int)fileLabelInfo.LastPageRectangle.Bounds.Bottom + ySeparator * 2;

            // ===== Section 4: PDF/A-3 Relationship =====
            EnsureSpaceOnPage(ref crtYPos, ref currentPage, 110, pdfEditor, contentHeight, topMargin);
            crtYPos = AddSectionLabel(pdfEditor, ref currentPage, sectionFont,
                "4. PDF/A-3 attachment relationship",
                xLeft, crtYPos, ySeparator);

            crtYPos = AddCaption(pdfEditor, ref currentPage, bodyFont,
                "PDF/A-3 and PDF/A-4f require an /AFRelationship entry on every embedded " +
                "file, declaring how the attachment relates to the host document. The " +
                "PdfAttachmentRelationship enum covers the five standard values: Source, " +
                "Data, Alternative, Supplement, EncryptedPayload. The invoice.xml from " +
                "section 1 is tagged with Relationship = Source, the typical ZUGFeRD / " +
                "Factur-X e-invoicing pattern. The library ignores the property for " +
                "non-PDF/A-3 standards",
                xLeft, crtYPos, contentWidth) + ySeparator;

            byte[] outPdfBuffer = pdfEditor.Save();
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "PdfFileAttachmentsEditDemo.pdf";
            return fileResult;
        }

        // === Helpers ===

        // Adds a PdfFileAttachmentAnnotation with the requested icon at (x, y)
        // plus a bold caption beneath identifying the icon name.  Used by
        // Section 2 to show all four PdfAttachmentIcon values in a row.
        // iconSize is used only for caption placement -- the annotation
        // itself takes the library's default size 
        private void AddIconSample(PdfEditor editor, ref int currentPage, PdfFont captionFont,
            byte[] data, string fileName, string mimeType,
            string tooltip, string description,
            PdfAttachmentIcon icon, string caption,
            int x, int y, int pageNumber, int iconSize) {
            var ann = PdfFileAttachmentAnnotation.FromBytes(
                data, fileName, pageNumber: pageNumber, x: x, y: y);
            ann.MimeType = mimeType;
            ann.Description = description;
            ann.TooltipText = tooltip;
            ann.Icon = icon;
            editor.AddFileAttachmentAnnotation(ann);

            PdfTextElement label = new PdfTextElement(caption, captionFont)
            {
                X = x - 20, Y = y + iconSize + 4, Width = 110
            };
            editor.AddText(currentPage, label);
        }

        // Adds an indented bullet item showing a code snippet on the first
        // line and a wrapped description underneath.  Used by Section 1 to
        // describe each PdfFileAttachment factory method
        private int AddBulletItem(PdfEditor editor, ref int currentPage, PdfFont codeFont, PdfFont bodyFont,
            string code, string description,
            int x, int y, int width) {
            PdfTextElement codeLine = new PdfTextElement("• " + code, codeFont)
            {
                X = x, Y = y, Width = width
            };
            var codeLineInfo = editor.AddText(currentPage, codeLine);
            currentPage = codeLineInfo.LastPageRectangle.PageNumber;
            int codeBottom = (int)codeLineInfo.LastPageRectangle.Bounds.Bottom;

            PdfTextElement descLine = new PdfTextElement(description, bodyFont)
            {
                X = x + 12, Y = codeBottom + 2, Width = width - 12
            };
            var descLineInfo = editor.AddText(currentPage, descLine);
            currentPage = descLineInfo.LastPageRectangle.PageNumber;
            int descBottom = (int)descLineInfo.LastPageRectangle.Bounds.Bottom;
            return descBottom;
        }

        private int AddSectionLabel(PdfEditor editor, ref int currentPage, PdfFont sectionFont,
            string label, int x, int y, int separator) {
            PdfTextElement section = new PdfTextElement(label, sectionFont)
            { X = x, Y = y };
            section.Accessibility.StructureType = PdfStructureType.Heading2;
            var info = editor.AddText(currentPage, section);
            currentPage = info.LastPageRectangle.PageNumber;
            return (int)info.LastPageRectangle.Bounds.Bottom + separator;
        }

        private int AddCaption(PdfEditor editor, ref int currentPage, PdfFont bodyFont,
            string caption, int x, int y, int width) {
            PdfTextElement t = new PdfTextElement(caption, bodyFont)
            { X = x, Y = y, Width = width };
            t.Accessibility.StructureType = PdfStructureType.Artifact;
            var info = editor.AddText(currentPage, t);
            currentPage = info.LastPageRectangle.PageNumber;
            return (int)info.LastPageRectangle.Bounds.Bottom;
        }

        private void EnsureSpaceOnPage(ref int crtYPos, ref int currentPage, int requestedHeight, PdfEditor pdfEditor, int contentHeight, int topMargin)
        {
            if (crtYPos + requestedHeight > contentHeight + topMargin)
            {
                currentPage = pdfEditor.AddPage();
                crtYPos = topMargin;
            }
        }

        // === Synthetic file content ===

        private static string BuildSampleInvoiceXml()
        {
            return
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                "<Invoice>\n" +
                "  <InvoiceNumber>2026-0042</InvoiceNumber>\n" +
                "  <IssueDate>2026-05-18</IssueDate>\n" +
                "  <Customer>Acme Corporation</Customer>\n" +
                "  <Items>\n" +
                "    <Item><Name>Widget</Name><Quantity>10</Quantity><UnitPrice>1.50</UnitPrice></Item>\n" +
                "    <Item><Name>Gadget</Name><Quantity>5</Quantity><UnitPrice>3.75</UnitPrice></Item>\n" +
                "    <Item><Name>Sprocket</Name><Quantity>2</Quantity><UnitPrice>12.00</UnitPrice></Item>\n" +
                "  </Items>\n" +
                "  <Total>57.75</Total>\n" +
                "</Invoice>\n";
        }

        private static string BuildSampleCsv()
        {
            return
                "Item,Quantity,Price\n" +
                "Apple,10,1.50\n" +
                "Bread,2,3.75\n" +
                "Milk,1,2.99\n" +
                "Coffee,4,8.50\n";
        }

        private static string BuildSampleConfigJson()
        {
            return
                "{\n" +
                "  \"theme\": \"dark\",\n" +
                "  \"pageSize\": \"A4\",\n" +
                "  \"orientation\": \"portrait\",\n" +
                "  \"compression\": true,\n" +
                "  \"language\": \"en-US\"\n" +
                "}\n";
        }

        private static string BuildSampleMeasurementsCsv()
        {
            return
                "Timestamp,Temperature,Humidity\n" +
                "2026-05-18 09:00,21.4,42\n" +
                "2026-05-18 12:00,23.1,38\n" +
                "2026-05-18 15:00,24.6,35\n" +
                "2026-05-18 18:00,22.9,40\n";
        }

        private static string BuildSampleTagsText()
        {
            return
                "category: invoice\n" +
                "department: finance\n" +
                "fiscal-year: 2026\n" +
                "review-status: approved\n";
        }

        private Add_File_Attachments_to_Existing_PDF_ViewModel SetViewModel()
        {
            var model = new Add_File_Attachments_to_Existing_PDF_ViewModel();

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
                0, currentPageUrl.Length - "Add_File_Attachments_to_Existing_PDF".Length);

            // Default input is empty.pdf so this demo edits a fresh blank A4 page
            model.PdfFileUrl = rootUrl + "/DemoAppFiles/Input/PDF_Files/empty.pdf";

            return model;
        }

        private string GetDemoFilesPath() => m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/";
        private string GetDemoFontsPath() => Path.Combine(GetDemoFilesPath(), "Font_Files");
        private string GetDemoTextsPath() => Path.Combine(GetDemoFilesPath(), "Text_Files");
    }
}
