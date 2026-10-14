aws ecs describe-tasks --cluster pdf-cluster --tasks $TASK --query "tasks[0].stoppedReason"
