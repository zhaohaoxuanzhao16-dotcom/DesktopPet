using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClosetUI : MonoBehaviour
{
    [Header("数据")]
    public ClosetDatabase database;

    [Header("UI")]
    public Transform gridParent;
    public GameObject slotPrefab;

    [Header("要控制的模型")]
    public Transform petModel;

    [Header("换装子物体（按分类分组）")]
    public OutfitEntry[] hats;       // 帽子类
    public OutfitEntry[] glasses;    // 眼镜类

    // 当前装备的 id
    private string currentHatId = "";
    private string currentGlassesId = "";

    [System.Serializable]
    public class OutfitEntry
    {
        public string id;
        public GameObject model;
    }

    void Start()
    {
        Debug.Log("=== ClosetUI Start ===");

        if (database == null || slotPrefab == null || gridParent == null)
        {
            Debug.LogWarning("ClosetUI: 引用没拖全");
            return;
        }

        // 读取上次换装
        currentHatId = PlayerPrefs.GetString("CurrentHat", "");
        currentGlassesId = PlayerPrefs.GetString("CurrentGlasses", "");

        ApplyAll();
        BuildUI();
    }

    void BuildUI()
    {
        foreach (Transform child in gridParent)
            Destroy(child.gameObject);

        if (database.items == null || database.items.Length == 0) return;

        foreach (var item in database.items)
        {
            if (item == null || string.IsNullOrEmpty(item.id)) continue;

            GameObject slot = Instantiate(slotPrefab, gridParent);

            var iconImage = slot.transform.Find("Icon")?.GetComponent<Image>();
            if (iconImage != null) iconImage.sprite = item.icon;

            var nameText = slot.transform.Find("Name")?.GetComponent<TextMeshProUGUI>();
            if (nameText != null) nameText.text = item.displayName;

            var btn = slot.GetComponent<Button>();
            var captured = item;
            if (btn != null)
                btn.onClick.AddListener(() => OnItemClicked(captured));
        }
    }

    void OnItemClicked(ClosetItem item)
    {
        if (item == null || string.IsNullOrEmpty(item.id)) return;

        Debug.Log($"选择: {item.displayName} ({item.category})");

        // 按分类更新
        switch (item.category)
        {
            case ClosetCategory.Hat:
                // 再点同一个 = 摘掉
                if (currentHatId == item.id) currentHatId = "";
                else currentHatId = item.id;
                PlayerPrefs.SetString("CurrentHat", currentHatId);
                break;

            case ClosetCategory.Glasses:
                if (currentGlassesId == item.id) currentGlassesId = "";
                else currentGlassesId = item.id;
                PlayerPrefs.SetString("CurrentGlasses", currentGlassesId);
                break;
        }

        PlayerPrefs.Save();
        ApplyAll();
    }

    void ApplyAll()
    {
        ApplyCategory(hats, currentHatId);
        ApplyCategory(glasses, currentGlassesId);
    }

    void ApplyCategory(OutfitEntry[] list, string id)
    {
        if (list == null) return;

        // 全部隐藏
        foreach (var o in list)
            if (o.model != null) o.model.SetActive(false);

        // 只显示匹配的
        if (string.IsNullOrEmpty(id)) return;

        foreach (var o in list)
        {
            if (o.id == id && o.model != null)
            {
                o.model.SetActive(true);
                Debug.Log($"显示 {o.model.name}");
                break;
            }
        }
    }
}