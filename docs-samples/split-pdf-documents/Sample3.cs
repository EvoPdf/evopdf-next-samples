using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
    public class Split_PDF_DocumentsController : Controller
    {
        private readonly IWebHostEnvironment m_hostingEnvironment;

        public Split_PDF_DocumentsController(IWebHostEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = SetViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SplitPdf(Split_PDF_Documents_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelStateHelper.GetModelErrors(ModelState);
                throw new ValidationException(errorMessage);
            }

            // Set the license key received after purchase to use the library in licensed mode; leave it commented for demo mode
            // Licensing.LicenseKey = "your-license-key";

            byte[] inputPdfBytes = await ReadInputPdf(model);
            string password = !string.IsNullOrEmpty(model.OwnerPassword) ? model.OwnerPassword :
                !string.IsNullOrEmpty(model.UserPassword) ? model.UserPassword : null;

            using PdfSplit pdfSplit = new PdfSplit(inputPdfBytes, password);

            if (model.SetDocumentTitle)
            {
                // The document information, security options and viewer preferences set on the splitter are
                // written in every document it produces; the other properties come from the loaded document
                pdfSplit.PdfDocumentInfo.Title = model.DocumentTitle;
            }

            int pageCount = pdfSplit.PageCount;

            switch (model.SplitOperation)
            {
                case "ExtractPage":
                    {
                        CheckPage(model.PageNumber, pageCount);
                        // One page of the loaded document, with its links, bookmarks and fields
                        byte[] pagePdf = pdfSplit.ExtractPage(model.PageNumber);
                        return PdfResult(pagePdf, "Split_PDF_Documents_Page_" + model.PageNumber + ".pdf");
                    }

                case "ExtractPages":
                    {
                        CheckPage(model.FirstPage, pageCount);
                        CheckPage(model.LastPage, pageCount);
                        if (model.FirstPage > model.LastPage)
                            throw new Exception("The first page must not be after the last page");
                        byte[] pagesPdf = pdfSplit.ExtractPages(model.FirstPage, model.LastPage);
                        return PdfResult(pagesPdf, "Split_PDF_Documents_Pages_" + model.FirstPage + "_" + model.LastPage + ".pdf");
                    }

                case "ExtractPageRanges":
                    {
                        // The pages of several ranges, for example "1,3-4,6-", are extracted in the order of the ranges
                        byte[] rangesPdf = pdfSplit.ExtractPages(model.PageRanges);
                        return PdfResult(rangesPdf, "Split_PDF_Documents_Ranges.pdf");
                    }

                case "RemovePages":
                    {
                        // The loaded document without the pages of the given ranges
                        byte[] remainingPdf = pdfSplit.RemovePages(model.RemovePageRanges);
                        return PdfResult(remainingPdf, "Split_PDF_Documents_Removed.pdf");
                    }

                case "SplitByPageCount":
                    {
                        if (model.PagesPerDocument < 1)
                            throw new Exception("The number of pages per document must be at least 1");
                        // Documents of the given number of pages; the last document may have fewer pages
                        List<byte[]> parts = pdfSplit.SplitByPageCount(model.PagesPerDocument);
                        return ZipResult(parts, "Split_PDF_Documents_by_Page_Count.zip");
                    }

                case "SplitByPageRanges":
                    {
                        // One document for each range, for example "1-2;3-5;6-"
                        string[] ranges = model.SplitPageRanges.Split(';', StringSplitOptions.RemoveEmptyEntries);
                        List<byte[]> parts = pdfSplit.SplitByPageRanges(ranges);
                        return ZipResult(parts, "Split_PDF_Documents_by_Page_Ranges.zip");
                    }

                default:
                    throw new Exception("Unknown split operation");
            }
        }

        private static void CheckPage(int pageNumber, int pageCount)
        {
            if (pageNumber < 1 || pageNumber > pageCount)
                throw new Exception("The page number must be between 1 and " + pageCount);
        }

        private static FileResult PdfResult(byte[] pdfBuffer, string fileName)
        {
            FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
            fileResult.FileDownloadName = fileName;
            return fileResult;
        }

        // The documents of a split are downloaded together in a zip archive
        private static FileResult ZipResult(List<byte[]> documents, string fileName)
        {
            using MemoryStream zipStream = new MemoryStream();
            using (ZipArchive zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                for (int i = 0; i < documents.Count; i++)
                {
                    ZipArchiveEntry entry = zipArchive.CreateEntry("Document_" + (i + 1) + ".pdf", CompressionLevel.Optimal);
                    using Stream entryStream = entry.Open();
                    entryStream.Write(documents[i], 0, documents[i].Length);
                }
            }

            FileResult fileResult = new FileContentResult(zipStream.ToArray(), "application/zip");
            fileResult.FileDownloadName = fileName;
            return fileResult;
        }

        private async Task<byte[]> ReadInputPdf(Split_PDF_Documents_ViewModel model)
        {
            // If an uploaded file exists, use it with priority
            if (model.PdfFile != null && model.PdfFile.Length > 0)
            {
                try
                {
                    using var ms = new MemoryStream();
                    await model.PdfFile.CopyToAsync(ms);
                    return ms.ToArray();
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to read the uploaded PDF file", ex);
                }
            }

            // Otherwise, fall back to the URL
            string pdfUrl = model.PdfFileUrl?.Trim();
            if (string.IsNullOrWhiteSpace(pdfUrl))
                throw new Exception("No PDF file provided: upload a file or specify a URL");

            try
            {
                if (pdfUrl.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                {
                    string localPath = new Uri(pdfUrl).LocalPath;
                    return await System.IO.File.ReadAllBytesAsync(localPath);
                }

                using var httpClient = new System.Net.Http.HttpClient();
                return await httpClient.GetByteArrayAsync(pdfUrl);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not download the PDF file from URL", ex);
            }
        }

        private Split_PDF_Documents_ViewModel SetViewModel()
        {
            var model = new Split_PDF_Documents_ViewModel();

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
            string rootUrl = currentPageUrl.Substring(0, currentPageUrl.Length - "Split_PDF_Documents".Length);

            model.PdfFileUrl = rootUrl + "/DemoAppFiles/Input/PDF_Files/Multi_Page_Document.pdf";

            return model;
        }

        private string GetDemoFilesPath() => m_hostingEnvironment.ContentRootPath + "/wwwroot" + "/DemoAppFiles/Input/";
    }
}
