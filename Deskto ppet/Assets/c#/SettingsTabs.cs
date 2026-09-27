using UnityEngine;
using UnityEngine.UI;

public class SettingsTabs : MonoBehaviour
{
    [Header("Tab 按钮")]
    public Button tabCloset;
    public Button tabEaster;
    public Button tabSettings;

    [Header("对应页面")]
    public GameObject pageCloset;
    public GameObject pageEaster;
    public GameObject pageSettings;

    void Start()
    {
        if (tabCloset != null)   tabCloset.onClick.AddListener(() => ShowPage(0));
        if (tabEaster != null)   tabEaster.onClick.AddListener(() => ShowPage(1));
        if (tabSettings != null) tabSettings.onClick.AddListener(() => ShowPage(2));

        // 默认显示"衣柜"页
        ShowPage(0);
    }

    void ShowPage(int index)
    {
        // 隐藏所有页面，只打开一个
        if (pageCloset != null)   pageCloset.SetActive(index == 0);
        if (pageEaster != null)   pageEaster.SetActive(index == 1);
        if (pageSettings != null) pageSettings.SetActive(index == 2);

        Debug.Log($"切换到第 {index} 页");
    }
}