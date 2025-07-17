using WorkflowEngine.Models;

namespace WorkflowEngine.DTOs;

/// <summary>
/// Data transfer object for creating a new workflow definition.
/// </summary>
public class CreateWorkflowDefinitionRequest
{
    /// <summary>
    /// Name of the workflow definition.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Collection of states in this workflow.
    /// </summary>
    public List<State> States { get; set; } = new();
    
    /// <summary>
    /// Collection of actions (transitions) in this workflow.
    /// </summary>
    public List<WorkflowAction> Actions { get; set; } = new();
    
    /// <summary>
    /// Optional description of the workflow.
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// Data transfer object for executing an action on a workflow instance.
/// </summary>
public class ExecuteActionRequest
{
    /// <summary>
    /// ID of the action to execute.
    /// </summary>
    public string ActionId { get; set; } = string.Empty;
}

/// <summary>
/// Standard API response wrapper for error handling.
/// </summary>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates if the operation was successful.
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// The response data if successful.
    /// </summary>
    public T? Data { get; set; }
    
    /// <summary>
    /// Error message if the operation failed.
    /// </summary>
    public string? Error { get; set; }
    
    /// <summary>
    /// Creates a successful response.
    /// </summary>
    public static ApiResponse<T> Ok(T data) => new() { Success = true, Data = data };
    
    /// <summary>
    /// Creates an error response.
    /// </summary>
    public static ApiResponse<T> Fail(string error) => new() { Success = false, Error = error };
}
