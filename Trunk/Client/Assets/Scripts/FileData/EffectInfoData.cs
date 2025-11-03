public class EffectInfoData : FileData
{
    public string id;
    public string name;
    public string runTime;

    public override void Init(string[] datas)
    {
        id = datas[0];
        name = datas[1];
        runTime = datas[2];
    }
}
