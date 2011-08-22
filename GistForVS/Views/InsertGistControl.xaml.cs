using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GistForVS.ViewModels;
using ReactiveUI;

namespace GistForVS.Views
{
	/// <summary>
	/// Interaction logic for InsertGistControl.xaml
	/// </summary>
	public partial class InsertGistControl : UserControl
	{
        public InsertGistViewModel ViewModel { get; protected set; }

		public InsertGistControl()
		{
            ViewModel = new InsertGistViewModel();

			this.InitializeComponent();

		    var enter = Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(x => MouseEnter += x, x => MouseEnter -= x);
		    var exit = Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(x => MouseLeave += x, x => MouseLeave -= x);

		    var inRegionState = Observable.Merge(
		        enter.Select(_ => "Base"),
		        exit.Select(_ => "ButtonMode")).StartWith("ButtonMode");

            var viewState = Observable.CombineLatest(
                ViewModel.WhenAny(x => x.SelectionText, x => !String.IsNullOrWhiteSpace(x.Value)),
                inRegionState,
                (hasSelection, selectionState) => hasSelection ? selectionState : "NoSelection");

		    viewState.Subscribe(x => VisualStateManager.GoToElementState(LayoutRoot, x, true));
		}
	}
}