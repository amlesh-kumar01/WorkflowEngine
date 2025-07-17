using WorkflowEngine.Models;
using WorkflowEngine.DTOs;

namespace WorkflowEngine.Services;

/// <summary>
/// Core service for managing workflow definitions and instances.
/// Handles validation, state transitions, and business logic.
/// </summary>
public class WorkflowService
{
    private readonly InMemoryWorkflowStore _store;
    
    public WorkflowService(InMemoryWorkflowStore store)
    {
        _store = store;
    }
    
    #region Workflow Definition Management
    
    /// <summary>
    /// Creates a new workflow definition with comprehensive validation.
    /// </summary>
    /// <param name="request">The workflow definition creation request.</param>
    /// <returns>The created workflow definition.</returns>
    /// <exception cref="InvalidOperationException">Thrown when validation fails.</exception>
    public WorkflowDefinition CreateWorkflowDefinition(CreateWorkflowDefinitionRequest request)
    {
        var definition = new WorkflowDefinition
        {
            Name = request.Name,
            States = request.States,
            Actions = request.Actions,
            Description = request.Description
        };
        
        // Validate the definition before saving
        ValidateWorkflowDefinition(definition);
        
        return _store.SaveDefinition(definition);
    }
    
    /// <summary>
    /// Retrieves a workflow definition by its ID.
    /// </summary>
    /// <param name="id">The ID of the workflow definition.</param>
    /// <returns>The workflow definition if found, otherwise null.</returns>
    public WorkflowDefinition? GetWorkflowDefinition(string id)
    {
        return _store.GetDefinition(id);
    }
    
    /// <summary>
    /// Retrieves all workflow definitions.
    /// </summary>
    /// <returns>A list of all workflow definitions.</returns>
    public List<WorkflowDefinition> GetAllWorkflowDefinitions()
    {
        return _store.GetAllDefinitions();
    }
    
    /// <summary>
    /// Validates a workflow definition according to business rules.
    /// </summary>
    /// <param name="definition">The workflow definition to validate.</param>
    /// <exception cref="InvalidOperationException">Thrown when validation fails.</exception>
    private void ValidateWorkflowDefinition(WorkflowDefinition definition)
    {
        // Basic validation
        if (string.IsNullOrWhiteSpace(definition.Name))
            throw new InvalidOperationException("Workflow definition name cannot be empty.");
        
        if (definition.States.Count == 0)
            throw new InvalidOperationException("Workflow definition must have at least one state.");
        
        // Check for duplicate state IDs
        var stateIds = definition.States.Select(s => s.Id).ToList();
        if (stateIds.Count != stateIds.Distinct().Count())
            throw new InvalidOperationException("Workflow definition contains duplicate state IDs.");
        
        // Check for duplicate action IDs
        var actionIds = definition.Actions.Select(a => a.Id).ToList();
        if (actionIds.Count != actionIds.Distinct().Count())
            throw new InvalidOperationException("Workflow definition contains duplicate action IDs.");
        
        // Validate states
        ValidateStates(definition.States);
        
        // Validate actions
        ValidateActions(definition.Actions, stateIds);
    }
    
    /// <summary>
    /// Validates states in a workflow definition.
    /// </summary>
    /// <param name="states">The states to validate.</param>
    /// <exception cref="InvalidOperationException">Thrown when validation fails.</exception>
    private void ValidateStates(List<State> states)
    {
        // Check for exactly one initial state
        var initialStates = states.Where(s => s.IsInitial).ToList();
        if (initialStates.Count != 1)
            throw new InvalidOperationException($"Workflow definition must have exactly one initial state. Found: {initialStates.Count}");
        
        // Validate state IDs and names
        foreach (var state in states)
        {
            if (string.IsNullOrWhiteSpace(state.Id))
                throw new InvalidOperationException("State ID cannot be empty.");
            
            if (string.IsNullOrWhiteSpace(state.Name))
                throw new InvalidOperationException($"State '{state.Id}' name cannot be empty.");
        }
    }
    
    /// <summary>
    /// Validates actions in a workflow definition.
    /// </summary>
    /// <param name="actions">The actions to validate.</param>
    /// <param name="validStateIds">List of valid state IDs in the workflow.</param>
    /// <exception cref="InvalidOperationException">Thrown when validation fails.</exception>
    private void ValidateActions(List<WorkflowAction> actions, List<string> validStateIds)
    {
        foreach (var action in actions)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(action.Id))
                throw new InvalidOperationException("Action ID cannot be empty.");
            
            if (string.IsNullOrWhiteSpace(action.Name))
                throw new InvalidOperationException($"Action '{action.Id}' name cannot be empty.");
            
            if (string.IsNullOrWhiteSpace(action.ToState))
                throw new InvalidOperationException($"Action '{action.Id}' must have a target state.");
            
            if (action.FromStates.Count == 0)
                throw new InvalidOperationException($"Action '{action.Id}' must have at least one source state.");
            
            // Check that ToState references a valid state
            if (!validStateIds.Contains(action.ToState))
                throw new InvalidOperationException($"Action '{action.Id}' references non-existent target state '{action.ToState}'.");
            
