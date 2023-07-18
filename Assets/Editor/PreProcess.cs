using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class PreProcess : EditorWindow
{
    [MenuItem("Window/PreProcess")]
    private static void Init()
    {
        PreProcess window = (PreProcess)EditorWindow.GetWindow(typeof(PreProcess));
        window.Show();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Process CSV"))
        {
            ProcessCSV();
        }
    }

    private void ProcessCSV()
    {
        // 选择要读取的CSV文件
        string csvPath = EditorUtility.OpenFilePanel("Select CSV File", "", "csv");
        Debug.Log(csvPath);
        string csvFileName = Path.GetFileNameWithoutExtension(csvPath);
        if (!string.IsNullOrEmpty(csvPath))
        {
            // 读取CSV文件的内容
            string[] csvLines = File.ReadAllLines(csvPath);

            // 创建一个字典，用于按照 Track Number 存储行数据
            Dictionary<string, List<string>> trackDataDict = new Dictionary<string, List<string>>();

            // 跳过标题行，从第二行开始读取数据
            for (int i = 1; i < csvLines.Length; i++)
            {
                string[] data = csvLines[i].Split(',');

                // 确保行数据格式正确
                if (data.Length >= 8)
                {
                    string trackNumber = data[0].Trim();

                    // 检查是否已存在该 Track Number 的数据列表，若不存在则创建新列表
                    if (!trackDataDict.ContainsKey(trackNumber))
                    {
                        trackDataDict[trackNumber] = new List<string>();
                    }

                    // 获取需要的列的数据，按照指定顺序组合为新行数据
                    string newLine = string.Format("{0},{1},{2},{3}", trackNumber, data[5].Trim(), data[6].Trim(), data[7].Trim());

                    // 将新行数据添加到对应 Track Number 的数据列表中
                    trackDataDict[trackNumber].Add(newLine);
                }
            }

            // 选择要保存的文件夹
            string resourceFolderPath = Application.dataPath + "/Resources";
            string saveFolder = EditorUtility.OpenFolderPanel("Select Save Folder", resourceFolderPath, "");

            if (!string.IsNullOrEmpty(saveFolder))
            {
                // 遍历每个 Track Number 的数据列表，保存为单独的 CSV 文件
                foreach (var kvp in trackDataDict)
                {
                    string trackNumber = kvp.Key;
                    List<string> trackData = kvp.Value;

                    // 创建保存路径
                    string savePath = Path.Combine(saveFolder, $"{csvFileName}_{trackNumber.Substring(6, 2)}.csv");

                    // 将数据列表转换为字符串数组，准备写入文件
                    string[] linesToWrite = new string[trackData.Count + 1];
                    linesToWrite[0] = "Track Number,Latitude,Longitude,Date and Time";
                    trackData.CopyTo(linesToWrite, 1);

                    // 将处理后的内容保存到新的 CSV 文件中
                    File.WriteAllLines(savePath, linesToWrite);
                }

                Debug.Log("CSV processed and saved successfully!");
            }
            else
            {
                Debug.LogWarning("Save folder not selected. CSV not saved.");
            }
        }
        else
        {
            Debug.LogWarning("CSV file not selected. Processing aborted.");
        }
    }
}
