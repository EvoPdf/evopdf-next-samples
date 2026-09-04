using System.IO;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using EvoPdf_Next_AspNetDemo.Models;
using EvoPdf_Next_AspNetDemo.Models.PDF_Creator;

// Use EVO PDF Namespace
using EvoPdf.Next;

namespace EvoPdf_Next_AspNetDemo.Controllers.PDF_Creator
{
    public class Create_PDF_Documents_with_File_AttachmentsController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Create_PDF_Documents_with_File_AttachmentsController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = new Create_PDF_Documents_with_File_Attachments_ViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult CreatePdf(Create_PDF_Documents_with_File_Attachments_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            Licensing.LicenseKey = "3FJDU0ZDU0NTQkddQ1NAQl1CQV1KSkpKU0M=";

            PdfDocumentCreateSettings pdfCreateSettings = new PdfDocumentCreateSettings()
            {
                PageSize = PdfPageSize.A4,
                PageOrientation = PdfPageOrientation.Portrait,
                Margins = new PdfMargins(36, 36, 36, 36),
                PdfStandard = model.PdfStandard,
                Language = "en-US"
            };

            using PdfDocument pdfDocument = new PdfDocument(pdfCreateSettings);
            pdfDocument.PdfDocumentInfo.Title = "PDF File Attachments Demo";

            // Open the Attachments panel on document load
            pdfDocument.PdfViewerPreferences.PageMode = ViewerPageMode.UseAttachments;
            // Display the document title as required by PDF/UA-1 and PDF/UA-2
            pdfDocument.PdfViewerPreferences.DisplayDocTitle = true;

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

            const int xLeft = 0;
            const int ySeparator = 10;
            const int pageNumber = 1;
            int crtYPos = 0;

            // ===== Title =====
            PdfTextElement titleElement = new PdfTextElement(
                "PDF File Attachments Demo", titleFont)
            {
                X = xLeft,
                Y = crtYPos,
                Alignment = PdfTextAlignment.Center,
                Width = pdfDocument.ContentWidth
            };
            titleElement.Accessibility.StructureType = PdfStructureType.Heading1;
            crtYPos = (int)pdfDocument.AddText(titleElement).LastPageRectangle.Bounds.Bottom + ySeparator * 2;

            // ===== Section 1: Document-level attachments (PdfFileAttachment) =====
            crtYPos = AddSectionLabel(pdfDocument, sectionFont,
                "1. Document-level attachments (PdfFileAttachment)",
                xLeft, crtYPos, ySeparator);

            crtYPos = AddCaption(pdfDocument, bodyFont,
                "Document-level attachments appear in the viewer's Attachments panel " +
                "(opened with the paperclip icon in Acrobat's left sidebar). They have " +
                "no visible marker on any page. Two factory methods cover the common cases",
                xLeft, crtYPos, 540) + ySeparator;

            // FromBytes - embed an in-memory XML invoice
            byte[] invoiceXmlBytes = Encoding.UTF8.GetBytes(BuildSampleInvoiceXml());
            var xmlAttachment = PdfFileAttachment.FromBytes(invoiceXmlBytes, "invoice.xml");
            xmlAttachment.MimeType = "application/xml";
            xmlAttachment.Description = "Source XML invoice data";
            xmlAttachment.Relationship = PdfAttachmentRelationship.Source;
            pdfDocument.AddFileAttachment(xmlAttachment);

            crtYPos = AddBulletItem(pdfDocument, codeFont, bodyFont,
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
            pdfDocument.AddFileAttachment(textAttachment);

            crtYPos = AddBulletItem(pdfDocument, codeFont, bodyFont,
                code: "PdfFileAttachment.FromFile(\"Alphabet.txt\")",
                description: "Reads a file from disk and embeds its content. " +
                             "The file name shown in the Attachments panel is taken from the path",
                x: xLeft, y: crtYPos, width: 540);
            crtYPos += ySeparator * 2;

            // ===== Section 2: Page-level icon annotations (PdfFileAttachmentAnnotation) =====
            EnsureSpaceOnPage(ref crtYPos, 180, pdfDocument);
            crtYPos = AddSectionLabel(pdfDocument, sectionFont,
                "2. Page-level icon annotations (PdfFileAttachmentAnnotation)",
                xLeft, crtYPos, ySeparator);

            crtYPos = AddCaption(pdfDocument, bodyFont,
                "Page-level annotations show a clickable icon on the page and add the file " +
                "to the Attachments panel. Click any icon below to download or open the " +
                "attached file. The four standard PdfAttachmentIcon values are shown side-by-side",
                xLeft, crtYPos, 540) + ySeparator;

            const int iconSize = 24;
            int[] iconXs = { 30, 160, 290, 420 };

            // Paperclip (default) -- attaches a CSV inventory snippet
            byte[] csvBytes = Encoding.UTF8.GetBytes(BuildSampleCsv());
            AddIconSample(pdfDocument, iconCaptionFont,
                data: csvBytes, fileName: "inventory.csv", mimeType: "text/csv",
                tooltip: "Click to open inventory.csv",
                description: "Inline CSV inventory data",
                icon: PdfAttachmentIcon.Paperclip,
                caption: "Paperclip (default)",
                x: iconXs[0], y: crtYPos, pageNumber: pageNumber, iconSize: iconSize);

            // PushPin -- attaches a small JSON config snippet
            byte[] jsonBytes = Encoding.UTF8.GetBytes(BuildSampleConfigJson());
            AddIconSample(pdfDocument, iconCaptionFont,
                data: jsonBytes, fileName: "config.json", mimeType: "application/json",
                tooltip: "Click to open config.json",
                description: "Sample configuration values",
                icon: PdfAttachmentIcon.PushPin,
                caption: "PushPin",
                x: iconXs[1], y: crtYPos, pageNumber: pageNumber, iconSize: iconSize);

            // Graph -- attaches a CSV with numeric measurements
            byte[] measurementsBytes = Encoding.UTF8.GetBytes(BuildSampleMeasurementsCsv());
            AddIconSample(pdfDocument, iconCaptionFont,
                data: measurementsBytes, fileName: "measurements.csv", mimeType: "text/csv",
                tooltip: "Click to open measurements.csv",
                description: "Sample numeric measurements",
                icon: PdfAttachmentIcon.Graph,
                caption: "Graph",
                x: iconXs[2], y: crtYPos, pageNumber: pageNumber, iconSize: iconSize);

            // Tag -- attaches a key/value metadata snippet
            byte[] tagsBytes = Encoding.UTF8.GetBytes(BuildSampleTagsText());
            AddIconSample(pdfDocument, iconCaptionFont,
                data: tagsBytes, fileName: "tags.txt", mimeType: "text/plain",
                tooltip: "Click to open tags.txt",
                description: "Document tags and metadata",
                icon: PdfAttachmentIcon.Tag,
                caption: "Tag",
                x: iconXs[3], y: crtYPos, pageNumber: pageNumber, iconSize: iconSize);

            crtYPos += iconSize + 25 + ySeparator;

            // ===== Section 3: Embedded file from disk on a page =====
            EnsureSpaceOnPage(ref crtYPos, 110, pdfDocument);
            crtYPos = AddSectionLabel(pdfDocument, sectionFont,
                "3. Embedded file from disk with PdfFileAttachmentAnnotation.FromFile",
                xLeft, crtYPos, ySeparator);

            crtYPos = AddCaption(pdfDocument, bodyFont,
                "The FromFile factory reads a file from disk at AddFileAttachmentAnnotation " +
                "time, embeds its content in the PDF and places a clickable icon on the " +
                "specified page",
                xLeft, crtYPos, 540) + ySeparator;

            // Anchor a Paperclip icon next to a body paragraph describing
            // the file.  Clicking the icon opens the embedded Alphabet.txt
            var fileAnnotation = PdfFileAttachmentAnnotation.FromFile(
                alphabetFilePath,
                pageNumber: pageNumber, x: 0, y: crtYPos);
            fileAnnotation.MimeType = "text/plain";
            fileAnnotation.Description = "Embedded alphabet text from disk";
            fileAnnotation.TooltipText = "Click to open Alphabet.txt";
            fileAnnotation.Icon = PdfAttachmentIcon.Paperclip;
            pdfDocument.AddFileAttachmentAnnotation(fileAnnotation);

            // Caption next to the icon explains what the file is.
            PdfTextElement fileLabel = new PdfTextElement(
                "Alphabet.txt — read from disk and embedded in the PDF. " +
                "The file travels with the document, so the receiver does not " +
                "need access to the original location",
                bodyFont)
            {
                X = 35, Y = crtYPos, Width = 500
            };
            var fileLabelInfo = pdfDocument.AddText(fileLabel);
            crtYPos = (int)fileLabelInfo.LastPageRectangle.Bounds.Bottom + ySeparator * 2;

            // ===== Section 4: PDF/A-3 Relationship =====
            EnsureSpaceOnPage(ref crtYPos, 110, pdfDocument);
            crtYPos = AddSectionLabel(pdfDocument, sectionFont,
                "4. PDF/A-3 attachment relationship",
                xLeft, crtYPos, ySeparator);

            crtYPos = AddCaption(pdfDocument, bodyFont,
                "PDF/A-3 and PDF/A-4f require an /AFRelationship entry on every embedded " +
                "file, declaring how the attachment relates to the host document. The " +
                "PdfAttachmentRelationship enum covers the five standard values: Source, " +
                "Data, Alternative, Supplement, EncryptedPayload. The invoice.xml from " +
                "section 1 is tagged with Relationship = Source, the typical ZUGFeRD / " +
                "Factur-X e-invoicing pattern. The library ignores the property for " +
                "non-PDF/A-3 standards",
                xLeft, crtYPos, 540) + ySeparator;
            
            byte[] outPdfBuffer = pdfDocument.Save();
            FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "PdfFileAttachmentsDemo.pdf";
            return fileResult;
        }

        // === Helpers ===

        // Adds a PdfFileAttachmentAnnotation with the requested icon at (x, y)
        // plus a bold caption beneath identifying the icon name.  Used by
        // Section 2 to show all four PdfAttachmentIcon values in a row.
        // iconSize is used only for caption placement -- the annotation
        // itself takes the library's default size 
        private void AddIconSample(
            PdfDocument doc, PdfFont captionFont,
            byte[] data, string fileName, string mimeType,
            string tooltip, string description,
            PdfAttachmentIcon icon, string caption,
            int x, int y, int pageNumber, int iconSize)
        {
            var ann = PdfFileAttachmentAnnotation.FromBytes(
                data, fileName, pageNumber: pageNumber, x: x, y: y);
            ann.MimeType = mimeType;
            ann.Description = description;
            ann.TooltipText = tooltip;
            ann.Icon = icon;
            doc.AddFileAttachmentAnnotation(ann);

            PdfTextElement label = new PdfTextElement(caption, captionFont)
            {
                X = x - 20, Y = y + iconSize + 4, Width = 110
            };
            doc.AddText(label);
        }

        // Adds an indented bullet item showing a code snippet on the first
        // line and a wrapped description underneath.  Used by Section 1 to
        // describe each PdfFileAttachment factory method
        private int AddBulletItem(
            PdfDocument doc, PdfFont codeFont, PdfFont bodyFont,
            string code, string description,
            int x, int y, int width)
        {
            PdfTextElement codeLine = new PdfTextElement("• " + code, codeFont)
            {
                X = x, Y = y, Width = width
            };
            int codeBottom = (int)doc.AddText(codeLine).LastPageRectangle.Bounds.Bottom;

            PdfTextElement descLine = new PdfTextElement(description, bodyFont)
            {
                X = x + 12, Y = codeBottom + 2, Width = width - 12
            };
            int descBottom = (int)doc.AddText(descLine).LastPageRectangle.Bounds.Bottom;
            return descBottom;
        }

        private int AddSectionLabel(PdfDocument doc, PdfFont sectionFont,
            string label, int x, int y, int separator)
        {
            PdfTextElement section = new PdfTextElement(label, sectionFont)
            { X = x, Y = y };
            section.Accessibility.StructureType = PdfStructureType.Heading2;
            var info = doc.AddText(section);
            return (int)info.LastPageRectangle.Bounds.Bottom + separator;
        }

        private int AddCaption(PdfDocument doc, PdfFont bodyFont,
            string caption, int x, int y, int width)
        {
            PdfTextElement t = new PdfTextElement(caption, bodyFont)
            { X = x, Y = y, Width = width };
            t.Accessibility.StructureType = PdfStructureType.Artifact;
            var info = doc.AddText(t);
            return (int)info.LastPageRectangle.Bounds.Bottom;
        }

        private void EnsureSpaceOnPage(ref int crtYPos, int requestedHeight, PdfDocument pdfDocument)
        {
            if (crtYPos + requestedHeight > pdfDocument.ContentHeight)
            {
                pdfDocument.AddPage();
                crtYPos = 0;
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

        private string GetDemoFilesPath() => m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/";
        private string GetDemoFontsPath() => Path.Combine(GetDemoFilesPath(), "Font_Files");
        private string GetDemoTextsPath() => Path.Combine(GetDemoFilesPath(), "Text_Files");
    }
}
