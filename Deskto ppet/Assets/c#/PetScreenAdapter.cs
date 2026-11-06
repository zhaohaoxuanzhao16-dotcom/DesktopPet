using UnityEngine;

public class PetScreenAdapter : MonoBehaviour
{
    public bool rememberPosition = true;

    private global::Kirurobo.UniWindowController windowController;

    void Start()
    {
        windowController = FindObjectOfType<global::Kirurobo.UniWindowController>();
        if (windowController == null)
        {
            Debug.LogError("未找到 UniWindowController");
            return;
        }

        if (rememberPosition && PlayerPrefs.HasKey("PetPosX"))
        {
            Invoke(nameof(RestorePosition), 0.1f); 
        }
    }

    private void RestorePosition()
    {
        if (windowController != null)
        {
            windowController.windowPosition = new Vector2(
                PlayerPrefs.GetFloat("PetPosX"), 
                PlayerPrefs.GetFloat("PetPosY")
            );
        }
    }

    void LateUpdate()
    {
        if (windowController == null) return;

        Vector2 winSize = windowController.windowSize;
        Vector2 pos = windowController.windowPosition;

        // 获取屏幕逻辑尺寸
        float screenW = Display.main.systemWidth;
        float screenH = Display.main.systemHeight;

        // 计算最大可移动范围
        // X: 0 到 屏幕宽度减去窗口宽度
        // Y: 0 到 屏幕高度减去窗口高度
        float maxX = Mathf.Max(0, screenW - winSize.x);
        float maxY = Mathf.Max(0, screenH - winSize.y);

        // 关键修复：这里的 Clamp 直接限制在 0 到最大宽高之间
        // 这样你就能让桌宠的脚底完美对齐屏幕的最底部 (Y = 0)
        pos.x = Mathf.Clamp(pos.x, 0, maxX);
        pos.y = Mathf.Clamp(pos.y, 0, maxY);

        // 只有在位置被改变时才赋值，防止无意义的死循环拉扯导致闪烁
        if (windowController.windowPosition != pos)
        {
            windowController.windowPosition = pos;
        }
    }

    void OnApplicationQuit()
    {
        if (rememberPosition && windowController != null)
        {
            Vector2 pos = windowController.windowPosition;
            PlayerPrefs.SetFloat("PetPosX", pos.x);
            PlayerPrefs.SetFloat("PetPosY", pos.y);
            PlayerPrefs.Save();
        }
    }
}