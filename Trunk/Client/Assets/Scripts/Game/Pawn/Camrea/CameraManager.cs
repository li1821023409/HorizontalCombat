using System.Collections;
using UIFrame;
using UnityEngine;

namespace WNGameBase
{
    public class CameraManager : UnitySingleton<CameraManager>
    {
        /*2D游戏这里只需要两个相机，一个是主相机渲染场景，另一个是UI相机渲染Ui界面*/

        /// <summary>
        /// 主相机
        /// </summary>
        public Camera MainCamera = null;

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

        #region 地图的四个方向的范围，相机不能超出这个范围
        // 左边边移动距离
        private float m_LeftMapDistance = 100f;
        // 右边移动距离
        private float m_RightMapDistance = 100f;
        // 上面移动距离
        private float m_TopMapDistance = 100f;
        // 下面移动距离
        private float m_BelowMapDistance = 100f;
        #endregion
        private Vector3 CameraPosition = Vector3.zero;
        private const float CAMERA_Z_DISTANCE = -10f;

        public void Init()
        {
            // 初始化的时候从场景获取UI相机和本地玩家相机
            // 本地玩家相机默认为MainCamera
            MainCamera = Camera.main;
            UICamera = GameObject.Find("UICamera").GetComponent<Camera>();

            // TODU : 小地图相机现在应该是空，后面做小地图的时候再补上
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
            if (LocalPlayer != null && MainCamera != null)
            {
                // 这里先简单写一下，后面进行精细的处理
                CameraPosition = LocalPlayer.transform.position;
                CameraPosition = new Vector3(
                    Mathf.Clamp(CameraPosition.x, -m_LeftMapDistance, m_RightMapDistance),
                    Mathf.Clamp(CameraPosition.y, -m_TopMapDistance, m_BelowMapDistance),
                    CAMERA_Z_DISTANCE);

                MainCamera.transform.position = CameraPosition;
            }
        }
    }
}
