using F4E_design;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.TextFormatting;
using System.Xml;

namespace F4E_GUI
{
    class HostsFileAdapter
    {
        enum BlockingCategories
        {
            Social_Media, Gambling, News, Video_Players, Sports, Dating, Violence, Photos_Stack, Games, Life_Style
        }

        public static void Write(FilteringSettings filteringSettings)
        {
                try
                {
                    if (InternetBlocker.IsInternetReachable())
                    {
                        ServiceAdapter.CustomCommend((int)ServiceAdapter.CustomCommends.stopCatchFiles);

                        string path = Path.Combine(App.GetAppDataFolder(), "CustomBlackList");

                        StreamWriter stream = new StreamWriter(path);

                        IEnumerable<string> urlsBlacklist = new[] { "" };
                        urlsBlacklist = urlsBlacklist.Concat(UrlsBlacklistsByCategories(filteringSettings));
                        urlsBlacklist = urlsBlacklist.Concat(filteringSettings.GetCustomBlackList());
                        urlsBlacklist = urlsBlacklist.Except(filteringSettings.GetCustomExceptionsList());
                        string HostsText = GetSafeSearchHostsText(filteringSettings) + Environment.NewLine + UrlsListToHostsText(urlsBlacklist);

                        stream.Write(HostsText);
                        stream.Close();

                        ServiceAdapter.CustomCommend((int)ServiceAdapter.CustomCommends.updateHostsFile);
                        ServiceAdapter.CustomCommend((int)ServiceAdapter.CustomCommends.startCatchFiles);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "HostsFileAdapter.Write Error");
                }
        }

        private static string GetSafeSearchHostsText(FilteringSettings filteringSettings)
        {
            string safeSearchHostsText = "";
            if ((int)filteringSettings._youtubeFilteringLevel > 0)
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
            IEnumerable<string> toRetuen = new[] { "" };
            List<string> categoriesBlacklist = new List<string>();

            if (filteringSettings.isSocialNetworksBlocked)
                toRetuen = toRetuen.Concat(GetUrlListByCategory(BlockingCategories.Social_Media));

            if (filteringSettings.isGamblingBlocked)
                toRetuen = toRetuen.Concat(GetUrlListByCategory(BlockingCategories.Gambling));

            if (filteringSettings.isNewsBlocked)
                toRetuen = toRetuen.Concat(GetUrlListByCategory(BlockingCategories.News));

            if (filteringSettings.isVideoPlayersBlocked)
                toRetuen = toRetuen.Concat(GetUrlListByCategory(BlockingCategories.Video_Players));

            if (filteringSettings.isSportBlocked)
                toRetuen = toRetuen.Concat(GetUrlListByCategory(BlockingCategories.Sports));

            if (filteringSettings.isViolenceBlocked)
                toRetuen = toRetuen.Concat(GetUrlListByCategory(BlockingCategories.Violence));

            if (filteringSettings.isLifeStyleBlocked)
                toRetuen = toRetuen.Concat(GetUrlListByCategory(BlockingCategories.Life_Style));

            if (filteringSettings.isPhotosStackBlocked)
                toRetuen = toRetuen.Concat(GetUrlListByCategory(BlockingCategories.Photos_Stack));

            if (filteringSettings.isGamesBlocked)
                toRetuen = toRetuen.Concat(GetUrlListByCategory(BlockingCategories.Games));

            if (filteringSettings.isDatingBlocked)
                toRetuen = toRetuen.Concat(GetUrlListByCategory(BlockingCategories.Dating));


            return toRetuen.Concat(categoriesBlacklist).ToList();
        }

        private static string UrlToHostFormat(string url)
        {
            return "0.0.0.0 " + url + " www." + url + " https://" + url + " https://www." + url;
        }
   
        private static List<string> GetUrlListByCategory(BlockingCategories category)
        {
            List<string> urlsList = new List<string>();
            string path= "http://f4e.mmb.org.il/sites/"+category.ToString().ToLower()+".html";
            string[] socialMediaUrlList = Regex.Split(Tools.GetTextFromUri(path), "\r\n|\r|\n");
            foreach (string line in socialMediaUrlList)
            {
                urlsList.Add(line);
            }
            return urlsList;
        }
    }
}
