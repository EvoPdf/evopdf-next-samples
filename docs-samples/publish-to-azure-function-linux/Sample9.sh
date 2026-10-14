az group create --name pdf-app-rg --location eastus
az appservice plan create --name pdf-plan --resource-group pdf-app-rg --is-linux --sku B2
az storage account create --name mystorage --resource-group pdf-app-rg --sku Standard_LRS
az functionapp create --name pdf-func --resource-group pdf-app-rg --plan pdf-plan \
  --storage-account mystorage --functions-version 4 \
  --runtime dotnet-isolated --runtime-version 10 --os-type Linux
