using UnityEngine;

public class AssetIDData : FileData
{
    public string name;
    public string assetPath;
    public string initialSize;
    public string maxSize;

    public override void Init(string[] datas)
    {
        base.Init(datas);
        name = datas[2];
        assetPath = datas[3];
        initialSize = datas[4];
        maxSize = datas[5];
    }
}
