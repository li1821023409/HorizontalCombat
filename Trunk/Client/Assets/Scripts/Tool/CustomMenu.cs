using UnityEditor;
using UnityEngine;
using System.IO;
using ExcelDataReader;
using System.Text.RegularExpressions;
using System.Text;

namespace WNGameTool
{
    public class CustomMenu
    {
        // 在Tools菜单下添加一个自定义菜单项
        [MenuItem("Tools/Create ItemList")]
        private static void CreateItemList()
        {
            // 创建 SO_ItemList 的实例
            SO_ItemList itemList = ScriptableObject.CreateInstance<SO_ItemList>();

            // 使用保存路径可以根据需求定义
            string path = "Assets/Scripts/Game/Item/ItemList/New ItemList.asset";

            // 确保文件夹存在，如果没有，则创建它
            System.IO.Directory.CreateDirectory("Assets/Scripts/Game/Item/ItemList");

            // 创建资产并保存
            AssetDatabase.CreateAsset(itemList, path);
            AssetDatabase.SaveAssets();

            // 选中并聚焦到新创建的SO_ItemList
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = itemList;

            Debug.Log("SO_ItemList created at: " + path);
        }

        [MenuItem("Tools/Create InventoryDictionary")]
        private static void CreateInventoryDictionary()
        {
            // 创建 InventoryDictionary 的实例
            InventoryDictionary inventoryDictionary = ScriptableObject.CreateInstance<InventoryDictionary>();

            // 使用保存路径可以根据需求定义
            string path = StaticInventoryData.INVENTORY_DICTIONARY_PATH + "New InventoryDictionary.asset";

            // 确保文件夹存在，如果没有，则创建它
            System.IO.Directory.CreateDirectory("Assets/Scripts/Game/Item/InventoryDictionary");

            // 创建资产并保存
            AssetDatabase.CreateAsset(inventoryDictionary, path);
            AssetDatabase.SaveAssets();

            // 选中并聚焦到新创建的InventoryDictionary
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = inventoryDictionary;

            Debug.Log("inventoryDictionary created at: " + path);
        }

        public static InventoryDictionary CreateInventoryDictionary(string inventoryDicName = "")
        {
            // 创建 InventoryDictionary 的实例
            InventoryDictionary inventoryDictionary = ScriptableObject.CreateInstance<InventoryDictionary>();

            // 使用保存路径可以根据需求定义
            string path = inventoryDicName != "" ?
               StaticInventoryData.INVENTORY_DICTIONARY_PATH + "/" + inventoryDicName + ".asset" :
               StaticInventoryData.INVENTORY_DICTIONARY_PATH + "/" + "New InventoryDictionary.asset";

            // 确保文件夹存在，如果没有，则创建它
            System.IO.Directory.CreateDirectory(StaticInventoryData.INVENTORY_DICTIONARY_PATH);

            // 创建资产并保存
            AssetDatabase.CreateAsset(inventoryDictionary, path);
            AssetDatabase.SaveAssets();

            // 选中并聚焦到新创建的InventoryDictionary
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = inventoryDictionary;

            Debug.Log("inventoryDictionary created at: " + path);

            return inventoryDictionary;
        }

        [MenuItem("Tools/CreateCsv")]
        private static void CreateCsv()
        {
            // 显示文件选择对话框
            string selectedFile = EditorUtility.OpenFilePanel("选择Excel文件", CsvFilePath.XML_PATH, "xlsx");
            
            // 如果用户取消了选择，直接返回
            if (string.IsNullOrEmpty(selectedFile))
            {
                return;
            }
            
            // 将绝对路径转换为相对路径
            string relativePath = selectedFile.Replace(Application.dataPath, "Assets");
            
            string csvFolderPath = CsvFilePath.CSV_PATH;
            
            // 确保CSV文件夹存在
            if (!Directory.Exists(csvFolderPath))
            {
                Directory.CreateDirectory(csvFolderPath);
            }
            
            // 检查选择的文件是否存在
            if (!File.Exists(relativePath))
            {
                Debug.LogError($"选择的Excel文件不存在: {relativePath}");
                return;
            }
            
            try
            {
                // 使用ExcelDataReader读取Excel文件
                using (var stream = File.Open(relativePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        string csvFileName = Path.GetFileNameWithoutExtension(relativePath) + ".csv";
                        string csvFilePath = Path.Combine(csvFolderPath, csvFileName);
                        
                        using (StreamWriter writer = new StreamWriter(csvFilePath))
                        {
                            do
                            {
                                while (reader.Read())
                                {
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        if (i > 0)
                                            writer.Write(",");
                                        
                                        string value = reader.GetValue(i)?.ToString() ?? "";
                                        // 处理包含逗号的值
                                        if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
                                        {
                                            value = $"\"{value.Replace("\"", "\"\"")}\"";
                                        }
                                        writer.Write(value);
                                    }
                                    writer.WriteLine();
                                }
                            } while (reader.NextResult());
                        }
                        
                        Debug.Log($"成功转换: {Path.GetFileName(relativePath)} -> {csvFileName}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"转换文件 {Path.GetFileName(relativePath)} 时出错: {ex.Message}");
            }
            
            AssetDatabase.Refresh();
            Debug.Log("CSV转换完成！");
        }
        
