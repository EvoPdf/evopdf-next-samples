az appservice plan create --name pdf-plan --resource-group pdf-app-rg --is-linux --sku B2

az webapp create --name pdf-app --resource-group pdf-app-rg --plan pdf-plan \
  --container-image-name myregistry.azurecr.io/pdf-app:1 \
  --container-registry-user myregistry \
  --container-registry-password "$(az acr credential show --name myregistry --query 'passwords[0].value' -o tsv)"

az webapp config appsettings set --name pdf-app --resource-group pdf-app-rg --settings WEBSITES_PORT=8080
