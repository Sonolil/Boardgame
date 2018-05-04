using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Unit : HexObject, IEdifice, IDestroyable
{
    public Manager manager;
    public bool alive = false;
    protected int _team;
    
    public int attack = 0;
    public int defense = 3;
    public int speed = 2;

    private int bonus = 0;
    protected Color _baseColor;

    public HexFinder finder;

    private Canvas damageCanvas;
    private Text damageText;
    private Vector3 damageCanvasOrigin;

    private bool _active;
    public bool active
    {
        get { return _active; }
        set { _active = value; }
    }
    
    protected IList<MonoBehaviour> targets = new List<MonoBehaviour>();

    public void Start()
    {
        Init();
    }
    
    protected override void Init()
    {
        base.Init();
        curr.edifice = this;
        finder.Init(this, Color.blue);

        damageCanvas = GetComponentInChildren<Canvas>();
        damageText = GetComponentInChildren<Text>();
        damageText.enabled = false;
        damageText.text = "0";

        GameObject obj = GameObject.FindGameObjectWithTag("Manager");
        manager = obj.GetComponent<Manager>();

        alive = true;
        Challenge();
    }

    public void SetCurrent(HexTile tile)
    {
        curr = tile;
    }

    virtual public void AfterTurn()
    {

    }

    public int team
    {
        set
        {
            _team = value;
            switch (_team)
            {
                case 0:
                    _rend.material.color += Color.yellow * 1.0f;
                    _baseColor = _rend.material.color;
                    _team = 0;
                    break;

                case 1:
                    _rend.material.color += Color.cyan * 1.0f;
                    _baseColor = _rend.material.color;
                    _team = 1;
                    break;

                case 2:
                    _rend.material.color += Color.magenta * 1.0f;
                    _baseColor = _rend.material.color;
                    _team = 2;
                    break;
            }

        }
        get { return _team; }
    }

    public bool CheckAPath(HexTile tile)
    {
        if (finder.visited.Contains(tile))
            return true;

        else return false;
    }

    public void moveTo(HexTile tile)
    {
        curr.edifice = null;
        tile.edifice = this;
        curr = tile;
        _tr.position = curr.transform.position;
        finder.Init(this, Color.blue);
        HighlightThis(false);


        Challenge();
    }

    public void Challenge()
    {
        bonus = 0;
        foreach (HexTile tile in curr.nears)
        {
            if (tile.edifice == null)
                continue;

            if (tile.edifice.team == team)
                bonus++;
        }

        int dice = Mathf.FloorToInt(Random.Range(1.01f, 6.99f));
        int totalAttack = manager.red[team] + bonus + dice;
        bool nearEnemy = false;

        foreach (HexTile tile in curr.nears)
        {
            IDestroyable destroyable = (IDestroyable) tile.edifice;


            if (destroyable != null)
            {
                if (destroyable.team == team)
                    continue;

                nearEnemy = true;
                bool survived = destroyable.Hit(totalAttack);
                
                if (!survived)
                {
                    alive = false;
                }
            }
        }

        if (!nearEnemy)
            return;
        
        damageText.color = Color.yellow;
        damageText.text = totalAttack.ToString();
        StartCoroutine("HitMotion");
        nearEnemy = false;
    }

    public void HighlightThis(bool swtch)
    {
        if (swtch)
        {
            if (!_active)
            {
                _active = true;
                _rend.material.color += _baseColor * 2;
            }
        }

        else
        {
            _active = false;
            _rend.material.color = _baseColor;
        }
    }

    public void Highlight(bool swtch)
    {
        if (swtch)
            finder.StartSearch(speed);
        else finder.ClearSearch();
    }

    public bool Hit(int ATT)
    {
        bonus = 0;
        foreach (HexTile tile in curr.nears)
        {
            if (tile.edifice == null)
                continue;

            if (tile.edifice.team == team)
                bonus++;
        }

        damageText.color = Color.white;
        int totalDefense = defense + bonus + manager.blue[team];

        damageText.text = totalDefense.ToString();
        StartCoroutine("HitMotion");
        if (ATT > totalDefense)
        {
            alive = false;
            return true;
        }
        else return false;
    }

    IEnumerator HitMotion()
    {
        damageText.enabled = true;
        damageCanvasOrigin = damageCanvas.transform.position;
        for (int i = 0; i < 30; i++)
        {
            yield return null;
            damageCanvas.transform.Translate(new Vector3(0, 0.03f, 0));
        }
        damageText.enabled = false;
        damageCanvas.transform.position = damageCanvasOrigin;

        if (!alive)
        {
            DestroyThis();
        }
    }

    public void DestroyThis()
    {
        if (team != Manager.currentTeam)
        {
            manager.AddCoin(Manager.currentTeam, 3);
            Debug.Log("Enemy: " + team + ", Team: " + Manager.currentTeam);
        }
            
        curr.edifice = null;
        Manager.allGameObjects.Remove(gameObject);
        Destroy(gameObject);
    }

    protected IEnumerator Trace()
    {
        yield return null;
        /*
        for (int i = 0; i < curr.nears.Count; i++)
        {
            Infra temp = curr.nears[i].infra;
            if (temp && temp.team != team)
            {
                target = temp;
                StartCoroutine("Work");
                yield break;
            }
        }

        if (!target || !finder.CheckTarget())
        {
            finder.Reset();
            HexTile tile = finder.Search();
            if (!tile) {
                StartCoroutine("Idle");
                yield break;
            }  else target = tile.infra;
        }

        StartCoroutine("Move");
        */
    }

    /*
    protected IEnumerator Move()
    {
        next = finder.path[0];
        if (next.infra)
        {
            finder.SetTemporaryObstacle(next);
            StartCoroutine("Idle");
            yield break;
        }

        curr.infra = null;
        next.infra = this;
        Transform tempTr = next.transform;
        while (_tr.position != tempTr.position)
        {
            yield return null;
            _tr.position = Vector3.MoveTowards(_tr.position, tempTr.position, Time.deltaTime * 2.0f);
        }

        curr = next;
        next = null;
        finder.path.RemoveAt(0);
        StartCoroutine("Trace");
    }

    protected virtual IEnumerator Work()
    {
        while (target.alive)
        {
            yield return new WaitForSeconds(0.3f);
            target.Hit(ATT);
        }
        
        target = null;
        StartCoroutine("Idle");
    }
    */
}
