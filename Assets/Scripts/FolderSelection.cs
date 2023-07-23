using UnityEngine;
using System;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;
using System.Collections;

public class FolderSelection : MonoBehaviour
{
    string stringPath = "";

    public string targetSceneName; // 在Inspector面板中指定要切换到的场景名称

    public void OpenFolderDialog()
    {
        OpenFileName ofn = new OpenFileName();

        ofn.structSize = Marshal.SizeOf(ofn);
        //可进行修改选择的文件类型
        ofn.file = new string(new char[256]);

        ofn.maxFile = ofn.file.Length;

        ofn.fileTitle = new string(new char[64]);

        ofn.maxFileTitle = ofn.fileTitle.Length;
        string path = Application.streamingAssetsPath;
        path = path.Replace('/', '\\');
        //默认路径
        ofn.initialDir = path;

        ofn.title = "选择需要替换的图片";

        ofn.defExt = "JPG";//显示文件的类型
                           //注意 一下项目不一定要全选 但是0x00000008项不要缺少
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR

        if (WindowDll.GetOpenFileName(ofn))
        {
            Debug.Log(ofn.file);
            // 获取其父目录
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
            // 这里可以显示加载界面或进度条
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            Debug.Log("Loading progress: " + (progress * 100) + "%");

            yield return null;
        }
    }
}
