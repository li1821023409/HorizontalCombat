using System.Collections.Generic;

public class DialogueData : FileData
{
    public string name;
    public string diaLogue;

    public override void Init(string[] datas)
    {
        name = datas[0];
        diaLogue = datas[1];
    }
}
