using UnityEngine;
using System.Collections;

public class HQ : Flag {
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
        if (Manager.currentTeam == team)
            manager.AddCoin(team, 1);
    }

    override public void DestroyThis()
    {
        if (team != Manager.currentTeam)
        {
            manager.AddCoin(Manager.currentTeam, 5);
            Debug.Log("Enemy: " + team + ", Team: " + Manager.currentTeam);
        }

        curr.edifice = null;
        manager.hqs[team] = null;
        Manager.allGameObjects.Remove(gameObject);
        Destroy(gameObject);
    }
}
