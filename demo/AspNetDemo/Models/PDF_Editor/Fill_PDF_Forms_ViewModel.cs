using Microsoft.AspNetCore.Http;

namespace EvoPdf_Next_AspNetDemo.Models.PDF_Editor
{
    public class Fill_PDF_Forms_ViewModel
    {
        public string PdfFileUrl { get; set; }
        public IFormFile PdfFile { get; set; }

        public string UserPassword { get; set; } = string.Empty;
        public string OwnerPassword { get; set; } = string.Empty;

        public bool FillFields { get; set; } = true;
        public string FullName { get; set; } = "John Smith";
        public string Email { get; set; } = "john.smith@example.com";
        public string Country { get; set; } = "DE";
        public string TicketType { get; set; } = "premium";
        public bool Newsletter { get; set; } = true;
        public bool Terms { get; set; } = true;
        public string Sessions { get; set; } = "Keynote, Panel discussion";
        public string Comments { get; set; } = "Vegetarian menu";

        public bool ImportSampleData { get; set; } = false;
        public IFormFile DataFile { get; set; }

        public bool ReplaceCountryOptions { get; set; } = false;
        public bool SetFieldColors { get; set; } = false;
        public bool SetFieldFont { get; set; } = false;
        public float FieldFontSize { get; set; } = 10;
        public bool ReadOnlyFields { get; set; } = false;
        public string CommentsVisibility { get; set; } = "Visible";
        public bool RenameCommentsField { get; set; } = false;
        public bool RemoveAccessCodeField { get; set; } = false;
        public bool GenerateAppearances { get; set; } = true;
        public bool UseSubstitutionFont { get; set; } = false;

        public string FlattenMode { get; set; } = "None";
        public string OutputFormat { get; set; } = "Pdf";
        public bool AddFieldsListPage { get; set; } = true;
    }
}
