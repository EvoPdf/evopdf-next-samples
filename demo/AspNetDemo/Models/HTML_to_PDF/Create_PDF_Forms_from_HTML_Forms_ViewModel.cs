namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF
{
    public class Create_PDF_Forms_from_HTML_Forms_ViewModel
    {
        public string HtmlPageSource { get; set; } = "Html";
        public string HtmlString { get; set; }
        public string BaseUrl { get; set; }
        public string Url { get; set; }

        public bool GeneratePdfFormFields { get; set; } = true;

        public string FieldNamingMode { get; set; } = "PreserveHtmlNamesWhenSafe";
        public bool EnableButtonActions { get; set; } = true;
        public bool MapHtmlSubmitButtonsToPdfActions { get; set; } = true;
        public bool MapHtmlResetButtonsToPdfActions { get; set; } = true;
        public bool SubmitOnlyFieldsFromButtonForm { get; set; } = true;
        public bool ResetOnlyFieldsFromButtonForm { get; set; } = true;
        public bool ExcludeDisabledFieldsFromSubmit { get; set; } = true;
        public bool IncludeReadOnlyFieldsInSubmit { get; set; } = true;
        public bool ApplyRequiredAsPdfRequiredFlag { get; set; } = true;
        public string SubmitBehavior { get; set; } = "NativePdfAction";
        public string SubmitFormat { get; set; } = "Html";
        public string ValidationBehavior { get; set; } = "None";
        public bool UseFormFieldFont { get; set; } = false;
    }
}
