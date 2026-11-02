// All the email addresses, phone numbers and bank accounts
pdfRedactor.RedactPersonalData(PdfPersonalDataTypes.EmailAddress | PdfPersonalDataTypes.PhoneNumber |
    PdfPersonalDataTypes.Iban);

// The payment card numbers found, reviewed one by one
foreach (PdfTextMatch match in pdfRedactor.FindPersonalData(PdfPersonalDataTypes.PaymentCardNumber))
{
    if (match.PageNumber == 1)
        pdfRedactor.AddRedaction(match);
}
