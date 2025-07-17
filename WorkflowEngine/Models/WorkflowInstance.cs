namespace WorkflowEngine.Models;

/// <summary>
/// Represents a runtime instance of a workflow definition.
/// Each instance tracks its current state and execution history.
/// </summary>
public class WorkflowInstance
{
    /// <summary>
    /// Unique identifier for the workflow instance.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Reference to the workflow definition this instance is based on.
    /// </summary>
    public string DefinitionId { get; set; } = string.Empty;
    
    /// <summary>
    /// ID of the current state of this workflow instance.
    /// </summary>
    public string CurrentStateId { get; set; } = string.Empty;
    
    /// <summary>
    /// Basic history of actions executed on this instance.
    /// Ordered chronologically from oldest to newest.
    /// </summary>
    public List<HistoryEntry> History { get; set; } = new();
    
    /// <summary>
    /// Timestamp when the workflow instance was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Gets the latest history entry if any actions have been executed.
    /// </summary>
    /// <returns>The most recent history entry, or null if no actions have been executed.</returns>
    public HistoryEntry? GetLatestHistoryEntry() => History.LastOrDefault();
}
