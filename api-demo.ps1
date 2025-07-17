# Workflow Engine API Demo Script (PowerShell)
# This file contains sample PowerShell commands to demonstrate the Workflow Engine API
# Run these commands after starting the application with: dotnet run

# Base URL
$BASE_URL = "http://localhost:5000"

Write-Host "=== Health Check ===" -ForegroundColor Green
$healthResponse = Invoke-RestMethod -Uri "$BASE_URL/health" -Method Get
$healthResponse | ConvertTo-Json
Write-Host ""

Write-Host "=== Creating Workflow Definition ===" -ForegroundColor Green
$workflowDefinition = @{
    name = "Document Approval Process"
    description = "Simple document approval workflow"
    states = @(
        @{
            id = "draft"
            name = "Draft"
            isInitial = $true
            isFinal = $false
            enabled = $true
            description = "Document is being drafted"
        },
        @{
            id = "review"
            name = "Under Review"
            isInitial = $false
            isFinal = $false
            enabled = $true
            description = "Document is being reviewed"
        },
        @{
            id = "approved"
            name = "Approved"
            isInitial = $false
            isFinal = $true
            enabled = $true
            description = "Document has been approved"
        },
        @{
            id = "rejected"
            name = "Rejected"
            isInitial = $false
            isFinal = $true
            enabled = $true
            description = "Document has been rejected"
        }
    )
    actions = @(
        @{
            id = "submit"
            name = "Submit for Review"
            enabled = $true
            fromStates = @("draft")
            toState = "review"
            description = "Submit document for review"
        },
        @{
            id = "approve"
            name = "Approve"
            enabled = $true
            fromStates = @("review")
            toState = "approved"
            description = "Approve the document"
        },
        @{
            id = "reject"
            name = "Reject"
            enabled = $true
            fromStates = @("review")
            toState = "rejected"
            description = "Reject the document"
        },
        @{
            id = "revise"
            name = "Send for Revision"
            enabled = $true
            fromStates = @("review")
            toState = "draft"
            description = "Send document back for revision"
        }
    )
}

$workflowJson = $workflowDefinition | ConvertTo-Json -Depth 10
$workflowResponse = Invoke-RestMethod -Uri "$BASE_URL/api/workflows" -Method Post -Body $workflowJson -ContentType "application/json"
$workflowResponse | ConvertTo-Json
Write-Host ""

$workflowId = $workflowResponse.data.id
Write-Host "Created workflow with ID: $workflowId" -ForegroundColor Yellow
Write-Host ""

Write-Host "=== Getting All Workflow Definitions ===" -ForegroundColor Green
$allWorkflows = Invoke-RestMethod -Uri "$BASE_URL/api/workflows" -Method Get
$allWorkflows | ConvertTo-Json
Write-Host ""

Write-Host "=== Getting Specific Workflow Definition ===" -ForegroundColor Green
$specificWorkflow = Invoke-RestMethod -Uri "$BASE_URL/api/workflows/$workflowId" -Method Get
$specificWorkflow | ConvertTo-Json
Write-Host ""

Write-Host "=== Creating Workflow Instance ===" -ForegroundColor Green
$instanceResponse = Invoke-RestMethod -Uri "$BASE_URL/api/workflows/$workflowId/instances" -Method Post
$instanceResponse | ConvertTo-Json
Write-Host ""

$instanceId = $instanceResponse.data.id
Write-Host "Created instance with ID: $instanceId" -ForegroundColor Yellow
Write-Host ""

Write-Host "=== Getting Instance Status ===" -ForegroundColor Green
$instanceStatus = Invoke-RestMethod -Uri "$BASE_URL/api/instances/$instanceId" -Method Get
$instanceStatus | ConvertTo-Json
Write-Host ""

Write-Host "=== Executing Action: Submit for Review ===" -ForegroundColor Green
$submitAction = @{ actionId = "submit" }
$submitJson = $submitAction | ConvertTo-Json
$submitResponse = Invoke-RestMethod -Uri "$BASE_URL/api/instances/$instanceId/actions" -Method Post -Body $submitJson -ContentType "application/json"
$submitResponse | ConvertTo-Json
Write-Host ""

Write-Host "=== Instance Status After Submit ===" -ForegroundColor Green
$instanceStatusAfterSubmit = Invoke-RestMethod -Uri "$BASE_URL/api/instances/$instanceId" -Method Get
$instanceStatusAfterSubmit | ConvertTo-Json
Write-Host ""

Write-Host "=== Executing Action: Approve ===" -ForegroundColor Green
$approveAction = @{ actionId = "approve" }
$approveJson = $approveAction | ConvertTo-Json
$approveResponse = Invoke-RestMethod -Uri "$BASE_URL/api/instances/$instanceId/actions" -Method Post -Body $approveJson -ContentType "application/json"
$approveResponse | ConvertTo-Json
Write-Host ""

Write-Host "=== Final Instance Status ===" -ForegroundColor Green
$finalInstanceStatus = Invoke-RestMethod -Uri "$BASE_URL/api/instances/$instanceId" -Method Get
$finalInstanceStatus | ConvertTo-Json
Write-Host ""

Write-Host "=== Getting All Instances ===" -ForegroundColor Green
$allInstances = Invoke-RestMethod -Uri "$BASE_URL/api/instances" -Method Get
$allInstances | ConvertTo-Json
Write-Host ""

Write-Host "=== Trying to Execute Action on Final State (Should Fail) ===" -ForegroundColor Green
try {
    $invalidAction = @{ actionId = "submit" }
    $invalidJson = $invalidAction | ConvertTo-Json
    $invalidResponse = Invoke-RestMethod -Uri "$BASE_URL/api/instances/$instanceId/actions" -Method Post -Body $invalidJson -ContentType "application/json"
    $invalidResponse | ConvertTo-Json
} catch {
    Write-Host "Expected error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

Write-Host "=== Demo Complete ===" -ForegroundColor Green
