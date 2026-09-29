PdfTextAnnotation ann = PdfTextAnnotation.Create(
    contents: "Please confirm the issue date before mailing.",
    pageNumber: 1, x: 30, y: 200);
ann.Icon = PdfTextAnnotationIcon.Comment;
ann.Author = "Jane Reviewer";
ann.Open = true;
pdfDocument.AddTextAnnotation(ann);
