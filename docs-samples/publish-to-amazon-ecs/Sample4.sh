aws ecr create-repository --repository-name pdf-app --region us-east-1
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 123456789012.dkr.ecr.us-east-1.amazonaws.com
docker tag pdf-app 123456789012.dkr.ecr.us-east-1.amazonaws.com/pdf-app:1
docker push 123456789012.dkr.ecr.us-east-1.amazonaws.com/pdf-app:1
