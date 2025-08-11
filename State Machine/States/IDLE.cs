using UnityEngine;

public class IDLE : StateBase
{
    public override void OnEnable()
    {
        base.OnEnable();
        stateType = States.IDLE;
    }

    /// <summary>
    /// Force IDLE unit to move a little
    /// </summary>
    public override void UpdateState()
    {
        HeuristicSearch.Instance.Run_IEnumerator(HeuristicSearch.Instance.MoveUnitOnce(_unit));
        ExitState();
    }

    public override bool ExitState()
    {
        base.ExitState();

        /* 
         * For some god forsaken reason, not overriding the Idle ExitState causes a compile error in Visual Studio.
         * It runs fine otherwise and with any other IDE but I like using Visual Studio, so here it stays.
        */
        
        return true;
    }

}
