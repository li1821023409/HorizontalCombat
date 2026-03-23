# FileData类自动生成器使用说明

## 功能概述

这个工具可以根据CSV配置文件自动生成对应的FileData子类，简化数据配置类的开发工作。

## 使用方法

1. **准备CSV文件**：确保CSV文件位于 `Assets/Resources/Config/Csv/` 目录下
2. **CSV文件格式要求**：
   - 第一行：中文列名说明
   - 第二行：英文列名（实际字段名）
   - 第三行开始：数据
   - 前两列必须是：ID和AssetType

3. **生成FileData类**：
   - 在Unity编辑器中，点击菜单：`Tools -> Generate FileData Classes`
   - 工具会自动扫描CSV文件夹，为每个CSV文件生成对应的FileData子类
   - 生成的类文件保存在 `Assets/Scripts/FileData/` 目录下

## 示例

### CSV文件示例 (ItemInfo.csv)
```csv
道具id,资产类型,道具名,道具详细介绍,道具创建路径
ItemID,AssetType（详细查看AssetTypeEnum枚举类型）,ItemName,ItemDetailedDescription,ItemPath（Assets/Resources）
1001,1,双手,勤劳的双手,Prefabs/Items/Hand
1002,1,锄头,一把普通的锄头，可以用于锄地，将草地改为农耕土地,Prefabs/Items/Hoe
```

### 生成的FileData类 (ItemInfoData.cs)
```csharp
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
```

## 功能特性

- **自动检测更新**：如果CSV文件结构发生变化，工具会自动更新对应的FileData类
- **字段名转换**：自动将CSV列名转换为合法的C#字段名
- **错误处理**：提供详细的错误信息和日志输出
- **批量处理**：一次性处理所有CSV文件

## 注意事项

- 确保CSV文件的编码为UTF-8
- 字段名转换规则：移除特殊字符，转换为驼峰命名法
- 如果手动修改了生成的类文件，再次生成时可能会被覆盖
- 建议在修改CSV结构后重新生成FileData类

## 支持的CSV文件

当前支持的CSV文件：
- AssetID.csv
- EffectInfo.csv  
- ItemInfo.csv
- PawnInfo.csv

## 技术实现

- 使用StreamReader读取CSV文件
- 使用正则表达式进行字段名转换
- 使用Unity的MenuItem特性创建编辑器菜单
- 自动处理文件创建和更新逻辑