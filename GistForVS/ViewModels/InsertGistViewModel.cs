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

        string _LastGistUrl;
        public string LastGistUrl {
            get { return _LastGistUrl;  }
            set { this.RaiseAndSetIfChanged(x => x.LastGistUrl, value); }
        }

        public ReactiveAsyncCommand CreateGist { get; protected set; }
        public ReactiveCommand CopyToClipboard { get; protected set; }

        public InsertGistViewModel()
        {
            //var client = new GitHubClient() { Username = "octocat", Password = "FillMeInHere" };

            var privateImage = new BitmapImage(new Uri(@"pack://application:,,,/data/private.png"));
            var publicImage = new BitmapImage(new Uri(@"pack://application:,,,/data/public.png"));

            _PublicPrivateIcon = this.WhenAny(x => x.IsPrivateGist, x => x.Value)
                .Select(x => x ? privateImage : publicImage)
                .ToProperty(this, x => x.PublicPrivateIcon);

            CreateGist = new ReactiveAsyncCommand();

            CreateGist.RegisterAsyncObservable(_ => client.CreateGist(SelectionText, !IsPrivateGist))
                .Select(x => x.html_url)
                .BindTo(this, x => x.LastGistUrl);

            CopyToClipboard = new ReactiveCommand(
                this.WhenAny(x => x.LastGistUrl, x => !String.IsNullOrWhiteSpace(x.Value)));

            CopyToClipboard.Subscribe(_ => Clipboard.SetText(LastGistUrl));

            this.WhenAny(x => x.SelectionText, x => x.Value)
                .Where(_ => LastGistUrl != null)
                .Subscribe(_ => LastGistUrl = null);
        }
    }
}
