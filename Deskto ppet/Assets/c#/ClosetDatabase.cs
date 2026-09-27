using UnityEngine;

[CreateAssetMenu(fileName = "ClosetDatabase", menuName = "桌宠/衣柜数据库")]
public class ClosetDatabase : ScriptableObject
{
    public ClosetItem[] items;
}