using System;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]

public class Tile : MonoBehaviour, IEquatable<Tile>, IComparable<Tile>
{
    #region Variables
    
    /// Inspector & Unity ///
    public Vector2 coordinate = new Vector2();  // (X,Y) Co-ordinates of this tile in the HexGrid
    [SerializeField] LayerMask tileLayer;       // Layer that all Tile objects exist on

    /// Navigation ///
    [Header("Navigation")]
    public Tile[] neighbours;                   // Tiles connected to this Tile
    public bool occupied = false;               // Flag for if this tile is occupied by a unit

    /// Pathfinding ///
    [Header("Pathfinding")]
    public Tile _NavParent = null;              // Parent of this Tile on the Path 
    public float _NavNeighbourCost = 0f;        // Cost from this Tile to the Parent Neighbour
    public float _NavGoalCost = 0f;             // Cost from this Tile to the Goal Tile
    public float _NavCostModifier = 0f;         // Cost Modifier: Alter the cost based on unique paramiters - This allows flocking and steering around other units.
    public float _NavTotalCost = 0f;            // Total Cost of Navigating this Tile

    #endregion

    #region Interface Implimentation
    // Allow Comparisons //

    public bool Equals(Tile other)
    {
        if(this.name == other.name) { return true; }
        else { return false; }
    }

    public int CompareTo(Tile other)
    {
        if (this._NavTotalCost < other._NavTotalCost) return -1;
        else if (this._NavTotalCost > other._NavTotalCost) return 1;
        else return 0;
    }

    #endregion

    #region Runtime Methods

    /// <summary>
    /// Grab neighbours at runtime if not pre-given
    /// </summary>
    void Start()
    {
        if(neighbours.Count() == 0)
        {
            SetNeighbours();
        }
    }

    #endregion

    #region Gets & Sets

    /// <summary>
    /// Set tile name in heirarchy to its coordinate in the grid.
    /// </summary>
    public void SetName()
    {
        name = "Tile [" + coordinate.x + "," + coordinate.y + "]";
    }

    /// <summary>
    /// Grabs neighbouring tiles using physics engine and adds them to this tile's neighbours array.
    /// </summary>
    public void SetNeighbours()
    {
        Collider[] overlaps = Physics.OverlapSphere(transform.position, 4f, tileLayer);

        neighbours = new Tile[overlaps.Length - 1];
        
        int j = 0;
        for (int i = 0; i < (overlaps.Length); i++)
        {
            if (!(overlaps[i].gameObject.Equals(this.gameObject)))
            {
                neighbours[j] = overlaps[i].gameObject.GetComponent<Tile>();
                j++;
            }
        }
    }

    /// <summary>
    /// Calculates total weight of navigating this node, including modifiers.
    /// </summary>
    public void SetTotalCost()
    {
        _NavTotalCost = (_NavNeighbourCost + _NavGoalCost + _NavCostModifier);
    }

    /// <summary>
    /// Calculates navigation modifier using agent current position.
    /// </summary>
    /// <param name="originUnitPos">Location of agent to be steered towards/away from.</param>
    /// <param name="flockTo">If agents should be steered towards or away from other unit.</param>
    public void SetModifiedCost(Vector3 originUnitPos, bool flockTo = true)
    {
        float mod = (originUnitPos - transform.position).magnitude;
        mod = Mathf.Abs(mod);

        switch (flockTo)
        {
            case true:
                _NavCostModifier = -mod;
                break;

            case false:
                _NavCostModifier = mod;
                break;
        }

    }

    #endregion

    #region Pathfinding Methods

    /// <summary>
    /// Cleans the tile, allowing pathfinding for other units.
    /// </summary>
    public void ResetNavigation()
    {
        _NavParent = null;
        _NavNeighbourCost = 0f;
        _NavGoalCost = 0f;
        _NavCostModifier = 0f;
        _NavTotalCost = 0f;
}

    #endregion

    #region Gizmos

    /// <summary>
    /// Lets you see connected tiles in Scene View 
    /// </summary>
    private void OnDrawGizmosSelected()
    {

        foreach (Tile x in neighbours)
        {
            Debug.DrawLine(transform.position, x.transform.position, Color.cyan);
        }

    }

    #endregion

}
