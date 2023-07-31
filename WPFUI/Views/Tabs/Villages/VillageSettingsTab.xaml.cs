using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs.Villages;

namespace WPFUI.Views.Tabs.Villages
{
    public class VillageSettingsTabBase : ReactiveUserControl<VillageSettingsViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class VillageSettingsTab : VillageSettingsTabBase
    {
        public VillageSettingsTab()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.ExportCommand, v => v.ExportButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.ImportCommand, v => v.ImportButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveButton).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.UseHeroRes, v => v.UseHeroResCheckBox.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.IgnoreRoman, v => v.IgnoreRomanAdvantageCheckBox.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.AutoComplete, v => v.Complete.ViewModel).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.WatchAds, v => v.WatchAds.ViewModel).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.IsAutoRefresh, v => v.RefreshCheckBox.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.AutoRefresh, v => v.Refresh.ViewModel).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.AutoNPCCrop, v => v.AutoNPC.ViewModel).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.AutoNPCResource, v => v.AutoNPCWarehouse.ViewModel).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.IsAutoNPCOverflow, v => v.NPCCheckBox.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.RatioNPC, v => v.AutoNPCRatio.ViewModel).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.BarrackTraining, v => v.BarrackTroop.ViewModel).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.StableTraining, v => v.StableTroop.ViewModel).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.WorkshopTraining, v => v.WorkshopTroop.ViewModel).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.IsEnableTrainTroopBasedOnTimer, v => v.TrainTroopBasedOnTimer.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.TimeTrain, v => v.TimeTrain.ViewModel).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.IsMaxTrain, v => v.IsMaxTrain.IsChecked).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.IsBarrackTrain, v => v.BarrackTrain.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.IsStableTrain, v => v.StableTrain.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.IsWorkshopTrain, v => v.WorkshopTrain.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.IsGreatBarrackTrain, v => v.GreatBarrackTrain.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.IsGreatStableTrain, v => v.GreatStableTrain.IsChecked).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.BarrackFillTime, v => v.FillTimeBarrack.ViewModel).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.StableFillTime, v => v.FillTimeStable.ViewModel).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.WorkshopFillTime, v => v.FillTimeWorkshop.ViewModel).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.GreatBarrackFillTime, v => v.FillTimeGreatBarrack.ViewModel).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.GreatStableFillTime, v => v.FillTimeGreatStable.ViewModel).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.TrainTroopBasedOnResource, v => v.TrainTroopBasedOnResource.ViewModel).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.PercentResForBarrack, v => v.BarrackResTrain.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.PercentResForStable, v => v.StableResTrain.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.PercentResForWorkshop, v => v.WorkshopResTrain.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.PercentResForGreatBarrack, v => v.GreatBarrackResTrain.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.PercentResForGreatStable, v => v.GreatStableResTrain.Value).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.IsAutoClaimQuest, v => v.AutoClaimQuestCheckBox.IsChecked).DisposeWith(d);
            });
        }
    }
}