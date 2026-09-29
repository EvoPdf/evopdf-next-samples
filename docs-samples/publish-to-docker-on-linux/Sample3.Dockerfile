FROM mcr.microsoft.com/dotnet/aspnet:10.0

# Install EvoPdf dependencies
RUN apt-get update && \
    apt-get install -y \
        libnss3 \
        libatk-bridge2.0-0 \
        libcairo2 \
        libpango-1.0-0 && \
    rm -rf /var/lib/apt/lists/*

# Set the working directory
WORKDIR /app

# Copy the published ASP.NET Core app files from the publish folder into the image
COPY publish/linux-x64-net10.0/ .

# Ensure execute permissions for EvoPdf HTML to PDF runtime
RUN chmod +x /app/evopdf_runtimes/linux-x64/native/evopdf_loadhtml

# Ensure execute permissions for EvoPdf PDF Processor runtime
RUN chmod +x /app/evopdf_runtimes/linux-x64/native/evopdf_pdfprocessor

# Expose the port used by the app
EXPOSE 27101

# Set the ASP.NET Core application to listen on port 27101
ENV ASPNETCORE_URLS=http://+:27101

# Start the application
ENTRYPOINT ["dotnet", "EvoPdf_Next_AspNetDemo_Linux_net10.0.dll"]
