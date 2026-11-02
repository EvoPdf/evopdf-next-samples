digitalSignature.DigestAlgorithm = PdfSignatureDigestAlgorithm.Sha384;
digitalSignature.TimestampServerUrl = "http://timestamp.example.com";
digitalSignature.TimestampServerUsername = "user";
digitalSignature.TimestampServerPassword = "password";
digitalSignature.TrustedCaCertificates = new[] { caCertificate.RawData };
