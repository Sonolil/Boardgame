using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BuildMenu : MonoBehaviour {
    public List<GameObject> buttons = new List<GameObject>();

    public void SetButton(List<int> buttonTypes, int coins)
    {
        reset();
        int buttonNum = buttonTypes.Count;
        GetComponent<RectTransform>().sizeDelta = new Vector2(buttonNum * 50 + 50, 50);
        
        for (int i = 0; i < buttonNum; i++)
        {
            GameObject button = buttons[buttonTypes[i]];
            button.transform.localPosition = new Vector3(- buttonNum * 25 + 75 * i, 30, 0);
        }
    }

    private void reset()
    {
        foreach (GameObject button in buttons)
        {
            button.transform.localPosition = new Vector3(0, -100, 0);
        }
    }

}
