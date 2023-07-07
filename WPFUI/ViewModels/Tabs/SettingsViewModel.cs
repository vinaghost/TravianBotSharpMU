using MainCore;
using MainCore.Enums;
using MainCore.Services.Interface;
using MainCore.Tasks.UpdateTasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.Models;
using WPFUI.Store;
using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.Uc;

namespace WPFUI.ViewModels.Tabs
{
    public class SettingsViewModel : AccountTabBaseViewModel
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ITaskManager _taskManager;

        private readonly WaitingOverlayViewModel _waitingOverlay;

        public SettingsViewModel(SelectedItemStore selectedItemStore, IDbContextFactory<AppDbContext> contextFactory, ITaskManager taskManager, WaitingOverlayViewModel waitingWindow) : base(selectedItemStore)
        {
            _contextFactory = contextFactory;
            _taskManager = taskManager;
            _waitingOverlay = waitingWindow;

            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
            ExportCommand = ReactiveCommand.Create(ExportTask);
            ImportCommand = ReactiveCommand.Create(ImportTask);

            Tribes = new(Enum.GetValues<TribeEnums>().Skip(1).Where(x => x != TribeEnums.Nature && x != TribeEnums.Natars).Select(x => new TribeComboBox() { Tribe = x }).ToList());
        }

        private TribeComboBox _selectedTribe;

        public TribeComboBox SelectedTribe
        {
            get => _selectedTribe;
            set => this.RaiseAndSetIfChanged(ref _selectedTribe, value);
        }

        public ObservableCollection<TribeComboBox> Tribes { get; }
        public ToleranceViewModel ClickDelay { get; } = new();
        public ToleranceViewModel TaskDelay { get; } = new();
        public ToleranceViewModel WorkTime { get; } = new();
        public ToleranceViewModel SleepTime { get; } = new();

        private bool _isSleepBetweenProxyChanging;

        public bool IsSleepBetweenProxyChanging
        {
            get => _isSleepBetweenProxyChanging;
            set => this.RaiseAndSetIfChanged(ref _isSleepBetweenProxyChanging, value);
        }

        private bool _isDontLoadImage;

        public bool IsDontLoadImage
        {
            get => _isDontLoadImage;
            set => this.RaiseAndSetIfChanged(ref _isDontLoadImage, value);
        }

        private bool _isMinimized;

        public bool IsMinimized
        {
            get => _isMinimized;
            set => this.RaiseAndSetIfChanged(ref _isMinimized, value);
        }

        private bool _isAutoStartAdventure;

        public bool IsAutoStartAdventure
        {
            get => _isAutoStartAdventure;
            set => this.RaiseAndSetIfChanged(ref _isAutoStartAdventure, value);
        }

        private bool _isAutoSetPoint;

        public bool IsAutoSetPoint
        {
            get => _isAutoSetPoint;
            set => this.RaiseAndSetIfChanged(ref _isAutoSetPoint, value);
        }

        private int _fightingPoint;

        public int FightingPoint
        {
            get => _fightingPoint;
            set => this.RaiseAndSetIfChanged(ref _fightingPoint, value);
        }

        private int _offBonusPoint;

        public int OffBonusPoint
        {
            get => _offBonusPoint;
            set => this.RaiseAndSetIfChanged(ref _offBonusPoint, value);
        }

        private int _defBonusPoint;

        public int DefBonusPoint
        {
            get => _defBonusPoint;
            set => this.RaiseAndSetIfChanged(ref _defBonusPoint, value);
        }

        private int _resourcePoint;

        public int ResourcePoint
        {
            get => _resourcePoint;
            set => this.RaiseAndSetIfChanged(ref _resourcePoint, value);
        }

        protected override void Init(int accountId)
        {
            LoadData(accountId);
        }

        private void LoadData(int index)
        {
            using var context = _contextFactory.CreateDbContext();
            var settings = context.AccountsSettings.Find(index);
            var info = context.AccountsInfo.Find(index);

            RxApp.MainThreadScheduler.Schedule(() =>
            {
                SelectedTribe = Tribes.FirstOrDefault(x => x.Tribe == info.Tribe);

                IsSleepBetweenProxyChanging = settings.IsSleepBetweenProxyChanging;
                IsDontLoadImage = settings.IsDontLoadImage;
                IsMinimized = settings.IsMinimized;
                IsAutoStartAdventure = settings.IsAutoAdventure;
                IsAutoSetPoint = settings.IsAutoHeroPoint;
                FightingPoint = settings.HeroFightingPoint;
                OffBonusPoint = settings.HeroOffPoint;
                DefBonusPoint = settings.HeroDefPoint;
                ResourcePoint = settings.HeroResourcePoint;
            });

            ClickDelay.LoadData(settings.ClickDelayMin, settings.ClickDelayMax);
            TaskDelay.LoadData(settings.TaskDelayMin, settings.TaskDelayMax);
            WorkTime.LoadData(settings.WorkTimeMin, settings.WorkTimeMax);
            SleepTime.LoadData(settings.SleepTimeMin, settings.SleepTimeMax);
        }

