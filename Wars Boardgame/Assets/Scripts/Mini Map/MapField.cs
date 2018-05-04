using UnityEngine;
using System.Collections;

public class MapField : MonoBehaviour {
    public Vector2 capture;
    public int mapWidth;
    public int mapHeight;
    public Camera mainCamera;
    public RectTransform view;

    private RectTransform tr;

    void Start () {
        tr = gameObject.GetComponent<RectTransform>();
        //view.position = new Vector3();

        float tempX = 1 - tr.sizeDelta.x / Screen.width;
        float tempY = 1 - tr.sizeDelta.y / Screen.height;

        tr.anchorMin = new Vector2(tempX, tempY);
        tr.sizeDelta = Vector2.zero;

        view.sizeDelta = new Vector2(Screen.width / 7, Screen.height / 7);

        view.position = tr.position - new Vector3(3, 3, 0);
    }

}
