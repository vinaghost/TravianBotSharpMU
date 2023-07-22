using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    public class SettingsTabBase : ReactiveUserControl<SettingsViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsTab : SettingsTabBase
    {
        public SettingsTab()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.ExportCommand, v => v.ExportButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.ImportCommand, v => v.ImportButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveButton).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Tribes, v => v.Tribe.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.SelectedTribe, v => v.Tribe.SelectedItem).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.ClickDelay, v => v.ClickDelay.ViewModel).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.TaskDelay, v => v.TaskDelay.ViewModel).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.WorkTime, v => v.WorkTime.ViewModel).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.SleepTime, v => v.SleepTime.ViewModel).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.IsSleepBetweenProxyChanging, v => v.SleepBetweenChangingProxy.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.IsDontLoadImage, v => v.DisableImageCheckBox.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.IsMinimized, v => v.MinimizedCheckBox.IsChecked).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.IsAutoStartAdventure, v => v.AutoStartAdventureCheckBox.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.IsAutoEquipBeforeAdventure, v => v.AutoEquipBeforeAdventureCheckBox.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.IsAutoSetPoint, v => v.AutoSetPointCheckBox.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.FightingPoint, v => v.Fighting.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.OffBonusPoint, v => v.OffBonus.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.DefBonusPoint, v => v.DefBonus.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.ResourcePoint, v => v.Resource.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.IsAutoHeroRevive, v => v.AutoReviveHero.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.IsUseHeroResourceRevive, v => v.AutoReviveHeroWithHeroResource.IsChecked).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ReviveVillages, v => v.ReviveVillage.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.SelectedReviveVillage, v => v.ReviveVillage.SelectedItem).DisposeWith(d);
            });
        }
    }
}