using UnityEngine;
using System.Collections.Generic;

public class Comb : Infra {
    public GameObject regen;
    public Unit unit;

    void Awake()
    {
        _rend = GetComponentInChildren<Renderer>();
        _tr = transform;
    }

    void Start()
    {
        Init();
        StartCoroutine("Wake");
    }

    protected override void Init()
    {
        base.Init();
    }

    override public void AfterTurn()
    {
        if (unit)
            return;

        if (team != Manager.currentTeam)
            return;
        
        GameObject obj = Instantiate(regen);
        Manager.allGameObjects.Add(obj);
        unit = obj.GetComponentInChildren<Unit>();
        unit.team = team;
        curr.Shuffle();

        foreach (HexTile near in curr.nears)
        {
            if (near.edifice != null)
                continue;
        
            else {
                unit.curr = near;
                near.edifice = unit;
                break;
            }
        }
    }
}
