using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Hosting;
using System.Drawing;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers.PDF_Creator
{
    public class PDF_Creator_Text_ElementsController : Controller
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment m_hostingEnvironment;
        public PDF_Creator_Text_ElementsController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        // GET: PDF_Creator_Text_Elements
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreatePdf(IFormCollection collection)
        {
            // Create a PDF document
            Document pdfDocument = new Document();

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the converter in demo mode
            pdfDocument.LicenseKey = "4W9+bn19bn5ue2B+bn1/YH98YHd3d3c=";

            try
            {
                // The result of adding Text Elements to PDF document
                AddElementResult addTextResult = null;

                // The titles font used to mark various sections of the PDF document
                PdfFont titleFont = pdfDocument.AddFont(new Font("Times New Roman", 10, FontStyle.Bold, GraphicsUnit.Point));
                titleFont.IsUnderline = true;

                // The position on X anf Y axes where to add the next element
                float yLocation = 5;
                float xLocation = 5;

                // Create a PDF page in PDF document
                PdfPage pdfPage = pdfDocument.AddPage();

                // Text Elements Using Fonts Installed in System

                // Add section title
                TextElement titleTextElement = new TextElement(xLocation, yLocation, "Text Elements Using Fonts Installed in System", titleFont);
                titleTextElement.ForeColor = Color.DarkGreen;
                addTextResult = pdfPage.AddElement(titleTextElement);
                yLocation = addTextResult.EndPageBounds.Bottom + 10;
                xLocation += 5;
                pdfPage = addTextResult.EndPdfPage;

                // Embed in PDF document a font with Normal style installed in system
                Font systemFontNormal = new Font("Times New Roman", 10, GraphicsUnit.Point);
                PdfFont embeddedSystemFontNormal = pdfDocument.AddFont(systemFontNormal);

                // Add a text element using a font with Normal style installed in system
                TextElement embeddedSystemFontNormalTextElement = new TextElement(xLocation, yLocation, "This text uses a font with Normal style installed in system", embeddedSystemFontNormal);
                addTextResult = pdfPage.AddElement(embeddedSystemFontNormalTextElement);
                yLocation = addTextResult.EndPageBounds.Bottom + 3;
                pdfPage = addTextResult.EndPdfPage;

                // Embed in PDF document a font with Bold style installed in system
                Font systemFontBold = new Font("Times New Roman", 10, FontStyle.Bold, GraphicsUnit.Point);
                PdfFont embeddedSystemFontBold = pdfDocument.AddFont(systemFontBold);

                // Add a text element using a font with Bold style installed in system
                TextElement embeddedSystemFontBoldTextElement = new TextElement(xLocation, yLocation, "This text uses a font with Bold style installed in system", embeddedSystemFontBold);
                addTextResult = pdfPage.AddElement(embeddedSystemFontBoldTextElement);
                yLocation = addTextResult.EndPageBounds.Bottom + 3;
                pdfPage = addTextResult.EndPdfPage;

                // Embed in PDF document a font with Italic style installed in system
                Font systemFontItalic = new Font("Times New Roman", 10, FontStyle.Italic, GraphicsUnit.Point);
                PdfFont embeddedSystemFontItalic = pdfDocument.AddFont(systemFontItalic);

                // Add a text element using a font with Italic style installed in system
                TextElement embeddedSystemFontItalicTextElement = new TextElement(xLocation, yLocation, "This text uses a font with Italic style installed in system", embeddedSystemFontItalic);
                addTextResult = pdfPage.AddElement(embeddedSystemFontItalicTextElement);
                yLocation = addTextResult.EndPageBounds.Bottom + 3;
                pdfPage = addTextResult.EndPdfPage;

                // Text Elements Using Fonts From Local Files

                // Add section title
                xLocation -= 5;
                yLocation += 10;
                titleTextElement = new TextElement(xLocation, yLocation, "Text Elements Using Fonts From Local Files", titleFont);
                titleTextElement.ForeColor = Color.Navy;
                addTextResult = pdfPage.AddElement(titleTextElement);
                yLocation = addTextResult.EndPageBounds.Bottom + 10;
                xLocation += 5;
                pdfPage = addTextResult.EndPdfPage;

                // Embed a True Type font from a local file in PDF document
                PdfFont localTrueTypeFont = pdfDocument.AddFont(m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Fonts/TrueType.ttf");

                // Add a text element using the local True Type font to PDF document
                TextElement localFontTtfTextElement = new TextElement(xLocation, yLocation, "This text uses a True Type Font loaded from a local file", localTrueTypeFont);
                addTextResult = pdfPage.AddElement(localFontTtfTextElement);
                yLocation = addTextResult.EndPageBounds.Bottom;
                pdfPage = addTextResult.EndPdfPage;

                // Embed an OpenType font with TrueType Outlines in PDF document
                PdfFont localOpenTypeTrueTypeFont = pdfDocument.AddFont(m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Fonts/OpenTypeTrueType.otf");

                // Add a text element using the local OpenType font with TrueType Outlines to PDF document
                TextElement localOpenTypeTrueTypeFontTextElement = new TextElement(xLocation, yLocation, "This text uses an Open Type Font with TrueType Outlines loaded from a local file", localOpenTypeTrueTypeFont);
                addTextResult = pdfPage.AddElement(localOpenTypeTrueTypeFontTextElement);
                yLocation = addTextResult.EndPageBounds.Bottom + 3;
                pdfPage = addTextResult.EndPdfPage;

                //  Embed an OpenType font with PostScript Outlines in PDF document
                PdfFont localOpenTypePostScriptFont = pdfDocument.AddFont(m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Fonts/OpenTypePostScript.otf");

                // Add a text element using the local OpenType font with PostScript Outlines to PDF document
                TextElement localOpenTypePostScriptFontTextElement = new TextElement(xLocation, yLocation, "This text uses an Open Type Font with PostScript Outlines loaded from a local file", localOpenTypePostScriptFont);
                addTextResult = pdfPage.AddElement(localOpenTypePostScriptFontTextElement);
                yLocation = addTextResult.EndPageBounds.Bottom + 3;
                pdfPage = addTextResult.EndPdfPage;

                // Text Elements Using Standard PDF Fonts

                // Add section title
                xLocation -= 5;
                yLocation += 10;
                titleTextElement = new TextElement(xLocation, yLocation, "Text Elements Using Standard PDF Fonts", titleFont);
                titleTextElement.ForeColor = Color.DarkGreen;
                addTextResult = pdfPage.AddElement(titleTextElement);
                yLocation = addTextResult.EndPageBounds.Bottom + 10;
                xLocation += 5;
                pdfPage = addTextResult.EndPdfPage;

                // Create a standard PDF font with Normal style
                PdfFont standardPdfFontNormal = pdfDocument.AddFont(StdFontBaseFamily.Helvetica);
                standardPdfFontNormal.Size = 10;

                // Add a text element using the standard PDF font with Normal style
                TextElement standardPdfFontNormalTextElement = new TextElement(xLocation, yLocation, "This text uses a standard PDF font with Normal style", standardPdfFontNormal);
                addTextResult = pdfPage.AddElement(standardPdfFontNormalTextElement);
                yLocation = addTextResult.EndPageBounds.Bottom + 3;
                pdfPage = addTextResult.EndPdfPage;

                // Create a standard PDF font with Bold style
                PdfFont standardPdfFontBold = pdfDocument.AddFont(StdFontBaseFamily.HelveticaBold);
                standardPdfFontBold.Size = 10;

                // Add a text element using the standard PDF font with Bold style
                TextElement standardPdfFontBoldTextElement = new TextElement(xLocation, yLocation, "This text uses a standard PDF font with Bold style", standardPdfFontBold);
                addTextResult = pdfPage.AddElement(standardPdfFontBoldTextElement);
                yLocation = addTextResult.EndPageBounds.Bottom + 3;
                pdfPage = addTextResult.EndPdfPage;

                // Create a standard PDF font with Italic style
                PdfFont standardPdfFontItalic = pdfDocument.AddFont(StdFontBaseFamily.HelveticaOblique);
                standardPdfFontItalic.Size = 10;

                // Add a text element using the standard PDF font with Italic style
                TextElement standardPdfFontItalicTextElement = new TextElement(xLocation, yLocation, "This text uses a standard PDF font with Italic style", standardPdfFontItalic);
                addTextResult = pdfPage.AddElement(standardPdfFontItalicTextElement);
                yLocation = addTextResult.EndPageBounds.Bottom + 3;
                pdfPage = addTextResult.EndPdfPage;

                // Text Elements with Vertical Text

                // Add section title
                xLocation -= 5;
                yLocation += 10;
                titleTextElement = new TextElement(xLocation, yLocation, "Vertical Text", titleFont);
                titleTextElement.ForeColor = Color.Navy;
                addTextResult = pdfPage.AddElement(titleTextElement);
                yLocation = addTextResult.EndPageBounds.Bottom + 10;
                xLocation += 5;
                pdfPage = addTextResult.EndPdfPage;

                // Add a top to bottom vertical text
                string topBottomText = "This is a Top to Bottom Vertical Text";
                float topBottomTextWidth = embeddedSystemFontNormal.GetTextWidth(topBottomText);

                TextElement topBottomVerticalTextElement = new TextElement(0, 0, topBottomText, embeddedSystemFontNormal);
                topBottomVerticalTextElement.Translate(xLocation + 25, yLocation);
                topBottomVerticalTextElement.Rotate(90);
                pdfPage.AddElement(topBottomVerticalTextElement);

                // Add a bottom to top vertical text
                string bottomTopText = "This is a Bottom to Top Vertical Text";
                float bottomTopTextWidth = embeddedSystemFontNormal.GetTextWidth(bottomTopText);

                TextElement bottomTopVerticalTextElement = new TextElement(0, 0, bottomTopText, embeddedSystemFontNormal);
                bottomTopVerticalTextElement.Translate(xLocation + 125, yLocation + bottomTopTextWidth);
                bottomTopVerticalTextElement.Rotate(-90);
                pdfPage.AddElement(bottomTopVerticalTextElement);

                yLocation += bottomTopTextWidth + 10;

                // Add a text element that flows freely in width and height

                string text = System.IO.File.ReadAllText(m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Text_Files/Text_File.txt");

                // Add section title
                xLocation -= 5;
                yLocation += 10;
                titleTextElement = new TextElement(xLocation, yLocation, "Text Element that flows freely in width and height", titleFont);
                titleTextElement.ForeColor = Color.DarkGreen;
                addTextResult = pdfPage.AddElement(titleTextElement);
                yLocation = addTextResult.EndPageBounds.Bottom + 10;
                xLocation += 5;
                pdfPage = addTextResult.EndPdfPage;

                // Add the text element
                TextElement freeWidthAndHeightTextElement = new TextElement(xLocation, yLocation, text, embeddedSystemFontNormal);
                addTextResult = pdfPage.AddElement(freeWidthAndHeightTextElement);
                yLocation = addTextResult.EndPageBounds.Bottom + 3;
                pdfPage = addTextResult.EndPdfPage;

                // Add a text element with a given width that flows freely in height

                // Add section title
                xLocation -= 5;
                yLocation += 10;
                titleTextElement = new TextElement(xLocation, yLocation, "Text Element with a given width that flows freely in height", titleFont);
                titleTextElement.ForeColor = Color.Navy;
                addTextResult = pdfPage.AddElement(titleTextElement);
                yLocation = addTextResult.EndPageBounds.Bottom + 10;
                xLocation += 5;
                pdfPage = addTextResult.EndPdfPage;

                // Add the text element
                TextElement freeHeightTextElement = new TextElement(xLocation, yLocation, 400, text, embeddedSystemFontNormal);
                addTextResult = pdfPage.AddElement(freeHeightTextElement);
                yLocation = addTextResult.EndPageBounds.Bottom + 3;
                pdfPage = addTextResult.EndPdfPage;

                // Add a bounding rectangle for text element
                RectangleElement border = new RectangleElement(addTextResult.EndPageBounds.X, addTextResult.EndPageBounds.Y,
                            addTextResult.EndPageBounds.Width, addTextResult.EndPageBounds.Height);
                pdfPage.AddElement(border);

                // Add a text element with a given width and height

                // Add section title
                xLocation -= 5;
                yLocation += 10;
                titleTextElement = new TextElement(xLocation, yLocation, "Text Element with a given width and height", titleFont);
                titleTextElement.ForeColor = Color.DarkGreen;
                addTextResult = pdfPage.AddElement(titleTextElement);
                yLocation = addTextResult.EndPageBounds.Bottom + 10;
                xLocation += 5;
                pdfPage = addTextResult.EndPdfPage;

                // Add the text element
                TextElement boundedTextElement = new TextElement(xLocation, yLocation, 400, 50, text, embeddedSystemFontNormal);
                addTextResult = pdfPage.AddElement(boundedTextElement);
                yLocation = addTextResult.EndPageBounds.Bottom + 3;
                pdfPage = addTextResult.EndPdfPage;

                // Add a bounding rectangle for text element
                border = new RectangleElement(addTextResult.EndPageBounds.X, addTextResult.EndPageBounds.Y,
                            addTextResult.EndPageBounds.Width, addTextResult.EndPageBounds.Height);
                pdfPage.AddElement(border);

                // Add a text element that flows freely on next PDF page

                // Add section title
                xLocation -= 5;
                yLocation += 10;
                titleTextElement = new TextElement(xLocation, yLocation, "Text Element that flows freely on multiple PDF pages", titleFont);
                titleTextElement.ForeColor = Color.Navy;
                addTextResult = pdfPage.AddElement(titleTextElement);
                yLocation = addTextResult.EndPageBounds.Bottom + 10;
                xLocation += 5;
                pdfPage = addTextResult.EndPdfPage;

                // Add the text element
                string multiPageText = System.IO.File.ReadAllText(m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Text_Files/Large_Text_File.txt");
                TextElement multiPageTextElement = new TextElement(xLocation, yLocation, 575, multiPageText, embeddedSystemFontNormal);
                multiPageTextElement.BackColor = Color.WhiteSmoke;
                addTextResult = pdfPage.AddElement(multiPageTextElement);
                yLocation = addTextResult.EndPageBounds.Bottom + 3;
                pdfPage = addTextResult.EndPdfPage;

                // Add a line at the bottom of the multipage text element

                LineElement bottomLine = new LineElement(addTextResult.EndPageBounds.X, addTextResult.EndPageBounds.Bottom + 1,
                    addTextResult.EndPageBounds.X + addTextResult.EndPageBounds.Width, addTextResult.EndPageBounds.Bottom + 1);
                pdfPage.AddElement(bottomLine);

                // Add a text stamp to a PDF document

                // Create a .NET font
                Font timesNewRomanFont = new Font("Times New Roman", 24, GraphicsUnit.Point);
                // Create a PDF font
                PdfFont stampPdfFont = pdfDocument.AddFont(timesNewRomanFont, true);
                // The stamp text
                string stampText = String.Format("Text Stamp {0}", DateTime.Now.ToString("d"));
                // Measure the text 
                float textWidth = stampPdfFont.GetTextWidth(stampText);
                foreach (PdfPage page in pdfDocument.Pages)
                {
                    // Get the PDF page drawable area width and height
                    float pdfPageWidth = page.ClientRectangle.Width;
                    float pdfPageHeight = page.ClientRectangle.Height;

                    // Calculate the PDF page diagonal
                    float pdfPageDiagonal = (float)Math.Sqrt(pdfPageWidth * pdfPageWidth + pdfPageHeight * pdfPageHeight);

                    // The text location on PDF page diagonal
                    float xStampLocation = (pdfPageDiagonal - textWidth) / 2;

                    // Create the stamp as a rotated text element
                    TextElement stampTextElement = new TextElement(xStampLocation, 0, stampText, stampPdfFont);
                    stampTextElement.ForeColor = Color.Coral;
                    stampTextElement.Rotate((float)(Math.Atan(pdfPageHeight / pdfPageWidth) * (180 / Math.PI)));
                    stampTextElement.Opacity = 75;

                    // Add the stamp to PDF page
                    page.AddElement(stampTextElement);
                }

                // Save the PDF document in a memory buffer
                byte[] outPdfBuffer = pdfDocument.Save();
                
                // Send the PDF file to browser
                FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "Text_Elements.pdf";

                return fileResult;
            }
            finally
            {
                // Close the PDF document
                pdfDocument.Close();
            }
        }
    }
}
