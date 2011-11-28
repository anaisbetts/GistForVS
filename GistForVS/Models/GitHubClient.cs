using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Subjects;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Hammock;
using Hammock.Authentication.Basic;
using Hammock.Serialization;
using Hammock.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ReactiveUI;

namespace GistForVS.Models
{

    namespace GitHub.Api
    {
        public class GitHubClient : IEnableLogger
        {
            static readonly string userAgent;
            static string password;
            static string username;
            RestClient restClient;

            static GitHubClient()
            {
                userAgent = string.Format("GistForVS/{0}", Assembly.GetExecutingAssembly().GetName().Version);
            }

            public string Password
            {
                get { return password; }
                set { password = value; }
            }

            public int RateLimit { get; private set; }
            public int RateLimitRemaining { get; private set; }

            const string authority = "https://api.github.com";

            RestClient RestClient
            {
                get
                {
                    if (restClient == null)
                    {
                        this.Log().InfoFormat("Creating REST Client with user '{0}'", Username);
                        restClient = new RestClient
                        {
                            Authority = authority,
                            Credentials = new BasicAuthCredentials
                            {
                                Username = Username,
                                Password = Password
                            },
                            UserAgent = userAgent,
                            Deserializer = new HammockJsonDotNetSerializer(),
                            Serializer = new HammockJsonDotNetSerializer()
                        };
                    }
                    return restClient;
                }
            }

            public string Username
            {
                get { return username; }
                set { username = value; }
            }

            //
            // Sync versions of Async Functions
            //

#if FALSE
            public RestResponse<GitHubRepository> CreateRepository(GitHubRepository repo, string orgLogin = null)
            {
                return CreateRepositoryAsync(repo, orgLogin).First();
            }

            public IObservable<RestResponse<GitHubRepository>> CreateRepositoryAsync(GitHubRepository repo, string orgLogin = null)
            {
                var request = new RestRequest
                {
                    Path = orgLogin == null ? "user/repos" : string.Format("orgs/{0}/repos", orgLogin),
                    Entity = new
                    {
                        name = repo.Name,
                        @public = !repo.Private,
                        description = repo.Description,
                        has_issues = repo.HasIssues,
                        has_downloads = repo.HasDownloads,
                        has_wiki = repo.HasWiki,
                    }
                };

                return RestClient.RequestAsync<GitHubRepository>(request);
            }
#endif

            public IObservable<GistModel> CreateGist(string content, bool isPublic)
            {
                var rq = new RestRequest() {
                    Path = "gists",
                    Method = WebMethod.Post,
                    Encoding = Encoding.UTF8,
                    Entity = new {
                        @public = isPublic ? "true" : "false",
                        files = new[] {
                            new { name = "test.cs", content }
                        }.ToDictionary(k => k.name, v => new { content }),
                    }
                };

                return RestClient.RequestAsync<GistModel>(rq)
                    .ThrowOnRestResponseFailure()
                    .Select(x => x.ContentEntity)
                    .Catch(Observable.Return(GetErrorGistModel()));
            }

            public GistModel GetErrorGistModel()
            {
                return new GistModel() {
                    html_url = "[An error has occured]",
                };
            }
        }
    }

    public static class ObservableEx
    {
        public static T[] WaitUntilFinished<T>(this IObservable<T> This)
        {
            return This.ToArray().First();
        }

        public static IObservable<T> ThrowOnRestResponseFailure<T>(this IObservable<T> This)
            where T : RestResponseBase
        {
            return This.SelectMany(x =>
            {
                if (x.InnerException != null)
                {
                    return Observable.Throw<T>(x.InnerException);
                }

                if ((int)x.StatusCode >= 400)
                {
                    return Observable.Throw<T>(new WebException(x.Content));
                }

                return Observable.Return(x);
            });
        }

        public static IObservable<RestResponse> RequestAsync(this RestClient This, RestRequest request)
        {
            var ret = new AsyncSubject<RestResponse>();

            This.BeginRequest(request, (rq, resp, state) =>
            {
                try
                {
                    ret.OnNext(resp);
                    ret.OnCompleted();
                }
                catch (Exception ex)
                {
                    ret.OnError(ex);
                }
            });

            return ret;
        }

        public static IObservable<RestResponse<T>> RequestAsync<T>(this RestClient This, RestRequest request)
        {
            var ret = new AsyncSubject<RestResponse<T>>();

            This.BeginRequest<T>(request, (rq, resp, state) =>
            {
                try
                {
                    ret.OnNext(resp);
                    ret.OnCompleted();
                }
                catch (Exception ex)
                {
                    ret.OnError(ex);
                }
            });

            return ret;
        }
    }

    public class HammockJsonDotNetSerializer : ISerializer, IDeserializer
    {
        readonly JsonSerializerSettings settings;

        public HammockJsonDotNetSerializer()
            : this(new JsonSerializerSettings { ContractResolver = new CustomContractResolver() })
        {
        }

        public HammockJsonDotNetSerializer(JsonSerializerSettings settings)
        {
            this.settings = settings;
        }

        #region IDeserializer Members

        public object Deserialize(RestResponse response, Type type)
        {
            return JsonConvert.DeserializeObject(response.Content, type, settings);
        }

        public T Deserialize<T>(RestResponse<T> response)
        {
            return JsonConvert.DeserializeObject<T>(response.Content, settings);
        }

        public dynamic DeserializeDynamic(RestResponse<object> response)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISerializer Members

        public virtual string Serialize(object instance, Type type)
        {
            return JsonConvert.SerializeObject(instance, Formatting.None/*, settings*/);
        }

        public virtual string ContentType
        {
            get { return "application/json"; }
        }

        public virtual Encoding ContentEncoding
        {
            get { return Encoding.UTF8; }
        }

        #endregion
    }

    public class CustomContractResolver : CamelCasePropertyNamesContractResolver
    {
        #region Protected Methods

        protected override string ResolvePropertyName(string propertyName)
        {
            return ToUnderscores(propertyName);
        }

        #endregion

        #region Private Methods

        static string ToUnderscores(string str)
        {
            return Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + "_" + char.ToLower(m.Value[1]));
        }

        #endregion
    }
}
