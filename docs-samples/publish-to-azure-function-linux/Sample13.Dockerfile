FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated10.0

# Install the packages required by the HTML to PDF Converter
RUN apt-get update && \
    apt-get install -y \
        libnss3 \
        libatk-bridge2.0-0 \
        libcairo2 \
        libpango-1.0-0 && \
    rm -rf /var/lib/apt/lists/*

ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

# Copy the published function app
COPY publish/ /home/site/wwwroot

# Ensure execute permissions for the HTML to PDF Converter and PDF Processor runtimes
RUN chmod +x /home/site/wwwroot/evopdf_runtimes/linux-x64/native/evopdf_loadhtml \
    /home/site/wwwroot/evopdf_runtimes/linux-x64/native/evopdf_pdfprocessor
