namespace WorkflowEngine.Models;

/// <summary>
/// Represents a transition action in a workflow definition.
/// Actions define how workflow instances can move between states.
/// </summary>
public class WorkflowAction
{
    /// <summary>
    /// Unique identifier for the action within a workflow definition.
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Human-readable name for the action.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Indicates if this action is currently enabled.
    /// Disabled actions cannot be executed.
    /// </summary>
    public bool Enabled { get; set; } = true;
    
    /// <summary>
    /// Collection of state IDs from which this action can be executed.
    /// An action can have multiple source states but only one target state.
    /// </summary>
    public List<string> FromStates { get; set; } = new();
    
    /// <summary>
    /// The state ID to which this action transitions the workflow instance.
    /// </summary>
    public string ToState { get; set; } = string.Empty;
    
    /// <summary>
    /// Optional description providing additional context about the action.
    /// </summary>
    public string? Description { get; set; }
}
