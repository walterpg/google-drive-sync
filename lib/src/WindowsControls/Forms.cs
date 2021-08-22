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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace KPSyncForDrive.WindowsControls
{
    public static class Forms
    {
        const int INDEX_LOGPIXELSX = 88;

        static int? s_dpi;

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(HandleRef hWnd);

        [DllImport("user32.dll")]
        static extern int ReleaseDC(HandleRef hWnd, HandleRef hDC);

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(HandleRef hDC, int nIndex);

        static int DPI
        {
            get
            {
                if (s_dpi is null)
                {
                    HandleRef winHwnd = new HandleRef(null, IntPtr.Zero);
                    IntPtr hDC = GetDC(winHwnd);
                    HandleRef winDC = new HandleRef(null, hDC);
                    try
                    {
                        s_dpi = GetDeviceCaps(winDC, INDEX_LOGPIXELSX);
                    }
                    finally
                    {
                        ReleaseDC(winHwnd, winDC);
                    }
                }
                return s_dpi.Value;
            }
        }

        static double DIPixelsPerPixel => (double)96 / DPI;

        static int DIPixelsToPixels(double diPixels)
        {
            return (int)(diPixels / DIPixelsPerPixel);
        }

        static Size SizeFromWpfSize(System.Windows.Size wpfSize)
        {
            return new Size(DIPixelsToPixels(wpfSize.Width),
                DIPixelsToPixels(wpfSize.Height));
        }

        public static Form GetNewAboutForm(IAboutData data,
            Icon formIco = null)
        {
            Form f = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterParent,
                AutoSize = true,
            };
            if (formIco != null)
            {
                f.Icon = formIco;
            }
            About ctlAbout = new About
            {
                DataContext = data,
                Tag = f,
            };
            ctlAbout.RandomClick += HandleSplashClose;
            f.Controls.Add(new ElementHost
            {
                Child = ctlAbout,
                Dock = DockStyle.Fill,
                AutoSize = true,
            });
            ctlAbout.UpdateLayout();
            f.Size = SizeFromWpfSize(ctlAbout.DesiredSize);
            return f;
        }

        static void HandleSplashClose(object sender, EventArgs e)
        {
            if (sender is About ctlAbout &&
                ctlAbout.Tag is Form f)
            {
                f.Close();
            }
        }

    }
}
