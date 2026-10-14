gcloud config set project PROJECT_ID
gcloud services enable run.googleapis.com artifactregistry.googleapis.com
gcloud artifacts repositories create pdf-images --repository-format=docker --location=us-central1
