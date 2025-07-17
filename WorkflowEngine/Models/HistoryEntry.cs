namespace WorkflowEngine.Models;

/// <summary>
/// Represents a single entry in a workflow instance's execution history.
/// </summary>
public class HistoryEntry
{
    /// <summary>
    /// ID of the action that was executed.
    /// </summary>
    public string ActionId { get; set; } = string.Empty;
    
    /// <summary>
    /// Name of the action that was executed.
    /// </summary>
    public string ActionName { get; set; } = string.Empty;
    
    /// <summary>
    /// ID of the state before the action was executed.
    /// </summary>
    public string FromStateId { get; set; } = string.Empty;
    
    /// <summary>
    /// ID of the state after the action was executed.
    /// </summary>
    public string ToStateId { get; set; } = string.Empty;
    
    /// <summary>
    /// Timestamp when the action was executed.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
