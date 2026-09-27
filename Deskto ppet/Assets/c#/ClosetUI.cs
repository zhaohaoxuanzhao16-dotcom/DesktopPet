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

    [Header("换装子物体（预先挂好的装饰）")]
    public OutfitEntry[] outfits;

    private string currentOutfitId = "";

    [System.Serializable]
    public class OutfitEntry
    {
        public string id;
        public GameObject model;
    }

    void Start()
    {
        Debug.Log("=== ClosetUI Start ===");
        Debug.Log($"database 为空: {database == null}");
        Debug.Log($"gridParent 为空: {gridParent == null}");
        Debug.Log($"slotPrefab 为空: {slotPrefab == null}");
        Debug.Log($"petModel 为空: {petModel == null}");

        if (database == null || slotPrefab == null || gridParent == null)
        {
            Debug.LogWarning("ClosetUI: 引用没拖全，UI 不会生成");
            return;
        }

        currentOutfitId = PlayerPrefs.GetString("CurrentOutfit", "");
        Debug.Log($"读取上次换装: {currentOutfitId}");

        ApplyOutfitById(currentOutfitId);
        BuildUI();
    }

    void BuildUI()
    {
        foreach (var item in database.items)
{
    // 跳过空物品
    if (item == null || string.IsNullOrEmpty(item.id))
        continue;

    GameObject slot = Instantiate(slotPrefab, gridParent);
    // ... 原有代码
}
        Debug.Log("BuildUI 开始");
        foreach (Transform child in gridParent)
            Destroy(child.gameObject);

        if (database.items == null || database.items.Length == 0)
        {
            Debug.LogWarning("ClosetUI: 数据库里没有物品");
            return;
        }
        

        foreach (var item in database.items)
        {
            GameObject slot = Instantiate(slotPrefab, gridParent);
            Debug.Log($"生成格子: {item.displayName}");

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
    // ... 原有代码

        Debug.Log($"选择物品: {item.displayName}");
        currentOutfitId = item.id;
        PlayerPrefs.SetString("CurrentOutfit", currentOutfitId);
        PlayerPrefs.Save();

        ApplyOutfitById(currentOutfitId);
    }

    void ApplyOutfitById(string id)
    {
        if (outfits == null) return;

        foreach (var o in outfits)
            if (o.model != null) o.model.SetActive(false);

        foreach (var o in outfits)
            if (o.id == id && o.model != null) o.model.SetActive(true);
    }
}