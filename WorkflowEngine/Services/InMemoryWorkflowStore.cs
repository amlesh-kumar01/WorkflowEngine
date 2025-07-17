using WorkflowEngine.Models;
using System.Collections.Concurrent;

namespace WorkflowEngine.Services;

/// <summary>
/// Simple in-memory persistence for workflow definitions and instances.
/// Uses thread-safe concurrent collections for basic concurrency support.
/// 
/// TODO: For production use, consider:
/// - File-based persistence for data durability
/// - Database integration for scalability
/// - Distributed caching for multi-instance deployments
/// </summary>
public class InMemoryWorkflowStore
{
    private readonly ConcurrentDictionary<string, WorkflowDefinition> _definitions = new();
    private readonly ConcurrentDictionary<string, WorkflowInstance> _instances = new();
    
    #region Workflow Definition Operations
    
    /// <summary>
    /// Saves a workflow definition to the in-memory store.
    /// </summary>
    /// <param name="definition">The workflow definition to save.</param>
    /// <returns>The saved workflow definition.</returns>
    public WorkflowDefinition SaveDefinition(WorkflowDefinition definition)
    {
        _definitions[definition.Id] = definition;
        return definition;
    }
    
    /// <summary>
    /// Retrieves a workflow definition by its ID.
    /// </summary>
    /// <param name="id">The ID of the workflow definition to retrieve.</param>
    /// <returns>The workflow definition if found, otherwise null.</returns>
    public WorkflowDefinition? GetDefinition(string id)
    {
        return _definitions.TryGetValue(id, out var definition) ? definition : null;
    }
    
    /// <summary>
    /// Retrieves all workflow definitions from the store.
    /// </summary>
    /// <returns>A list of all workflow definitions.</returns>
    public List<WorkflowDefinition> GetAllDefinitions()
    {
        return _definitions.Values.ToList();
    }
    
    /// <summary>
    /// Checks if a workflow definition exists in the store.
    /// </summary>
    /// <param name="id">The ID of the workflow definition to check.</param>
    /// <returns>True if the definition exists, otherwise false.</returns>
    public bool DefinitionExists(string id)
    {
        return _definitions.ContainsKey(id);
    }
    
    #endregion
    
    #region Workflow Instance Operations
    
    /// <summary>
    /// Saves a workflow instance to the in-memory store.
    /// </summary>
    /// <param name="instance">The workflow instance to save.</param>
    /// <returns>The saved workflow instance.</returns>
    public WorkflowInstance SaveInstance(WorkflowInstance instance)
    {
        _instances[instance.Id] = instance;
        return instance;
    }
    
    /// <summary>
    /// Retrieves a workflow instance by its ID.
    /// </summary>
    /// <param name="id">The ID of the workflow instance to retrieve.</param>
    /// <returns>The workflow instance if found, otherwise null.</returns>
    public WorkflowInstance? GetInstance(string id)
    {
        return _instances.TryGetValue(id, out var instance) ? instance : null;
    }
    
    /// <summary>
    /// Retrieves all workflow instances from the store.
    /// </summary>
    /// <returns>A list of all workflow instances.</returns>
    public List<WorkflowInstance> GetAllInstances()
    {
        return _instances.Values.ToList();
    }
    
    /// <summary>
    /// Retrieves all workflow instances for a specific definition.
    /// </summary>
    /// <param name="definitionId">The ID of the workflow definition.</param>
    /// <returns>A list of workflow instances for the specified definition.</returns>
    public List<WorkflowInstance> GetInstancesByDefinitionId(string definitionId)
    {
        return _instances.Values
            .Where(i => i.DefinitionId == definitionId)
            .ToList();
    }
    
    /// <summary>
    /// Checks if a workflow instance exists in the store.
    /// </summary>
    /// <param name="id">The ID of the workflow instance to check.</param>
    /// <returns>True if the instance exists, otherwise false.</returns>
    public bool InstanceExists(string id)
    {
        return _instances.ContainsKey(id);
    }
    
    #endregion
}
