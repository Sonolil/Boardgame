using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Infra : HexObject, IEdifice, IDestroyable
{
    public Manager manager;
    public bool alive = false;

    public int defense = 5;
    public int range = 5;
    private int bonus = 0;

    protected int _team;
    private bool _active;
    
    private Canvas damageCanvas;
    private Text damageText;
    private Vector3 damageCanvasOrigin;

    public bool active
    {
        get { return _active; }
        set { _active = value; }
    }

    private Color _baseColor;
    public HexFinder finder;

    void Start()
    {
        Init();
        StartCoroutine("Wake");
    }

    public void SetCurrent(HexTile tile)
    {
        curr = tile;
    }
    protected override void Init()
    {
        base.Init();
        curr.edifice = this;
        finder.Init(this, Color.green/3);
        
        damageCanvas = GetComponentInChildren<Canvas>();
        damageText = GetComponentInChildren<Text>();
        damageText.enabled = false;
        damageText.text = "0";

        GameObject obj = GameObject.FindGameObjectWithTag("Manager");
        manager = obj.GetComponent<Manager>();
    }

    public void ReturnColor()
    {
        _rend.material.color = _baseColor;
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

    protected IEnumerator Wake()
    {
        //Waking Animation
        yield return new WaitForSeconds(0.3f);
        alive = true;
        StartCoroutine("Idle");
    }
    
    protected virtual IEnumerator Idle()
    {
        yield return new WaitForSeconds(0.3f);
        StartCoroutine("Active");
    }

    protected virtual IEnumerator Active()
    {
        yield return new WaitForSeconds(10.0f);
    }

    public void moveTo(HexTile tile)
    {

    }

    public bool CheckAPath(HexTile tile)
    {
        if (finder.visited.Contains(tile))
            return true;

        else return false;
    }

    public void Highlight(bool swtch)
    {
        if (swtch)
        {
            finder.StartSearch(range);
            finder.RemoveInfraNears();
        }
            
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
        int totalDefense = defense + manager.blue[team];


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

    virtual public void DestroyThis()
    {
        if (team != Manager.currentTeam)
        {
            manager.AddCoin(Manager.currentTeam, 1);
            Debug.Log("Enemy: " + team + ", Team: " + Manager.currentTeam);
        }

        curr.edifice = null;
        Manager.allGameObjects.Remove(gameObject);
        Destroy(gameObject);
    }

    protected virtual IEnumerator Die()
    {
        //Dying Animation
        SetColor(Color.black);
        yield return new WaitForSeconds(0.5f);

        //Destroy Object
        curr.edifice = null;
        curr = null;
        _tr = null;
        _rend = null;
        
        Destroy(this.gameObject);
    }

    /*
    //use to damage infra
    public void Hit(int ATT)
    {
        HP -= ATT;
        _bar.UpdateBar(HP / MaxHP);
        //gameObject.GetComponentInChildren<>;
        if (HP <= 0)
        {
            alive = false;
            StopAllCoroutines();
            StartCoroutine("Die");
            return;
        }
        else StartCoroutine("HitMotion");
    }

    private IEnumerator HitMotion()
    {
        _mat.SetColor("_Color", Color.gray);
        yield return new WaitForSeconds(0.1f);
        _manager.SetColor(this);
        StopCoroutine("Hit");
    }
    */
}
