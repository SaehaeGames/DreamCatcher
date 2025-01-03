using System;

[Serializable]
public class InventoryData
{
    public int Id { get; set; }             // 아이템 id(== 도감 id)
    public string Content { get; set; }     // 아이템 설명(드림캐쳐 : 제작 재료 / 그 외 : 도감 설명 참조)
    public int Count { get; set; }          // 아이템 개수

    public InventoryData()
    {
        Id = Count = 0;
        Content = string.Empty;
    }

    public InventoryData(int id, string content, int count)
    {
        Id = id;
        Content = content;
        Count = count;
    }
}
