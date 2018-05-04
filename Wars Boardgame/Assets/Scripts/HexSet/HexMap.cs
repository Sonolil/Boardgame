using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class HexMap : MonoBehaviour {
	public float scale = 1;
    public int floor = 0;

    private HexTile[] tiles;
    private Vector3[] directions = { new Vector3(-1, 0, 0), new Vector3(0, 0, -1), new Vector3(1, 0, -1),
                                     new Vector3(-1, 0, 1), new Vector3(0, 0, 1), new Vector3(1, 0, 0)};

    public static Dictionary<Vector3, HexTile> grid = new Dictionary<Vector3, HexTile>();
    
    void Awake () {
        grid.Clear();
		tiles = gameObject.GetComponentsInChildren <HexTile> ();

        foreach (HexTile tile in tiles)
        {
            if (grid.ContainsKey(tile.location))
            {
                Destroy(tile.gameObject);
            }
            else grid.Add(tile.location, tile);
        }

        foreach (HexTile tile in tiles)
        {
            AssignNears(tile);
        }
    }

    private void AssignNears(HexTile tile)
    {
        Vector3 loc = tile.location;
        List<HexTile> list = tile.nears;
        Vector3 key;

        for (int i=-1; i < 2; i++)
        {
            Vector3 f = new Vector3 (0, i, 0);
            for (int j=0; j < directions.Length; j++)
            {
                key = loc + f + directions[j];
                if(grid.ContainsKey(key))
                    list.Add(grid[key]);
            }
        }
    }
}
