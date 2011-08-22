using System.Windows;
using System.Windows.Controls;

namespace GistForVS.Views
{
	/// <summary>
	/// Interaction logic for InsertGistControl.xaml
	/// </summary>
	public partial class InsertGistControl : UserControl
	{
		public InsertGistControl()
		{
			this.InitializeComponent();
		    VisualStateManager.GoToElementState(this.LayoutRoot, "ButtonMode", false);
		}
	}
}