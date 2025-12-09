using System.Collections.Generic;
using UnityEngine;

public class FileData
{
    // ID和是Type基础的，一定要有的
    public string id;
    public string type;

    // 之后的数据赋值从datas[2]开始
    public virtual void Init(string[] datas)
    {
        id = datas[0];
        type = datas[1];
    }
}
