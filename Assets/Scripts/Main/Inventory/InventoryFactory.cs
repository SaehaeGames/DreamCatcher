using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InventoryFactory
{
    public static InventoryEntry FromFeather(FeatherData data)
    {
        return new InventoryEntry
        {
            id = data.bird_id,
            type = InventoryItemType.Feather,
            name = "",
            description = "",
            count = data.feather_number
        };
    }

    public static InventoryEntry FromDreamCatcher(DreamCatcher data)
    {
        return new InventoryEntry
        {
            id = data.DCid,
            type = InventoryItemType.DreamCatcher,
            name = "드림 캐쳐",
            description = "",
            count = 1
        };
    }
}
