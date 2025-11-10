# OrchestationDemo

docker compose up --build -d


curl --location 'http://localhost:5000/api/workflow/start' \
--header 'Content-Type: application/json' \
--data '{"OrderId":"12345"}'
