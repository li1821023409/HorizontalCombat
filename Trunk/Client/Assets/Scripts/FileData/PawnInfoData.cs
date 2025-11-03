public class PawnInfoData : FileData
{
    public string id;
    public string name;
    public string assetType;
    public string healthPoint;
    public string attack;
    public string moveSpeed;
    public string jumpForce;
    public string skillID;


    public override void Init(string[] datas)
    {
        id = datas[0];
        name = datas[1];
        assetType = datas[2];
        healthPoint = datas[3];
        attack = datas[4];
        moveSpeed = datas[5];
        jumpForce = datas[6];
        skillID = datas[7];
    }
}
