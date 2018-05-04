using UnityEngine;
using UnityEditor;
using System.Collections;

//맵 에디터

[CustomEditor (typeof(HexTile))]
[CanEditMultipleObjects]
public class MapEditor : Editor {
	HexMap map;
	float sqr = Mathf.Sqrt(3) / 2;
	float scale;

	public void OnEnable()
	{
		map = GameObject.FindWithTag("Map").GetComponent<HexMap>();
		scale = map.scale;
		SceneView.onSceneGUIDelegate = MapUpdate;
	}
	
	public void MapUpdate(SceneView view) {
		Event e = Event.current;
		Ray r = Camera.current.ScreenPointToRay(new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight));
		Vector3 mousePos = r.origin;

		if (e.isKey && e.character == 'a') {
			float tempY = Mathf.Floor (0.5f * scale + mousePos.z / (0.75f * scale));
			float tempX;
			if(tempY % 2==0) {
				tempX = Mathf.Round ( mousePos.x / (scale * sqr));
			} else {
				tempX = Mathf.Floor ( mousePos.x / (scale * sqr));
			} tempX -= Mathf.Floor (tempY/2);

			GameObject obj;
			Object prefab = PrefabUtility.GetPrefabParent(Selection.activeObject);
			

			if (prefab)
			{
				obj = (GameObject) PrefabUtility.InstantiatePrefab(prefab);
				obj.transform.parent = map.gameObject.transform;
				HexTile tile = obj.GetComponent<HexTile>();
			    tile.location = new Vector3(tempY, map.floor, tempX);
			} 
		} 

		else if (e.isKey && e.character == 'd')
		{
			foreach (GameObject obj in Selection.gameObjects) {
				if (obj.GetComponent<HexTile>() != null) {
					//HexTile tile = obj.GetComponent<HexTile>();
					DestroyImmediate(obj);
				}
			}
		}
	}
}
