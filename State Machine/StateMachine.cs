using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Design of FSM largly inspired by previous FSM created with the help of DR T THOMPSON
/// Full explination in written report.
/// </summary>

public class StateMachine : MonoBehaviour
{
    #region Fields
    /// Data
    [SerializeField] States startState;                                                     // Starting state
    public StateBase _currentState { get; private set; }                                    // Currently Executing State

    /// References    
    StateBase[] allBases = new StateBase[]                                                  
    { 
        new IDLE(), 
        new ATTACKING(), 
        new RETREATING()
    };                                              // List of accessable state bases

    public Dictionary<States, StateBase> _StateRefDictionary { get; private set; }          // Dictionary allowing reference to states via StateBase enum
    private AIUnitBase _unit;

    #endregion

    #region Runtime

    /// <summary>
    /// Assign references and state references
    /// </summary>
    private void Awake()
    {
        _currentState = null;
        _StateRefDictionary = new Dictionary<States, StateBase>();
        _unit = GetComponent<AIUnitBase>();

        for(int i = 0; i < allBases.Length; i++)
        {
            allBases[i]._unit = GetComponent<AIUnitBase>();
            allBases[i]._fsm = this;
            _StateRefDictionary.Add((States)i, allBases[i]);                                // Assignment via allBases[i].stateType would only ever be IDLE - investigate later
        }

    }

    /// <summary>
    /// Set FSM to start
    /// </summary>
    private void Start()
    {
        EnterState(_StateRefDictionary[startState]);
    }

    #endregion

    #region Update Functionality

    // Method to be called in Update() for FSM to run
    public void Tick()
    {
        _currentState._unit.OnTurn();
    }

    #endregion

    #region Entering States
    /// <summary>
    /// Allows entry to states based on dictionary
    /// </summary>
    /// <param name="nextState"></param>
    public void EnterState(StateBase nextState)
    {
        if(nextState == null) { return; }
        if(_currentState != null) { _currentState.ExitState(); }

        _currentState = nextState;
        _currentState.EnterState();
    }
    /// <summary>
    /// Allows entry to states based on dictionary
    /// </summary>
    /// <param name="nextState"></param>
    public void EnterState(States nextState)
    {
        if (_StateRefDictionary.ContainsKey(nextState))
        {
            StateBase state = _StateRefDictionary[nextState];

            EnterState(state);
        }
    }

    #endregion

}
