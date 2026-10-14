aws ecr create-repository --repository-name pdf-function --region us-east-1
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 123456789012.dkr.ecr.us-east-1.amazonaws.com
docker tag pdf-function:latest 123456789012.dkr.ecr.us-east-1.amazonaws.com/pdf-function:latest
docker push 123456789012.dkr.ecr.us-east-1.amazonaws.com/pdf-function:latest
