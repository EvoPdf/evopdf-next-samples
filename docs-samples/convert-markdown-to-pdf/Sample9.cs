byte[] inputMarkdownBytes = System.IO.File.ReadAllBytes(markdownFilePath);
string markdownString = Encoding.UTF8.GetString(inputMarkdownBytes);
string baseUrl = "file://" + markdownFilePath;
