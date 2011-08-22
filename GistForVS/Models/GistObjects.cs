using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GistForVS.Models
{
    public class User
    {
        public string login { get; set; }
        public int id { get; set; }
        public string avatar_url { get; set; }
        public string url { get; set; }
    }
     
    public class GistFile
    {
        public int size { get; set; }
        public string filename { get; set; }
        public string raw_url { get; set; }
        public string content { get; set; }
    }
     
    public class Fork
    {
        public User user { get; set; }
        public string url { get; set; }
        public string created_at { get; set; }
    }
     
    public class ChangeStatus
    {
        public int deletions { get; set; }
        public int additions { get; set; }
        public int total { get; set; }
    }
     
    public class History
    {
        public string url { get; set; }
        public string version { get; set; }
        public User user { get; set; }
        public ChangeStatus change_status { get; set; }
        public string committed_at { get; set; }
    }
     
    public class GistModel
    {
        public string url { get; set; }
        public string id { get; set; }
        public string description { get; set; }
        public bool @public { get; set; }
        public User user { get; set; }
        public Dictionary<string, GistFile> files { get; set; }
        public int comments { get; set; }
        public string html_url { get; set; }
        public string git_pull_url { get; set; }
        public string git_push_url { get; set; }
        public string created_at { get; set; }
        public Fork[] forks { get; set; }
        public History[] history { get; set; }
    }
}
