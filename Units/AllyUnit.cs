using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyUnit : AIUnitBase
{
    protected override void Awake()
    {
        base.Awake();
        _Faction = Faction.ALLY;
    }

    /// <summary>
    /// If enemies are close, attack. otherwise idle.
    /// </summary>
    public override void EvaluateState()
    {
        Collider[] aoeCheck = Physics.OverlapSphere(transform.position, 15f, LayerMask.NameToLayer("Ship"));

        foreach(Collider check in aoeCheck)
        {
            if (check.tag.Equals("Enemy"))
            {
                _GoalTile = check.GetComponentInParent<UnitBase>()._CurrentTile;
                _fsm.EnterState(States.ATTACKING);
            }
            else
            {
                _fsm.EnterState(States.IDLE);
            }
        }
    }

}
