
public class Farm : Infra {

    protected override void Init()
    {
        base.Init();
    }


    override public void AfterTurn()
    {
        if (Manager.currentTeam == team)
            manager.AddCoin(team, 2);
    }

}