        [MenuItem("Tools/CreateCsv For All")]
        private static void CreateCsvForAll()
        {
            string xmlFolderPath = CsvFilePath.XML_PATH;
            string csvFolderPath = CsvFilePath.CSV_PATH;
            
            // 确保CSV文件夹存在
            if (!Directory.Exists(csvFolderPath))
            {
                Directory.CreateDirectory(csvFolderPath);
            }
            
            // 检查Xml文件夹是否存在
            if (!Directory.Exists(xmlFolderPath))
            {
                Debug.LogError($"Xml文件夹不存在: {xmlFolderPath}");
                return;
            }
            
            // 获取所有.xlsx文件
            string[] excelFiles = Directory.GetFiles(xmlFolderPath, "*.xlsx");
            
            if (excelFiles.Length == 0)
            {
                Debug.LogWarning($"在 {xmlFolderPath} 中没有找到.xlsx文件");
                return;
            }
            
            int convertedCount = 0;
            
            foreach (string excelFile in excelFiles)
            {
                try
                {
                    // 使用ExcelDataReader读取Excel文件
                    using (var stream = File.Open(excelFile, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            string csvFileName = Path.GetFileNameWithoutExtension(excelFile) + ".csv";
                            string csvFilePath = Path.Combine(csvFolderPath, csvFileName);
                            
                            using (StreamWriter writer = new StreamWriter(csvFilePath))
                            {
                                do
                                {
                                    while (reader.Read())
                                    {
                                        for (int i = 0; i < reader.FieldCount; i++)
                                        {
                                            if (i > 0)
                                                writer.Write(",");
                                            
                                            string value = reader.GetValue(i)?.ToString() ?? "";
                                            // 处理包含逗号的值
                                            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
                                            {
                                                value = $"\"{value.Replace("\"", "\"\"")}\"";
                                            }
                                            writer.Write(value);
                                        }
                                        writer.WriteLine();
                                    }
                                } while (reader.NextResult());
                            }
                            
                            convertedCount++;
                            Debug.Log($"成功转换: {Path.GetFileName(excelFile)} -> {csvFileName}");
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"转换文件 {Path.GetFileName(excelFile)} 时出错: {ex.Message}");
                }
            }
            
