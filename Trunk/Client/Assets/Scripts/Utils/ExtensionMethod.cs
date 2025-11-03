// Create by DongShengLi At 2024/5/20
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 字典扩展方法
/// </summary>
public static class DictionaryExtension
{
    public static string ToKeyValueString(this Dictionary<string, string> dic)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var item in dic)
        {
            sb.Append(item.Key);
            sb.Append("=");
            sb.Append(item.Value?.ToString() ?? "null");
            sb.Append(",");
        }
        return sb.ToString();
    }
}

/// <summary>
/// Unity扩展类
/// Gameobject
/// </summary>
public static class GameobjectExtension
{

    /// <summary>
    /// 改变对象的层级
    /// </summary>
    /// <param name="gameobject">对象</param>
    /// <param name="layerName">层级名称</param>
    public static void ChangeLayer(this GameObject gameobject, string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        if (gameobject)
        {
            foreach (Transform traform in gameobject.GetComponentsInChildren<Transform>(true))
            {
                traform.gameObject.layer = layer;
            }
        }
    }

    /// <summary>
    /// 改变对象的层级
    /// </summary>
    /// <param name="gameobject">对象</param>
    /// <param name="layer">层级编号</param>
    public static void ChangeLayer(this GameObject gameobject, int layer)
    {
        if (gameobject)
        {
            foreach (Transform traform in gameobject.GetComponentsInChildren<Transform>(true))
            {
                traform.gameObject.layer = layer;
            }
        }
    }

    /// <summary>
    /// 给对象加上元件
    /// 对象身上不存在这个元件
    /// 存在则不作处理
    /// </summary>
    /// <typeparam name="T">元件</typeparam>
    /// <param name="gameObject">对象</param>
    /// <returns></returns>
    public static T AddComponentIfNotFound<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();

        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }

        return component;
    }

    /// <summary>
    /// 移除物体上挂载的脚本
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="gameObject"></param>
    public static void Remove<T>(this GameObject gameObject) where T : MonoBehaviour
    {
        if (gameObject.GetComponent<T>())
        {
            GameObject.Destroy(gameObject.GetComponent<T>());
        }
    }
}

