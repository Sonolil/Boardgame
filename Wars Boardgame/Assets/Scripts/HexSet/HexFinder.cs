using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class HexFinder {
    public bool found = false;
    public List<HexTile> path = new List<HexTile>();

    private HexTile _next;
    private HexTile _dest;
    private HexTile _origin;
    private HexObject _obj;
    private Color _highlight;

    public List<HexTile> visited = new List<HexTile>();
    private List<HexTile> _thisStep = new List<HexTile>();
    private List<HexTile> _nextStep = new List<HexTile>();

    public void Init(HexObject obj, Color highlight)
    {
        _obj = obj;
        _origin = obj.curr;
        _highlight = highlight;
    }

    public void StartSearch(int step)
    {
        _thisStep.Add(_origin);

        while (step > 0)
        {
            step--;
            for (int i=0; i < _thisStep.Count; i++)
            {
                List<HexTile> nears = _thisStep[i].nears;
                for (int j = 0; j < nears.Count; j++)
                {
                    if (nears[j].pass == false)
                        continue;
                    
                    if (_obj is Unit)
                    {
                        if (nears[j].edifice != null)
                            continue;
                    }

                    _nextStep.Add(nears[j]);
                }
            }

            for (int i = 0; i < _thisStep.Count; i++)
            {
                if (!visited.Contains(_thisStep[i]))
                visited.Add(_thisStep[i]);
            }

            for (int i = 0; i < _nextStep.Count; i++)
            {
                _thisStep.Add(_nextStep[i]);
            }
        }

        for (int i = 0; i < _thisStep.Count; i++)
        {
            if (!visited.Contains(_thisStep[i]))
                visited.Add(_thisStep[i]);
        }

        for(int i = 0; i < visited.Count; i++)
        {
            if (visited[i].edifice != null)
                continue;
            visited[i].Highlight(true, _highlight);
        }

        _origin.Highlight(false, _highlight);
        visited.Remove(_origin);
    }

    public void ClearSearch()
    {
        for (int i = 0; i < visited.Count; i++)
        {
            visited[i].Highlight(false, _highlight);
        }
        visited.Clear();
        _thisStep.Clear();
        _nextStep.Clear();
    }

    public void RemoveInfraNears()
    {
        for (int i = 0; i < visited.Count; i++)
        {
            List<HexTile> nears = visited[i].nears;

            for (int j = 0; j < nears.Count; j++)
            {
                if (nears[j].edifice is Infra)
                {
                    visited[i].Highlight(false, _highlight);
                    visited.Remove(visited[i]);
                    i--;
                    break;
                }
            }
        }
    }
    
}
