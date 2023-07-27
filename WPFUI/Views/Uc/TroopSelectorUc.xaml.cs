using ReactiveUI;
using System.Reactive.Disposables;
using System.Windows;
using WPFUI.ViewModels.Uc;

namespace WPFUI.Views.Uc
{
    public class TroopSelectorBase : ReactiveUserControl<TroopSelectorViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for TroopSelector.xaml
    /// </summary>
    public partial class TroopSelectorUc : TroopSelectorBase
    {
        public TroopSelectorUc()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Troops, v => v.TroopComboBox.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.SelectedTroop, v => v.TroopComboBox.SelectedItem).DisposeWith(d);
            });
        }

        public static readonly DependencyProperty TextProperty =
           DependencyProperty.Register("Text", typeof(string), typeof(TroopSelectorUc), new PropertyMetadata(default(string)));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
    }
}