using WorkflowEngine.Models;
using WorkflowEngine.Services;
using WorkflowEngine.DTOs;
using Xunit;

namespace WorkflowEngine.Tests;

/// <summary>
/// Unit tests for the WorkflowService class.
/// These tests validate core functionality and help clarify design decisions.
/// </summary>
public class WorkflowServiceTests
{
    private readonly InMemoryWorkflowStore _store;
    private readonly WorkflowService _service;

    public WorkflowServiceTests()
    {
        _store = new InMemoryWorkflowStore();
        _service = new WorkflowService(_store);
    }

    #region Workflow Definition Tests

    [Fact]
    public void CreateWorkflowDefinition_ValidDefinition_ReturnsDefinition()
    {
        // Arrange
        var request = CreateSampleWorkflowDefinitionRequest();

        // Act
        var result = _service.CreateWorkflowDefinition(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.States.Count, result.States.Count);
        Assert.Equal(request.Actions.Count, result.Actions.Count);
        Assert.NotEmpty(result.Id);
    }

    [Fact]
    public void CreateWorkflowDefinition_EmptyName_ThrowsException()
    {
        // Arrange
        var request = CreateSampleWorkflowDefinitionRequest();
        request.Name = "";

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            _service.CreateWorkflowDefinition(request));
        Assert.Contains("name cannot be empty", exception.Message);
    }

    [Fact]
    public void CreateWorkflowDefinition_NoInitialState_ThrowsException()
    {
        // Arrange
        var request = CreateSampleWorkflowDefinitionRequest();
        request.States.ForEach(s => s.IsInitial = false);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            _service.CreateWorkflowDefinition(request));
        Assert.Contains("exactly one initial state", exception.Message);
    }

    [Fact]
    public void CreateWorkflowDefinition_MultipleInitialStates_ThrowsException()
    {
        // Arrange
        var request = CreateSampleWorkflowDefinitionRequest();
        request.States.ForEach(s => s.IsInitial = true);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            _service.CreateWorkflowDefinition(request));
        Assert.Contains("exactly one initial state", exception.Message);
    }

    [Fact]
    public void CreateWorkflowDefinition_DuplicateStateIds_ThrowsException()
    {
        // Arrange
        var request = CreateSampleWorkflowDefinitionRequest();
        request.States.Add(new State { Id = "draft", Name = "Duplicate Draft" });

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            _service.CreateWorkflowDefinition(request));
        Assert.Contains("duplicate state IDs", exception.Message);
    }

    [Fact]
    public void CreateWorkflowDefinition_InvalidActionReference_ThrowsException()
    {
        // Arrange
        var request = CreateSampleWorkflowDefinitionRequest();
        request.Actions.Add(new WorkflowAction 
        { 
            Id = "invalid", 
            Name = "Invalid Action", 
            FromStates = new List<string> { "nonexistent" }, 
            ToState = "draft" 
        });

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            _service.CreateWorkflowDefinition(request));
        Assert.Contains("non-existent source state", exception.Message);
    }

    #endregion

    #region Workflow Instance Tests

    [Fact]
    public void CreateWorkflowInstance_ValidDefinition_CreatesInstanceWithInitialState()
    {
        // Arrange
        var request = CreateSampleWorkflowDefinitionRequest();
        var definition = _service.CreateWorkflowDefinition(request);

        // Act
        var instance = _service.CreateWorkflowInstance(definition.Id);

        // Assert
        Assert.NotNull(instance);
        Assert.Equal(definition.Id, instance.DefinitionId);
        Assert.Equal("draft", instance.CurrentStateId);
        Assert.Empty(instance.History);
    }

    [Fact]
    public void CreateWorkflowInstance_NonExistentDefinition_ThrowsException()
    {
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            _service.CreateWorkflowInstance("nonexistent"));
        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public void ExecuteAction_ValidAction_TransitionsState()
    {
        // Arrange
        var request = CreateSampleWorkflowDefinitionRequest();
        var definition = _service.CreateWorkflowDefinition(request);
        var instance = _service.CreateWorkflowInstance(definition.Id);

        // Act
        var updatedInstance = _service.ExecuteAction(instance.Id, "submit");

        // Assert
        Assert.Equal("review", updatedInstance.CurrentStateId);
        Assert.Single(updatedInstance.History);
        Assert.Equal("submit", updatedInstance.History[0].ActionId);
        Assert.Equal("draft", updatedInstance.History[0].FromStateId);
        Assert.Equal("review", updatedInstance.History[0].ToStateId);
    }

    [Fact]
    public void ExecuteAction_InvalidTransition_ThrowsException()
    {
        // Arrange
        var request = CreateSampleWorkflowDefinitionRequest();
        var definition = _service.CreateWorkflowDefinition(request);
        var instance = _service.CreateWorkflowInstance(definition.Id);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            _service.ExecuteAction(instance.Id, "approve")); // approve can't be executed from draft
        Assert.Contains("cannot be executed from current state", exception.Message);
    }

    [Fact]
    public void ExecuteAction_FinalState_ThrowsException()
    {
        // Arrange
        var request = CreateSampleWorkflowDefinitionRequest();
        var definition = _service.CreateWorkflowDefinition(request);
        var instance = _service.CreateWorkflowInstance(definition.Id);
        
        // Move to review state
        _service.ExecuteAction(instance.Id, "submit");
        // Move to approved state (final)
        _service.ExecuteAction(instance.Id, "approve");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            _service.ExecuteAction(instance.Id, "submit"));
        Assert.Contains("final state", exception.Message);
    }

    [Fact]
    public void ExecuteAction_DisabledAction_ThrowsException()
    {
        // Arrange
        var request = CreateSampleWorkflowDefinitionRequest();
        request.Actions.First(a => a.Id == "submit").Enabled = false;
        var definition = _service.CreateWorkflowDefinition(request);
        var instance = _service.CreateWorkflowInstance(definition.Id);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            _service.ExecuteAction(instance.Id, "submit"));
        Assert.Contains("disabled and cannot be executed", exception.Message);
    }

    [Fact]
    public void ExecuteAction_NonExistentAction_ThrowsException()
    {
        // Arrange
        var request = CreateSampleWorkflowDefinitionRequest();
        var definition = _service.CreateWorkflowDefinition(request);
        var instance = _service.CreateWorkflowInstance(definition.Id);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            _service.ExecuteAction(instance.Id, "nonexistent"));
        Assert.Contains("not found in workflow definition", exception.Message);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Creates a sample workflow definition request for testing.
    /// Represents a simple document approval workflow.
    /// </summary>
    private CreateWorkflowDefinitionRequest CreateSampleWorkflowDefinitionRequest()
    {
        return new CreateWorkflowDefinitionRequest
        {
            Name = "Document Approval Workflow",
            Description = "A simple document approval process",
            States = new List<State>
            {
                new State 
                { 
                    Id = "draft", 
                    Name = "Draft", 
                    IsInitial = true, 
                    IsFinal = false, 
                    Enabled = true,
                    Description = "Document is being drafted"
                },
                new State 
                { 
                    Id = "review", 
                    Name = "Under Review", 
                    IsInitial = false, 
                    IsFinal = false, 
                    Enabled = true,
                    Description = "Document is being reviewed"
                },
                new State 
                { 
                    Id = "approved", 
                    Name = "Approved", 
                    IsInitial = false, 
                    IsFinal = true, 
                    Enabled = true,
                    Description = "Document has been approved"
                },
                new State 
                { 
                    Id = "rejected", 
                    Name = "Rejected", 
                    IsInitial = false, 
                    IsFinal = true, 
                    Enabled = true,
                    Description = "Document has been rejected"
                }
            },
            Actions = new List<WorkflowAction>
            {
                new WorkflowAction
                {
                    Id = "submit",
                    Name = "Submit for Review",
                    Enabled = true,
                    FromStates = new List<string> { "draft" },
                    ToState = "review",
                    Description = "Submit document for review"
                },
                new WorkflowAction
                {
                    Id = "approve",
                    Name = "Approve",
                    Enabled = true,
                    FromStates = new List<string> { "review" },
                    ToState = "approved",
                    Description = "Approve the document"
                },
                new WorkflowAction
                {
                    Id = "reject",
                    Name = "Reject",
                    Enabled = true,
                    FromStates = new List<string> { "review" },
                    ToState = "rejected",
                    Description = "Reject the document"
                },
                new WorkflowAction
                {
                    Id = "revise",
                    Name = "Send for Revision",
                    Enabled = true,
                    FromStates = new List<string> { "review" },
                    ToState = "draft",
                    Description = "Send document back for revision"
                }
            }
        };
    }

    #endregion
}
