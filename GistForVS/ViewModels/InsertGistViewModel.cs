using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
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

        ObservableAsPropertyHelper<BitmapImage> _PublicPrivateIcon;
        public BitmapImage PublicPrivateIcon {
            get { return _PublicPrivateIcon.Value; }
        }

        public ReactiveAsyncCommand CreateGist { get; protected set; }

        public InsertGistViewModel()
        {
            //var client = new GitHubClient() { Username = "octocat", Password = "FillMeInHere" };

            var privateImage = new BitmapImage(new Uri(@"pack://application:,,,/data/private.png"));
            var publicImage = new BitmapImage(new Uri(@"pack://application:,,,/data/public.png"));

            _PublicPrivateIcon = this.WhenAny(x => x.IsPrivateGist, x => x.Value)
                .Select(x => x ? privateImage : publicImage)
                .ToProperty(this, x => x.PublicPrivateIcon);

            CreateGist = new ReactiveAsyncCommand();

            var result = CreateGist.RegisterAsyncObservable(_ =>
                client.CreateGist(SelectionText, !IsPrivateGist));

            result.Subscribe(x => MessageBox.Show(x.html_url));
        }
    }
}
