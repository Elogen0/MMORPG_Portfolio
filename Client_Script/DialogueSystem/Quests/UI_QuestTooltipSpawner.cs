using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kame.Quests
{
    public class UI_QuestTooltipSpawner : UI_TooltipSpawner
    {
        public override void UpdateTooltip(GameObject tooltip)
        {
            QuestRecord questRecord = GetComponent<UI_QuestItem>().GetQuest();
            tooltip.GetComponent<UI_QuestTooltip>().Setup(questRecord);
        }

        public override bool CanCreateTooltip()
        {
            return true;
        }
    }
    
}
