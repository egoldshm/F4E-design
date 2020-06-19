﻿using System;
using System.Collections.Generic;
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
        public void addSiteToBlacklist(string url)
        {
            _customBlacklist.Add(url);
        }
        public void removeSiteFromBlacklist(string toRemove)
        {
            _customBlacklist.Remove(toRemove);
        }

        public List<string> GetCustomExceptionsList()
        {
            return _customExceptionsList;
        }
        public void removeSiteFromExceptionsList(string toRemove)
        {
            _customExceptionsList.Remove(toRemove);
        }
        public void addSiteToExceptionsList(string url)
        {
            _customExceptionsList.Add(url);
        }

        public void SetAdminPassword(string password)
        {
            _password = PasswordEncryption.Encrypt(password);
        }
        public string GetAdminPassword()
        {
            return _password;
        }
    }
}