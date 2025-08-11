
#region State & Execution Enums
public enum ExecutionState { NULL, EXECUTING, EXECUTED, ERRORED};

public enum States
{
    IDLE,
    ATTACKING,
    RETREATING,
};
#endregion

public abstract class StateBase
{
    #region Fields
    public AIUnitBase _unit;                                            // Attatched UnitBase
    public StateMachine _fsm;                                           // Attatched FSM

    public ExecutionState executionState { get; protected set; }        // Current state of execution - for debugging
    public States stateType { get; protected set; }                     // List of states in an easily accessable data type
    #endregion

    #region Runtime
    public virtual void OnEnable()
    {
        executionState = ExecutionState.NULL;
    }
    #endregion

    #region State Functionality

    /// <summary>
    /// Sets pre-conditions for state
    /// </summary>
    /// <returns></returns>
    public virtual bool EnterState()
    {
        executionState = ExecutionState.EXECUTING;

        bool output = true;
        if(_unit == null) { output = false; }

        return output;
    }

    /// <summary>
    /// Actions to be performed while in state
    /// </summary>
    public abstract void UpdateState();

    /// <summary>
    /// Exits state cleanly
    /// </summary>
    /// <returns></returns>
    public virtual bool ExitState()
    {
        bool output = true;
        executionState = ExecutionState.EXECUTED;
        return output;
    }

    #endregion

}
