using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Hosting;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers.PDF_Editor
{
    public class PDF_Editor_Split_PDFController : Controller
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment m_hostingEnvironment;
        public PDF_Editor_Split_PDFController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        private void SetCurrentViewData()
        {
            ViewData["ContentRootPath"] = m_hostingEnvironment.ContentRootPath + "/wwwroot";

            HttpRequest request = this.ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            if (request.Host.Port != null)
                uriBuilder.Port = (int)request.Host.Port;
            uriBuilder.Path = request.PathBase.ToString() + request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();

            ViewData["CurrentPageUrl"] = uriBuilder.Uri.AbsoluteUri;
        }

        // GET: PDF_Editor_Split_PDF
        public ActionResult Index()
        {
            SetCurrentViewData();

            return View();
        }

        [HttpPost]
        public ActionResult SplitPdf(IFormCollection collection)
        {
            Document pdfDocumentToSplit = null;
            Document splitResultDocument1 = null;
            Document splitResultDocument2 = null;
            try
            {
                // Load the PDF document to split
                // The document must remain opened until the PDF documents resulted after split are saved
                string pdfFileToSplitPath = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/PDF_Files/PDF_Document.pdf";
                pdfDocumentToSplit = new Document(pdfFileToSplitPath);

                // Get the page count of the document to split
                int pageCount = pdfDocumentToSplit.Pages.Count;

                // Check if the document has the minimum required number of pages
                if (pageCount < 2)
                {
                    byte[] outPdfBuffer = pdfDocumentToSplit.Save();

                    // Send the input PDF file to browser
                    FileResult fileResultInitial = new FileContentResult(outPdfBuffer, "application/pdf");
                    fileResultInitial.FileDownloadName = "Split_PDF.pdf";

                    return fileResultInitial;
                }

                // Create the first PDF document resulted after split
                splitResultDocument1 = new Document();
                // Set license key received after purchase
                splitResultDocument1.LicenseKey = "4W9+bn19bn5ue2B+bn1/YH98YHd3d3c=";
                // Copy pages from loaded document into split result document
                splitResultDocument1.AppendDocument(pdfDocumentToSplit, 0, pageCount / 2);
                // Save the first PDF document document resulted after split in a memory buffer
                byte[] outPdfBuffer1 = splitResultDocument1.Save();

                // Create the second PDF document resulted after split  
                splitResultDocument2 = new Document();
                // Set license key received after purchase
                splitResultDocument2.LicenseKey = "4W9+bn19bn5ue2B+bn1/YH98YHd3d3c=";
                // Copy pages from loaded document into split result document
                splitResultDocument2.AppendDocument(pdfDocumentToSplit, pageCount / 2, pageCount - pageCount / 2);
                // Save the second PDF document document resulted after split in a memory buffer
                byte[] outPdfBuffer2 = splitResultDocument2.Save();
                
                // Send the PDF file to browser
                FileResult fileResult = new FileContentResult(outPdfBuffer1, "application/pdf");
                fileResult.FileDownloadName = "Split_PDF_1.pdf";

                return fileResult;
            }
            finally
            {
                // Close all the PDF documents after the PDF documents resulted after split were already saved

                // Close the first split result document
                if (splitResultDocument1 != null)
                    splitResultDocument1.Close();

                // Close the second split result document
                if (splitResultDocument2 != null)
                    splitResultDocument2.Close();

                // Close the document to split
                if (pdfDocumentToSplit != null)
                    pdfDocumentToSplit.Close();
            }
        }
    }
}