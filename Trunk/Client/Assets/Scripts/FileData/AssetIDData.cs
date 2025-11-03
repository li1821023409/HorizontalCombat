public class AssetIDData : FileData
{
    public string id;
    public string name;
    public string assetPath;
    public string assetType;
    public string initialSize;
    public string maxSize;

    public override void Init(string[] datas)
    {
        id = datas[0];
        name = datas[1];
        assetPath = datas[2];
        assetType = datas[3];
        initialSize = datas[4];
        maxSize = datas[5];
    }
}
