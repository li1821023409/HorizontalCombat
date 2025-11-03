using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 实现普通的单例模式
// where 限制模板的类型, new()指的是这个类型必须要能被实例化
public class Singleton<T> where T : new () {
    private static T _instance;
    private static object mutex = new object ();
    public static T Instance {
        get {
            if (_instance == null) {
                lock (mutex) { // 保证我们的单例，是线程安全的;
                    if (_instance == null) {
                        _instance = new T ();
                    }
                }
            }
            return _instance;
        }
    }
}

// Monobeavior: 声音, 网络
// Unity单例

public class UnitySingleton<T> : MonoBehaviour where T : Component {
    private static object _lock = new object ();
    private static T _instance = null;

    public static T Instance {
        get {
            if (applicationIsQuitting) {
                // Debug.LogWarning ("[Singleton] Instance '" + typeof (T) +
                //     "' already destroyed on application quit." +
                //     " Won't create again - returning null.");
                return _instance;
            }

            if (_instance == null) {
                _instance = (T) FindObjectOfType (typeof (T));

                if (FindObjectsOfType (typeof (T)).Length > 1) {
                    Debug.LogError ("[Singleton] Something went really wrong " +
                        " - there should never be more than 1 singleton!" +
                        " Reopenning the scene might fix it.");
                    return _instance;
                }

                lock (_lock) {
                    if (_instance == null) {
                        GameObject singleton = new GameObject ();
                        _instance = singleton.AddComponent<T> ();
                        //移过来
                        _instance.hideFlags = HideFlags.DontSave;
                        singleton.name = "(singleton) " + typeof (T).ToString ();

                        DontDestroyOnLoad (singleton);

                        Debug.Log ("[Singleton] An instance of " + typeof (T) +
                            " is needed in the scene, so '" + singleton +
                            "' was created with DontDestroyOnLoad.");
                    } else {
                        Debug.Log ("[Singleton] Using instance already created: " +
                            _instance.gameObject.name);
                    }
                }
            }

            return _instance;

        }
    }

    private static bool applicationIsQuitting = false;
    /// <summary>
    /// When Unity quits, it destroys objects in a random order.
    /// In principle, a Singleton is only destroyed when application quits.
    /// If any script calls Instance after it have been destroyed, 
    ///   it will create a buggy ghost object that will stay on the Editor scene
    ///   even after stopping playing the Application. Really bad!
    /// So, this was made to be sure we're not creating that buggy ghost object.
    /// </summary>
    private void OnApplicationQuit () {
        applicationIsQuitting = true;
    }
}