# API Usage Examples

This document provides comprehensive examples of how to use the Workflow Engine API.

## Base URL
```
http://localhost:5000
```

## 1. Health Check

Check if the API is running:

```bash
curl -X GET http://localhost:5000/health
```

**Response:**
```json
{
  "status": "Healthy",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

## 2. Create Workflow Definition

Create a simple approval workflow:

```bash
curl -X POST http://localhost:5000/api/workflows \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Document Approval",
    "description": "Simple document approval workflow",
    "states": [
      {
        "id": "draft",
        "name": "Draft",
        "isInitial": true,
        "isFinal": false,
        "isDisabled": false
      },
      {
        "id": "review",
        "name": "Under Review",
        "isInitial": false,
        "isFinal": false,
        "isDisabled": false
      },
      {
        "id": "approved",
        "name": "Approved",
        "isInitial": false,
        "isFinal": true,
        "isDisabled": false
      },
      {
        "id": "rejected",
        "name": "Rejected",
        "isInitial": false,
        "isFinal": true,
        "isDisabled": false
      }
    ],
    "actions": [
      {
        "id": "submit",
        "name": "Submit for Review",
        "fromStates": ["draft"],
        "toState": "review"
      },
      {
        "id": "approve",
        "name": "Approve Document",
        "fromStates": ["review"],
        "toState": "approved"
      },
      {
        "id": "reject",
        "name": "Reject Document",
        "fromStates": ["review"],
        "toState": "rejected"
      }
    ]
  }'
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "name": "Document Approval",
    "description": "Simple document approval workflow",
    "states": [...],
    "actions": [...],
    "createdAt": "2024-01-15T10:30:00Z"
  }
}
```

## 3. Get All Workflow Definitions

```bash
curl -X GET http://localhost:5000/api/workflows
```

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "name": "Document Approval",
      "description": "Simple document approval workflow",
      "states": [...],
      "actions": [...],
      "createdAt": "2024-01-15T10:30:00Z"
    }
  ]
}
```

## 4. Get Specific Workflow Definition

```bash
curl -X GET http://localhost:5000/api/workflows/550e8400-e29b-41d4-a716-446655440000
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "name": "Document Approval",
    "description": "Simple document approval workflow",
    "states": [...],
    "actions": [...],
    "createdAt": "2024-01-15T10:30:00Z"
  }
}
```

## 5. Create Workflow Instance

Create an instance of the workflow:

```bash
curl -X POST http://localhost:5000/api/workflows/550e8400-e29b-41d4-a716-446655440000/instances \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Contract Review - Client ABC"
  }'
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": "660e8400-e29b-41d4-a716-446655440001",
    "name": "Contract Review - Client ABC",
    "workflowDefinitionId": "550e8400-e29b-41d4-a716-446655440000",
    "currentState": "draft",
    "createdAt": "2024-01-15T10:35:00Z",
    "lastUpdatedAt": "2024-01-15T10:35:00Z",
    "history": [
      {
        "fromState": null,
        "toState": "draft",
        "actionId": null,
        "timestamp": "2024-01-15T10:35:00Z"
      }
    ]
  }
}
```

## 6. Get All Workflow Instances

