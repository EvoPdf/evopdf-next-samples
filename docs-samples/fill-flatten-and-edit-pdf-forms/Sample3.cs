// Only the fields with a value become static content; the other fields stay editable
foreach (string fieldName in form.FieldNames)
{
    string value = form.GetValue(fieldName);
    if (!string.IsNullOrEmpty(value) && value != "Off")
        form.FlattenField(fieldName);
}
