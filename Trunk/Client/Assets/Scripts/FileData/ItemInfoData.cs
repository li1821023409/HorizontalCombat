using UnityEngine;

public class ItemInfoData : FileData
{
    public string itemName;
    public string itemDetailedDescription;
    public string itemPath;

    public override void Init(string[] datas)
    {
        base.Init(datas);
        itemName = datas[2];
        itemDetailedDescription = datas[3];
        itemPath = datas[4];
    }
}