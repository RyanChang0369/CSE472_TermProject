using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Small tester script that tests tilemaps.
/// 
/// <br/>
/// 
/// Authors: Ryan Chang (2023)
/// </summary>
[RequireComponent(typeof(Tilemap))]
[ExecuteInEditMode]
public class TilemapTester : MonoBehaviour
{
    [SerializeField]
    [ReadOnly]
    private Tilemap tilemap;

    [SerializeField]
    [ReadOnly]
    private TileBase tile;

    [SerializeField]
    private Vector3Int testPoint;

    private Tilemap Tilemap
    {
        get
        {
            this.AutofillComponent(ref tilemap);
            return tilemap;
        }
    }

    private void Update()
    {
        Bounds bounds = new(Tilemap.GetCellCenterWorld(testPoint), Tilemap.cellSize);

        DebugExt.UseDebug(Color.red, Time.deltaTime);
        DebugExt.DrawCrossBounds(bounds);

        tile = Tilemap.GetTile(testPoint);
    }
}