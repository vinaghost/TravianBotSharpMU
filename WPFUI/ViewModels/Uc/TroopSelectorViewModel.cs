using DynamicData;
using MainCore.Enums;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Uc
{
    public class TroopSelectorViewModel : ViewModelBase
    {
        public void LoadData(IEnumerable<TroopInfo> troops, TroopEnums selectedTroop)
        {
            Troops.Clear();
            Troops.Add(new(TroopEnums.None));
            Troops.AddRange(troops);
            SelectedTroop = Troops.FirstOrDefault(x => x.Troop == selectedTroop) ?? Troops.First();
        }

        public TroopEnums GetData()
        {
            return SelectedTroop?.Troop ?? TroopEnums.None;
        }

        public ObservableCollection<TroopInfo> Troops { get; } = new();
        private TroopInfo _selectedItem;

        public TroopInfo SelectedTroop
        {
            get => _selectedItem;
            set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
        }
    }
}