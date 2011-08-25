using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using GistForVS.Tests.TestHelpers;
using GistForVS.Views;
using Xunit;

namespace GistForVS.Tests
{
    public class Scratch
    {
        [Fact]
        public void TestFail()
        {
            Assert.True(false);
        }

        [Fact]
        public void CreateTestWindow()
        {
            Application.ResourceAssembly = typeof (InsertGistControl).Assembly;

            WpfHelper.RunBlockAsSTA(() => {
                var grid = new Grid();
                var lhs = new ColumnDefinition() {Width = new GridLength(0.5, GridUnitType.Star)};
                var rhs = new ColumnDefinition() {Width = new GridLength(0.5, GridUnitType.Star)};
                grid.ColumnDefinitions.Add(lhs);    grid.ColumnDefinitions.Add(rhs);

                var textBox = new TextBox();
                var control = new InsertGistControl() {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };

                Grid.SetColumn(textBox, 0);
                Grid.SetColumn(control, 1);
                textBox.TextChanged += (o, e) => control.ViewModel.SelectionText = textBox.Text;
                grid.Children.Add(textBox); grid.Children.Add(control);

                var wnd = new Window() {
                    Content = grid,
                };

                wnd.ShowDialog();
            });
        }
    }
}
