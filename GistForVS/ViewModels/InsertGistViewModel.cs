using System;
using System.Reactive.Linq;
using System.Windows;
using GistForVS.Models.GitHub.Api;
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
            //var client = new GitHubClient() { Username = "octocat", Password = "FillMeInHere" };

            CreateGist = new ReactiveAsyncCommand();

            var result = CreateGist.RegisterAsyncObservable(_ =>
                client.CreateGist(SelectionText, !IsPrivateGist));

            result.Subscribe(x => MessageBox.Show(x.html_url));
        }
    }
}
