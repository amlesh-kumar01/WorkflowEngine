namespace WorkflowEngine.Models;

/// <summary>
/// Represents a workflow definition consisting of states and actions.
/// This is the template from which workflow instances are created.
/// </summary>
public class WorkflowDefinition
{
    /// <summary>
    /// Unique identifier for the workflow definition.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Human-readable name for the workflow definition.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Collection of states that define the workflow's structure.
    /// Must contain exactly one initial state.
    /// </summary>
    public List<State> States { get; set; } = new();
    
    /// <summary>
    /// Collection of actions that define valid transitions between states.
    /// </summary>
    public List<WorkflowAction> Actions { get; set; } = new();
    
    /// <summary>
    /// Optional description providing additional context about the workflow.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Timestamp when the workflow definition was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Gets the initial state of this workflow definition.
    /// </summary>
    /// <returns>The initial state, or null if none exists.</returns>
    public State? GetInitialState() => States.FirstOrDefault(s => s.IsInitial);
}
