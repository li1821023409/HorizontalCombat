using System.Collections;
using System.Collections.Generic;
using UIFrame;
using UnityEngine;
using UnityEngine.InputSystem;
using WNEngine;

namespace WNGameBase
{
    /// <summary>
    /// 游戏运行的相关信息和开关
    /// </summary>
    public class GameInfo : Singleton<GameInfo>
    {
        /// <summary>
        /// NonePawns列表
        /// </summary>
        public List<Pawn> m_NonePawns = new List<Pawn>();

        /// <summary>
        /// 玩家可能存在可以切换的Pawn对象，这里创建成列表
        /// </summary>
        public List<Pawn> m_LocalPlayerPawns = new List<Pawn>();

        /// <summary>
        /// FriendlyForcesPawns列表
        /// </summary>
        public List<Pawn> m_FriendlyForcesPawns = new List<Pawn>();

        /// <summary>
        /// EnemyPawns列表
        /// </summary>
        public List<Pawn> m_EnemyPawns = new List<Pawn>();

        /// <summary>
        /// SceneObjectPawns列表
        /// </summary>
        public List<Pawn> m_SceneObjectPawns = new List<Pawn>();

        public LocalPlayerPawn m_LocalPlayerPawn = null;

        public PawnInfo m_LocalPlayerPawnInfo = null;

        public void Init()
        {
        }
    }
}
