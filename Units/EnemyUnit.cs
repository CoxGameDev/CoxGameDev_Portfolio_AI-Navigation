using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : AIUnitBase
{
    protected override void Awake()
    {
        base.Awake();
        _Faction = Faction.ENEMY;
    }

    /// <summary>
    /// if enemies outnumber allies, attack. otherwise retreat.
    /// </summary>
    public override void EvaluateState()
    {
        EnemyUnit[] check = GameObject.FindObjectsOfType<EnemyUnit>();

        if(check.Length == 1)
        {
            _fsm.EnterState(States.RETREATING);
        }
        else
        {
            _fsm.EnterState(States.ATTACKING);
        }
    }
}
