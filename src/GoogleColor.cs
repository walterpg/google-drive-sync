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

using KeePassLib.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace GoogleSyncPlugin
{
    /// <summary>
    /// Google doesn't provide names in it's folder color list - only rgb
    /// values.  This class attempts to fix this accessiblity issue by 
    /// by providing a "best-fit" name using .NET's KnownColor enumeration.
    /// To help resolve name duplication in lists, a color-space distance
    /// between .NET KnownColor and the given rgb value is provided by the 
    /// NameDistance property.  Distances to KnownColor values are 
    /// maintained in a sorted list, so that a resolver may choose, in the
    /// case of duplicates, which ColorProxy instance should take a
    /// "next-best" KnownColor name, via the PopTopColor() method.
    /// </summary>
    [Serializable]
    public class GoogleColor :
        IComparer<Tuple<string, double>>, ISerializable
    {
        const string RGBKEY = "RGB";
        const string NAMEKEY = "NAME";

        class Binder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                AssemblyName sname = new AssemblyName(assemblyName);
                AssemblyName aname = typeof(GoogleColor).Assembly.GetName();
                if (sname.Name == aname.Name &&
                    sname.Version <= aname.Version &&
                    sname.GetPublicKeyToken().SequenceEqual(aname.GetPublicKeyToken()) &&
                    typeName == typeof(GoogleColor).FullName)
                {
                    return typeof(GoogleColor);
                }
                throw new SerializationException();
            }
        }

        static Color[] m_knownColors = null;
        static Dictionary<Color, string> m_colorNames = null;
        static GoogleColor m_default = new GoogleColor(Color.White, "Google Default");

        static Color[] KnownColors
        {
            get
            {
                if (m_knownColors == null)
                {
                    Array colorIDs = Enum.GetValues(typeof(KnownColor));
                    m_knownColors = colorIDs.Cast<KnownColor>()
                                            .Select(id => Color.FromKnownColor(id))
                                            .Where(kc => !kc.IsSystemColor)
                                            .ToArray();
                }
                return m_knownColors;
            }
        }

        static Dictionary<Color, string> NameDictionary
        {
            get
            {
                if (m_colorNames == null)
                {
                    m_colorNames = new Dictionary<Color, string>(KnownColors.Length);

                    // KnownColor "names" are not localized, but are rather
                    // the pascal-cased enumerated value identifiers.  Make
                    // them a little prettier.
                    foreach (Color kcVal in KnownColors)
                    {
                        string id = kcVal.Name;
                        StringBuilder sb = new StringBuilder(id.Length + 2);
                        sb.Append(id.First());
                        foreach (char c in id.Substring(1))
                        {
                            if (char.IsUpper(c))
                            {
                                sb.Append(' ');
                            }
                            sb.Append(c);
                        }
                        m_colorNames[kcVal] = sb.ToString();
                    }
                }
                return m_colorNames;
            }
        }

        public static GoogleColor Default
        {
            get
            {
                return m_default;
            }
        }

        public static string SerializeToString(GoogleColor c)
        {
            if (c == null)
            {
                return null;
            }

            IFormatter formatter = new BinaryFormatter()
            {
                Binder = new GoogleColor.Binder(),
            };
            MemoryStream ms = new MemoryStream();
            using (ms)
            {
                formatter.Serialize(ms, c);
                return MemUtil.ByteArrayToHexString(ms.ToArray());
            }
        }

        public static GoogleColor DeserializeFromString(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }
            try
            {
                IFormatter formatter = new BinaryFormatter()
                {
                    Binder = new GoogleColor.Binder()
                };
                byte[] bytes = MemUtil.HexStringToByteArray(s);
                MemoryStream ms = new MemoryStream(bytes);
                using (ms)
                {
                    return formatter.Deserialize(ms) as GoogleColor;
                }
            }
            catch
            {
                return null;
            }
        }

        // Expect "#hhhhhh".
        static Color ParseHtmlColor(string htmlColor)
        {
            int argb;
            if (string.IsNullOrEmpty(htmlColor))
            {
                throw new ArgumentNullException();
            }
            else if (htmlColor[0] != '#' ||
                !int.TryParse(htmlColor.Substring(1).TrimEnd(),
                            NumberStyles.HexNumber,
                            NumberFormatInfo.InvariantInfo,
                            out argb))
            {
                throw new FormatException();
            }
            argb = (int)((uint)argb | 0xFF000000);
            return Color.FromArgb(argb);
        }

        string m_name;
        List<Tuple<string, double>> m_distances;

        public GoogleColor(Color c)
            : this(c, null)
        {
        }

        public GoogleColor(Color c, string name)
        {
            Color = c;
            m_name = name;
            if (name == null && c.IsKnownColor && !c.IsSystemColor)
            {
                m_name = c.ToString();
            }
        }

        public GoogleColor(string htmlColor)
            : this(ParseHtmlColor(htmlColor))
        {
        }

        protected GoogleColor(SerializationInfo info, StreamingContext ctx)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            int rgb = info.GetInt32(RGBKEY);
            Color = Color.FromArgb(rgb);

            byte[] bytes = (byte[])info.GetValue(NAMEKEY, typeof(byte[]));
            m_name = Encoding.UTF8.GetString(bytes);
        }

        public Color Color { get; private set; }

        public string HtmlHexString
        {
            get
            {
                return string.Format("#{0:x}{1:x}{2:x}",
                                        Color.R, Color.G, Color.B);
            }
        }

        public string Name
        {
            get
            {
                if (m_name == null)
                {
                    m_name = Deltas.First().Item1;
                }
                return m_name;
            }
        }

        public double NameDistance
        {
            get
            {
                return Deltas.First().Item2;
            }
        }

        public double SecondNameDistance
        {
            get
            {
                return Deltas.ElementAt(1).Item2;
            }
        }

        public int ColorCount
        {
            get
            {
                return Deltas.Count;
            }
        }

        public void PopTopColor()
        {
            Deltas.Remove(Deltas.First());
            m_name = null;
        }

        public override string ToString()
        {
            return Name;
        }

        Tuple<string, double> GetNameAndDistance(Color kc)
        {
            // Determine distance between our color and the given color.
            double r, g, b;
            r = kc.R - Color.R;
            r *= r;
            g = kc.G - Color.G;
            g *= g;
            b = kc.B - Color.B;
            b *= b;
            double distance = Math.Sqrt(r + g + b);

            Debug.Assert(kc.IsKnownColor);
            string name = NameDictionary[kc];

            return new Tuple<string, double>(name, distance);
        }

        // List of KnownColor names, and their rgb distance from Color.
        List<Tuple<string, double>> Deltas
        {
            get
            {
                if (m_distances == null)
                {
                    m_distances = KnownColors
                            .Select(kc => GetNameAndDistance(kc))
                            .ToList();
                    m_distances.Sort(this);
                }
                return m_distances;
            }
        }

        // Used By List.Sort to list items in ascending distance order.
        public int Compare(Tuple<string, double> x, Tuple<string, double> y)
        {
            return (int)(x.Item2 - y.Item2);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            // Just save the name and rgb value.
            info.AddValue(RGBKEY, Color.ToArgb());
            byte[] nameBytes = Encoding.UTF8.GetBytes(Name);
            info.AddValue(NAMEKEY, nameBytes, typeof(byte[]));
        }
    }

    class GoogleColorCollection : IEnumerable<GoogleColor>
    {
        static void ReduceDuplicates(IGrouping<string, GoogleColor> group,
            GoogleColor dup)
        {
            foreach (GoogleColor other in group
                .Where(c => c != dup && c.ColorCount > 1))
            {
                other.PopTopColor();
            }
        }

        readonly GoogleColor[] m_colors;

        public GoogleColorCollection(IEnumerable<Color> colors)
            : this(colors.Select(c => new GoogleColor(c)))
        {
        }

        public GoogleColorCollection(IEnumerable<GoogleColor> colors)
        {
            m_colors = colors.ToArray();

            // Resolve duplicate-named ColorProxy instances using a simple,
            // flawed algorithm.  If an instance of ColorProxy in a set has
            // no other alternative names, it keeps the duplicate name.  
            // Otherwise, the instance in the set with the longest distance
            // to its next-closest name uses the duplicate name.  Others in the
            // set with alternatives use their next-best fit name.  This 
            // process is repeated until no ColorProxy instance simultaneously
            // has a duplicated name and an alternative name.
            IGrouping<string, GoogleColor> group;
            while (null != (group = m_colors.GroupBy(cp => cp.Name)
                                .Where(g => g.Count() > 1 &&
                                        !g.All(c => c.ColorCount == 1))
                                .FirstOrDefault()))
            {
                GoogleColor onlyColor = group.Where(c => c.ColorCount == 1)
                                            .FirstOrDefault();
                if (onlyColor != null)
                {
                    // Give it to the color that has no other alternatives.
                    ReduceDuplicates(group, onlyColor);
                    continue;
                }

                if (group.All(c => c.ColorCount > 1))
                {
                    // The color whose second choice has the longest distance
                    // keeps the color; the others pop.
                    foreach (GoogleColor lostColor in group
                                .OrderByDescending(c => c.SecondNameDistance)
                                .Skip(1))
                    {
                        lostColor.PopTopColor();
                    }
                    continue;
                }

                GoogleColor worstColor = group
                                .OrderByDescending(c => c.NameDistance)
                                .First();
                ReduceDuplicates(group, worstColor);
            }
        }

        public IEnumerator<GoogleColor> GetEnumerator()
        {
            return m_colors.Cast<GoogleColor>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}
