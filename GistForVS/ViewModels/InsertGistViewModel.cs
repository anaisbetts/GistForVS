using System;
using System.Reactive.Linq;
using System.Windows;
using ReactiveUI;
using ReactiveUI.Xaml;

namespace GistForVS.ViewModels
{
    public class InsertGistViewModel : ReactiveObject
    {
        string _SelectionText;
        public string SelectionText {
            get { return _SelectionText;  }
            set { this.RaiseAndSetIfChanged(x => x.SelectionText, value); }
        }

        bool _IsPrivateGist;
        public bool IsPrivateGist {
            get { return _IsPrivateGist; }
            set { this.RaiseAndSetIfChanged(x => x.IsPrivateGist, value); }
        }

        public ReactiveAsyncCommand CreateGist { get; protected set; }

        public InsertGistViewModel()
        {
        }
    }
}
