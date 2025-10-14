using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DreamCatcherFactory
{
    // QuestDreamCatcherInfo_Object -> DreamCatcher 변환
    public static DreamCatcher ConvertToDreamCatcher(QuestDreamCatcherInfo_Object questDreamCatcherObject)
    {
        // null 체크
        if(questDreamCatcherObject == null)
        {
            Debug.LogError("QuestDreamCatcherInfo_Object is null 변환 불가");
            return null;
        }

        // 혹시 Parse가 안된 상태라면 Parse 실행
        if(questDreamCatcherObject.lines == null || questDreamCatcherObject.beads==null)
        {
            questDreamCatcherObject.Parse();
        }

        return new DreamCatcher(
            questDreamCatcherObject.id, 
            questDreamCatcherObject.lines, 
            questDreamCatcherObject.beads, 
            questDreamCatcherObject.color, 
            questDreamCatcherObject.feather1, 
            questDreamCatcherObject.feather2, 
            questDreamCatcherObject.feather3);
    }
}
