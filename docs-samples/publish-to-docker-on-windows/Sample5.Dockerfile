FROM mcr.microsoft.com/dotnet/aspnet:10.0-windowsservercore-ltsc2025

SHELL ["powershell", "-Command"]

# Copy fonts from build context into the image Fonts folder
COPY Fonts/ C:/Windows/Fonts/

# Register fonts in container's Windows registry
RUN $ErrorActionPreference = 'Stop'; \
    Get-ChildItem 'C:\Windows\Fonts' -Include *.ttf, *.ttc, *.otf -Recurse | ForEach-Object { \
        $file = $_.Name; \
        $name = [System.IO.Path]::GetFileNameWithoutExtension($file); \
        $regName = $name + ' (TrueType)'; \
        New-ItemProperty -Path 'HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts' -Name $regName -PropertyType String -Value $file -Force | Out-Null \
    }

# Set working directory for the app
WORKDIR C:/app

# Copy the published ASP.NET Core app files from the publish folder into the image
COPY publish/windows-x64-net10.0/ .

# Expose the application port
EXPOSE 27102

# Configure ASP.NET Core to listen on all interfaces and port 27102
ENV ASPNETCORE_URLS=http://+:27102

# Run the application
ENTRYPOINT ["powershell", "-Command", "Start-Sleep -Seconds 3; dotnet C:\\app\\EvoPdf_Next_AspNetDemo_Windows_net10.0.dll"]
