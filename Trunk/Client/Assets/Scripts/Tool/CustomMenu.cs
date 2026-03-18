using UnityEditor;
using UnityEngine;

public class CustomMenu
{
    // 在Tools菜单下添加一个自定义菜单项
    [MenuItem("Tools/Create ItemList")]
    private static void CreateItemList()
    {
        // 创建 SO_ItemList 的实例
        SO_ItemList itemList = ScriptableObject.CreateInstance<SO_ItemList>();

        // 使用保存路径可以根据需求定义
        string path = "Assets/Scripts/Game/Item/ItemList/New ItemList.asset";

        // 确保文件夹存在，如果没有，则创建它
        System.IO.Directory.CreateDirectory("Assets/Scripts/Game/Item/ItemList");

        // 创建资产并保存
        AssetDatabase.CreateAsset(itemList, path);
        AssetDatabase.SaveAssets();

        // 选中并聚焦到新创建的SO_ItemList
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = itemList;

        Debug.Log("SO_ItemList created at: " + path);
    }

    // 在Tools菜单下添加一个分割线
    [MenuItem("Tools/Settings")]
    private static void Settings()
    {
        // 用于设置的工具，可以包含更多功能
        Debug.Log("Settings Tool Clicked!");
    }

    // 使用Validate方法来控制菜单项的启用状态
    [MenuItem("Tools/Another Tool", true)]
    private static bool ValidateAnotherTool()
    {
        // 返回true则菜单项可用，返回false则禁用
        return Application.isPlaying; // 例如，只在播放模式下显示该菜单项
    }
}