            Debug.Log($"转换完成！共转换了 {convertedCount} 个文件");
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Generate FileData Classes")]
        private static void GenerateFileDataClasses()
        {
            // 显示文件选择对话框
            string selectedFile = EditorUtility.OpenFilePanel("选择CSV文件", CsvFilePath.CSV_PATH, "csv");
            
            // 如果用户取消了选择，直接返回
            if (string.IsNullOrEmpty(selectedFile))
            {
                return;
            }
            
            // 将绝对路径转换为相对路径
            string relativePath = selectedFile.Replace(Application.dataPath, "Assets");
            
            string fileDataFolderPath = CsvFilePath.FILE_DATA_PATH;
            
            // 确保FileData文件夹存在
            if (!Directory.Exists(fileDataFolderPath))
            {
                Directory.CreateDirectory(fileDataFolderPath);
                AssetDatabase.Refresh();
            }
            
            // 检查选择的文件是否存在
            if (!File.Exists(relativePath))
            {
                Debug.LogError($"选择的CSV文件不存在: {relativePath}");
                return;
            }
            
            try
            {
                string fileName = Path.GetFileNameWithoutExtension(relativePath);
                string className = fileName + "Data";
                string filePath = Path.Combine(fileDataFolderPath, className + ".cs");
                
                // 读取CSV文件获取列名
                string[] columnNames = GetColumnNamesFromCsv(relativePath);
                
                if (columnNames == null || columnNames.Length <= 0)
                {
                    Debug.LogWarning($"CSV文件 {fileName} 列数不足，跳过生成, 列数为：" + columnNames.Length);
                    return;
                }
                
                // 检查文件是否已存在
                bool fileExists = File.Exists(filePath);
                
                if (fileExists)
                {
                    // 检查是否需要更新
                    if (NeedUpdateFileDataClass(filePath, columnNames))
                    {
                        UpdateFileDataClass(filePath, className, columnNames);
                        Debug.Log($"更新FileData类: {className}");
                    }
                    else
                    {
                        Debug.Log($"FileData类已是最新: {className}");
                    }
                }
                else
                {
                    // 生成新的FileData类
                    GenerateNewFileDataClass(filePath, className, columnNames);
                    Debug.Log($"生成FileData类: {className}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"处理CSV文件 {Path.GetFileName(relativePath)} 时出错: {ex.Message}");
            }
            
            AssetDatabase.Refresh();
            Debug.Log($"FileData类生成完成！");
        }
        
        [MenuItem("Tools/Generate FileData Classes For All")]
        private static void GenerateFileDataClassesForAll()
        {
            string csvFolderPath = CsvFilePath.CSV_PATH;
            string fileDataFolderPath = CsvFilePath.FILE_DATA_PATH;
            
            // 确保FileData文件夹存在
            if (!Directory.Exists(fileDataFolderPath))
            {
                Directory.CreateDirectory(fileDataFolderPath);
                AssetDatabase.Refresh();
            }
            
            // 检查CSV文件夹是否存在
            if (!Directory.Exists(csvFolderPath))
            {
                Debug.LogError($"CSV文件夹不存在: {csvFolderPath}");
                return;
            }
            
            // 获取所有.csv文件
            string[] csvFiles = Directory.GetFiles(csvFolderPath, "*.csv");
            
            if (csvFiles.Length == 0)
            {
                Debug.LogWarning($"在 {csvFolderPath} 中没有找到.csv文件");
                return;
            }
            
            int generatedCount = 0;
            int updatedCount = 0;
            
            foreach (string csvFile in csvFiles)
            {
                try
                {
                    string fileName = Path.GetFileNameWithoutExtension(csvFile);
                    string className = fileName + "Data";
                    string filePath = Path.Combine(fileDataFolderPath, className + ".cs");
                    
                    // 读取CSV文件获取列名
                    string[] columnNames = GetColumnNamesFromCsv(csvFile);
                    
                    if (columnNames == null || columnNames.Length <= 0)
                    {
                        Debug.LogWarning($"CSV文件 {fileName} 列数不足，跳过生成");
                        continue;
                    }
                    
                    // 检查文件是否已存在
                    bool fileExists = File.Exists(filePath);
                    
                    if (fileExists)
                    {
                        // 检查是否需要更新
                        if (NeedUpdateFileDataClass(filePath, columnNames))
                        {
                            UpdateFileDataClass(filePath, className, columnNames);
                            updatedCount++;
                            Debug.Log($"更新FileData类: {className}");
                        }
                        else
                        {
                            Debug.Log($"FileData类已是最新: {className}");
                        }
                    }
                    else
                    {
                        // 生成新的FileData类
                        GenerateNewFileDataClass(filePath, className, columnNames);
                        generatedCount++;
                        Debug.Log($"生成FileData类: {className}");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"处理CSV文件 {Path.GetFileName(csvFile)} 时出错: {ex.Message}");
                }
            }
            
            AssetDatabase.Refresh();
            Debug.Log($"FileData类生成完成！新生成: {generatedCount} 个，更新: {updatedCount} 个");
        }
        
        private static string[] GetColumnNamesFromCsv(string csvFilePath)
        {
            try
            {
                using (StreamReader reader = new StreamReader(csvFilePath))
                {
                    // 读取第一行（中文列名）
                    string chineseLine = reader.ReadLine();
                    // 读取第二行（英文列名）
                    string englishLine = reader.ReadLine();
                    
                    if (string.IsNullOrEmpty(englishLine))
                    {
                        return null;
                    }
                    
                    string[] columns = englishLine.Split(',');
                    
                    // 跳过前两列（id和type），返回其他列名
                    if (columns.Length > 2)
                    {
                        string[] result = new string[columns.Length - 2];
                        for (int i = 2; i < columns.Length; i++)
                        {
                            result[i - 2] = columns[i];
                        }
                        return result;
                    }
                    
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"读取CSV文件列名时出错: {ex.Message}");
                return null;
            }
        }
        
        private static bool NeedUpdateFileDataClass(string filePath, string[] newColumnNames)
        {
            try
            {
                string content = File.ReadAllText(filePath);
                
                // 检查现有的字段
                foreach (string columnName in newColumnNames)
                {
                    string fieldName = ConvertToFieldName(columnName);
                    if (!content.Contains($"public string {fieldName}"))
                    {
                        return true;
                    }
                }
                
                return false;
            }
            catch
            {
                return true;
            }
        }
        
        private static void UpdateFileDataClass(string filePath, string className, string[] columnNames)
        {
            try
            {
                string content = File.ReadAllText(filePath);
                
                // 生成新的字段定义
                string newFields = GenerateFieldsCode(columnNames);
                
                // 生成新的Init方法
                string newInitMethod = GenerateInitMethod(columnNames);
                
                // 使用正则表达式替换字段和Init方法
                string pattern = @"public class " + className + @" : FileData\s*\{[^}]*\}";
                string replacement = $"public class {className} : FileData\n{{\n{newFields}\n\n{newInitMethod}\n}}";
                
                content = Regex.Replace(content, pattern, replacement, RegexOptions.Singleline);
                
                File.WriteAllText(filePath, content);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"更新FileData类时出错: {ex.Message}");
            }
        }
        
        private static void GenerateNewFileDataClass(string filePath, string className, string[] columnNames)
        {
            string fieldsCode = GenerateFieldsCode(columnNames);
            string initMethod = GenerateInitMethod(columnNames);
            
            string classCode = $"using UnityEngine;\n\npublic class {className} : FileData\n{{\n{fieldsCode}\n\n    public override void Init(string[] datas)\n    {{\n        base.Init(datas);\n{initMethod}\n    }}\n}}";
            
            File.WriteAllText(filePath, classCode);
        }
        
        private static string GenerateFieldsCode(string[] columnNames)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
            foreach (string columnName in columnNames)
            {
                string fieldName = ConvertToFieldName(columnName);
                sb.AppendLine($"    public string {fieldName};");
            }
            
            return sb.ToString().TrimEnd();
        }
        
