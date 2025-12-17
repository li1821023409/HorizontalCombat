using System.Collections;
using UIFrame;
using UnityEngine;
using Cinemachine;

namespace WNGameBase
{
    public class CameraManager : UnitySingleton<CameraManager>
    {
        /// <summary>
        /// 玩家相机
        /// </summary>
        private Camera m_PlayerCamera = null;
        public Camera PlayerCamera
        {  
            get { return m_PlayerCamera; } 
            set 
            { 
                if (PlayerCinemachineBrain == null && value != null)
                {
                    PlayerCinemachineBrain = value.gameObject.GetComponent<CinemachineBrain>();
                    m_PlayerCamera = value;
                }
                else
                {
                    Debug.LogError("[aoandouli] PlayerCamera not bound CinemachineBrain");
                }
            }
        }

        /// <summary>
        /// 玩家虚拟相机控制器
        /// </summary>
        private CinemachineBrain m_PlayerCinemachineBrain = null;
        public CinemachineBrain PlayerCinemachineBrain
        {
            get { return m_PlayerCinemachineBrain; }
            set { m_PlayerCinemachineBrain = value; }
        }

        /// <summary>
        /// 玩家正在使用的虚拟相机
        /// </summary>
        private CinemachineVirtualCamera m_PlayerVirtualCamera = null;
        public CinemachineVirtualCamera PlayerVirtualCamera
        {
            get { return m_PlayerVirtualCamera; }
            set { m_PlayerVirtualCamera = value; }
        }

        /// <summary>
        /// UI相机
        /// </summary>
        public Camera UICamera = null;

        /// <summary>
        /// 小地图相机
        /// </summary>
        public Camera MiniMapCamera = null;

        private Pawn m_LocalPlayer;
        public Pawn LocalPlayer
        {
            set { m_LocalPlayer = value; }
            get 
            {
                if (m_LocalPlayer != null)
                {
                    return m_LocalPlayer;
                }
                else
                {
                    if (GameInfo.Instance != null && GameInfo.Instance.m_LocalPlayerPawns.Count > 0)
                    {
                        return GameInfo.Instance.m_LocalPlayerPawns[0];
                    }
                }
                return null;
            }
        }

        public void VirtualCameraFollow()
        {
            if (PlayerCinemachineBrain != null && LocalPlayer != null)
            {
                PlayerVirtualCamera = PlayerCinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera;
                if (PlayerVirtualCamera != null)
                {
                    PlayerVirtualCamera.Follow = LocalPlayer.gameObject.transform;
                    PlayerVirtualCamera.LookAt = LocalPlayer.gameObject.transform;
                }
                else
                {
                    Debug.LogError("[aoandouli] CinemachineBrain not bound CinemachineVirtualCamera");
                }
            }
        }

        public void Init()
        {
            // TODO : 小地图相机现在应该是空，后面做小地图的时候再补上
            //MiniMapCamera = GameObject.Find("MiniMapCamera").GetComponent<Camera>();
            StartCoroutine(RegularlyUpdated());
        }


        // TODO : 这里应该是获取小地图数据，然后定时更新的，但是现在没有小地图，后面补上
        //public virtual 

        /// <summary>
        /// 定期更新相机相关数据
        /// 这里的延迟刷新主要是刷新小地图中ui的变化，相机位置跟随这里不要延迟刷新
        /// </summary>
        /// <returns></returns>
        IEnumerator RegularlyUpdated()
        {
            while (true)
            {
                UpdateCameraMove();
                yield return null;
            }
        }

        private void UpdateCameraMove()
        {
        }
    }
}
