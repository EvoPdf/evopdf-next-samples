foreach (PdfFormFieldInfo field in pdfEditor.Form.Fields)
{
    string value = field.Type == PdfFormFieldType.ListBox && field.Values.Count > 0
        ? string.Join(", ", field.Values) : field.Value;
    Console.WriteLine("{0} ({1}): {2}", field.Name, field.Type, value);
}
