FROM mcr.microsoft.com/windows/server:ltsc2022

# Use PowerShell as default shell
SHELL ["powershell", "-Command"]

# Install .NET 8.0 ASP.NET Core Runtime from ZIP
RUN Invoke-WebRequest -Uri 'https://builds.dotnet.microsoft.com/dotnet/aspnetcore/Runtime/8.0.22/aspnetcore-runtime-8.0.22-win-x64.zip' -OutFile 'C:\\dotnet.zip'; \
    Expand-Archive -Path 'C:\\dotnet.zip' -DestinationPath 'C:\\dotnet'; \
    Remove-Item 'C:\\dotnet.zip' -Force

ENV DOTNET_ROOT=C:\dotnet

WORKDIR C:/app

# Copy the published ASP.NET Core app files from the publish folder into the image
COPY publish/windows-x64-net8.0/ .

EXPOSE 27102
ENV ASPNETCORE_URLS=http://+:27102

ENTRYPOINT ["C:\\dotnet\\dotnet.exe", "C:\\app\\EvoPdf_Next_AspNetDemo_Windows_net8.0.dll"]
