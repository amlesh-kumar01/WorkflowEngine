# Workflow Engine API Demo Script

# This file contains sample curl commands to demonstrate the Workflow Engine API
# Run these commands after starting the application with: dotnet run

# Base URL
BASE_URL="http://localhost:5000"

# 1. Health Check
echo "=== Health Check ==="
curl -X GET "$BASE_URL/health"
echo -e "\n"

# 2. Create a Workflow Definition
echo "=== Creating Workflow Definition ==="
WORKFLOW_RESPONSE=$(curl -s -X POST "$BASE_URL/api/workflows" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Document Approval Process",
    "description": "Simple document approval workflow",
    "states": [
      {
        "id": "draft",
        "name": "Draft",
        "isInitial": true,
        "isFinal": false,
        "enabled": true,
        "description": "Document is being drafted"
      },
      {
        "id": "review",
        "name": "Under Review",
        "isInitial": false,
        "isFinal": false,
        "enabled": true,
        "description": "Document is being reviewed"
      },
      {
        "id": "approved",
        "name": "Approved",
        "isInitial": false,
        "isFinal": true,
        "enabled": true,
        "description": "Document has been approved"
      },
      {
        "id": "rejected",
        "name": "Rejected",
        "isInitial": false,
        "isFinal": true,
        "enabled": true,
        "description": "Document has been rejected"
      }
    ],
    "actions": [
      {
        "id": "submit",
        "name": "Submit for Review",
        "enabled": true,
        "fromStates": ["draft"],
        "toState": "review",
        "description": "Submit document for review"
      },
      {
        "id": "approve",
        "name": "Approve",
        "enabled": true,
        "fromStates": ["review"],
        "toState": "approved",
        "description": "Approve the document"
      },
      {
        "id": "reject",
        "name": "Reject",
        "enabled": true,
        "fromStates": ["review"],
        "toState": "rejected",
        "description": "Reject the document"
      },
      {
        "id": "revise",
        "name": "Send for Revision",
        "enabled": true,
        "fromStates": ["review"],
        "toState": "draft",
        "description": "Send document back for revision"
      }
    ]
  }')

echo $WORKFLOW_RESPONSE | jq '.'
echo -e "\n"

# Extract workflow ID (assuming jq is available)
WORKFLOW_ID=$(echo $WORKFLOW_RESPONSE | jq -r '.data.id')
echo "Created workflow with ID: $WORKFLOW_ID"
echo -e "\n"

# 3. Get All Workflow Definitions
echo "=== Getting All Workflow Definitions ==="
curl -s -X GET "$BASE_URL/api/workflows" | jq '.'
echo -e "\n"

# 4. Get Specific Workflow Definition
echo "=== Getting Specific Workflow Definition ==="
curl -s -X GET "$BASE_URL/api/workflows/$WORKFLOW_ID" | jq '.'
echo -e "\n"

# 5. Create a Workflow Instance
echo "=== Creating Workflow Instance ==="
INSTANCE_RESPONSE=$(curl -s -X POST "$BASE_URL/api/workflows/$WORKFLOW_ID/instances")
echo $INSTANCE_RESPONSE | jq '.'
echo -e "\n"

# Extract instance ID
INSTANCE_ID=$(echo $INSTANCE_RESPONSE | jq -r '.data.id')
echo "Created instance with ID: $INSTANCE_ID"
echo -e "\n"

# 6. Get Instance Status
echo "=== Getting Instance Status ==="
curl -s -X GET "$BASE_URL/api/instances/$INSTANCE_ID" | jq '.'
echo -e "\n"

# 7. Execute Actions
echo "=== Executing Action: Submit for Review ==="
curl -s -X POST "$BASE_URL/api/instances/$INSTANCE_ID/actions" \
  -H "Content-Type: application/json" \
  -d '{"actionId": "submit"}' | jq '.'
echo -e "\n"

echo "=== Instance Status After Submit ==="
curl -s -X GET "$BASE_URL/api/instances/$INSTANCE_ID" | jq '.'
echo -e "\n"

echo "=== Executing Action: Approve ==="
curl -s -X POST "$BASE_URL/api/instances/$INSTANCE_ID/actions" \
  -H "Content-Type: application/json" \
  -d '{"actionId": "approve"}' | jq '.'
echo -e "\n"

echo "=== Final Instance Status ==="
curl -s -X GET "$BASE_URL/api/instances/$INSTANCE_ID" | jq '.'
echo -e "\n"

# 8. Get All Instances
echo "=== Getting All Instances ==="
curl -s -X GET "$BASE_URL/api/instances" | jq '.'
echo -e "\n"

# 9. Try to execute action on final state (should fail)
echo "=== Trying to Execute Action on Final State (Should Fail) ==="
curl -s -X POST "$BASE_URL/api/instances/$INSTANCE_ID/actions" \
  -H "Content-Type: application/json" \
  -d '{"actionId": "submit"}' | jq '.'
echo -e "\n"

echo "=== Demo Complete ==="
