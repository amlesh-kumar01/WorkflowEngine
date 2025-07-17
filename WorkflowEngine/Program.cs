using WorkflowEngine.Services;
using WorkflowEngine.DTOs;
using WorkflowEngine.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddOpenApi();

// Register workflow services as singletons for in-memory storage
builder.Services.AddSingleton<InMemoryWorkflowStore>();
builder.Services.AddSingleton<WorkflowService>();

// Add CORS for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors("AllowAll");
}

// Workflow Definition Endpoints

/// <summary>
/// Creates a new workflow definition.
/// </summary>
app.MapPost("/api/workflows", (CreateWorkflowDefinitionRequest request, WorkflowService service) =>
{
    try
    {
        var definition = service.CreateWorkflowDefinition(request);
        return Results.Created($"/api/workflows/{definition.Id}", ApiResponse<WorkflowDefinition>.Ok(definition));
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(ApiResponse<WorkflowDefinition>.Fail(ex.Message));
    }
    catch (Exception ex)
    {
        return Results.Problem($"An unexpected error occurred: {ex.Message}");
    }
})
.WithName("CreateWorkflowDefinition")
.WithSummary("Create a new workflow definition")
.WithDescription("Creates a new workflow definition with states and actions. Validates that there is exactly one initial state and all references are valid.")
.WithOpenApi();

/// <summary>
/// Retrieves all workflow definitions.
/// </summary>
app.MapGet("/api/workflows", (WorkflowService service) =>
{
    try
    {
        var definitions = service.GetAllWorkflowDefinitions();
        return Results.Ok(ApiResponse<List<WorkflowDefinition>>.Ok(definitions));
    }
    catch (Exception ex)
    {
        return Results.Problem($"An unexpected error occurred: {ex.Message}");
    }
})
.WithName("GetAllWorkflowDefinitions")
.WithSummary("Get all workflow definitions")
.WithDescription("Retrieves a list of all workflow definitions in the system.")
.WithOpenApi();

/// <summary>
/// Retrieves a specific workflow definition by ID.
/// </summary>
app.MapGet("/api/workflows/{id}", (string id, WorkflowService service) =>
{
    try
    {
        var definition = service.GetWorkflowDefinition(id);
        if (definition == null)
        {
            return Results.NotFound(ApiResponse<WorkflowDefinition>.Fail($"Workflow definition with ID '{id}' not found."));
        }
        return Results.Ok(ApiResponse<WorkflowDefinition>.Ok(definition));
    }
    catch (Exception ex)
    {
        return Results.Problem($"An unexpected error occurred: {ex.Message}");
    }
})
.WithName("GetWorkflowDefinition")
.WithSummary("Get a workflow definition by ID")
.WithDescription("Retrieves a specific workflow definition by its unique identifier.")
.WithOpenApi();

// Workflow Instance Endpoints

/// <summary>
/// Creates a new workflow instance from a workflow definition.
/// </summary>
app.MapPost("/api/workflows/{definitionId}/instances", (string definitionId, WorkflowService service) =>
{
    try
    {
        var instance = service.CreateWorkflowInstance(definitionId);
        return Results.Created($"/api/instances/{instance.Id}", ApiResponse<WorkflowInstance>.Ok(instance));
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(ApiResponse<WorkflowInstance>.Fail(ex.Message));
    }
    catch (Exception ex)
    {
        return Results.Problem($"An unexpected error occurred: {ex.Message}");
    }
})
.WithName("CreateWorkflowInstance")
.WithSummary("Create a new workflow instance")
.WithDescription("Creates a new workflow instance from a workflow definition. The instance starts in the initial state.")
.WithOpenApi();

/// <summary>
/// Retrieves all workflow instances.
/// </summary>
app.MapGet("/api/instances", (WorkflowService service) =>
{
    try
    {
        var instances = service.GetAllWorkflowInstances();
        return Results.Ok(ApiResponse<List<WorkflowInstance>>.Ok(instances));
    }
    catch (Exception ex)
    {
        return Results.Problem($"An unexpected error occurred: {ex.Message}");
    }
})
.WithName("GetAllWorkflowInstances")
.WithSummary("Get all workflow instances")
.WithDescription("Retrieves a list of all workflow instances in the system.")
.WithOpenApi();

/// <summary>
/// Retrieves a specific workflow instance by ID.
/// </summary>
app.MapGet("/api/instances/{id}", (string id, WorkflowService service) =>
{
    try
    {
        var instance = service.GetWorkflowInstance(id);
        if (instance == null)
        {
            return Results.NotFound(ApiResponse<WorkflowInstance>.Fail($"Workflow instance with ID '{id}' not found."));
        }
        return Results.Ok(ApiResponse<WorkflowInstance>.Ok(instance));
    }
    catch (Exception ex)
    {
        return Results.Problem($"An unexpected error occurred: {ex.Message}");
    }
})
.WithName("GetWorkflowInstance")
.WithSummary("Get a workflow instance by ID")
.WithDescription("Retrieves a specific workflow instance by its unique identifier, including current state and execution history.")
.WithOpenApi();

/// <summary>
/// Executes an action on a workflow instance.
/// </summary>
app.MapPost("/api/instances/{id}/actions", (string id, ExecuteActionRequest request, WorkflowService service) =>
{
    try
    {
        var instance = service.ExecuteAction(id, request.ActionId);
        return Results.Ok(ApiResponse<WorkflowInstance>.Ok(instance));
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(ApiResponse<WorkflowInstance>.Fail(ex.Message));
    }
    catch (Exception ex)
    {
        return Results.Problem($"An unexpected error occurred: {ex.Message}");
    }
})
.WithName("ExecuteAction")
.WithSummary("Execute an action on a workflow instance")
.WithDescription("Executes an action on a workflow instance, transitioning it to a new state. Validates that the action can be executed from the current state.")
.WithOpenApi();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
.WithName("HealthCheck")
.WithSummary("Health check endpoint")
.WithDescription("Returns the current health status of the workflow engine.")
.WithOpenApi();

app.Run();
