using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GistForVS.Views;
using Microsoft.VisualStudio.Text.Editor;
using ReactiveUI;

namespace GistForVS
{
    /// <summary>
    /// Adornment class that draws a square box in the top right hand corner of the viewport
    /// </summary>
    class GistForVS
    {
        private InsertGistControl _control;
        private IWpfTextView _view;
        private IAdornmentLayer _adornmentLayer;

        static GistForVS()
        {
            Application.ResourceAssembly = typeof (GistForVS).Assembly;
            RxApp.TaskpoolScheduler = Scheduler.ThreadPool;
        }

        /// <summary>
        /// Creates a square image and attaches an event handler to the layout changed event that
        /// adds the the square in the upper right-hand corner of the TextView via the adornment layer
        /// </summary>
        /// <param name="view">The <see cref="IWpfTextView"/> upon which the adornment will be drawn</param>
        public GistForVS(IWpfTextView view)
        {
            _view = view;

            _control = new InsertGistControl(); //new Image();
            RxApp.DeferredScheduler = new DispatcherScheduler(_control.Dispatcher);
            view.Selection.SelectionChanged += (o, e) =>
            {
                var sel = (ITextSelection)o;
                _control.ViewModel.SelectionText = String.Join("", 
                    sel.SelectedSpans.Select(x => x.GetText()));
            };
            //_image.Source = drawingImage;

            //Grab a reference to the adornment layer that this adornment should be added to
            _adornmentLayer = view.GetAdornmentLayer("GistForVS");

            _view.ViewportHeightChanged += delegate { this.onSizeChange(); };
            _view.ViewportWidthChanged += delegate { this.onSizeChange(); };
        }

        public void onSizeChange()
        {
            //clear the adornment layer of previous adornments
            _adornmentLayer.RemoveAllAdornments();

            //Place the image in the top right hand corner of the Viewport
            Canvas.SetLeft(_control, _view.ViewportRight - 148 /*XXX: HACK*/ - 16);
            Canvas.SetTop(_control, _view.ViewportTop + 16);

            //add the image to the adornment layer and make it relative to the viewport
            _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, _control, null);
        }
    }
}