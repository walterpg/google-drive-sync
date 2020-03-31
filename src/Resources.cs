/**
 * Google Sync Plugin for KeePass Password Safe
 * Copyright(C) 2012-2016  DesignsInnovate
 * Copyright(C) 2014-2016  Paul Voegler
 * Copyright(C) 2020       Walter Goodwin
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

using KeePass;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

namespace GoogleSyncPlugin
{
    class SingleAssemblyResourcesManager : ResourceManager
    {
        CultureInfo m_neutralResourcesCulture;
        Dictionary<CultureInfo, ResourceSet> m_resxSets;
        readonly object m_resxSetsCxLock;

        public SingleAssemblyResourcesManager(string resxName, Assembly asm)
            : base(resxName, asm)
        {
            m_resxSetsCxLock = new object();
        }

        // Replaces deprecated base class property 'ResourceSets'.
        Dictionary<CultureInfo, ResourceSet> Resources
        {
            get
            {
                if (m_resxSets == null)
                {
                    lock (m_resxSetsCxLock)
                    {
                        if (m_resxSets == null)
                        {
                            m_resxSets = new Dictionary<CultureInfo, ResourceSet>();
                        }
                    }
                }
                return m_resxSets;
            }
        }

        protected override ResourceSet InternalGetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents)
        {
            ResourceSet rs;
            if (!Resources.TryGetValue(culture, out rs))
            {
                if (m_neutralResourcesCulture == null)
                {
                    m_neutralResourcesCulture = GetNeutralResourcesLanguage(MainAssembly);
                }
                if (m_neutralResourcesCulture.Equals(culture))
                {
                    culture = CultureInfo.InvariantCulture;
                }
                string resourceFileName = GetResourceFileName(culture);
                Stream resStream = MainAssembly.GetManifestResourceStream(resourceFileName);

                if (resStream == null && !culture.IsNeutralCulture)
                {
                    // try two-letter culture
                    resourceFileName = GetResourceFileName(culture.Parent);
                    resStream = MainAssembly.GetManifestResourceStream(resourceFileName);
                }
                if (resStream == null)
                {
                    rs = base.InternalGetResourceSet(culture, createIfNotExists, tryParents);
                }
                else
                {
                    using (resStream)
                    {
                        rs = new ResourceSet(resStream);
                    }
                }
                if (rs != null)
                {
                    lock (Resources)
                    {
                        ResourceSet ex;
                        if (!Resources.TryGetValue(culture, out ex))
                        {
                            Resources.Add(culture, rs);
                        }
                        else if (!object.ReferenceEquals(ex, rs))
                        {
                            rs.Dispose();
                            rs = ex;
                        }
                    }
                }
            }
            return rs;
        }
    }

    internal sealed class Resources
    {
        static object s_lock = new object();
        static Resources s_instance = null;

        public static Resources Instance
        {
            get
            {
                if (s_instance == null)
                {
                    lock(s_lock)
                    {
                        if (s_instance == null)
                        {
                            s_instance = new Resources();
                        }
                    }
                }
                return s_instance;
            }
        }

        readonly ResourceManager m_strings;
        readonly ResourceManager m_images;

        Resources()
        {
            Type thisType = GetType();
            m_strings = new SingleAssemblyResourcesManager("GoogleSyncPlugin.Strings",
                                            thisType.Assembly);
            m_images = new ResourceManager(thisType.Namespace + ".Images", thisType.Assembly);
        }

        CultureInfo CurrentCulture
        {
            get
            {
                try
                {
                    // From KeePass MainForm.cs
                    string strIso6391 = Program.Translation.Properties.Iso6391Code;
                    return CultureInfo.CreateSpecificCulture(strIso6391);
                }
                catch 
                {
                    return CultureInfo.CurrentUICulture;
                }
            }
        }

        public static string GetFormat(string label, params object[] args)
        {
            string format = GetString(label);
            if (args == null)
            {
                args = new object[] { };
            }
            if (!string.IsNullOrEmpty(format))
            {
                format = string.Format(CultureInfo.CurrentUICulture, format, args);
            }
            else
            {
                // No format found, so create CSV string of label + args.
                format = args.Aggregate(new StringBuilder(label),
                                (acc, arg) =>
                                {
                                    acc.Append(", ");
                                    if (arg == null)
                                    {
                                        arg = "<null>";
                                    }
                                    acc.Append(arg.ToString());
                                    return acc;
                                },
                                acc => acc.ToString());
            }
            return format;
        }

        public static string GetString(string label)
        {
            return Instance.m_strings.GetString(label,
                                        Instance.CurrentCulture);
        }

        public static Bitmap GetBitmap(string name)
        {
            return (Bitmap)Instance.m_images.GetObject(name);
        }

        public static MemoryStream GetImageStream(string name, ImageFormat fmt)
        {
            Bitmap image = GetBitmap(name);
            MemoryStream stream = new MemoryStream(Defs.DefaultDotNetFileBufferSize);
            image.Save(stream, fmt);
            stream.Position = 0;
            return stream;
        }
    }
}
