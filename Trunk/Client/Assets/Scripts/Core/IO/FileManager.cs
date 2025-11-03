using UnityEngine;
using UIFrame;
using System.Collections.Generic;
using System.IO;
using System.Collections;

namespace FileIO
{
    public class FileManager : BaseManager<FileManager>
    {
        /// <summary>
        /// 存储读取过的csv文件数据
        /// </summary>
        public Dictionary<string, List<FileData>> fileDictionary = new Dictionary<string, List<FileData>>();

        /// <summary>
        /// 读取csv文件
        /// 存储类型为list列表
        /// </summary>
        /// <param name="csvFileData"></param>
        /// <returns></returns>
        public List<T> ReadCSVFilesToList<T>(string fileName) where T : FileData, new()
        {
            if (!fileName.StartsWith('/'))
                fileName = FilePaths.root + fileName + ".csv";

            List<T> datas = new List<T>();
            List<FileData> fileDatas = new List<FileData>();

            try
            {
                using (StreamReader st = new StreamReader(fileName))
                {
                    while (!st.EndOfStream)
                    {
                        string data = st.ReadLine();
                        var values = data.Split(',');
                        if (!string.IsNullOrEmpty(data))
                        {
                            T csvData = new T();
                            csvData.Init(values);
                            datas.Add(csvData);
                        }
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                Debug.LogError($"找不到文件'{ex.FileName}'");
            }

            return datas;
        }

        public List<string> ReadTextAsset(string filePath)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(filePath);
            if (textAsset == null)
            {
                Debug.LogError($"找不到文件'{filePath}'");
            }
            return ReadTextAsset(textAsset);
        }

        public List<string> ReadTextAsset(TextAsset asset)
        {
            List<string> lines = new List<string>();
            using (StreamReader st = new StreamReader(asset.text))
            {
                string line = st.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    Debug.LogError("存在空数据，请检查文件数据是否错误");
                    return null;
                }
                var values = line.Split(',');
                if (!string.IsNullOrEmpty(line))
                {
                    lines.Add(line);
                }
            }
            return lines;
        }

        /// <summary>
        /// 存储类型为字典
        /// 字典的Key为第一列表内的数据，一般为ID（如果不一致，请在获取的列表中单独标明）
        /// </summary>
        /// <param name="csvFileData"></param>
        /// <returns></returns>
        public Dictionary<string, T> ReadCSVFilesToDictionary<T>(string fileName) where T : FileData, new()
        {
            if (!fileName.StartsWith('/'))
                fileName = FilePaths.root + fileName + ".csv";

            Dictionary<string, T> datas = new Dictionary<string, T>();
            try
            {
                using (StreamReader st = new StreamReader(fileName))
                {
                    while (!st.EndOfStream)
                    {
                        string data = st.ReadLine();
                        var values = data.Split(',');
                        if (!string.IsNullOrEmpty(data))
                        {
                            T csvData = new T();
                            csvData.Init(values);
                            datas.Add(values[0], csvData);
                        }
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                Debug.LogError($"找不到文件'{ex.FileName}'");
            }

            return datas;
        }

        public override void Init()
        {

        }
    }
}