        private async Task SaveTask()
        {
            if (!IsSettingValid()) return;
            _waitingOverlay.ShowCommand.Execute("saving account's settings").Subscribe();

            await Task.Run(() =>
            {
                var accountId = AccountId;
                Save(accountId);
                TaskBasedSetting(accountId);
            });
            _waitingOverlay.CloseCommand.Execute().Subscribe();

            MessageBox.Show("Saved.");
        }

        private void ImportTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var account = context.Accounts.Find(AccountId);
            var ofd = new OpenFileDialog
            {
                InitialDirectory = AppContext.BaseDirectory,
                Filter = "TBS files (*.tbs)|*.tbs|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = $"{account.Username}_settings.tbs",
            };

            if (ofd.ShowDialog() == true)
            {
                var jsonString = File.ReadAllText(ofd.FileName);
                try
                {
                    var setting = JsonSerializer.Deserialize<MainCore.Models.Database.AccountSetting>(jsonString);
                    var accountId = AccountId;
                    setting.AccountId = accountId;
                    context.Update(setting);
                    context.SaveChanges();
                    LoadData(accountId);
                    TaskBasedSetting(accountId);
                }
                catch
                {
                    MessageBox.Show("Invalid file.", "Warning");
                    return;
                }
            }
        }

        private void ExportTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var account = context.Accounts.Find(AccountId);
            if (account is null) return;
            var svd = new SaveFileDialog
            {
                InitialDirectory = AppContext.BaseDirectory,
                Filter = "TBS files (*.tbs)|*.tbs|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = $"{account.Username}_settings.tbs",
            };

            var accountSetting = context.AccountsSettings.Find(AccountId);
            var jsonString = JsonSerializer.Serialize(accountSetting);
            if (svd.ShowDialog() == true)
            {
                File.WriteAllText(svd.FileName, jsonString);
            }
        }

        private void Save(int index)
        {
            using var context = _contextFactory.CreateDbContext();
            var accountSetting = context.AccountsSettings.Find(index);

            accountSetting.IsSleepBetweenProxyChanging = IsSleepBetweenProxyChanging;
            accountSetting.IsDontLoadImage = IsDontLoadImage;
            accountSetting.IsMinimized = IsMinimized;
            accountSetting.IsAutoAdventure = IsAutoStartAdventure;
            accountSetting.IsAutoHeroPoint = IsAutoSetPoint;
            accountSetting.HeroFightingPoint = FightingPoint;
            accountSetting.HeroOffPoint = OffBonusPoint;
            accountSetting.HeroDefPoint = DefBonusPoint;

            (accountSetting.ClickDelayMin, accountSetting.ClickDelayMax) = ClickDelay.GetData();
            (accountSetting.TaskDelayMin, accountSetting.TaskDelayMax) = TaskDelay.GetData();
            (accountSetting.WorkTimeMin, accountSetting.WorkTimeMax) = WorkTime.GetData();
            (accountSetting.SleepTimeMin, accountSetting.SleepTimeMax) = SleepTime.GetData();

            context.Update(accountSetting);
            var accountInfo = context.AccountsInfo.Find(index);
            accountInfo.Tribe = SelectedTribe?.Tribe ?? TribeEnums.Any;
            context.Update(accountInfo);
            context.SaveChanges();
        }

        private bool IsSettingValid()
        {
            var sumHeroPoint = FightingPoint + OffBonusPoint + DefBonusPoint + ResourcePoint;
            if (sumHeroPoint != 4)
            {
                MessageBox.Show("Sum of hero point settings must be 4.", "Warning");
                return false;
            }
            return true;
        }

        private void TaskBasedSetting(int index)
        {
            var tasks = _taskManager.GetList(index);
            var taskUpdateAdventures = tasks.OfType<UpdateAdventures>().FirstOrDefault();
            if (IsAutoStartAdventure)
            {
                if (taskUpdateAdventures is null)
                {
                    _taskManager.Add<UpdateAdventures>(index);
                }
                else
                {
                    taskUpdateAdventures.ExecuteAt = DateTime.Now;
                    _taskManager.ReOrder(index);
                }
            }
            else
            {
                if (taskUpdateAdventures is not null) _taskManager.Remove(index, taskUpdateAdventures);
            }
        }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportCommand { get; }
    }
}