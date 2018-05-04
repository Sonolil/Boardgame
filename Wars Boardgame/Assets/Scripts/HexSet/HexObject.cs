using UnityEngine;
using System.Collections;

public class HexObject : MonoBehaviour {
    public HexTile curr;
    public bool isObstacle;
    protected Transform _tr;
    protected Renderer _rend;

    void Awake()
    {
        _rend = GetComponentInChildren<Renderer>();
        _tr = transform;

    }

    void Start ()
    {
        Init();
    }

    protected virtual void Init()
    {
        if(isObstacle)
            curr.pass = false;

        _tr.position = curr.transform.position;
        
    }

    public void SetColor(Color c)
    {
        if (_rend.material)
            _rend.material.color = c;
    }
}
