using UnityEngine;
using UIFrame;
using UnityEngine.SceneManagement;
using WNEngine;

namespace WNGameBase
{
    public class Login : MonoBehaviour
    {
        public GameBuilder m_GameBuilder;

        void Start()
        {
            m_GameBuilder = GameBuilder.Instance;
            m_GameBuilder.SceneLoader.LoadPersistentScene(m_GameBuilder.MapSceneName);
        }
    }
}
