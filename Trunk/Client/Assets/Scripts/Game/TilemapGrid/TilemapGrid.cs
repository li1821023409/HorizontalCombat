using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WNGameBase
{
    public class TilemapGrid : MonoBehaviour
    {
        /// <summary>
        /// TilemapGrid根节点
        /// </summary>
        public Transform TilemapGridRoot;
        /// <summary>
        /// 界面层级节点:Instances 这里用于创建Pawn
        /// </summary>
        public Transform InstancesLevel;

        /*下面的层级节点用于创建掉落物、家具、NPC等*/
        /// <summary>
        /// 界面层级节点:BoolCanPlaceFumiture
        /// </summary>
        public Transform BoolCanPlaceFumitureLevel;
        /// <summary>
        /// 界面层级节点:BoolCanFropItem
        /// </summary>
        public Transform BoolCanFropItemLevel;
        /// <summary>
        /// 界面层级节点:BoolDiggable
        /// </summary>
        public Transform BoolDiggableLevel;
        /// <summary>
        /// 界面层级节点:BoolPath
        /// </summary>
        public Transform BoolPathLevel;
        /// <summary>
        /// 界面层级节点:BoolNpcObstacie
        /// </summary>
        public Transform BoolNpcObstacieLevel;
    }
}
