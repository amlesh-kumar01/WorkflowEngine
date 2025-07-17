namespace WorkflowEngine.Models;

/// <summary>
/// Represents a state in a workflow definition.
/// Each state has a unique ID, name, and configuration flags.
/// </summary>
public class State
{
    /// <summary>
    /// Unique identifier for the state within a workflow definition.
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Human-readable name for the state.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Indicates if this is the initial state of the workflow.
    /// Each workflow must have exactly one initial state.
    /// </summary>
    public bool IsInitial { get; set; }
    
    /// <summary>
    /// Indicates if this is a final state of the workflow.
    /// No actions can be executed from final states.
    /// </summary>
    public bool IsFinal { get; set; }
    
    /// <summary>
    /// Indicates if this state is currently enabled.
    /// Actions cannot be executed from disabled states.
    /// </summary>
    public bool Enabled { get; set; } = true;
    
    /// <summary>
    /// Optional description providing additional context about the state.
    /// </summary>
    public string? Description { get; set; }
}
