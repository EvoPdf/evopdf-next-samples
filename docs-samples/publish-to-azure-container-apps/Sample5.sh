docker login myregistry.azurecr.io -u myregistry
docker tag pdf-app myregistry.azurecr.io/pdf-app:1
docker push myregistry.azurecr.io/pdf-app:1
