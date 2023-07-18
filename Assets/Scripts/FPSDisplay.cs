using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    private float deltaTime = 0.0f;
    public Color textColor = Color.black;

    private void Update()
    {
        // 计算帧率
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    private void OnGUI()
    {
        // 获取帧率
        float fps = 1.0f / deltaTime;

        // 创建GUIStyle并设置颜色
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.normal.textColor = textColor;

        // 在屏幕上显示帧率
        GUI.Label(new Rect(10, 10, 100, 20), "FPS: " + Mathf.Round(fps), style);
    }
}
