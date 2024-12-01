# dotnet-aws-sqs-playground

### K6 Run script
docker run -v "./k-scripts:/scripts" -v ./report:/report -p 5665:5665 -it --rm ghcr.io/grafana/xk6-dashboard:latest run --out=dashboard=report=/report/test-report.html /scripts/send-sqs-api-test-script.js