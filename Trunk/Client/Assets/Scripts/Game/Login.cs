using UnityEngine;
using UIFrame;
using UnityEngine.SceneManagement;

namespace WNGameBase
{
    public class Login : MonoBehaviour
    {
        void Start()
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync("Loading");
        }
    }
}
