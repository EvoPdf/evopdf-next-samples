# .NET 10, for the Dockerfile with the ASP.NET Core Runtime 10.0 base image
dotnet publish -c Release -r win-x64 -o publish/windows-x64-net10.0 EvoPdf_Next_AspNetDemo_Windows_net10.0.csproj

# .NET 8, for the Dockerfile with the ASP.NET Core Runtime 8.0 base image
dotnet publish -c Release -r win-x64 -o publish/windows-x64-net8.0 EvoPdf_Next_AspNetDemo_Windows_net8.0.csproj
