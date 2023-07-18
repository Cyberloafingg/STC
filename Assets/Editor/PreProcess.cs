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
        // ѡ��Ҫ��ȡ��CSV�ļ�
        string csvPath = EditorUtility.OpenFilePanel("Select CSV File", "", "csv");
        Debug.Log(csvPath);
        string csvFileName = Path.GetFileNameWithoutExtension(csvPath);
        if (!string.IsNullOrEmpty(csvPath))
        {
            // ��ȡCSV�ļ�������
            string[] csvLines = File.ReadAllLines(csvPath);

            // ����һ���ֵ䣬���ڰ��� Track Number �洢������
            Dictionary<string, List<string>> trackDataDict = new Dictionary<string, List<string>>();

            // ���������У��ӵڶ��п�ʼ��ȡ����
            for (int i = 1; i < csvLines.Length; i++)
            {
                string[] data = csvLines[i].Split(',');

                // ȷ�������ݸ�ʽ��ȷ
                if (data.Length >= 8)
                {
                    string trackNumber = data[0].Trim();

                    // ����Ƿ��Ѵ��ڸ� Track Number �������б����������򴴽����б�
                    if (!trackDataDict.ContainsKey(trackNumber))
                    {
                        trackDataDict[trackNumber] = new List<string>();
                    }

                    // ��ȡ��Ҫ���е����ݣ�����ָ��˳�����Ϊ��������
                    string newLine = string.Format("{0},{1},{2},{3}", trackNumber, data[5].Trim(), data[6].Trim(), data[7].Trim());

                    // ������������ӵ���Ӧ Track Number �������б���
                    trackDataDict[trackNumber].Add(newLine);
                }
            }

            // ѡ��Ҫ������ļ���
            string resourceFolderPath = Application.dataPath + "/Resources";
            string saveFolder = EditorUtility.OpenFolderPanel("Select Save Folder", resourceFolderPath, "");

            if (!string.IsNullOrEmpty(saveFolder))
            {
                // ����ÿ�� Track Number �������б�����Ϊ������ CSV �ļ�
                foreach (var kvp in trackDataDict)
                {
                    string trackNumber = kvp.Key;
                    List<string> trackData = kvp.Value;

                    // ��������·��
                    string savePath = Path.Combine(saveFolder, $"{csvFileName}_{trackNumber.Substring(6, 2)}.csv");

                    // �������б�ת��Ϊ�ַ������飬׼��д���ļ�
                    string[] linesToWrite = new string[trackData.Count + 1];
                    linesToWrite[0] = "Track Number,Latitude,Longitude,Date and Time";
                    trackData.CopyTo(linesToWrite, 1);

                    // �����������ݱ��浽�µ� CSV �ļ���
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
