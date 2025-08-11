using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton Setup

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    #endregion

    #region Fields

    /// Overall Gamestate Handling //
    private enum GameState { PLAYING, LOSS, WIN};
    private GameState current_GameState = GameState.PLAYING;

    /// Round Handling
    bool turnInProgress = false;
    [SerializeField] List<AIUnitBase> activeUnits;

    /// Player ///
    [SerializeField] PlayerUnit player;
    public PlayerUnit GetPlayerRef() { return player; }

    #endregion

    private void Update()
    {

        if(current_GameState == GameState.PLAYING && !turnInProgress)   // While game is playing AND there isn't a round already happening
        {
            turnInProgress = true;
            StartCoroutine("Processing");
        }

        if (player.isUnitsTurn)                                         // Allow player input
        {
            player.OnClick();
        }

    }

    /// <summary>
    /// Code that runs each ROUND of turns
    /// </summary>
    /// <returns></returns>
    IEnumerator Processing()
    {
        player.StartTurn();
        yield return new WaitUntil(() => player.isUnitsTurn == false);

        foreach (AIUnitBase currentUnit in activeUnits)
        {
            currentUnit.StartTurn();
            currentUnit._fsm.Tick();
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(1f);
        turnInProgress = false;
    }

}
