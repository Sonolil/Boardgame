using UnityEngine;
using System.Collections;

public class HPBar : MonoBehaviour {
    private RectTransform tr;
    private int max;
    private int curr;
    private bool dead;

    void Start () {
        tr = GetComponent<RectTransform>();
	}

    public void UpdateBar(float percent)
    {
        if (percent < 0)
            percent = 0;
        tr.localScale = new Vector3(percent, 1, 1);
    }

}
