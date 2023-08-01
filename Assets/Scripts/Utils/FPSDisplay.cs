using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    private float deltaTime = 0.0f;
    public Color textColor = Color.black;

    private void Update()
    {
        // ����֡��
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    private void OnGUI()
    {
        // ��ȡ֡��
        float fps = 1.0f / deltaTime;

        // ����GUIStyle��������ɫ
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.normal.textColor = textColor;

        // ����Ļ����ʾ֡��
        GUI.Label(new Rect(10, 10, 100, 20), "FPS: " + Mathf.Round(fps), style);
    }
}
