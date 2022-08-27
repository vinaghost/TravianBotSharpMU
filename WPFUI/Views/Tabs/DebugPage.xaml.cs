﻿using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Views.Tabs
{
    /// <summary>
    /// Interaction logic for DebugPage.xaml
    /// </summary>
    public partial class DebugPage : ReactivePage<DebugViewModel>
    {
        public DebugPage()
        {
            ViewModel = new();
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel,
                    vm => vm.Logs,
                    v => v.LogGrid.ItemsSource)
                .DisposeWith(d);

                this.OneWayBind(ViewModel,
                    vm => vm.Tasks,
                    v => v.TaskGird.ItemsSource)
                .DisposeWith(d);

                this.BindCommand(ViewModel,
                    vm => vm.Button,
                    v => v.ReportButton)
                .DisposeWith(d);

                ViewModel.OnActived();
            });
        }
    }
}