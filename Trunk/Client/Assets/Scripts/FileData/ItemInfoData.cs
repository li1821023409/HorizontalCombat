public class ItemInfoData : FileData
{
    public string itemId;
    public string itemType;
    public string itemName;
    public string itemDetailedDescription;
    public string itemPath;


    public override void Init(string[] datas)
    {
        itemId = datas[0];
        itemType = datas[1];
        itemName = datas[2];
        itemDetailedDescription = datas[3];
        itemPath = datas[4];
    }
}