```bash
curl -X GET http://localhost:5000/api/instances
```

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": "660e8400-e29b-41d4-a716-446655440001",
      "name": "Contract Review - Client ABC",
      "workflowDefinitionId": "550e8400-e29b-41d4-a716-446655440000",
      "currentState": "draft",
      "createdAt": "2024-01-15T10:35:00Z",
      "lastUpdatedAt": "2024-01-15T10:35:00Z",
      "history": [...]
    }
  ]
}
```

## 7. Get Specific Workflow Instance

```bash
curl -X GET http://localhost:5000/api/instances/660e8400-e29b-41d4-a716-446655440001
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": "660e8400-e29b-41d4-a716-446655440001",
    "name": "Contract Review - Client ABC",
    "workflowDefinitionId": "550e8400-e29b-41d4-a716-446655440000",
    "currentState": "draft",
    "createdAt": "2024-01-15T10:35:00Z",
    "lastUpdatedAt": "2024-01-15T10:35:00Z",
    "history": [
      {
        "fromState": null,
        "toState": "draft",
        "actionId": null,
        "timestamp": "2024-01-15T10:35:00Z"
      }
    ]
  }
}
```

## 8. Execute Action on Workflow Instance

Submit the document for review:

```bash
curl -X POST http://localhost:5000/api/instances/660e8400-e29b-41d4-a716-446655440001/actions \
  -H "Content-Type: application/json" \
  -d '{
    "actionId": "submit"
  }'
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": "660e8400-e29b-41d4-a716-446655440001",
    "name": "Contract Review - Client ABC",
    "workflowDefinitionId": "550e8400-e29b-41d4-a716-446655440000",
    "currentState": "review",
    "createdAt": "2024-01-15T10:35:00Z",
    "lastUpdatedAt": "2024-01-15T10:40:00Z",
    "history": [
      {
        "fromState": null,
        "toState": "draft",
        "actionId": null,
        "timestamp": "2024-01-15T10:35:00Z"
      },
      {
        "fromState": "draft",
        "toState": "review",
        "actionId": "submit",
        "timestamp": "2024-01-15T10:40:00Z"
      }
    ]
  }
}
```

## 9. Complete Workflow - Approve Document

```bash
curl -X POST http://localhost:5000/api/instances/660e8400-e29b-41d4-a716-446655440001/actions \
  -H "Content-Type: application/json" \
  -d '{
    "actionId": "approve"
  }'
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": "660e8400-e29b-41d4-a716-446655440001",
    "name": "Contract Review - Client ABC",
    "workflowDefinitionId": "550e8400-e29b-41d4-a716-446655440000",
    "currentState": "approved",
    "createdAt": "2024-01-15T10:35:00Z",
    "lastUpdatedAt": "2024-01-15T10:45:00Z",
    "history": [
      {
        "fromState": null,
        "toState": "draft",
        "actionId": null,
        "timestamp": "2024-01-15T10:35:00Z"
      },
      {
        "fromState": "draft",
        "toState": "review",
        "actionId": "submit",
        "timestamp": "2024-01-15T10:40:00Z"
      },
      {
        "fromState": "review",
        "toState": "approved",
        "actionId": "approve",
        "timestamp": "2024-01-15T10:45:00Z"
      }
    ]
  }
}
```

## Error Examples

### Invalid Action Execution
Trying to execute an invalid action:

```bash
curl -X POST http://localhost:5000/api/instances/660e8400-e29b-41d4-a716-446655440001/actions \
  -H "Content-Type: application/json" \
  -d '{
    "actionId": "invalid_action"
  }'
```

**Response:**
```json
{
  "success": false,
  "error": "Action 'invalid_action' is not valid for current state 'draft'"
}
```

### Action on Final State
Trying to execute action on a final state:

```bash
curl -X POST http://localhost:5000/api/instances/660e8400-e29b-41d4-a716-446655440001/actions \
  -H "Content-Type: application/json" \
  -d '{
    "actionId": "submit"
  }'
```

**Response:**
```json
{
  "success": false,
  "error": "Cannot execute actions on final state 'approved'"
}
```

## Complex Workflow Example

Here's a more complex e-commerce order workflow:

```bash
curl -X POST http://localhost:5000/api/workflows \
  -H "Content-Type: application/json" \
  -d '{
    "name": "E-commerce Order Processing",
    "description": "Complete order processing workflow",
    "states": [
      {
        "id": "pending",
        "name": "Pending Payment",
        "isInitial": true,
        "isFinal": false,
        "isDisabled": false
      },
      {
        "id": "paid",
        "name": "Payment Confirmed",
        "isInitial": false,
        "isFinal": false,
        "isDisabled": false
      },
      {
        "id": "processing",
        "name": "Processing Order",
        "isInitial": false,
        "isFinal": false,
        "isDisabled": false
      },
      {
        "id": "shipped",
        "name": "Shipped",
        "isInitial": false,
        "isFinal": false,
        "isDisabled": false
      },
      {
        "id": "delivered",
        "name": "Delivered",
        "isInitial": false,
        "isFinal": true,
        "isDisabled": false
      },
      {
        "id": "cancelled",
        "name": "Cancelled",
        "isInitial": false,
        "isFinal": true,
        "isDisabled": false
      }
    ],
    "actions": [
      {
        "id": "pay",
        "name": "Confirm Payment",
        "fromStates": ["pending"],
        "toState": "paid"
      },
      {
        "id": "process",
        "name": "Start Processing",
        "fromStates": ["paid"],
        "toState": "processing"
      },
      {
        "id": "ship",
        "name": "Ship Order",
        "fromStates": ["processing"],
        "toState": "shipped"
      },
      {
        "id": "deliver",
        "name": "Mark as Delivered",
        "fromStates": ["shipped"],
        "toState": "delivered"
      },
      {
        "id": "cancel",
        "name": "Cancel Order",
        "fromStates": ["pending", "paid", "processing"],
        "toState": "cancelled"
      }
    ]
  }'
```

This workflow demonstrates:
- Multiple states with different properties
- Actions that can be executed from multiple states
- Final states that end the workflow
- Complex business logic flow
