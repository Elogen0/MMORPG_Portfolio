using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kame.Core;
using Kame.Define;
using Kame.Game.Data;
using TMPro;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.UI;

namespace Kame.Quests
{
    public class UI_QuestTooltip : UI_Base
    {
        enum Texts
        {
            tmp_title,
            tmp_description,
            tmp_rewards,
            tmp_objective_desc,
            tmp_objective_prog
        }

        private void OnDisable()
        {
            SetText((int) Texts.tmp_title, string.Empty);
            SetText((int) Texts.tmp_description, string.Empty);
            
            SetText((int) Texts.tmp_objective_desc, string.Empty);
            SetText((int) Texts.tmp_objective_prog, string.Empty);
            
            SetText((int)Texts.tmp_rewards,string.Empty);
        }

        public override void Init()
        {
            Bind<TextMeshProUGUI>(typeof(Texts));
        }
        
        public void Setup(QuestRecord questRecord)
        {
            StartCoroutine(CoSetup(questRecord));
        }

        IEnumerator CoSetup(QuestRecord questRecord)
        {
            SetText((int) Texts.tmp_title, questRecord.Data.title);
            SetText((int) Texts.tmp_description, questRecord.Data.description);
            
            var objective = questRecord.Objective;
            SetText((int) Texts.tmp_objective_desc, objective.description);
            SetText((int) Texts.tmp_objective_prog, $"{questRecord.Progress}/{questRecord.CompletionCount}");
            
            SetText((int)Texts.tmp_rewards,GetRewardText(questRecord));
            yield return null;
        }

        private string GetRewardText(QuestRecord questRecord)
        {
            string rewardText = "";

            foreach (var reward in questRecord.Rewards)
            {
                if (rewardText != string.Empty)
                {
                    rewardText += ",";
                }

                rewardText += DataManager.ItemDict[reward.itemId].name;

                if (reward.amount > 1)
                {
                    rewardText += $"({reward.amount}) ";
                }
            }

            if (rewardText == "")
            {
                rewardText = "No reward.";
            }
            
            return rewardText;
        }

        
    }
    
}
