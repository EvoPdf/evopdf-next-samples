dotnet publish -c Release -o publish
az functionapp deployment source config-zip --name pdf-func --resource-group pdf-app-rg --src publish.zip
