﻿namespace MainCore.Models.Database
{
    public class AccountSetting
    {
        public int AccountId { get; set; }
        public int ClickDelayMin { get; set; }
        public int ClickDelayMax { get; set; }
        public int TaskDelayMin { get; set; }
        public int TaskDelayMax { get; set; }
        public int WorkTimeMin { get; set; }
        public int WorkTimeMax { get; set; }
        public int SleepTimeMin { get; set; }
        public int SleepTimeMax { get; set; }
        public bool IsSleepBetweenProxyChanging { get; set; }
        public bool IsDontLoadImage { get; set; }
        public bool IsMinimized { get; set; }
        public bool IsClosedIfNoTask { get; set; }
        public bool IsAutoAdventure { get; set; }
        public bool IsAutoEquipBeforeAdventure { get; set; }
        public int FarmIntervalMin { get; set; }
        public int FarmIntervalMax { get; set; }
        public bool IsAutoHeroPoint { get; set; }
        public int HeroFightingPoint { get; set; }
        public int HeroOffPoint { get; set; }
        public int HeroDefPoint { get; set; }
        public int HeroResourcePoint { get; set; }

        public bool IsAutoHeroRevive { get; set; }
        public int HeroReviveVillageId { get; set; }
        public bool IsUseHeroResToRevive { get; set; }
    }
}