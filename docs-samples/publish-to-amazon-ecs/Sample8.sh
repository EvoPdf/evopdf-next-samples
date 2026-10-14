VPC=$(aws ec2 describe-vpcs --filters Name=isDefault,Values=true --query "Vpcs[0].VpcId" --output text)
SUBNET=$(aws ec2 describe-subnets --filters Name=vpc-id,Values=$VPC Name=default-for-az,Values=true --query "Subnets[0].SubnetId" --output text)
SG=$(aws ec2 create-security-group --group-name pdf-app-sg --description "pdf-app" --vpc-id $VPC --query GroupId --output text)
aws ec2 authorize-security-group-ingress --group-id $SG --protocol tcp --port 8080 --cidr 0.0.0.0/0

TASK=$(aws ecs run-task --cluster pdf-cluster --launch-type FARGATE --task-definition pdf-app \
  --network-configuration "awsvpcConfiguration={subnets=[$SUBNET],securityGroups=[$SG],assignPublicIp=ENABLED}" \
  --query "tasks[0].taskArn" --output text)
aws ecs wait tasks-running --cluster pdf-cluster --tasks $TASK

ENI=$(aws ecs describe-tasks --cluster pdf-cluster --tasks $TASK --query "tasks[0].attachments[0].details[?name=='networkInterfaceId'].value" --output text)
aws ec2 describe-network-interfaces --network-interface-ids $ENI --query "NetworkInterfaces[0].Association.PublicIp" --output text
