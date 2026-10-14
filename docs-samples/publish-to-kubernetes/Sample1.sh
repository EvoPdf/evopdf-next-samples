az aks create --resource-group pdf-app-rg --name pdf-aks --node-count 1 --node-vm-size Standard_D2s_v3 \
  --attach-acr myregistry --generate-ssh-keys
az aks get-credentials --resource-group pdf-app-rg --name pdf-aks
kubectl get nodes
