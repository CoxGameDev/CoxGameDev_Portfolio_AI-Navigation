using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUnit : UnitBase
{
    #region Fields
    /// Inspector Values for UnitBase ///
    [Header("Unit Values")]
    [SerializeField] int health = 1;
    [SerializeField] int movement = 3;
    [SerializeField] Faction faction;
    [SerializeField] bool turn = true;

    /// Refs ///
    [Header("References")]
    [SerializeField] Button button_EndTurn = null;
    [SerializeField] Button button_MovePlayer = null;
    [SerializeField] LayerMask tileLayer;
    [SerializeField] Transform ui_SelectedTile = null;

    #endregion

    #region Runtime

    private void OnEnable()
    {
        // Assign inspector values
        _HP = health;
        _Movement = movement;
        _Faction = faction;
        isUnitsTurn = turn;

        // Set current tile to be tile unit is on
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

    #region Turn Management

    /// <summary>
    /// Allow interaction with UI
    /// </summary>
    public override void StartTurn()
    {
        base.StartTurn();
        _Movement = movement;

        button_MovePlayer.interactable = true;
        button_EndTurn.interactable = true;

    }

    /// <summary>
    /// Method for END TURN button
    /// </summary>
    public override void EndTurn()
    {
        base.EndTurn();
        button_MovePlayer.interactable = false;
        button_EndTurn.interactable = false;
    }

    #endregion

    #region Player Controls

    /// <summary>
    /// Get Input and convert it into a valid tile if available
    /// </summary>
    public void OnClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

            if (hit && hitInfo.collider.CompareTag("Tile")) // if (hit) & (insurance check)
            {
                _GoalTile = hitInfo.collider.gameObject.GetComponent<Tile>();
                ui_SelectedTile.position = _GoalTile.transform.position;

            }
            else if(hit && hitInfo.collider.CompareTag("Enemy"))
            {
                /// Player Attack Functionality Removed:
                /// Game plays much more interestingly if only allies can attack, encourages more strategy.
            }

        }
    }

    /// <summary>
    /// Method for the MOVE button
    /// </summary>
    public void SubmitMove()
    {
        StartCoroutine(HeuristicSearch.Instance.MoveUnitTo(this));
    }

    #endregion
}
