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

namespace EvoHtmlToPdfDemo.Controllers.HTML_to_PDF.PDF_Security
{
    public class Digital_SignaturesController : Controller
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment m_hostingEnvironment;
        public Digital_SignaturesController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public ActionResult ConvertHtmlToPdf(IFormCollection collection)
        {
            // Create a HTML to PDF converter object with default settings
            HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the converter in demo mode
            htmlToPdfConverter.LicenseKey = "4W9+bn19bn5ue2B+bn1/YH98YHd3d3c=";

            // Set an adddional delay in seconds to wait for JavaScript or AJAX calls after page load completed
            // Set this property to 0 if you don't need to wait for such asynchcronous operations to finish
            htmlToPdfConverter.ConversionDelay = 2;

            Document pdfDocument = null;
            try
            {
                string htmlWithDigitalSignatureMarker = collection["htmlStringTextBox"];
                string baseUrl = collection["baseUrlTextBox"];

                // Convert a HTML string with a marker for digital signature to a PDF document object
                pdfDocument = htmlToPdfConverter.ConvertHtmlToPdfDocumentObject(htmlWithDigitalSignatureMarker, baseUrl);

                // Make the HTML element with 'digital_signature_element' mapping ID a link to digital signature properties
                HtmlElementMapping digitalSignatureMapping = htmlToPdfConverter.HtmlElementsMappingOptions.HtmlElementsMappingResult.GetElementByMappingId("digital_signature_element");
                if (digitalSignatureMapping != null)
                {
                    PdfPage digitalSignaturePage = digitalSignatureMapping.PdfRectangles[0].PdfPage;
                    RectangleF digitalSignatureRectangle = digitalSignatureMapping.PdfRectangles[0].Rectangle;

                    string certificateFilePath = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/Certificates/evopdf.pfx";

                    // Get the certificate from password protected PFX file
                    DigitalCertificatesCollection certificates = DigitalCertificatesStore.GetCertificates(certificateFilePath, "evopdf");
                    DigitalCertificate certificate = certificates[0];

                    // Create the digital signature
                    DigitalSignatureElement signature = new DigitalSignatureElement(digitalSignatureRectangle, certificate);
                    signature.Reason = "Protect the document from unwanted changes";
                    signature.ContactInfo = "The contact email is support@evopdf.com";
                    signature.Location = "Development server";
                    digitalSignaturePage.AddElement(signature);
                }

                // Save the PDF document in a memory buffer
                byte[] outPdfBuffer = pdfDocument.Save();
                
                // Send the PDF file to browser
                FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "Digital_Signatures.pdf";

                return fileResult;
            }
            finally
            {
                // Close the PDF document
                if (pdfDocument != null)
                    pdfDocument.Close();
            }
        }
    }
}