            // Check that all FromStates reference valid states
            foreach (var fromState in action.FromStates)
            {
                if (!validStateIds.Contains(fromState))
                    throw new InvalidOperationException($"Action '{action.Id}' references non-existent source state '{fromState}'.");
            }
        }
    }
    
    #endregion
    
    #region Workflow Instance Management
    
    /// <summary>
    /// Creates a new workflow instance from a workflow definition.
    /// </summary>
    /// <param name="definitionId">The ID of the workflow definition.</param>
    /// <param name="name">The name for the workflow instance.</param>
    /// <returns>The created workflow instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the definition doesn't exist.</exception>
    public WorkflowInstance CreateWorkflowInstance(string definitionId, string name = "")
    {
        var definition = _store.GetDefinition(definitionId);
        if (definition == null)
            throw new InvalidOperationException($"Workflow definition with ID '{definitionId}' not found.");
        
        var initialState = definition.GetInitialState();
        if (initialState == null)
            throw new InvalidOperationException("Workflow definition does not have an initial state.");
        
        var instance = new WorkflowInstance
        {
            DefinitionId = definitionId,
            Name = name,
            CurrentStateId = initialState.Id
        };
        
        return _store.SaveInstance(instance);
    }
    
    /// <summary>
    /// Retrieves a workflow instance by its ID.
    /// </summary>
    /// <param name="id">The ID of the workflow instance.</param>
    /// <returns>The workflow instance if found, otherwise null.</returns>
    public WorkflowInstance? GetWorkflowInstance(string id)
    {
        return _store.GetInstance(id);
    }
    
    /// <summary>
    /// Retrieves all workflow instances.
    /// </summary>
    /// <returns>A list of all workflow instances.</returns>
    public List<WorkflowInstance> GetAllWorkflowInstances()
    {
        return _store.GetAllInstances();
    }
    
    /// <summary>
    /// Executes an action on a workflow instance with full validation.
    /// </summary>
    /// <param name="instanceId">The ID of the workflow instance.</param>
    /// <param name="actionId">The ID of the action to execute.</param>
    /// <returns>The updated workflow instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the action cannot be executed.</exception>
    public WorkflowInstance ExecuteAction(string instanceId, string actionId)
    {
        // Get the workflow instance
        var instance = _store.GetInstance(instanceId);
        if (instance == null)
            throw new InvalidOperationException($"Workflow instance with ID '{instanceId}' not found.");
        
        // Get the workflow definition
        var definition = _store.GetDefinition(instance.DefinitionId);
        if (definition == null)
            throw new InvalidOperationException($"Workflow definition with ID '{instance.DefinitionId}' not found.");
        
        // Get the current state
        var currentState = definition.States.FirstOrDefault(s => s.Id == instance.CurrentStateId);
        if (currentState == null)
            throw new InvalidOperationException($"Current state '{instance.CurrentStateId}' not found in workflow definition.");
        
        // Validate current state conditions
        ValidateCurrentStateForAction(currentState);
        
        // Get the action
        var action = definition.Actions.FirstOrDefault(a => a.Id == actionId);
        if (action == null)
            throw new InvalidOperationException($"Action with ID '{actionId}' not found in workflow definition.");
        
        // Validate action execution
        ValidateActionExecution(action, instance.CurrentStateId);
        
        // Get the target state
        var targetState = definition.States.FirstOrDefault(s => s.Id == action.ToState);
        if (targetState == null)
            throw new InvalidOperationException($"Target state '{action.ToState}' not found in workflow definition.");
        
        // Execute the action
        ExecuteActionOnInstance(instance, action, currentState, targetState);
        
        return _store.SaveInstance(instance);
    }
    
    /// <summary>
    /// Validates that the current state allows action execution.
    /// </summary>
    /// <param name="currentState">The current state.</param>
    /// <exception cref="InvalidOperationException">Thrown when the state doesn't allow actions.</exception>
    private void ValidateCurrentStateForAction(State currentState)
    {
        if (currentState.IsFinal)
            throw new InvalidOperationException($"Cannot execute actions on workflow instance in final state '{currentState.Name}'.");
        
        if (!currentState.Enabled)
            throw new InvalidOperationException($"Cannot execute actions on workflow instance in disabled state '{currentState.Name}'.");
    }
    
    /// <summary>
    /// Validates that an action can be executed from the current state.
    /// </summary>
    /// <param name="action">The action to validate.</param>
    /// <param name="currentStateId">The current state ID.</param>
    /// <exception cref="InvalidOperationException">Thrown when the action cannot be executed.</exception>
    private void ValidateActionExecution(WorkflowAction action, string currentStateId)
    {
        if (!action.Enabled)
            throw new InvalidOperationException($"Action '{action.Name}' is disabled and cannot be executed.");
        
        if (!action.FromStates.Contains(currentStateId))
            throw new InvalidOperationException($"Action '{action.Name}' cannot be executed from current state.");
    }
    
    /// <summary>
    /// Executes an action on a workflow instance, updating its state and history.
    /// </summary>
    /// <param name="instance">The workflow instance.</param>
    /// <param name="action">The action to execute.</param>
    /// <param name="currentState">The current state.</param>
    /// <param name="targetState">The target state.</param>
    private void ExecuteActionOnInstance(WorkflowInstance instance, WorkflowAction action, State currentState, State targetState)
    {
        // Record the action in history
        instance.History.Add(new HistoryEntry
        {
            ActionId = action.Id,
            ActionName = action.Name,
            FromStateId = currentState.Id,
            ToStateId = targetState.Id,
            Timestamp = DateTime.UtcNow
        });
        
        // Update the current state
        instance.CurrentStateId = targetState.Id;
    }
    
    #endregion
}
