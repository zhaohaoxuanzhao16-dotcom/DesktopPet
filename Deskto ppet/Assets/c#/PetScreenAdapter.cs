using UnityEngine;

public class PetScreenAdapter : MonoBehaviour
{
    private global::Kirurobo.UniWindowController windowController;

    void Start()
    {
        windowController = FindObjectOfType<global::Kirurobo.UniWindowController>();
        if (windowController == null)
            Debug.LogError("未找到 UniWindowController");
    }

    void LateUpdate()
    {
        if (windowController == null) return;

        Vector2 winSize = windowController.windowSize;
        Vector2 pos = windowController.windowPosition;

        // macOS 上窗口坐标使用的是“逻辑点”，而 Screen.currentResolution 返回物理像素
        // 所以需要除以 2（Retina 缩放）
        float screenW = Screen.currentResolution.width;
        float screenH = Screen.currentResolution.height;

#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        screenW /= 2f;
        screenH /= 2f;
#endif

        float maxX = Mathf.Max(0, screenW - winSize.x);
        float maxY = Mathf.Max(0, screenH - winSize.y);

        pos.x = Mathf.Clamp(pos.x, 0, maxX);
        pos.y = Mathf.Clamp(pos.y, 0, maxY);

        windowController.windowPosition = pos;
    }
}