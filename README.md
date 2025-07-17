# Configurable Workflow Engine

**Assignment Submission for Infonetica**

A configurable workflow engine implementation using .NET 8 and C# that allows users to define workflows as state machines and execute them with comprehensive validation.

## Overview

This project implements a workflow engine that manages business processes through state machines. Users can define workflows with states and actions, create instances of these workflows, and execute actions to transition between states while maintaining validation rules and history tracking.

## Assumptions Made

During the implementation of this workflow engine, the following assumptions were made:

### Workflow Definition Assumptions
- Each workflow must have exactly one initial state to ensure deterministic starting point
- State and action IDs must be unique within a workflow definition to avoid conflicts
- All states and actions must have meaningful names for better usability
- Final states cannot have outgoing transitions as they represent end points
- Disabled states cannot execute actions but can be transitioned to

### State Machine Assumptions
- State transitions are atomic operations that either succeed completely or fail
- History tracking is required for all state changes for audit purposes
- Multiple actions can lead to the same target state from different source states
- Actions can have multiple source states but only one target state

### Data Persistence Assumptions
- In-memory storage is acceptable for this assignment scope
- Thread safety is required for concurrent access to workflow data
- Data does not need to persist between application restarts
- Unique identifiers (GUIDs) are sufficient for entity identification

### API Design Assumptions
- RESTful design principles should be followed for consistency
- All responses should follow a consistent format with success/error indicators
- Error messages should be descriptive enough for troubleshooting
- OpenAPI documentation should be available for API exploration

### Validation Assumptions
- Business rule validation should occur at the service layer
- Invalid operations should throw meaningful exceptions
- All user inputs should be validated before processing
- State transition validation should prevent invalid workflow states

### Performance Assumptions
- The system should handle moderate concurrent load for demonstration purposes
- Response times under 100ms are acceptable for typical operations
- Memory usage should be reasonable for the scope of this assignment
- Database persistence is not required but architecture should support future enhancement

## Known Limitations

- **Data Persistence**: Uses in-memory storage, data is lost when application restarts
- **Concurrency**: Basic thread safety using ConcurrentDictionary, not optimized for high concurrency
- **Validation**: Business rules are hardcoded, not configurable
- **Authentication**: No authentication or authorization implemented
- **Logging**: Minimal logging, uses default ASP.NET Core logging
- **Error Handling**: Generic exception handling, could be more specific
- **Performance**: Not optimized for large numbers of workflows or instances

## Features

- Define workflows as state machines with states and actions
- Create workflow instances from definitions
- Execute actions on instances with validation
- Track workflow history and current state
- Comprehensive validation rules for state transitions
- RESTful API for all operations
- Thread-safe in-memory storage

## Technology Stack

- .NET 8
- C# 12
- ASP.NET Core Minimal APIs
- Swagger/OpenAPI for documentation
- xUnit for testing

## Getting Started

### Prerequisites
- .NET 8 SDK

### Running the Application
```bash
cd WorkflowEngine
dotnet run
```

The API will be available at: `http://localhost:5000`

### Testing
```bash
dotnet test
```

### API Documentation
Open `http://localhost:5000/swagger` for interactive API documentation

## Quick Start Instructions

### Build and Run
```bash
# Clone the repository
git clone <repository-url>
cd WorkflowEngine

# Build the project
dotnet build

# Run the application
dotnet run --project WorkflowEngine

# Run tests
dotnet test
```

### Environment Notes
- Requires .NET 8 SDK
- Application runs on http://localhost:5000
- Uses in-memory storage (data is lost on restart)
- CORS enabled for development

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/health` | Health check |
| POST | `/api/workflows` | Create workflow definition |
| GET | `/api/workflows` | List all workflow definitions |
| GET | `/api/workflows/{id}` | Get specific workflow definition |
| POST | `/api/workflows/{id}/instances` | Create workflow instance |
| GET | `/api/instances` | List all workflow instances |
| GET | `/api/instances/{id}` | Get specific workflow instance |
| POST | `/api/instances/{id}/actions` | Execute action on instance |

## Project Structure

```
WorkflowEngine/
├── WorkflowEngine/
│   ├── Models/                    # Domain models
│   ├── Services/                  # Business logic
│   ├── DTOs/                      # Data transfer objects
│   └── Program.cs                 # API endpoints
├── WorkflowEngine.Tests/          # Unit tests
└── WorkflowEngine.http           # HTTP test file
```

## Implementation Details

### Models
- **State**: Represents a workflow state with configuration
- **WorkflowAction**: Defines transitions between states
- **WorkflowDefinition**: Template for creating workflow instances
- **WorkflowInstance**: Runtime instance of a workflow
- **HistoryEntry**: Audit trail for state changes

### Services
- **WorkflowService**: Core business logic for workflow operations
- **InMemoryWorkflowStore**: Thread-safe in-memory persistence

### Validation Rules
- Exactly one initial state per workflow
- No duplicate state or action IDs
- All action references must point to valid states
- Actions cannot be executed on final or disabled states
- State transitions must follow defined rules

## Usage Example

For detailed API usage and sample requests/responses, see [API_EXAMPLES.md](API_EXAMPLES.md).

Creating a workflow definition:
```bash
curl -X POST http://localhost:5000/api/workflows \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Order Processing",
    "states": [
      { "id": "pending", "name": "Pending", "isInitial": true },
      { "id": "processing", "name": "Processing" },
      { "id": "completed", "name": "Completed", "isFinal": true }
    ],
    "actions": [
      { "id": "start", "name": "Start Processing", "fromStates": ["pending"], "toState": "processing" },
      { "id": "complete", "name": "Complete Order", "fromStates": ["processing"], "toState": "completed" }
    ]
  }'
```

## Testing

The project includes comprehensive unit tests covering:
- Workflow definition creation and validation
- State machine transitions
- Action execution with business rules
- Error handling scenarios

## Design Decisions

### Architecture
Clean separation between models, services, and DTOs following SOLID principles with dependency injection for testability.

### State Machine Implementation
Immutable state transitions with comprehensive validation and complete history tracking for audit purposes.

### Thread Safety
Used ConcurrentDictionary for thread-safe operations in the in-memory store.

### API Design
RESTful endpoints with consistent error handling and OpenAPI documentation for better developer experience.

## Author

Amlesh Kumar
Email: amleshkr396@gmail.com
Contact No.:6299855492
Roll No.: 23MT10005