using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Faction { PLAYER, ALLY, ENEMY}

/// <summary>
/// Basis for all Units.
/// </summary>
public abstract class UnitBase : MonoBehaviour
{
    #region Fields
    /// Unit Data ///
    [SerializeField] public int _HP { get; protected set; } 
    [SerializeField] public int _Movement { get; internal set; }
    [SerializeField] public Faction _Faction { get; protected set; }

    /// Traversal Data ///
    [SerializeField] public Tile _CurrentTile { get; private set; }
    [SerializeField] public Tile _GoalTile { get; internal set; }

    /// FSM Data ///
    public bool isUnitsTurn { get; internal set; } = false;
    public StateMachine _fsm { get; protected set; }

    #endregion

    #region Turn Handling

    public virtual void StartTurn()
    {
        ToggleUnitTurn(true);
    }

    public virtual void OnTurn() { }

    public virtual void EndTurn()
    {
        ToggleUnitTurn();
    }

    #region Handling Helper Methods
    public void ToggleUnitTurn()
    {
        isUnitsTurn = !isUnitsTurn;
    }
    public void ToggleUnitTurn(bool set)
    {
        isUnitsTurn = set;
    }
    #endregion

    #endregion

    #region Runtime
    private void Awake()
    {
        _fsm = GetComponent<StateMachine>();
    }
    #endregion

    #region Gets & Sets

    public void Set_CurrentTile(Tile tile)
    {
        // Old tile
        if (_CurrentTile != null)
        {
            _CurrentTile.occupied = false;
        }

        // New tile
        _CurrentTile = tile;
        transform.position = tile.transform.position;
        _CurrentTile.occupied = true;

    }

    #endregion
}