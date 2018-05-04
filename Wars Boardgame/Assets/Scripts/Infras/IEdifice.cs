
public interface IEdifice {
    bool Hit(int ATT);
    void Highlight(bool swtch);
    bool CheckAPath(HexTile tile);
    void moveTo(HexTile tile);
    int team { get; set; }
    void SetCurrent(HexTile tile);
    void AfterTurn();
    void HighlightThis(bool swtch);
    bool active { get; set; }
    void DestroyThis();
}
