using UnityEngine;

public class PetScreenAdapter : MonoBehaviour
{
    private global::Kirurobo.UniWindowController windowController;

    public bool rememberPosition = true;
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

    // 用 UniWindowController 自己维护的逻辑屏幕尺寸
    // 或者用 Display.main.systemWidth / systemHeight
    float screenW = Display.main.systemWidth;
    float screenH = Display.main.systemHeight;

#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
    // macOS Retina 下物理像素是逻辑点的 2 倍
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