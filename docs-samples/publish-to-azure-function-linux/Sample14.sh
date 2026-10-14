docker build --platform linux/amd64 -t pdf-func .

az functionapp create --name pdf-func --resource-group pdf-app-rg --plan pdf-plan \
  --storage-account mystorage --functions-version 4 \
  --image myregistry.azurecr.io/pdf-func:1 --registry-server myregistry.azurecr.io \
  --registry-username myregistry \
  --registry-password "$(az acr credential show --name myregistry --query 'passwords[0].value' -o tsv)"
