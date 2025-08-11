using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StateMachine))]
public abstract class AIUnitBase : UnitBase
{
    #region Fields
    /// Inspector Values for UnitBase ///
    [Header("Unit Values")]
    [SerializeField] int health = 1;
    [SerializeField] int movement = 3;
    [SerializeField] bool turn = false;

    /// AI Unit Base ///
    [SerializeField] LayerMask shipLayer;
    #endregion

    #region Runtime

    protected virtual void Awake()
    {
        _HP = health;
        _Movement = movement;
        isUnitsTurn = turn;
        _fsm = GetComponent<StateMachine>();

        Collider[] overlaps = Physics.OverlapSphere(transform.position, 1f);
        foreach (Collider i in overlaps)
        {
            if (i.CompareTag("Tile"))
            {
                Set_CurrentTile(i.gameObject.GetComponent<Tile>());
                break;
            }
        }
    }

    #endregion

    #region Turn Implimentation

    public override void StartTurn()
    {
        base.StartTurn();
        _Movement = movement;
        EvaluateState();
    }

    public override void OnTurn()
    {
        _fsm._currentState.UpdateState();
        Attack();
    }

    public override void EndTurn()
    {
        base.EndTurn();
    }

    #endregion

    #region FSM & Behaviour

    /// <summary>
    /// Call @ End of Turn - Attack Beaviour for disputing factions
    /// </summary>
    public void Attack()
    {
        Collider[] surroundings = Physics.OverlapSphere(this.transform.position, 5f, shipLayer);

        foreach(Collider other in surroundings)
        {
            if (other.transform.parent.gameObject.Equals(this.gameObject))
            {
                continue;
            }

            if (CheckIfEnemies(other.transform.parent.GetComponent<UnitBase>()))
            {
                this.Set_CurrentTile(other.transform.parent.GetComponent<UnitBase>()._CurrentTile);
                other.transform.parent.gameObject.SetActive(false);
                return;
            }

        }

    }

    /// <summary>
    /// Lookup for if this unit and the given unit are enemies (or !enemies AKA friends)
    /// </summary>
    /// <param name="otherUnit"></param>
    /// <returns></returns>
    public bool CheckIfEnemies(UnitBase otherUnit)
    {
        bool output = false;

        switch (this._Faction)
        {
            default:
                return output;

            case (Faction.ALLY):                    // Allies are...
                switch (otherUnit._Faction)
                {
                    case (Faction.ALLY):                // friendly to other allies
                        output = false;
                        break;

                    case (Faction.PLAYER):              // friendly to the player
                        output = false;
                        break;

                    case (Faction.ENEMY):               // hostile to enemies
                        output = true;
                        break;
                }
                break;

            case (Faction.ENEMY):                   // Enemies are...
                switch (otherUnit._Faction)
                {
                    case (Faction.ALLY):                // hostile to allies
                        output = true;
                        break;

                    case (Faction.PLAYER):              // hostile to player
                        output = true;
                        break;

                    case (Faction.ENEMY):               // friendly to their own kind
                        output = false;
                        break;
                }
                break;
        }
        return output;
    }

    /// <summary>
    /// Evaluate Current State Of World
    /// </summary>
    public abstract void EvaluateState();

    #endregion
}
