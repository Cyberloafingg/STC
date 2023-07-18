using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVReader
{
    /// <summary>
    /// ��ȡ CSV �ļ���
    /// </summary>
    /// <param name="text">Ҫ������text��</param>
    /// <param name="ignore_rows">Ҫ���Ե�ͷ��������</param>
    /// <returns>��ȡ�����֡�</returns>
    /// <exception cref="System.ArgumentException">����ļ�������ʱ�׳��쳣��</exception>
    static public string[][] CSVReadText(string text, int ignore_rows)
    {
        // if (!System.IO.File.Exists(path))
        //     throw new System.ArgumentException("CSVRead path \"" + path + "\" �����ڡ�");

        // string file = System.IO.File.ReadAllText(path);
        string[] lines = text.Split("\n");

        string[][] result = new string[lines.Length - ignore_rows][];
        for (int i = ignore_rows; i < lines.Length; i++)
            result[i - ignore_rows] = lines[i].Split(",");
        return result;
    }

    /// <summary>
    /// ��ȡ CSV �ļ���
    /// </summary>
    /// <param name="testAsset">Ҫ��ȡ�� TextAsset��</param>
    /// <param name="ignore_rows">Ҫ���Ե�ͷ��������</param>
    /// <returns>��ȡ�����֡�</returns>
    /// <exception cref="System.ArgumentException">����ļ�������ʱ�׳��쳣��</exception>
    static public string[][] CSVReadAsset(TextAsset testAsset, int ignore_rows)
    {
        return CSVReadText(testAsset.text, ignore_rows);
    }

    static public float[][] CSVToFloatTable(TextAsset csv, int ignore_rows)
    {
        Debug.Log("Parsing file: " + csv.name);
        string[][] text_table = CSVReadAsset(csv, ignore_rows);
        float[][] table = new float[text_table.Length][];
        for (int i = 0; i < text_table.Length; i++)
        {
            table[i] = new float[text_table[i].Length];
            for (int j = 0; j < text_table[i].Length; j++)
            {
                if (text_table[i][j].Trim() != "")
                {
                    // Debug.Log("Parsing (i, j) = (" + i + ", " + j + ")");
                    table[i][j] = float.Parse(text_table[i][j]);
                }
            }
        }
        return table;
    }

    static public int[][] CSVToIntTable(TextAsset csv, int ignore_rows)
    {
        string[][] text_table = CSVReadAsset(csv, ignore_rows);
        int[][] table = new int[text_table.Length][];
        for (int i = 0; i < text_table.Length; i++)
        {
            table[i] = new int[text_table[i].Length];
            for (int j = 0; j < text_table[i].Length; j++)
            {
                if (text_table[i][j].Trim() != "")
                {
                    table[i][j] = int.Parse(text_table[i][j]);
                }
            }
        }
        return table;
    }


}
