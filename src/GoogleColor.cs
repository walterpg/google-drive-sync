/**
 * Google Sync Plugin for KeePass Password Safe
 * Copyright(C) 2012-2016  DesignsInnovate
 * Copyright(C) 2014-2016  Paul Voegler
 * 
 * KPSync for Google Drive
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;

namespace KPSyncForDrive
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
    public class GoogleColor : IComparer<Tuple<string, double>>
    {
        static Color[] s_knownColors = null;
        static Dictionary<Color, string> s_colorNames = null;
        static GoogleColor s_default = new GoogleColor(Color.White,
                        Resources.GetString("DropDown_GoogleDefaultColor"));

        static Color[] KnownColors
        {
            get
            {
                if (s_knownColors == null)
                {
                    Array colorIDs = Enum.GetValues(typeof(KnownColor));
                    s_knownColors = colorIDs.Cast<KnownColor>()
                                            .Select(id => Color.FromKnownColor(id))
                                            .Where(kc => !kc.IsSystemColor)
                                            .ToArray();
                }
                return s_knownColors;
            }
        }

        static Dictionary<Color, string> NameDictionary
        {
            get
            {
                if (s_colorNames == null)
                {
                    s_colorNames = new Dictionary<Color, string>(KnownColors.Length);

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
                        s_colorNames[kcVal] = sb.ToString();
                    }
                }
                return s_colorNames;
            }
        }

        public static GoogleColor Default
        {
            get
            {
                return s_default;
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
