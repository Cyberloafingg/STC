using UnityEngine;
using System;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;
using System.Collections;

public class FolderSelection : MonoBehaviour
{
    string stringPath = "";

    public string targetSceneName; // ��Inspector�����ָ��Ҫ�л����ĳ�������

    public void OpenFolderDialog()
    {
        OpenFileName ofn = new OpenFileName();

        ofn.structSize = Marshal.SizeOf(ofn);
        //�ɽ����޸�ѡ����ļ�����
        ofn.file = new string(new char[256]);

        ofn.maxFile = ofn.file.Length;

        ofn.fileTitle = new string(new char[64]);

        ofn.maxFileTitle = ofn.fileTitle.Length;
        string path = Application.streamingAssetsPath;
        path = path.Replace('/', '\\');
        //Ĭ��·��
        ofn.initialDir = path;

        ofn.title = "ѡ����Ҫ�滻��ͼƬ";

        ofn.defExt = "JPG";//��ʾ�ļ�������
                           //ע�� һ����Ŀ��һ��Ҫȫѡ ����0x00000008�Ҫȱ��
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR

        if (WindowDll.GetOpenFileName(ofn))
        {
            Debug.Log(ofn.file);
            // ��ȡ�丸Ŀ¼
            stringPath = System.IO.Path.GetDirectoryName(ofn.file);
            PlayerPrefs.SetString("SelectedFolderPath", stringPath);
            Debug.Log(stringPath);
            LoadTargetSceneAsync();
        }
    }


    public void LoadTargetSceneAsync()
    {
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetSceneName);

        while (!asyncLoad.isDone)
        {
            // ���������ʾ���ؽ���������
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            Debug.Log("Loading progress: " + (progress * 100) + "%");

            yield return null;
        }
    }
}
