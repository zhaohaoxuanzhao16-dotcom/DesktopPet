using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsPanelController : MonoBehaviour
{
    [Header("UI 引用")]
    public GameObject settingsPanel;
    public Button openButton;
    public Button closeButton;
    public Toggle toggleRememberPos;
    public Toggle toggleEasterEgg;

    [Header("要控制的脚本")]
    public PetScreenAdapter petAdapter;
    public TypeSequenceLoop easterEgg;

    void Start()
    {
        Debug.Log("=== SettingsPanelController Start ===");
        Debug.Log($"settingsPanel 是否为空: {settingsPanel == null}");
        Debug.Log($"openButton 是否为空: {openButton == null}");
        Debug.Log($"closeButton 是否为空: {closeButton == null}");
        Debug.Log($"toggleRememberPos 是否为空: {toggleRememberPos == null}");
        Debug.Log($"toggleEasterEgg 是否为空: {toggleEasterEgg == null}");
        Debug.Log($"petAdapter 是否为空: {petAdapter == null}");
        Debug.Log($"easterEgg 是否为空: {easterEgg == null}");

        // 读取设置
        if (toggleRememberPos != null)
            toggleRememberPos.isOn = PlayerPrefs.GetInt("RememberPos", 1) == 1;
        if (toggleEasterEgg != null)
            toggleEasterEgg.isOn = PlayerPrefs.GetInt("EasterEgg", 1) == 1;

        ApplySettings();

        // 绑定按钮，带日志
        if (openButton != null)
        {
            openButton.onClick.AddListener(() =>
            {
                Debug.Log("【打开按钮】被点击了");
                if (settingsPanel != null)
                {
                    settingsPanel.SetActive(true);
                    Debug.Log("【打开按钮】Panel 已显示");
                }
                else
                {
                    Debug.LogError("【打开按钮】settingsPanel 是 null，无法显示");
                }
            });
            Debug.Log("打开按钮监听已绑定");
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(() =>
            {
                Debug.Log("【关闭按钮】被点击了");
                if (settingsPanel != null)
                {
                    settingsPanel.SetActive(false);
                    Debug.Log("【关闭按钮】Panel 已隐藏");
                }
            });
            Debug.Log("关闭按钮监听已绑定");
        }

        if (toggleRememberPos != null)
            toggleRememberPos.onValueChanged.AddListener(_ => ApplySettings());
        if (toggleEasterEgg != null)
            toggleEasterEgg.onValueChanged.AddListener(_ => ApplySettings());

        Debug.Log("=== SettingsPanelController 初始化完成 ===");
    }

    void ApplySettings()
    {
        Debug.Log("ApplySettings 被调用");

        if (petAdapter != null && toggleRememberPos != null)
            petAdapter.rememberPosition = toggleRememberPos.isOn;

        if (easterEgg != null && toggleEasterEgg != null)
            easterEgg.enabled = toggleEasterEgg.isOn;

        if (toggleRememberPos != null)
            PlayerPrefs.SetInt("RememberPos", toggleRememberPos.isOn ? 1 : 0);
        if (toggleEasterEgg != null)
            PlayerPrefs.SetInt("EasterEgg", toggleEasterEgg.isOn ? 1 : 0);

        PlayerPrefs.Save();
    }
}