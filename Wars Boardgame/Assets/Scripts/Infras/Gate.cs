
public class Gate : Infra
{
    public bool red;

    protected override void Init()
    {
        base.Init();
        if (red)
            manager.AddRed(team, 2);
        else manager.AddBlue(team, 2);
    }

    public override void DestroyThis()
    {
        base.DestroyThis();
        if (red)
            manager.AddRed(team, -2);
        else manager.AddBlue(team, -2);
    }



}
