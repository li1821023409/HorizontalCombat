public class EffectInfoData : FileData
{
    public string name;
    public string runTime;

    public override void Init(string[] datas)
    {
        base.Init(datas);
        name = datas[2];
        runTime = datas[3];
    }
}
