/**
 * Google Sync Plugin for KeePass Password Safe
 * Copyright(C) 2012-2016  DesignsInnovate
 * Copyright(C) 2014-2016  Paul Voegler
 * 
 * Google Drive Sync for KeePass Password Safe
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
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace GoogleDriveSync
{
    class CenterWin32Dlg : IDisposable
    {
        // https://docs.microsoft.com/en-us/windows/win32/winmsg/about-window-classes#system-classes
        const string WinSysDlgClass = "#32770";

        // https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-wndclassexa#members
        const int MaxlpszClassName = 256;

        delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lp);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumThreadWindows(uint dwThreadId,
            EnumThreadDelegate lpfn, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName,
            int nMaxCount);

        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y,
            int nWidth, int nHeight, bool bRepaint);


        readonly Form m_owner;
        int m_tries;
        bool m_disposed;

        public CenterWin32Dlg(Form owner)
        {
            m_owner = owner;
            m_tries = 0;
            m_disposed = false;
            m_owner.BeginInvoke(new MethodInvoker(DlgSearch));
        }

        bool FixWindowIfWinDlg(IntPtr hWnd, IntPtr lp)
        {
            int classNameSize = MaxlpszClassName + IntPtr.Size;
            StringBuilder sb = new StringBuilder(classNameSize);
            GetClassName(hWnd, sb, sb.Capacity);
            if (!WinSysDlgClass.Equals(sb.ToString(),
                                    StringComparison.Ordinal))
            {
                // Not a Win dialog.
                return true;
            }

            Reposition(hWnd);
            return false;
        }

        void Reposition(IntPtr hWnd)
        {
            // Owner dimensions.
            Rectangle frmRect = new Rectangle(m_owner.Location, m_owner.Size);

            // Win dlg dimensions.
            RECT dlgRect;
            GetWindowRect(hWnd, out dlgRect);

            // Translate Win dlg position.
            int x = frmRect.Left +
                    (frmRect.Width - dlgRect.Right + dlgRect.Left) / 2;
            int y = frmRect.Top +
                    (frmRect.Height - dlgRect.Bottom + dlgRect.Top) / 2;
            int nWidth = dlgRect.Right - dlgRect.Left;
            int nHeight = dlgRect.Bottom - dlgRect.Top;
            if (!MoveWindow(hWnd, x, y, nWidth, nHeight, true))
            {
                Debug.WriteLine("MoveWindow Failed!");
            }
        }

        void DlgSearch()
        {
            // Enumerate windows to find the Win32 dialog box.
            if (m_disposed ||
                m_owner.WindowState == FormWindowState.Minimized)
            {
                return;
            }
            if (EnumThreadWindows(GetCurrentThreadId(), FixWindowIfWinDlg,
                                    IntPtr.Zero) &&
                m_tries++ < 10)
            {
                m_owner.BeginInvoke(new MethodInvoker(DlgSearch));
            }
        }

        protected virtual void Dispose(bool bIsDisposing)
        {
            m_disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