        private static string GenerateInitMethod(string[] columnNames)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
            for (int i = 0; i < columnNames.Length; i++)
            {
                string fieldName = ConvertToFieldName(columnNames[i]);
                sb.AppendLine($"        {fieldName} = datas[{i + 2}];");
            }
            
            return sb.ToString().TrimEnd();
        }
        

        
        private static string ConvertToFieldName(string columnName)
        {
            // 将列名转换为合法的C#字段名
            string fieldName = columnName.ToLower();
            
            // 移除特殊字符，只保留字母数字
            fieldName = Regex.Replace(fieldName, @"[^a-zA-Z0-9]", " ");
            
            // 将每个单词首字母大写
            string[] words = fieldName.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (!string.IsNullOrEmpty(words[i]))
                {
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);
                }
            }
            
            // 将第一个字母小写（C#字段命名规范）
            string result = string.Join("", words);
            if (result.Length > 0)
            {
                result = char.ToLower(result[0]) + result.Substring(1);
            }
            
            return result;
        }

        // 在Tools菜单下添加一个分割线
        [MenuItem("Tools/Settings")]
        private static void Settings()
        {
            // 用于设置的工具，可以包含更多功能
            Debug.Log("Settings Tool Clicked!");
        }

        // 使用Validate方法来控制菜单项的启用状态
        [MenuItem("Tools/Another Tool", true)]
        private static bool ValidateAnotherTool()
        {
            // 返回true则菜单项可用，返回false则禁用
            return Application.isPlaying; // 例如，只在播放模式下显示该菜单项
        }
    }
}