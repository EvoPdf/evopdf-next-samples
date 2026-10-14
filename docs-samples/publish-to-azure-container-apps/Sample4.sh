az group create --name pdf-app-rg --location eastus
az acr create --resource-group pdf-app-rg --name myregistry --sku Basic --admin-enabled true
az acr credential show --name myregistry --query "passwords[0].value" -o tsv
