# Workflow Engine - Configurable State Machine API

A minimal .NET 8 backend service that provides a configurable workflow engine with state machine capabilities. This service allows clients to define workflows, start instances, execute actions, and inspect workflow states.

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK or later
- Any IDE that supports .NET development (Visual Studio, VS Code, JetBrains Rider)

### Running the Application
```bash
# Clone or download the project
cd WorkflowEngine

# Restore dependencies and run
dotnet run
```

The API will be available at:
- **Base URL**: `http://localhost:5000`
- **OpenAPI Documentation**: `http://localhost:5000/openapi/v1.json`
- **Health Check**: `http://localhost:5000/health`

## 📋 API Endpoints

### Workflow Definition Management
- `POST /api/workflows` - Create a new workflow definition
- `GET /api/workflows` - Get all workflow definitions
- `GET /api/workflows/{id}` - Get a specific workflow definition

### Workflow Instance Management
- `POST /api/workflows/{definitionId}/instances` - Create a new workflow instance
- `GET /api/instances` - Get all workflow instances
- `GET /api/instances/{id}` - Get a specific workflow instance
- `POST /api/instances/{id}/actions` - Execute an action on a workflow instance

### System
- `GET /health` - Health check endpoint

## 🔧 Usage Examples

### 1. Create a Workflow Definition

```bash
curl -X POST "http://localhost:5000/api/workflows" \
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
  }'
```

### 2. Create a Workflow Instance

```bash
curl -X POST "http://localhost:5000/api/workflows/{definitionId}/instances"
```

### 3. Execute an Action

```bash
curl -X POST "http://localhost:5000/api/instances/{instanceId}/actions" \
  -H "Content-Type: application/json" \
  -d '{
    "actionId": "submit"
  }'
```

### 4. Get Instance Status

```bash
curl -X GET "http://localhost:5000/api/instances/{instanceId}"
```

## 🏗️ Architecture & Design

### Core Components

1. **Models** (`/Models/`)
   - `State`: Represents workflow states with configuration flags
   - `WorkflowAction`: Represents transitions between states
   - `WorkflowDefinition`: Template containing states and actions
   - `WorkflowInstance`: Runtime instance with current state and history
   - `HistoryEntry`: Records action execution history

2. **Services** (`/Services/`)
   - `WorkflowService`: Core business logic and validation
   - `InMemoryWorkflowStore`: Simple in-memory persistence layer

3. **DTOs** (`/DTOs/`)
   - `CreateWorkflowDefinitionRequest`: API request for creating workflows
   - `ExecuteActionRequest`: API request for executing actions
   - `ApiResponse<T>`: Standard response wrapper

4. **API Layer** (`Program.cs`)
   - Minimal API endpoints with proper error handling
   - OpenAPI documentation support

### Design Principles

- **Clear Module Boundaries**: Each component has a single responsibility
- **Sensible Naming**: Self-documenting class and method names
- **Validation First**: Comprehensive validation before state changes
- **Error Handling**: Graceful error handling with helpful messages
- **Extensibility**: Easy to add new features without major rewrites

## 🔍 Validation Rules

### Workflow Definition Validation
- Must have exactly one initial state
- No duplicate state or action IDs
- All action references must point to valid states
- State and action names cannot be empty

### Action Execution Validation
- Action must belong to the instance's workflow definition
- Action must be enabled
- Current state must be in the action's `fromStates`
- Cannot execute actions on final states
- Cannot execute actions on disabled states

## 📊 Data Model

### State
```csharp
{
  "id": "string",           // Unique identifier
  "name": "string",         // Human-readable name
  "isInitial": bool,        // Marks the starting state
  "isFinal": bool,          // Marks end states
  "enabled": bool,          // Controls state availability
  "description": "string?"  // Optional description
}
```

### Action (Transition)
```csharp
{
  "id": "string",           // Unique identifier
  "name": "string",         // Human-readable name
  "enabled": bool,          // Controls action availability
  "fromStates": ["string"], // Source states (multiple allowed)
  "toState": "string",      // Target state (single)
  "description": "string?"  // Optional description
}
```

### Workflow Instance
```csharp
{
  "id": "string",           // Unique identifier
  "definitionId": "string", // Reference to workflow definition
  "currentStateId": "string", // Current state
  "history": [HistoryEntry], // Action execution history
  "createdAt": "datetime"   // Creation timestamp
}
```

## 🔧 Configuration & Assumptions

### Assumptions Made
1. **In-Memory Storage**: Data is lost on application restart (suitable for demo/testing)
2. **Single Instance**: No distributed deployment considerations
3. **No Authentication**: Public API without security (demo purposes only)
4. **Simple Concurrency**: Basic thread-safe collections, no complex locking
5. **No Persistence**: No database or file-based storage
6. **Minimal Dependencies**: Only uses built-in .NET libraries

### Known Limitations
1. **Data Persistence**: All data is lost when the application restarts
2. **Scalability**: Not designed for high-throughput scenarios
3. **Concurrency**: Basic thread safety, not optimized for heavy concurrent usage
4. **Monitoring**: No built-in metrics or monitoring capabilities
5. **Security**: No authentication, authorization, or input sanitization

## 🛠️ Development & Testing

### Project Structure
```
WorkflowEngine/
├── Models/
│   ├── State.cs
│   ├── WorkflowAction.cs
│   ├── WorkflowDefinition.cs
│   ├── WorkflowInstance.cs
│   └── HistoryEntry.cs
├── Services/
│   ├── WorkflowService.cs
│   └── InMemoryWorkflowStore.cs
├── DTOs/
│   └── ApiDTOs.cs
├── Program.cs
└── README.md
```

### Running Tests
```bash
# Build the project
dotnet build

# Run the application
dotnet run

# Check health
curl http://localhost:5000/health
```

## 🚀 Future Enhancements

### If More Time Was Available
1. **Persistence**: Add file-based or database persistence
2. **Authentication**: Implement API key or JWT authentication
3. **Validation**: Add more sophisticated validation rules
4. **Monitoring**: Add logging, metrics, and health checks
5. **Performance**: Optimize for high-throughput scenarios
6. **Testing**: Add comprehensive unit and integration tests
7. **Documentation**: Add more detailed API documentation
8. **Deployment**: Add Docker support and deployment scripts

### Potential Extensions
- **Workflow Versioning**: Support multiple versions of workflow definitions
- **Conditional Actions**: Add support for conditional transitions
- **Parallel Execution**: Support for parallel workflow branches
- **Timeouts**: Add timeout support for states and actions
- **Notifications**: Add event-based notifications for state changes
- **Audit Trail**: Enhanced audit logging and reporting
- **Visual Designer**: Web-based workflow designer interface

## 📄 License

This project is created as a take-home exercise and is intended for evaluation purposes.

---

**Note**: This is a minimal implementation focusing on core functionality and clean architecture. Production use would require additional security, persistence, and scalability considerations.
