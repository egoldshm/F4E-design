using F4E_design;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;

namespace F4E_GUI
{
    class HostsFileAdapter
    {
        private static readonly string HOSTS_FILE_PATH = @"C:\Windows\System32\drivers\etc\hosts";

        public static void Write(FilteringSettings filteringSettings)
        {
            IEnumerable<string> urlsBlacklist = new[] { "" };
            urlsBlacklist = urlsBlacklist.Concat(UrlsBlacklistsByCategories(filteringSettings));
            urlsBlacklist = urlsBlacklist.Concat(filteringSettings.GetCustomBlackList());
            urlsBlacklist = urlsBlacklist.Except(filteringSettings.GetCustomExceptionsList());
            string HostsText = GetSafeSearchHostsText(filteringSettings) + Environment.NewLine + UrlsListToHostsText(urlsBlacklist);
            File.WriteAllText(HOSTS_FILE_PATH, HostsText);
        }

        private static string GetSafeSearchHostsText(FilteringSettings filteringSettings)
        {
            string safeSearchHostsText = "";
            if((int)filteringSettings._youtubeFilteringLevel>0)
            {
                safeSearchHostsText += GetYoutubeSafeSearchHosts(filteringSettings._youtubeFilteringLevel);
            }
            return safeSearchHostsText;
        }

        private static string GetYoutubeSafeSearchHosts(FilteringSettings.YoutubeFilteringLevels youtubeFilteringLevel)
        {
            List<string> youtubeUrls = new List<string>();
            youtubeUrls.Add("youtube.com");
            youtubeUrls.Add("m.youtube.com");
            youtubeUrls.Add("youtubei.googleapis.com");
            youtubeUrls.Add("youtube.googleapis.com");
            youtubeUrls.Add("youtube-nocookie.com");

            string youtubeSafeSearchAddress = "";
            switch (youtubeFilteringLevel)
            {
                case FilteringSettings.YoutubeFilteringLevels.Moderate:
                    youtubeSafeSearchAddress = "216.239.38.119";
                    break;
                case FilteringSettings.YoutubeFilteringLevels.Strict:
                    youtubeSafeSearchAddress = "216.239.38.120";
                    break;
            }
            string safeSearchHostsText = Environment.NewLine + "---Youtube Safe Search -----------------------";
            foreach (string url in youtubeUrls)
            {
                safeSearchHostsText += Environment.NewLine + youtubeSafeSearchAddress + " " + url + " www." + url + " https://" + url + " https://www." + url;
            }
            return safeSearchHostsText;
        }

        private static string UrlsListToHostsText(IEnumerable<string> urlsBlacklist)
        {
            string hostsTxt = "--Custom Blacklist (Manual and Categories)-------------";
            foreach (string url in urlsBlacklist)
            {
                hostsTxt += Environment.NewLine + UrlToHostFormat(url);
            }
            return hostsTxt;
        }

        private static List<string> UrlsBlacklistsByCategories(FilteringSettings filteringSettings)
        {
            List<string> categoriesBlacklist = new List<string>();
            if (filteringSettings.isSocialNetworksBlocked)
            {
                categoriesBlacklist.Add("facebook.com");
            }
            if (filteringSettings.isGamblingBlocked)
            {
                categoriesBlacklist.Add("888.com");
            }
            return categoriesBlacklist;
        }

        private static string UrlToHostFormat(string url)
        {
            return "0.0.0.0 " + url + " www." + url + " https://" + url + " https://www." + url;
        }
   
    }
}
