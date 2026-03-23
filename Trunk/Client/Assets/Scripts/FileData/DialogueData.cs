using UnityEngine;

public class DialogueData : FileData
{
    public string name;
    public string diaLogue;

    public override void Init(string[] datas)
    {
        base.Init(datas);
        name = datas[0];
        diaLogue = datas[1];
    }
}
