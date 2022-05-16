using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Kame.Quests
{
    public class UI_QuestItem : UI_Base
    {
        private TextMeshProUGUI title;
        private TextMeshProUGUI progress;
        private QuestRecord _questRecord;

        enum tmp
        {
            tmp_title,
            tmp_progress
        }
        
        public override void Init()
        {
            Bind<TextMeshProUGUI>(typeof(tmp));
            title = GetText((int) tmp.tmp_title);
            progress = GetText((int) tmp.tmp_progress);
        }
        
        public void Setup(QuestRecord questRecord)
        {
            this._questRecord = questRecord;
            title.text = questRecord.Data.title;
            progress.text = $"{questRecord.Progress} / {questRecord.CompletionCount}";
        }

        public QuestRecord GetQuest()
        {
            return _questRecord;
        }
    }

}
