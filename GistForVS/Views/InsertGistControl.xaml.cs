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
		    VisualStateManager.GoToElementState(this.LayoutRoot, "ButtonMode", false);
		}
	}
}