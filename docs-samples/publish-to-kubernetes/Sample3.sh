kubectl apply -f pdf-app.yaml
kubectl rollout status deployment/pdf-app
kubectl get service pdf-app
