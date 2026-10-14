mkdir -p ~/bin
curl -sL "https://github.com/eksctl-io/eksctl/releases/latest/download/eksctl_Linux_amd64.tar.gz" | tar -xz -C ~/bin
curl -sLO "https://dl.k8s.io/release/$(curl -Ls https://dl.k8s.io/release/stable.txt)/bin/linux/amd64/kubectl" && chmod +x kubectl && mv kubectl ~/bin/
eksctl create cluster --name pdf-eks --region us-east-1 --nodegroup-name ng --node-type m5.xlarge --nodes 1 --managed
