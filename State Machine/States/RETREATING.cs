using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RETREATING : StateBase
{
    UnitBase target = null;

    public override void OnEnable()
    {
        base.OnEnable();
        stateType = States.RETREATING;
    }

    /// <summary>
    /// See ATTACKING.UpdateState, this is the inverse
    /// </summary>
    public override void UpdateState()
    {
        if (_unit.CheckIfEnemies(GameManager.Instance.GetPlayerRef()))
        {
            target = GameManager.Instance.GetPlayerRef();
        }
        else
        {
            target = GameObject.FindObjectOfType<EnemyUnit>();
        }

        _unit._GoalTile = target._CurrentTile;
        HeuristicSearch.Instance.Run_IEnumerator(HeuristicSearch.Instance.MoveUnitAway(_unit));

        ExitState();
    }

}
