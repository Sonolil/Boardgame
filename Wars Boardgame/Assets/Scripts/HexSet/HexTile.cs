using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[ExecuteInEditMode]
public class HexTile : MonoBehaviour {
	public int cost = 1;
	public bool pass = true;
    public bool build = true;
    public IEdifice edifice;
    public Vector3 location;
    public List<HexTile> nears = new List<HexTile>();
    public bool[] colony;
    
    private Color _baseColor;
    private Color _highlight;
    private Renderer _rend;

	void Awake () {
    }

    void Start()
    {
            _rend = gameObject.GetComponentInChildren<Renderer>();
            _baseColor = _rend.material.color;
            colony = new bool[3];
    }

#if UNITY_EDITOR
    void Update()
    {
        if (!Application.isPlaying)
        {
            Reposition(location);
        }
    }
#endif

    private void Reposition(Vector3 axial) {
		float row = axial.z + (axial.x - (axial.x % 1)) / 2;		
		float col = axial.x;
        float hgt = axial.y * 0.4f; //+ Random.Range(0, 0.1f);
        row *= Mathf.Sqrt(3) / 2;
		col *= 0.75f;
		transform.position = new Vector3 (row, hgt, col);
    }

    public void Highlight(bool swtch, Color color)
    {
        _highlight = color;
        if (swtch)
        {
            StartCoroutine("HighlightAnimation");
        }

        else
        {
            StopAllCoroutines();
            _rend.material.color = _baseColor;
        }
    }

    IEnumerator HighlightAnimation()
    {
        int i = 0;
        bool swtch = true;
        AddColor(_highlight * 5);

        while(true)
        {
            yield return null;

            if (swtch)
            {
                AddColor(_highlight / 12);

                if (i > 30)
                {
                    swtch = false;
                    continue;
                }
                    
                i++;

            }

            else
            {
                RemoveColor(_highlight / 12);

                if (i < 0)
                {
                    swtch = true;
                    continue;
                }

                i--;
            }
        }
    }
    
    public void AddColor(Color newColor)
    {
        _rend.material.color += newColor;
    }

    public void RemoveColor(Color newColor)
    {
        _rend.material.color -= newColor;
    }
    

    public void Shuffle()
    {
        for (int i = 0; i < nears.Count; i++)
        {
            HexTile temp = nears[i];
            int r = Random.Range(i, nears.Count);
            nears[i] = nears[r];
            nears[r] = temp;
        }
    }

    public void InstantiateHexObject(GameObject _obj)
    {
        GameObject obj = Instantiate(_obj);
        Manager.allGameObjects.Add(obj);
        IEdifice edifice = obj.GetComponentInChildren<IEdifice>();
        edifice.SetCurrent(this);
        edifice.team = Manager.currentTeam;
        //edifice.HighlightThis(true);
    }
}
