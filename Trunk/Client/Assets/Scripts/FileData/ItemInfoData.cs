public class ItemInfoData : FileData
{
    public string itemName;
    public string itemSpriteName; // 临时补的，后面需要加进来
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