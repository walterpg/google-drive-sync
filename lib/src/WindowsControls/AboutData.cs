/**
 * Google Sync Plugin for KeePass Password Safe
 * Copyright © 2012-2016  DesignsInnovate
 * Copyright © 2014-2016  Paul Voegler
 * 
 * KPSync for Google Drive
 * Copyright © 2020-2021 Walter Goodwin
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
**/

using System;

namespace KPSyncForDrive.WindowsControls
{
    public interface IAboutData
    {
        string WebsiteLinkText { get; }

        Uri WebsiteLink { get; }

        string PrivacyLinkText { get; }

        Uri PrivacyLink { get; }

        string CopyrightText { get; }

        string Gs3Attribution { get; }

        string SemVer { get; }
    }

    class AboutData : IAboutData
    {
        public AboutData()
        {
            SemVer = "v" + new Version(0, 0, 0) + "-unstable";
            WebsiteLinkText = "Visit KPSync.org";
            WebsiteLink = new Uri("https://www.kpsync.org/");
            PrivacyLinkText = "Privacy Policy";
            PrivacyLink = new Uri("https://www.kpsync.org/privacy/");
            CopyrightText = "Copyright @ 2020-2021";
            Gs3Attribution = "Based on KeePass Google Sync Plugin 3.0, Copyright @ 2016 DesignsInnovate.";
        }

        public string WebsiteLinkText { get; set; }

        public Uri WebsiteLink { get; set; }

        public string PrivacyLinkText { get; set; }

        public Uri PrivacyLink { get; set; }

        public string CopyrightText { get; set; }

        public string Gs3Attribution { get; set; }

        public string SemVer { get; set; }
    }
}
