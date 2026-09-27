using UnityEngine;

public class ShortcutManager : MonoBehaviour
{
    [Header("快捷键")]
    [Tooltip("按住这个修饰键")]
    public KeyCode modifierKey = KeyCode.LeftShift;

    [Tooltip("同时按下的主键")]
    public KeyCode triggerKey = KeyCode.A;

    [Header("要控制的物体")]
    public GameObject settingsButton;   // 要显示/隐藏的设置按钮
    public GameObject settingsPanel;    // 可选：直接打开面板

    [Header("行为")]
    [Tooltip("是否切换显示/隐藏")]
    public bool toggle = true;

    [Tooltip("直接打开面板（跳过按钮）")]
    public bool openPanelDirectly = false;

    void Update()
    {
        if (Input.GetKey(modifierKey) && Input.GetKeyDown(triggerKey))
        {
            Debug.Log("快捷键 Shift + A 触发");

            if (openPanelDirectly && settingsPanel != null)
            {
                bool newState = !settingsPanel.activeSelf;
                settingsPanel.SetActive(newState);
                Debug.Log($"面板状态: {newState}");
            }
            else if (settingsButton != null)
            {
                if (toggle)
                {
                    bool newState = !settingsButton.activeSelf;
                    settingsButton.SetActive(newState);
                    Debug.Log($"按钮状态: {newState}");
                }
                else
                {
                    settingsButton.SetActive(true);
                }
            }
        }
    }
}