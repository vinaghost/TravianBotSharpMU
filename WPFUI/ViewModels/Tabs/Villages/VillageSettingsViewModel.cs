using MainCore;
using MainCore.Enums;
using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using MainCore.Tasks.FunctionTasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using ReactiveUI;
using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.Models;
using WPFUI.Store;
using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.Uc;

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class VillageSettingsViewModel : VillageTabBaseViewModel
    {
        private readonly IUpgradeBuildingHelper _upgradeBuildingHelper;

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ITaskManager _taskManager;

        private readonly WaitingOverlayViewModel _waitingOverlay;

        public VillageSettingsViewModel(SelectedItemStore selectedItemStore, IUpgradeBuildingHelper upgradeBuildingHelper, IDbContextFactory<AppDbContext> contextFactory, ITaskManager taskManager, WaitingOverlayViewModel waitingWindow) : base(selectedItemStore)
        {
            _upgradeBuildingHelper = upgradeBuildingHelper;
            _contextFactory = contextFactory;
            _taskManager = taskManager;
            _waitingOverlay = waitingWindow;

            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
            ExportCommand = ReactiveCommand.Create(ExportTask);
            ImportCommand = ReactiveCommand.Create(ImportTask);
        }

        #region build settings

        private bool _useHeroRes;

        public bool UseHeroRes
        {
            get => _useHeroRes;
            set => this.RaiseAndSetIfChanged(ref _useHeroRes, value);
        }

        private bool _ignoreRoman;

        public bool IgnoreRoman
        {
            get => _ignoreRoman;
            set => this.RaiseAndSetIfChanged(ref _ignoreRoman, value);
        }

        public CheckBoxWithInputViewModel AutoComplete { get; } = new();
        public CheckBoxWithInputViewModel WatchAds { get; } = new();

        #endregion build settings

        #region refresh setting

        private bool _isAutoRefresh;

        public bool IsAutoRefresh
        {
            get => _isAutoRefresh;
            set => this.RaiseAndSetIfChanged(ref _isAutoRefresh, value);
        }

        public ToleranceViewModel AutoRefresh { get; } = new();

        #endregion refresh setting

        #region NPC settings

        public CheckBoxWithInputViewModel AutoNPCCrop { get; } = new();
        public CheckBoxWithInputViewModel AutoNPCResource { get; } = new();
        private bool _isAutoNPCOverflow;

        public bool IsAutoNPCOverflow
        {
            get => _isAutoNPCOverflow;
            set => this.RaiseAndSetIfChanged(ref _isAutoNPCOverflow, value);
        }

        public ResourcesViewModel RatioNPC { get; } = new();

        #endregion NPC settings

        #region Train troop settings

        public TroopSelectorViewModel BarrackTraining { get; } = new();
        public TroopSelectorViewModel StableTraining { get; } = new();
        public TroopSelectorViewModel WorkshopTraining { get; } = new();

        #endregion Train troop settings

        #region Train troop based on timer

        private bool _isEnableTrainTroopBasedOnTimer;

        public bool IsEnableTrainTroopBasedOnTimer
        {
            get => _isEnableTrainTroopBasedOnTimer;
            set => this.RaiseAndSetIfChanged(ref _isEnableTrainTroopBasedOnTimer, value);
        }

        public ToleranceViewModel TimeTrain { get; } = new();

        private bool _isMaxTrain;

        public bool IsMaxTrain
        {
            get => _isMaxTrain;
            set => this.RaiseAndSetIfChanged(ref _isMaxTrain, value);
        }

        private bool _isBarrackTrain;

        public bool IsBarrackTrain
        {
            get => _isBarrackTrain;
            set => this.RaiseAndSetIfChanged(ref _isBarrackTrain, value);
        }

        public ToleranceViewModel BarrackFillTime { get; } = new();

        private bool _isStableTrain;

        public bool IsStableTrain
        {
            get => _isStableTrain;
            set => this.RaiseAndSetIfChanged(ref _isStableTrain, value);
        }

        public ToleranceViewModel StableFillTime { get; } = new();

        private bool _isWorkshopTrain;

        public bool IsWorkshopTrain
        {
            get => _isWorkshopTrain;
            set => this.RaiseAndSetIfChanged(ref _isWorkshopTrain, value);
        }

        public ToleranceViewModel WorkshopFillTime { get; } = new();

        private bool _isGreatBarrackTrain;

        public bool IsGreatBarrackTrain
        {
            get => _isGreatBarrackTrain;
            set => this.RaiseAndSetIfChanged(ref _isGreatBarrackTrain, value);
        }

        public ToleranceViewModel GreatBarrackFillTime { get; } = new();

        private bool _isGreatStableTrain;

        public bool IsGreatStableTrain
        {
            get => _isGreatStableTrain;
            set => this.RaiseAndSetIfChanged(ref _isGreatStableTrain, value);
        }

        public ToleranceViewModel GreatStableFillTime { get; } = new();

        #endregion Train troop based on timer

        #region Train troop based on resource

        public CheckBoxWithInputViewModel TrainTroopBasedOnResource { get; } = new();

        private int _percentResForBarrack;

        public int PercentResForBarrack
        {
            get => _percentResForBarrack;
            set => this.RaiseAndSetIfChanged(ref _percentResForBarrack, value);
        }

        private int _percentResForStable;

        public int PercentResForStable
        {
            get => _percentResForStable;
            set => this.RaiseAndSetIfChanged(ref _percentResForStable, value);
        }

        private int _percentResForWorkshop;

        public int PercentResForWorkshop
        {
            get => _percentResForWorkshop;
            set => this.RaiseAndSetIfChanged(ref _percentResForWorkshop, value);
        }

        private int _percentResForGreatBarrack;

        public int PercentResForGreatBarrack
        {
            get => _percentResForGreatBarrack;
            set => this.RaiseAndSetIfChanged(ref _percentResForGreatBarrack, value);
        }

        private int _percentResForGreatStable;

        public int PercentResForGreatStable
        {
            get => _percentResForGreatStable;
            set => this.RaiseAndSetIfChanged(ref _percentResForGreatStable, value);
        }

        #endregion Train troop based on resource

        #region Quest settings

        private bool _isAutoClaimQuest;

        public bool IsAutoClaimQuest
        {
            get => _isAutoClaimQuest;
            set => this.RaiseAndSetIfChanged(ref _isAutoClaimQuest, value);
        }

        #endregion Quest settings

        protected override void Init(int villageId)
        {
            LoadData(villageId);
        }

        private void LoadData(int villageId)
        {
            Observable.Start(() =>
            {
                using var context = _contextFactory.CreateDbContext();
                var settings = context.VillagesSettings.Find(villageId);

                var account = context.Villages.Find(villageId);
                var accountInfo = context.AccountsInfo.Find(account.AccountId);
                var tribe = accountInfo.Tribe;

                return (settings, tribe);
            }, RxApp.TaskpoolScheduler)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe((data) =>
                {
                    var (settings, tribe) = data;
                    UseHeroRes = settings.IsUseHeroRes;
                    IgnoreRoman = settings.IsIgnoreRomanAdvantage;
                    IsAutoRefresh = settings.IsAutoRefresh;
                    IsAutoNPCOverflow = settings.IsNPCOverflow;

                    AutoComplete.LoadData(settings.IsInstantComplete, settings.InstantCompleteTime);
                    WatchAds.LoadData(settings.IsAdsUpgrade, settings.AdsUpgradeTime);

                    AutoRefresh.LoadData(settings.AutoRefreshTimeMin, settings.AutoRefreshTimeMax);

                    AutoNPCCrop.LoadData(settings.IsAutoNPC, settings.AutoNPCPercent);
                    AutoNPCResource.LoadData(settings.IsAutoNPCWarehouse, settings.AutoNPCWarehousePercent);

                    RatioNPC.LoadData(settings.AutoNPCWood, settings.AutoNPCClay, settings.AutoNPCIron, settings.AutoNPCCrop);

                    BarrackTraining.LoadData(tribe.GetInfantryTroops().Select(x => new TroopInfo(x)), (TroopEnums)settings.BarrackTroop);
                    StableTraining.LoadData(tribe.GetCavalryTroops().Select(x => new TroopInfo(x)), (TroopEnums)settings.StableTroop);
                    WorkshopTraining.LoadData(tribe.GetSiegeTroops().Select(x => new TroopInfo(x)), (TroopEnums)settings.WorkshopTroop);

                    TimeTrain.LoadData(settings.TroopTimeMin, settings.TroopTimeMax);
                    IsMaxTrain = settings.IsMaxTrain;

                    IsEnableTrainTroopBasedOnTimer = settings.IsTrainTroopBasedOnTimer;

                    IsBarrackTrain = settings.IsBarrack;
                    IsStableTrain = settings.IsStable;
                    IsWorkshopTrain = settings.IsWorkshop;
                    IsGreatBarrackTrain = settings.IsGreatBarrack;
                    IsGreatStableTrain = settings.IsGreatStable;

                    BarrackFillTime.LoadData(settings.BarrackTroopTimeMin, settings.BarrackTroopTimeMax);
                    StableFillTime.LoadData(settings.StableTroopTimeMin, settings.StableTroopTimeMax);
                    WorkshopFillTime.LoadData(settings.WorkshopTroopTimeMin, settings.WorkshopTroopTimeMax);
                    GreatBarrackFillTime.LoadData(settings.GreatBarrackTroopTimeMin, settings.GreatBarrackTroopTimeMax);
                    GreatStableFillTime.LoadData(settings.GreatStableTroopTimeMin, settings.GreatStableTroopTimeMax);

                    TrainTroopBasedOnResource.LoadData(settings.IsTrainTroopBasedOnRes, settings.PercentWarehouseTrainTroop);
                    PercentResForBarrack = settings.PercentResForBarrack;
                    PercentResForStable = settings.PercentResForStable;
                    PercentResForWorkshop = settings.PercentResForWorkshop;
                    PercentResForGreatBarrack = settings.PercentResForGreatBarrack;
                    PercentResForGreatStable = settings.PercentResForGreatStable;

                    IsAutoClaimQuest = settings.IsAutoCollectReward;
                });
        }

        private async Task SaveTask()
        {
            _waitingOverlay.Show("saving account's settings");
            await Task.Run(() =>
            {
                Save(VillageId);
                TaskBasedSetting(VillageId, AccountId);
            });
            _waitingOverlay.Close();

            MessageBox.Show("Saved.");
        }

        private void ImportTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var village = context.Villages.Find(VillageId);
            var ofd = new OpenFileDialog
            {
                InitialDirectory = AppContext.BaseDirectory,
                Filter = "TBS files (*.tbs)|*.tbs|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = $"{village.Name}_settings.tbs",
            };

            if (ofd.ShowDialog() == true)
            {
                var jsonString = File.ReadAllText(ofd.FileName);
                try
                {
                    var setting = JsonSerializer.Deserialize<MainCore.Models.Database.VillageSetting>(jsonString);
                    var villageId = VillageId;
                    setting.VillageId = villageId;
                    context.Update(setting);
                    context.SaveChanges();
                    LoadData(villageId);
                    var accountId = AccountId;
                    TaskBasedSetting(villageId, accountId);
                }
                catch
                {
                    MessageBox.Show("Invalid file.", "Warning");
                }
            }
        }

        private void ExportTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var villageId = VillageId;
            var svd = new SaveFileDialog
            {
                InitialDirectory = AppContext.BaseDirectory,
                Filter = "TBS files (*.tbs)|*.tbs|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = $"{villageId}_settings.tbs",
            };

            if (svd.ShowDialog() == true)
            {
                var setting = context.VillagesSettings.Find(villageId);
                var jsonString = JsonSerializer.Serialize(setting);
                File.WriteAllText(svd.FileName, jsonString);
            }
        }

        private void Save(int index)
        {
            using var context = _contextFactory.CreateDbContext();
            var settings = context.VillagesSettings.Find(index);

            settings.IsUseHeroRes = UseHeroRes;
            settings.IsIgnoreRomanAdvantage = IgnoreRoman;

            (settings.IsInstantComplete, settings.InstantCompleteTime) = AutoComplete.GetData();
            (settings.IsAdsUpgrade, settings.AdsUpgradeTime) = WatchAds.GetData();

            settings.IsAutoRefresh = IsAutoRefresh;
            (settings.AutoRefreshTimeMin, settings.AutoRefreshTimeMax) = AutoRefresh.GetData();

            (settings.IsAutoNPC, settings.AutoNPCPercent) = AutoNPCCrop.GetData();
            (settings.IsAutoNPCWarehouse, settings.AutoNPCWarehousePercent) = AutoNPCResource.GetData();

            settings.IsNPCOverflow = IsAutoNPCOverflow;

            (settings.AutoNPCWood, settings.AutoNPCClay, settings.AutoNPCIron, settings.AutoNPCCrop) = RatioNPC.GetData();

            TroopEnums troop;
            troop = BarrackTraining.GetData();
            settings.BarrackTroop = (int)troop;
            troop = StableTraining.GetData();
            settings.StableTroop = (int)troop;
            troop = WorkshopTraining.GetData();
            settings.WorkshopTroop = (int)troop;

            (settings.TroopTimeMin, settings.TroopTimeMax) = TimeTrain.GetData();
            settings.IsMaxTrain = IsMaxTrain;

            settings.IsTrainTroopBasedOnRes = IsEnableTrainTroopBasedOnTimer;
            settings.IsBarrack = IsBarrackTrain;
            settings.IsStable = IsStableTrain;
            settings.IsWorkshop = IsWorkshopTrain;
            settings.IsGreatBarrack = IsGreatBarrackTrain;
            settings.IsGreatStable = IsGreatStableTrain;

            (settings.BarrackTroopTimeMax, settings.BarrackTroopTimeMin) = BarrackFillTime.GetData();
            (settings.StableTroopTimeMax, settings.StableTroopTimeMin) = StableFillTime.GetData();
            (settings.WorkshopTroopTimeMax, settings.WorkshopTroopTimeMin) = WorkshopFillTime.GetData();
            (settings.GreatBarrackTroopTimeMax, settings.GreatBarrackTroopTimeMin) = GreatBarrackFillTime.GetData();
            (settings.GreatStableTroopTimeMax, settings.GreatStableTroopTimeMin) = GreatStableFillTime.GetData();

            (settings.IsTrainTroopBasedOnRes, settings.PercentWarehouseTrainTroop) = TrainTroopBasedOnResource.GetData();
            settings.PercentResForBarrack = PercentResForBarrack;
            settings.PercentResForStable = PercentResForStable;
            settings.PercentResForWorkshop = PercentResForWorkshop;
            settings.PercentResForGreatBarrack = PercentResForGreatBarrack;
            settings.PercentResForGreatStable = PercentResForGreatStable;

            settings.IsAutoCollectReward = IsAutoClaimQuest;

            context.Update(settings);
            context.SaveChanges();
        }

        private void TaskBasedSetting(int villageId, int accountId)
        {
            var list = _taskManager.GetList(accountId);

            using var context = _contextFactory.CreateDbContext();
            var settings = context.VillagesSettings.Find(villageId);
            {
                var tasks = list.OfType<InstantUpgrade>().ToList();
                if (settings.IsInstantComplete)
                {
                    if (!tasks.Any())
                    {
                        _upgradeBuildingHelper.RemoveFinishedCB(VillageId);
                        var currentBuildings = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId).ToList();
                        var count = currentBuildings.Count(x => x.Level != -1);
                        if (count > 0)
                        {
                            _taskManager.Add<InstantUpgrade>(accountId, villageId);
                        }
                    }
                }
                else
                {
                    foreach (var item in tasks)
                    {
                        _taskManager.Remove(accountId, item);
                    }
                }
            }
            {
                var tasks = list.OfType<RefreshVillage>().ToList();
                if (settings.IsAutoRefresh)
                {
                    if (!tasks.Any(x => x.VillageId == villageId))
                    {
                        _taskManager.Add<RefreshVillage>(accountId, villageId);
                    }
                }
                else
                {
                    var updateTasks = tasks.Where(x => x.VillageId == villageId);
                    foreach (var item in updateTasks)
                    {
                        _taskManager.Remove(accountId, item);
                    }
                }
            }
            {
                var tasks = list.OfType<TrainTroopsTask>().ToList();
                if (settings.BarrackTroop != 0 || settings.StableTroop != 0 || settings.WorkshopTroop != 0)
                {
                    if (!tasks.Any(x => x.VillageId == villageId))
                    {
                        _taskManager.Add<TrainTroopsTask>(accountId, villageId);
                    }
                }
                else
                {
                    var updateTasks = tasks.Where(x => x.VillageId == villageId);
                    foreach (var item in updateTasks)
                    {
                        _taskManager.Remove(accountId, item);
                    }
                }
            }
        }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportCommand { get; }
    }
}