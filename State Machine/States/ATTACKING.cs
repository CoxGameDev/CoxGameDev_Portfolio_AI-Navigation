using UnityEngine;

public class ATTACKING : StateBase
{
    UnitBase target = null;

    public override void OnEnable()
    {
        base.OnEnable();
        stateType = States.ATTACKING;

    }

    public override void UpdateState()
    {
        if (_unit.CheckIfEnemies(GameManager.Instance.GetPlayerRef()))                          // If current unit is friendly to the player...
        {
            target = GameManager.Instance.GetPlayerRef();                                           // Enemy - target player
        }
        else
        {
            target = GameObject.FindObjectOfType<EnemyUnit>();                                      // Ally - target any enemy
        }

        _unit._GoalTile = target._CurrentTile;
        HeuristicSearch.Instance.Run_IEnumerator(HeuristicSearch.Instance.MoveAITo(_unit));     // Move AI to target

        ExitState();

    }
}
