// Two pages per document
List<byte[]> parts = pdfSplit.SplitByPageCount(2);

// One document for each range
List<byte[]> chapters = pdfSplit.SplitByPageRanges(new[] { "1-2", "3-5", "6-" });
