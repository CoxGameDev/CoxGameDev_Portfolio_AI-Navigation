using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectTile : MonoBehaviour
{
    #region Fields

    // Values //
    Tile selectedTile = null;

    // References //
    [SerializeField] LayerMask tileLayer;
    [SerializeField] Transform ui_SelectedTile = null;

    #endregion

    #region Methods

    /// <summary>
    /// On input, ensure tile selected then show via UI.
    /// </summary>
    void OnClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, tileLayer);
            
            if(hit && hitInfo.transform.TryGetComponent(out Tile selectedTile))
            {
                ui_SelectedTile.position = selectedTile.transform.position;
            }

        }
    }

    #endregion

}
