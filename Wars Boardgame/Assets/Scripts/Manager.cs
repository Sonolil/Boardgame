using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Manager : MonoBehaviour {
    public static int currentTeam;


    private List<int> _costs = new List<int>();

    private List<int> _coins = new List<int>();
    public List<int> coins
    {
        get { return _coins; }
        set { _coins = value; }
    }

    private List<int> _blue = new List<int>();
    public List<int> blue
    {
        get { return _blue; }
        set { _blue = value; }
    }

    private List<int> _red = new List<int>();
    public List<int> red
    {
        get { return _red; }
        set { _red = value; }
    }



    public List<Text> coinTexts = new List<Text>();
    public List<Text> redTexts = new List<Text>();
    public List<Text> blueTexts = new List<Text>();

    public List<HQ> hqs = new List<HQ>();

    public int teamNumber = 3;
    public GameObject cam;

    private IEdifice _select;
    private enum GameState { Init = 0, Select, PopUp, PopDown, CheckInfra, CheckUnit, Build };
    private GameState _state;

    public List<GameObject> blueprints = new List<GameObject>();
    public enum ObjectNum { Comb = 0, Farm = 1, Flag = 2, Red, Blue }
    private int _buildInfra;

    public GameObject endImage;
    public Image menuImage;
    public BuildMenu menu;
    private Vector3 buildOpen = new Vector3(378, -73, 0);
    private Vector3 buildClose = new Vector3(378, 13, 0);

    public static List<GameObject> allGameObjects = new List<GameObject>();

    void Start()
    {
        currentTeam = 0;
        _state = GameState.Init;

        for (int i = 0; i < hqs.Count; i++) 
        {
            allGameObjects.Add(hqs[i].gameObject);
            hqs[i].team = i;
        }

        HighlightObjects();

        for (int i = 0; i < teamNumber; i++)
        {
            coins.Add(0);
            red.Add(0);
            blue.Add(0);
            AddCoin(i, 5);
            AddRed(i, 0);
            AddBlue(i, 0);
        }

        AddCoin(0, 1);

        _costs.Add(3);
        _costs.Add(5);
        _costs.Add(10);
        _costs.Add(7);
        _costs.Add(7);
    }

    private void HighlightObjects()
    {
        for (int i = 0; i < allGameObjects.Count; i++)
        {
            IEdifice edifice = allGameObjects[i].GetComponent<IEdifice>();
            if (edifice.team == currentTeam)
                edifice.HighlightThis(true);
            else edifice.HighlightThis(false);
        }
    }

    public void AddCoin(int team, int amount)
    {
        coins[team] += amount;
        coinTexts[team].text = coins[team].ToString();
    }

    public void AddRed(int team, int amount)
    {
        red[team] += amount;
        redTexts[team].text = red[team].ToString();
    }

    public void AddBlue(int team, int amount)
    {
        blue[team] += amount;
        blueTexts[team].text = blue[team].ToString();
    }

    void Update()
    {
        switch (_state)
        {
            case GameState.Init:
                _state = GameState.Select;
                break;

            case GameState.Select:
                SelectEdifice();
                break;

            case GameState.CheckInfra:
                CheckInfra();
                break;

            case GameState.CheckUnit:
                CheckUnit();
                break;

            case GameState.Build:
                Build();
                break;
        }
    }

    public void Sell()
    {

        if (_select is HQ)
            return;

        _select.DestroyThis();
        if (_select is Comb)
            AddCoin(currentTeam, 2);

        else if (_select is Farm)
            AddCoin(currentTeam, 3);

        else if (_select is Flag)
            AddCoin(currentTeam, 5);

        else if (_select is Gate)
            AddCoin(currentTeam, 4);

        Debug.Log("sold!");
        _state = GameState.Select;
    }

    private void SelectEdifice()
    {
        PopDown();
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                HexTile tile = hit.collider.GetComponentInParent<HexTile>();
                IEdifice edifice = hit.collider.GetComponentInParent<IEdifice>();

                if (tile)
                {
                    if (tile.edifice != null)
                    {
                        _select = tile.edifice;
                    }
                }

                if (edifice != null)
                {
                    _select = edifice;
                }
            }


            if (_select != null)
            {
                if (_select.team != currentTeam)
                    return;

                if (!_select.active)
                    return;

                if (_select is Infra)
                {
                    _state = GameState.CheckInfra;
                    return;
                }

                else if (_select is Unit)
                {
                    _select.Highlight(true);
                    _state = GameState.CheckUnit;
                    return;
                }
            }
        }
    }

    private void CheckInfra()
    {
        List<int> buttonTypes = new List<int>();
        
        if (_select is Flag || _select is HQ)
        {
            PopUp();
            buttonTypes.Add((int) ObjectNum.Comb);
            buttonTypes.Add((int) ObjectNum.Farm);
            buttonTypes.Add((int) ObjectNum.Flag);
            menu.SetButton(buttonTypes, _coins[currentTeam]);
        }

        if (_select is Comb)
        {
            PopUp();
            buttonTypes.Add((int)ObjectNum.Red);
            buttonTypes.Add((int)ObjectNum.Blue);
            menu.SetButton(buttonTypes, _coins[currentTeam]);
        }

        if (Input.GetMouseButtonDown(1))
        {
            _state = GameState.Select;
            _select.Highlight(false);
        }
    }

    public void EndTurn()
    {
        

        currentTeam++;
        if (currentTeam == teamNumber)
            currentTeam = 0;

        if (!hqs[currentTeam])
        {
            currentTeam++;
            if (currentTeam == teamNumber)
                currentTeam = 0;
        }

        for (int i = 0; i < allGameObjects.Count; i++)
        {
            IEdifice edifice = allGameObjects[i].GetComponent<IEdifice>();
            edifice.AfterTurn();
        }

        HighlightObjects();

        _state = GameState.Select;
    }

    public void BuildMode(int buildInfra)
    {
        if (_coins[currentTeam] < _costs[buildInfra])
            return;

        _buildInfra = buildInfra;
        _select.Highlight(true);
        _state = GameState.Build;
    }

    private void Build()
    {
        PopDown();
        if (Input.GetMouseButtonDown(1))
        {
            _state = GameState.Select;
            _select.Highlight(false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                HexTile tile = hit.collider.GetComponentInParent<HexTile>();

                if (tile)
                {
                    if (_select.CheckAPath(tile))
                    {
                        tile.InstantiateHexObject(blueprints[_buildInfra]);

                        AddCoin(currentTeam, -1 * _costs[_buildInfra]);
                        _select.Highlight(false);
                        _select = null;
                        _state = GameState.Select;
                    }
                }
            }
        }
    }

    private void CheckUnit()
    {
        PopDown();

        if (Input.GetMouseButtonDown(1))
        {
            _state = GameState.Select;
            _select.Highlight(false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                HexTile tile = hit.collider.GetComponentInParent<HexTile>();

                if (tile)
                {
                    if (_select.CheckAPath(tile))
                    {
                        _select.moveTo(tile);
                        _select.Highlight(false);
                        _select = null;
                        _state = GameState.Select;
                    }
                }
            }
        }
    }

    private void PopUp()
    {
        menuImage.rectTransform.position = Vector3.Lerp(menuImage.rectTransform.position, buildClose, 0.2f);
        if (menuImage.rectTransform.position == buildClose)
        {
            _state = GameState.CheckInfra;
            Debug.Log("up");
        }
    }

    private void PopDown()
    {
        menuImage.rectTransform.position = Vector3.Lerp(menuImage.rectTransform.position, buildOpen, 0.2f);
        if (menuImage.rectTransform.position == buildOpen)
        {
            _state = GameState.Select;
            Debug.Log("down");
        }
    }

}
