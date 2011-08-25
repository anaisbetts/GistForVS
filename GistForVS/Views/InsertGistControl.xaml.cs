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

		    LayoutRoot.DataContext = ViewModel;

		    var enter = Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(x => MouseEnter += x, x => MouseEnter -= x);
		    var exit = Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(x => MouseLeave += x, x => MouseLeave -= x);

		    string currentOpenState = null;
		    Observable.Merge(
                    ViewModel.WhenAny(x => x.SelectionText, _ => "Base"),
                    ViewModel.WhenAny(x => x.LastGistUrl, _ => "GistPosted").Where(x => x != null))
		        .Subscribe(x => currentOpenState = x);

		    var inRegionState = Observable.Merge(
                    enter.Select(_ => currentOpenState),
                    exit.Select(_ => "ButtonMode")).StartWith("ButtonMode")
		        .Throttle(TimeSpan.FromMilliseconds(800), RxApp.DeferredScheduler);

            var viewState = Observable.CombineLatest(
                ViewModel.WhenAny(x => x.SelectionText, x => !String.IsNullOrWhiteSpace(x.Value)),
                inRegionState,
                (hasSelection, selectionState) => hasSelection ? selectionState : "NoSelection");

		    viewState.Subscribe(x => VisualStateManager.GoToElementState(LayoutRoot, x, true));

		    ViewModel.WhenAny(x => x.LastGistUrl, x => x.Value)
		        .Where(x => !String.IsNullOrWhiteSpace(x))
		        .Subscribe(_ => VisualStateManager.GoToElementState(LayoutRoot, "GistPosted", true));
		}
	}
}