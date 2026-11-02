PdfTextAnnotation keyAnnotation = PdfTextAnnotation.Create("A key point", 1, 30, 300);
keyAnnotation.Icon = PdfTextAnnotationIcon.Key;
keyAnnotation.Open = true;
pdfDocument.AddTextAnnotation(keyAnnotation);
