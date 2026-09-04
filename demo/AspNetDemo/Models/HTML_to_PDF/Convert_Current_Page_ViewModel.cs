namespace EvoPdf_Next_AspNetDemo.Models.HTML_to_PDF
{
    public class Convert_Current_Page_ViewModel
    {
        public string FirstName { get; set; } = "John";
        public string LastName { get; set; } = "Smith";
        public string Gender { get; set; } = "Male";
        public bool HaveCar { get; set; } = true;
        public string CarType { get; set; } = "Volvo";
        public string Comments { get; set; } = "My comments\r\nLine 1\r\nLine 2";
    }
}