gcloud services enable container.googleapis.com
gcloud container clusters create pdf-gke --zone us-central1-a --num-nodes 1 --machine-type n2-standard-4 --disk-size 50
gcloud container clusters get-credentials pdf-gke --zone us-central1-a
PROJECT_NUMBER=$(gcloud projects describe PROJECT_ID --format="value(projectNumber)")
gcloud projects add-iam-policy-binding PROJECT_ID --member="serviceAccount:$PROJECT_NUMBER-compute@developer.gserviceaccount.com" --role="roles/artifactregistry.reader"
