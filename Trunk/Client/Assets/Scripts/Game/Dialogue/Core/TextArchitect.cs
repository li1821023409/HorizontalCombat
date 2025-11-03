using UnityEngine;
using TMPro;
using System.Collections;

namespace Dialogue
{
    public enum BuildMethod
    {
        None = 0,
        Instant = 1,    // ���̲���
        Typewriter = 2, // ���ֲ���
        Fade = 3,       // ����ʧ
    }

    /// <summary>
    /// 文本构架管理
    /// </summary>
    public class TextArchitect
    {
        public TextMeshProUGUI m_TMProUI;
        public TextMeshPro m_TMProWorld;
        public TMP_Text TMPro
        {
            get { return m_TMProUI != null ? m_TMProUI : m_TMProWorld; }
        }
        /// <summary>
        /// 当前文本
        /// </summary>
        public string CurremtText
        {
            get { return TMPro.text; }
        }

        /// <summary>
        /// 待处理文本
        /// 需要根据模式进行显示处理的文本
        /// </summary>
        public string TargetText { get; private set; } = "";
        /// <summary>
        /// 需要提前显示文本
        /// </summary>
        public string ProText { get; private set; } = "";
        public int ProTextLinght = 0;
        /// <summary>
        /// ����Ŀ���ı�
        /// </summary>
        public string FulltargetText
        {
            get { return TargetText + ProText; }
        }

        /// <summary>
        /// 文本生成模式
        /// </summary>
        public BuildMethod m_BuildMethod = BuildMethod.Typewriter;

        // 基本生成速度
        private const float m_BaseSpeed = 1;
        // 生成倍率
        private float m_SpeedMultiplier = 1;

        public float DialogueSpeed
        {
            set { m_SpeedMultiplier = value; }
            get { return  m_BaseSpeed * m_SpeedMultiplier; }
        }

        public Color m_TextColor
        {
            set { m_TextColor = value; }
            get { return m_TextColor; }
        }

        private bool m_IsHurryUp = false;
        /// <summary>
        /// 是否跳过
        /// </summary>
        public bool IsHurryUp
        {
            set { m_IsHurryUp = value; }
            get { return m_IsHurryUp; }
        }

        private int m_CharacterMultiplier = 1;
        /// <summary>
        /// 倍数增加倍率
        /// </summary>
        public int CharacterMultiplier
        {
            get
            {
                return DialogueSpeed <= 2f ? m_CharacterMultiplier :
                    DialogueSpeed < 3f ? m_CharacterMultiplier * 2 :
                    m_CharacterMultiplier * 3;
            }
        }

        /// <summary>
        /// 赋值m_TMProUI
        /// </summary>
        /// <param name="tMProUI"></param>
        public TextArchitect(TextMeshProUGUI tMProUI)
        {
            m_TMProUI = tMProUI;
        }

        /// <summary>
        /// 赋值m_TMProWorld
        /// </summary>
        /// <param name="tMProWorld"></param>
        public TextArchitect(TextMeshPro tMProWorld)
        {
            m_TMProWorld = tMProWorld;
        }

        /// <summary>
        /// 创建对话文本
        /// 每个对话开始时创建
        /// </summary>
        /// <returns></returns>
        public Coroutine Build(string text)
        {
            ProText = "";
            TargetText = text;
            StopBuild();
            BuildProcess = TMPro.StartCoroutine(Building(text));
            return BuildProcess;
        }

        /// <summary>
        /// 追加对话文本
        /// 在已经创建的对话文本后面增加对话文本
        /// </summary>
        /// <returns></returns>
        public Coroutine Append(string text)
        {
            ProText = TMPro.text;
            TargetText = text;
            StopBuild();
            BuildProcess = TMPro.StartCoroutine(Building(text));
            return BuildProcess;
        }


        /// <summary>
        /// 强制结束对话
        /// 用于强制显示当前所有对话内容
        /// </summary>
        public void ForceComplete()
        {
            switch (m_BuildMethod)
            {
                case BuildMethod.Instant:
                    break;
                case BuildMethod.Typewriter:
                    TMPro.maxVisibleCharacters = FulltargetText.Length;
                    break;
                case BuildMethod.Fade:
                    break;
            }
        }

        /// <summary>
        /// 停止对话生成
        /// </summary>
        public void StopBuild()
        {
            if (!IsBuild)
                return;
            TMPro.StopCoroutine(BuildProcess);
            BuildProcess = null;
            IsHurryUp = false;
            OnComplete();
        }

        /// <summary>
        /// 对话完成后数据处理
        /// </summary>
        private void OnComplete()
        {
            BuildProcess = null;
            IsHurryUp = false;
        }

        Coroutine BuildProcess;
        public bool IsBuild
        {
            get { return BuildProcess != null; }
        }

        IEnumerator Building(string text)
        {
            Prepare();
            switch (m_BuildMethod)
            {
                case BuildMethod.Instant:
                    break;
                case BuildMethod.Typewriter:
                    yield return BuildTypewriter();
                    break;
                case BuildMethod.Fade:
                    yield return BuildFade();
                    break;
            }
            OnComplete();
        }
        
        /// <summary>
        /// 文本创建前数据准备
        /// </summary>
        private void Prepare()
        {
            switch (m_BuildMethod)
            {
                case BuildMethod.Instant:
                    PrepareInstant();
                    break;
                case BuildMethod.Typewriter:
                    PrepareTypewriter();
                    break;
                case BuildMethod.Fade:
                    PrepareFade();
                    break;
            }
        }

        private void PrepareInstant()
        {
            TMPro.color = TMPro.color;
            TMPro.text = FulltargetText;
            // 强制更新文本内容
            TMPro.ForceMeshUpdate();
            // 最大显示数量为文本对应数量
            TMPro.maxVisibleCharacters = TMPro.textInfo.characterCount;
        }

        private void PrepareTypewriter()
        {
            // 重制颜色
            TMPro.color = TMPro.color;
            TMPro.maxVisibleCharacters = 0;
            // 显示需要提前显示的文本
            TMPro.text = ProText;
            if (ProText != "")
            {
                TMPro.ForceMeshUpdate();
                TMPro.maxVisibleCharacters = TMPro.textInfo.characterCount;
            }

            TMPro.text += TargetText;
            TMPro.ForceMeshUpdate();
        }

        private void PrepareFade()
        {

        }

        IEnumerator BuildTypewriter()
        {
            while (TMPro.maxVisibleCharacters < FulltargetText.Length)
            {
                TMPro.maxVisibleCharacters += m_IsHurryUp ? CharacterMultiplier * 5 : CharacterMultiplier; 

                yield return new WaitForSeconds(0.05f / DialogueSpeed);
            }
        }

        IEnumerator BuildFade()
        {
            yield return null;
        }


    }
}
