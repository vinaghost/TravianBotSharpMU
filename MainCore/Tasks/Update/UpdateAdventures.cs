﻿using MainCore.Helper;
using MainCore.Tasks.Sim;
using System;
using System.Linq;

namespace MainCore.Tasks.Update
{
    public class UpdateAdventures : AccountBotTask
    {
        public UpdateAdventures(int accountId) : base(accountId, "Update hero's adventures")
        {
        }

        public override void Execute()
        {
            IsFail = true;
            using var context = _contextFactory.CreateDbContext();
            NavigateHelper.AfterClicking(_chromeBrowser, context, AccountId);
            NavigateHelper.ToAdventure(_chromeBrowser, context, AccountId);
            if (Cts.IsCancellationRequested) return;
            UpdateHelper.UpdateAdventures(context, _chromeBrowser, AccountId);

            var taskUpdate = new UpdateInfo(AccountId);
            taskUpdate.CopyFrom(this);
            taskUpdate.Execute();

            var setting = context.AccountsSettings.Find(AccountId);
            if (setting.IsAutoAdventure)
            {
                var hero = context.Heroes.Find(AccountId);
                if (hero.Status != Enums.HeroStatusEnums.Home)
                {
                    _logManager.Warning(AccountId, "Hero is not at home. Cannot start adventures");
                    return;
                }
                var adventures = context.Adventures.Where(a => a.AccountId == AccountId);
                if (adventures.Any())
                {
                    var taskAutoSend = new StartAdventure(AccountId);
                    taskAutoSend.CopyFrom(this);
                    taskAutoSend.Execute();
                    taskUpdate.Execute();
                    NavigateHelper.Sleep(800, 1500);
                    NextExecute();
                }
            }
            IsFail = false;
        }

        private void NextExecute()
        {
            var html = _chromeBrowser.GetHtml();

#if TRAVIAN_OFFICIAL_HEROUI
            var tileDetails = html.GetElementbyId("heroAdventure");

#elif TTWARS || TRAVIAN_OFFICIAL
            var tileDetails = html.GetElementbyId("tileDetails");
#else

#error You forgot to define Travian version here

#endif
            if (tileDetails is null)
            {
                ExecuteAt = DateTime.Now.AddMinutes(Random.Shared.Next(5, 10));
                return;
            }
            var timer = tileDetails.Descendants("span").FirstOrDefault(x => x.HasClass("timer"));
            if (timer is null)
            {
                ExecuteAt = DateTime.Now.AddMinutes(Random.Shared.Next(5, 10));
                return;
            }

            int sec = int.Parse(timer.GetAttributeValue("value", "0"));
            if (sec < 0) sec = 0;
#if TRAVIAN_OFFICIAL_HEROUI || TRAVIAN_OFFICIAL
            ExecuteAt = DateTime.Now.AddSeconds(sec * 2 + Random.Shared.Next(20, 40));

#elif TTWARS
            ExecuteAt = DateTime.Now.AddSeconds(sec * 2 + 1);
#else

#error You forgot to define Travian version here

#endif
        }
    }
}