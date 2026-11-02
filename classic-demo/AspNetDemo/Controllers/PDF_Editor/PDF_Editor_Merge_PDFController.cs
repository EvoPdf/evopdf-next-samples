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
    public class PDF_Editor_Merge_PDFController : Controller
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment m_hostingEnvironment;
        public PDF_Editor_Merge_PDFController(IWebHostEnvironment hostingEnvironment)
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

        // GET: PDF_Editor_Merge_PDF
        public ActionResult Index()
        {
            SetCurrentViewData();

            return View();
        }

        [HttpPost]
        public ActionResult MergePdf(IFormCollection collection)
        {
            // Create the merge result PDF document
            Document mergeResultPdfDocument = new Document();

            // Automatically close the merged documents when the document resulted after merge is closed
            mergeResultPdfDocument.AutoCloseAppendedDocs = true;

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the converter in demo mode
            mergeResultPdfDocument.LicenseKey = "4W9+bn19bn5ue2B+bn1/YH98YHd3d3c=";

            try
            {
                // The documents to merge must remain opened until the PDF document resulted after merge is saved
                // The merged documents can be automatically closed when the document resulted after merge is closed
                // if the AutoCloseAppendedDocs property of the PDF document resulted after merge is set on true like
                // in this demo applcation

                // Load the first PDF document to merge
                string firstPdfFilePath = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/PDF_Files/Merge_Before_Conversion.pdf";
                Document firstPdfDocumentToMerge = new Document(firstPdfFilePath);
                // Merge the first PDF document
                mergeResultPdfDocument.AppendDocument(firstPdfDocumentToMerge);

                // Load the second PDF document to merge
                string secondPdfFilePath = m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/PDF_Files/Merge_After_Conversion.pdf";
                Document secondPdfDocumentToMerge = new Document(secondPdfFilePath);
                // Merge the second PDF document
                mergeResultPdfDocument.AppendDocument(secondPdfDocumentToMerge);

                // Save the merge result PDF document in a memory buffer
                byte[] outPdfBuffer = mergeResultPdfDocument.Save();
                
                // Send the PDF file to browser
                FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "Merge_PDF.pdf";

                return fileResult;
            }
            finally
            {
                // Close the PDF document resulted after merge
                // When the AutoCloseAppendedDocs property is true the merged PDF documents will also be closed
                mergeResultPdfDocument.Close();
            }
        }
    }
}