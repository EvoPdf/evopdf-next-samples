gcloud run deploy pdf-app --image us-central1-docker.pkg.dev/PROJECT_ID/pdf-images/pdf-app:1 \
  --region us-central1 --allow-unauthenticated \
  --memory 2Gi --cpu 2 --timeout 300 --execution-environment gen2
