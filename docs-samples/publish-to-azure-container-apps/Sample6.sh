az extension add --name containerapp --upgrade --yes
az provider register --namespace Microsoft.App --wait
az provider register --namespace Microsoft.OperationalInsights --wait

az containerapp env create --name pdf-env --resource-group pdf-app-rg --location eastus

az containerapp create --name pdf-app --resource-group pdf-app-rg --environment pdf-env \
  --image myregistry.azurecr.io/pdf-app:1 --registry-server myregistry.azurecr.io \
  --registry-username myregistry \
  --registry-password "$(az acr credential show --name myregistry --query 'passwords[0].value' -o tsv)" \
  --ingress external --target-port 8080 --cpu 2 --memory 4Gi --min-replicas 1 --max-replicas 3 \
  --query properties.configuration.ingress.fqdn -o tsv
