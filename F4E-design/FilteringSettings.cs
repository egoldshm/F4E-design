using F4E_GUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Net;

namespace F4E_design
{
    [Serializable]
    internal class FilteringSettings
    {
        public enum YoutubeFilteringLevels
        {
            None=0,
            Moderate = 1,
            Strict=2,
            Blocked=3
        }

        private string _adminName;
        private string _password;
        private string _adminMail;
        private string _computerName;
        public Boolean SystemStatus;
        public Boolean ScheduelStatus;
        private Boolean _isSafeServerOn;
        public Boolean isSafeServerOn
        {
            get
            {
                return _isSafeServerOn;
            }
            set 
            { 
                _isSafeServerOn = value;
                DnsController.SetMode(_isSafeServerOn);
            }
        }
        public Boolean isGamblingBlocked;
        public Boolean isSocialNetworksBlocked;
        public Boolean isNewsBlocked;
        public Boolean isSportBlocked;
        public Boolean isVideoPlayersBlocked;
        public Boolean isPhotosStackBlocked;
        public Boolean isGamesBlocked;
        public Boolean isDatingBlocked;
        public Boolean isViolenceBlocked;
        public Boolean isLifeStyleBlocked;
        public Boolean isAdBlockOn;
        public YoutubeFilteringLevels _youtubeFilteringLevel;

        private List<string> _customBlacklist;
        private List<string> _customExceptionsList;

        public FilteringSettings()
        {
            isSafeServerOn = true;
            isGamblingBlocked = false;
            isSocialNetworksBlocked = false;
            isNewsBlocked = false;
            isSportBlocked = false;
            isVideoPlayersBlocked = false;
            isAdBlockOn = false;
            isPhotosStackBlocked = false;
            isGamesBlocked = false;
            isDatingBlocked = false;
            isViolenceBlocked = false;
            isLifeStyleBlocked = false;
            _youtubeFilteringLevel = YoutubeFilteringLevels.Moderate;
            _customBlacklist = new List<string>();
            _customExceptionsList = new List<string>();
        }

        public void ToggleBollean(string flag)
        {
            switch(flag)
            {
                case "SafeServer":
                    isSafeServerOn = !isSafeServerOn;
                    break;
                case "Gambling":
                    isGamblingBlocked = !isGamblingBlocked;
                    break;
                case "SocialNetworks":
                    isSocialNetworksBlocked = !isSocialNetworksBlocked;
                    break;
                case "News":
                    isNewsBlocked = !isNewsBlocked;
                    break;
                case "Sport":
                    isSportBlocked = !isSportBlocked;
                    break;
                case "VideoPlayes":
                    isVideoPlayersBlocked = !isVideoPlayersBlocked;
                    break;
                case "PhotosStack":
                    isPhotosStackBlocked = !isPhotosStackBlocked;
                    break;
                case "Violence":
                    isViolenceBlocked = !isViolenceBlocked;
                    break;
                case "Games":
                    isGamesBlocked = !isGamesBlocked;
                    break;
                case "Dating":
                    isDatingBlocked = !isDatingBlocked;
                    break;
                case "LifeStyle":
                    isLifeStyleBlocked = !isLifeStyleBlocked;
                    break;
            }
        }

        internal void SetAdminName(string name)
        {
            _adminName = name;
        }

        internal void SetComputerName(string pcName)
        {
            _computerName = pcName;
        }

        internal string GetComputerName()
        {
            return _computerName;
        }

        internal string GetAdminName()
        {
            return _adminName;
        }

        internal string GetAdminMail()
        {
            return _adminMail;
        }

        internal void SetAdminMail(string mail)
        {
            _adminMail = mail;
        }

        public void SetYoutubeFilteringLevel(YoutubeFilteringLevels level)
        {
            _youtubeFilteringLevel = level;
        }
    
        public List<string> GetCustomBlackList()
        {
            return _customBlacklist;
        }

        public List<string> GetCustomExceptionsList()
        {
            return _customExceptionsList;
        }
        public string AddSiteToBlackList(string url)
        {
            if (url.CheckURLValid())
            {
                if (!GetCustomBlackList().Contains(url))
                {
                    if (!GetCustomExceptionsList().Contains(url) && url.Contains("mmb.org.il") == false)
                    {
                        _customBlacklist.Add(url);
                        HostsFileAdapter.Write(FilteringSystem.GetCurrentFilteringSettings());
                        FilteringSystem.SaveChanges();
                        return "";
                    }
                    else
                    {
                        return "כתובת זה מופיעה ברשימת החריגים, על מנת לחוסמה, יש להסירה תחילה מרשימה זו";
                    }
                }
                else
                {
                    return "הכתובת כבר קיימת ברשימת האתרים לחסימה";
                }
            }
            else
            {
                return "הכתובת אינה כתובת אינטרנט תקינה";
            }
        }
        public void RemoveSiteFromBlackList(string url)
        {
            _customBlacklist.Remove(url);
            HostsFileAdapter.Write(FilteringSystem.GetCurrentFilteringSettings());
            FilteringSystem.SaveChanges();
        }
        public string AddSiteToExceptionList(string url)
        {
            if (url.CheckURLValid())
            {
                if (!GetCustomExceptionsList().Contains(url))
                {
                    if (!GetCustomBlackList().Contains(url))
                    {
                        _customExceptionsList.Add(url);
                        HostsFileAdapter.Write(FilteringSystem.GetCurrentFilteringSettings());
                        FilteringSystem.SaveChanges();
                        return "";
                    }
                    else
                    {
                        return "כתובת זה מופיעה ברשימת החסומים, על מנת להחריגה, יש להסירה תחילה מרשימה זו";
                    }
                }
                else
                {
                    return "הכתובת כבר קיימת ברשימה";
                }
            }
            else
            {
                return "הכתובת אינה כתובת אינטרנט תקינה";
            }
        }
        public void RemoveSiteFromExceptionList(string url)
        {
            _customExceptionsList.Remove(url);
            HostsFileAdapter.Write(FilteringSystem.GetCurrentFilteringSettings());
            FilteringSystem.SaveChanges();
        }
        public void SetAdminPassword(string password)
        {
            _password = PasswordEncryption.Encrypt(password);

            Boolean savedSuccessfuly = false;
            int attempts = 0;
            Exception error = null;
            while (!savedSuccessfuly)
            {
                if (attempts < 4)
                {
                    try
                    {
                        FilesCathcer.StopCatchingSystemFiles();
                        string UninstallPasswordPath = "UniPass";
                        if (!File.Exists(UninstallPasswordPath))
                        {
                            File.Create(UninstallPasswordPath).Close();
                        }
                        File.WriteAllText(UninstallPasswordPath, _password);
                        FilesCathcer.CatchSystemFiles();
                        savedSuccessfuly = true;
                    }
                    catch (Exception e)
                    {
                        error = e;
                        attempts++;
                        System.Threading.Thread.Sleep(200);
                    }
                }
                else
                {
                    CustomMessageBox.ShowDialog(null, error.Message, "שגיאה בשמירת נתונים", CustomMessageBox.CustomMessageBoxTypes.Error, "הבנתי");
                    break;
                }
            }
        }
        public string GetAdminPassword()
        {
            return _password;
        }
    }
}