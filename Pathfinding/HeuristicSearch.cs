using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeuristicSearch : MonoBehaviour
{
    #region Singleton Setup

    public static HeuristicSearch Instance { get; private set; }

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

    public List<Tile> dirtyTiles = new List<Tile>();                                // Keeps track of the tiles that need to be cleaned between units using this class

    #region Heuristic Search

    /// <summary>
    /// Search tiles for goal and set path parent - A* approach.
    /// </summary>
    /// <param name="current"> Tile unit is currently occupying. </param>
    /// <param name="goal"> Tile to move towards. </param>
    /// <param name="inverse"> If goal is to be inverted, meaning the agent moves away instead of towards. </param>
    public void SearchTiles(Tile current, Tile goal, bool inverse = false)
    {
        PriorityQueue<Tile> pq = new PriorityQueue<Tile>();

        pq.Enqueue(current);

        while(pq.Count() > 0)
        {
            Tile parent = pq.Dequeue();

            if (parent.Equals(goal))
            {
                return;
            }
            else
            {
                foreach(Tile child in parent.neighbours)
                {
                    if (child == null)
                    {
                        Debug.Log("Null Child");
                    }

                    if(child._NavParent == null && child != current && !child.occupied)
                    {
                        // Set costs of traversing tiles
                        child._NavParent = parent;
                        child._NavNeighbourCost = 1;                                // Grid neighbours are equidistant.
                        child._NavGoalCost = (child.transform.position - goal.transform.position).magnitude;
                        
                        if (inverse) { child._NavGoalCost = -child._NavGoalCost; }  // This allows inversal of goal cost, meaning units can be pushed away from the goal.

                        // Sum all costs
                        child.SetTotalCost();                                       // Costs relating to unit proximity are added elsewhere but are included in this equation.

                        pq.Enqueue(child);
                        dirtyTiles.Add(child);                                      // Keep track of tiles so they can be cleaned later.
                    }
                }
            }
        }

    }

    #endregion

    #region Modify Tiles - Flocking/Steering

    [SerializeField] LayerMask tileLayer;                                           // Layer tiles exist on, for filtering

    /// <summary>
    /// Method to set tiles mod cost dependent on their faction
    /// </summary>
    /// <param name="currentUnit"></param>
    void ApplyModifiers(UnitBase currentUnit)
    {
        UnitBase[] components = GameObject.FindObjectsOfType<UnitBase>();

        foreach (UnitBase i in components)
        {
            if (!currentUnit.Equals(i))
            {
                ModifyingTiles(CalculateModifiers(currentUnit, i));
            }
        }
    }

    /// <summary>
    /// 2D switch statement that calculates faction modifiers
    /// </summary>
    /// <param name="currentUnit"></param>
    /// <param name="otherUnit"></param>
    /// <returns></returns>
    public float CalculateModifiers(UnitBase currentUnit, UnitBase otherUnit)
    {
        float value = 1f;
        float output = 0;

        switch (currentUnit._Faction)
        {
            case (Faction.PLAYER):                  // Players will always follow the shortest path.
                return output;

            case (Faction.ALLY):                    // Allies will be given a bonus to steer/flock with certain factions:
                switch (otherUnit._Faction)
                {
                    case (Faction.ALLY):                // Allies will seek allies
                        output = -value;
                        break;

                    case (Faction.PLAYER):              // Allies will seek the player
                        output = -value;
                        break;

                    case (Faction.ENEMY):               // Allies will avoid enemies 
                        output = value;
                        break;
                }
                break;

            case (Faction.ENEMY):                   // Enemies will be given a bonus to steer/flock with certain factions:
                switch (otherUnit._Faction)
                {
                    case (Faction.ALLY):                // Enemies will avoid allies
                        output = value;
                        break;

                    case (Faction.PLAYER):              // Enemies will seek to attack the Player
                        output = -value;
                        break;

                    case (Faction.ENEMY):               // Enemies will seek one another
                        output = -value;
                        break;
                }
                break;
        }
        return output;
    }

    /// <summary>
    /// Set tiles to be modified (and later cleaned)
    /// </summary>
    /// <param name="modifier"></param>
    void ModifyingTiles(float modifier = 0)
    {
        Collider[] aoe = Physics.OverlapSphere(transform.position, 10f, tileLayer);

        foreach (Collider i in aoe)
        {
            i.GetComponent<Tile>()._NavCostModifier = modifier;
            dirtyTiles.Add(i.GetComponent<Tile>());
        }

    }

    #endregion

    #region Methods

    #region Movement

    /// <summary>
    /// Push given unit away from their goal
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public IEnumerator MoveUnitAway(UnitBase unit)
    {
        ApplyModifiers(unit);
        SearchTiles(unit._CurrentTile, unit._GoalTile, true);

        int max = unit._Movement;
        for(int i = 0; i < max; i++)
        {
            Tile highestCost = null;
            foreach(Tile neighbour in unit._CurrentTile.neighbours)
            {
                if (neighbour.occupied) { continue; }

                if (highestCost == null) { highestCost = neighbour; }

                else if(highestCost._NavTotalCost > neighbour._NavTotalCost)
                {
                    highestCost = neighbour;
                }

                else if(neighbour._NavTotalCost > 0)
                {
                    highestCost = neighbour;
                }
            }

            unit.Set_CurrentTile(highestCost);

            yield return new WaitForSeconds(1f);
            unit._Movement--;
        }

        CleanTiles();

    }

    /// <summary>
    /// ORIGINAL - push player to their goal
    /// (This is the implimentation of pathfinding I aimed to impliment)
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public IEnumerator MoveUnitTo(UnitBase unit)
    {
        ApplyModifiers(unit);
        SearchTiles(unit._CurrentTile, unit._GoalTile);

        Tile[] path = GetPath(unit._GoalTile).ToArray();

        int max = unit._Movement;
        for (int i = 0; i < max; i++)
        {
            if (path.Length == i) { break; }

            if (!path[i].occupied)
            {
                unit.Set_CurrentTile(path[i]);
            }

            unit._Movement--;
            yield return new WaitForSecondsRealtime(1f);
        }

        CleanTiles();
    }

    /// <summary>
    /// FIX - Push AI to their goal
    /// (This was a fix due to the AI bugging out and moving greater distances than intended)
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public IEnumerator MoveAITo(UnitBase unit)
    {

        ApplyModifiers(unit);
        SearchTiles(unit._CurrentTile, unit._GoalTile);

        int max = unit._Movement;
        for (int i = 0; i < max; i++)
        {
            Tile lowestCost = null;
            foreach (Tile neighbour in unit._CurrentTile.neighbours)
            {
                if (neighbour.occupied) { continue; }

                if (lowestCost == null) { lowestCost = neighbour; }

                if (lowestCost._NavTotalCost > neighbour._NavTotalCost)
                {
                    lowestCost = neighbour;
                }

            }

            unit.Set_CurrentTile(lowestCost);

            yield return new WaitForSeconds(1f);
            unit._Movement--;
        }

        CleanTiles();

    }

    /// <summary>
    /// Move Given Unit in a Random Direction
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public IEnumerator MoveUnitOnce(UnitBase unit)
    {
        Tile goal = unit._CurrentTile.neighbours[Random.Range(0, unit._CurrentTile.neighbours.Length)];
        
        if (goal.occupied == false)
        {
            unit.Set_CurrentTile(goal);
            unit._Movement--;
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    /// <summary>
    /// Move Given Unit Randomly
    /// </summary>
    /// <param name="unit"></param>
    public void MoveUnitRandom(UnitBase unit)
    {
        int max = unit._Movement;
        for (int i = 0; i < max; i++)
        {
            StartCoroutine(MoveUnitOnce(unit));
        }

    }

    #endregion

    /// <summary>
    /// Allows running IEnumerators in non-monobehaviour Classes
    /// </summary>
    /// <param name="run"></param>
    public void Run_IEnumerator(IEnumerator run)
    {
        StartCoroutine(run);
    }

    /// <summary>
    /// Return the path TO a goal, after a search.
    /// </summary>
    /// <param name="goal"></param>
    /// <returns></returns>
    public List<Tile> GetPath(Tile goal)
    {
        List<Tile> path = new List<Tile>();
        Tile current = goal;
        path.Add(current);

        while(current._NavParent != null)
        {
            path.Add(current);
            current = current._NavParent;
        }

        path.Reverse();

        return path;
    }

    /// <summary>
    /// Clean Dirty Tiles
    /// </summary>
    public void CleanTiles()
    {
        foreach(Tile t in dirtyTiles)
        {
            t.ResetNavigation();
        }
        dirtyTiles.Clear();
    }
    
    #endregion
}
