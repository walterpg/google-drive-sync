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

using KeePass.UI;
using KeePassLib.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GoogleDriveSync
{
    partial class ConfigurationForm : Form
    {
        const string GSigninTabIcoKey = "gsignin";
        const string GeneralTabIcoKey = "general";
        const string AboutIcoKey = "about";

        readonly ConfigurationFormData m_data;
        readonly GoogleColor m_nullColor;
        bool m_bColorsQueried;

        public ConfigurationForm(ConfigurationFormData data)
        {
            InitializeComponent();
            
            EnsureCheckEnabledGroupBox(m_chkDefaultUseLegacyCreds,
                                        m_grpDriveAuthDefaults);
            EnsureCheckEnabledGroupBox(m_chkUseLegacyCreds, m_grpDriveAuth);

            Text = GdsDefs.ProductName;
            DatabaseFilePath = string.Empty;
            m_lblAboutVer.Text = GdsDefs.Version;
            m_lblAboutProd.Text = GdsDefs.ProductName;

            // Localize the form
            Control[] textCx = new Control[]
            {
                m_lnkGoogle,
                m_lnkHelp,
                m_lnkHome,
                m_lnkGoogle2,
                m_lnkHelp2,
                m_tabGSignIn,
                m_tabOptions,
                m_tabAbout,
                m_grpEntry,
                m_grpDriveAuth,
                m_lblAccount,
                m_lblClientId,
                m_lblClientSecret,
                m_chkDriveScope,
                m_chkLegacyClientId,
                m_grpDriveOptions,
                m_lblHintFolder,
                m_lblFolder,
                m_btnCancel,
                m_btnOK,
                m_grpCmdEnabled,
                m_chkSyncEnabled,
                m_chkUploadEnabled,
                m_chkDownloadEnabled,
                m_grpAutoSync,
                m_chkSyncOnOpen,
                m_chkSyncOnSave,
                m_lblDefaultClientId,
                m_lblDefaultClientSecret,
                m_chkDefaultDriveScope,
                m_chkDefaultLegacyClientId,
                m_grpFolderDefaults,
                m_lblHintDefaultFolder,
                m_lblDefaultFolderLabel,
                m_lblDefFolderColor,
                m_btnGetColors,
                m_lblAttribution,
                m_chkDefaultUseLegacyCreds,
                m_chkUseLegacyCreds,
            };
            foreach (Control c in textCx)
            {
                c.Text = Resources.GetString(c.Text);
            }

            m_data = data;

            // Wire events and such for the folder color picker. 
            m_nullColor = new GoogleColor(m_cbColors.BackColor,
                GoogleColor.Default.Name);
            m_cbColors.DrawMode = DrawMode.OwnerDrawVariable;
            m_cbColors.DrawItem += HandleColorsDrawItem;
            m_btnGetColors.Click += HandleColorsComboLazyLoad;
            m_cbColors.SelectedIndexChanged += HandleColorsSelectedIndexChanged;
            if (m_data.DefaultAppFolderColor == null ||
                m_data.DefaultAppFolderColor == GoogleColor.Default)
            {
                m_cbColors.Items.Add(m_nullColor);
                m_cbColors.SelectedIndex = 0;
                m_cbColors.Enabled = false;
            }
            else
            {
                GoogleColor[] userColors = new[]
                {
                    m_nullColor,
                    new GoogleColor(m_data.DefaultAppFolderColor.Color,
                                m_data.DefaultAppFolderColor.Name)
                };
                m_cbColors.Items.Clear();
                m_cbColors.Items.AddRange(userColors);
                m_cbColors.SelectedItem = userColors[1];
                m_cbColors.Enabled = string.IsNullOrWhiteSpace(m_txtFolderDefault.Text);
            }
            m_btnGetColors.Enabled = string.IsNullOrWhiteSpace(m_txtFolderDefault.Text);
            m_bColorsQueried = false;
        }

        static void EnsureCheckEnabledGroupBox(CheckBox chk, GroupBox grp)
        {
            grp.Enabled = chk.Checked;
            if (chk.Parent == grp)
            {
                // Move checkbox to the groupbox parent.
                grp.Parent.Controls.Add(chk);

                // Translate position relative to parent.
                chk.Location = new Point(
                    chk.Left + grp.Left,
                    chk.Top + grp.Top);

                // Checkbox covers the groupbox.
                chk.BringToFront();
            }
        }

        // Do most event and binding stitching here because KeePass likes to
        // dispose forms quickly.  Also, the presentation object can change
        // before the form is shown.
        protected override void OnLoad(EventArgs args)
        {
            // Do the "data bindings" with the presentation object.
            BindingSource bindingSource = m_data.EntryBindingSource;
            bindingSource.DataSource = m_data.Entries;
            m_cbAccount.DataSource = bindingSource;
            m_cbAccount.DisplayMember = "Title";
            m_cbAccount.ValueMember = "Entry";

            // Entry sync config controls.
            Binding binding;
            Debug.Assert(m_txtClientId is TextBox);
            binding = new Binding("Text",
                bindingSource,
                "ClientId");
            m_txtClientId.DataBindings.Add(binding);
            binding = new Binding("Enabled",
                bindingSource,
                "UseLegacyKp3ClientId", true);
            binding.Format += HandleBoolNegation;
            binding.Parse += HandleBoolNegation;
            m_txtClientId.DataBindings.Add(binding);
            Debug.Assert(m_txtClientSecret is TextBox);
            binding = new Binding("Text",
                bindingSource,
                "ClientSecret", true);
            binding.Format += HandleProtectedStringFormatting;
            binding.Parse += HandleProtectedStringParsing;
            m_txtClientSecret.DataBindings.Add(binding);
            binding = new Binding("Enabled",
                bindingSource,
                "UseLegacyKp3ClientId", true);
            binding.Format += HandleBoolNegation;
            binding.Parse += HandleBoolNegation;
            m_txtClientSecret.DataBindings.Add(binding);
            Debug.Assert(m_txtFolder is TextBox);
            binding = new Binding("Text",
                bindingSource,
                "ActiveFolder");
            m_txtFolder.DataBindings.Add(binding);
            Debug.Assert(m_chkDriveScope is CheckBox);
            binding = new Binding("Checked",
                bindingSource,
                "IsLegacyRestrictedDriveScope", true);
            binding.Format += HandleBoolNegation;
            binding.Parse += HandleBoolNegation;
            m_chkDriveScope.DataBindings.Add(binding);
            Debug.Assert(m_chkLegacyClientId is CheckBox);
            binding = new Binding("Checked",
                bindingSource,
                "UseLegacyKp3ClientId");
            binding.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            m_chkLegacyClientId.DataBindings.Add(binding);
            Debug.Assert(m_chkUseLegacyCreds is CheckBox);
            binding = new Binding("Checked",
                bindingSource,
                "UseLegacyCreds");
            binding.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            m_chkUseLegacyCreds.DataBindings.Add(binding);

            // Global default auth controls.
            Debug.Assert(m_txtFolderDefault is TextBox);
            binding = new Binding("Text",
                m_data, "DefaultAppFolder");
            m_txtFolderDefault.DataBindings.Add(binding);
            Debug.Assert(m_chkDefaultDriveScope is CheckBox);
            binding = new Binding("Checked",
                m_data, "DefaultIsLegacyRestrictedDriveScope");
            binding.Format += HandleBoolNegation;
            binding.Parse += HandleBoolNegation;
            m_chkDefaultDriveScope.DataBindings.Add(binding);
            Debug.Assert(m_txtDefaultClientId is TextBox);
            binding = new Binding("Text",
                m_data, "DefaultLegacyClientId");
            m_txtDefaultClientId.DataBindings.Add(binding);
            binding = new Binding("Enabled",
                m_data, "DefaultUseKpgs3ClientId", true);
            binding.Format += HandleBoolNegation;
            binding.Parse += HandleBoolNegation;
            m_txtDefaultClientId.DataBindings.Add(binding);
            Debug.Assert(m_txtDefaultClientSecret is TextBox);
            binding = new Binding("Text", m_data,
                "DefaultLegacyClientSecret", true);
            binding.Format += HandleProtectedStringFormatting;
            binding.Parse += HandleProtectedStringParsing;
            m_txtDefaultClientSecret.DataBindings.Add(binding);
            binding = new Binding("Enabled",
                m_data, "DefaultUseKpgs3ClientId", true);
            binding.Format += HandleBoolNegation;
            binding.Parse += HandleBoolNegation;
            m_txtDefaultClientSecret.DataBindings.Add(binding);
            Debug.Assert(m_chkDefaultLegacyClientId is CheckBox);
            binding = new Binding("Checked",
                m_data, "DefaultUseKpgs3ClientId");
            binding.DataSourceUpdateMode = 
                DataSourceUpdateMode.OnPropertyChanged;
            m_chkDefaultLegacyClientId.DataBindings.Add(binding);
            binding = new Binding("Checked",
                m_data, "DefaultUseLegacyCredentials");
            binding.DataSourceUpdateMode =
                DataSourceUpdateMode.OnPropertyChanged;
            m_chkDefaultUseLegacyCreds.DataBindings.Add(binding);

            // Enabled command controls.
            Debug.Assert(m_chkSyncEnabled is CheckBox);
            binding = new Binding("Checked",
                m_data, "CmdSyncEnabled");
            binding.DataSourceUpdateMode =
                DataSourceUpdateMode.OnPropertyChanged;
            m_chkSyncEnabled.DataBindings.Add(binding);
            Debug.Assert(m_chkDownloadEnabled is CheckBox);
            binding = new Binding("Checked",
                m_data, "CmdDownloadEnabled");
            m_chkDownloadEnabled.DataBindings.Add(binding);
            Debug.Assert(m_chkUploadEnabled is CheckBox);
            binding = new Binding("Checked",
                m_data, "CmdUploadEnabled");
            m_chkUploadEnabled.DataBindings.Add(binding);

            // Auto-sync controls.
            Debug.Assert(m_chkSyncOnOpen is CheckBox);
            binding = new Binding("Checked",
                m_data, "SyncOnOpen");
            m_chkSyncOnOpen.DataBindings.Add(binding);
            binding = new Binding("Enabled",
                m_data, "CmdSyncEnabled");
            m_chkSyncOnOpen.DataBindings.Add(binding);
            Debug.Assert(m_chkSyncOnSave is CheckBox);
            binding = new Binding("Checked",
                m_data, "SyncOnSave");
            m_chkSyncOnSave.DataBindings.Add(binding);
            binding = new Binding("Enabled",
                m_data, "CmdSyncEnabled");
            m_chkSyncOnSave.DataBindings.Add(binding);

            // Select first "active" entry in the accounts combo.
            IEnumerable<EntryConfiguration> actives = m_data.Entries
                .Where(e => e.ActiveAccount.HasValue &&
                            e.ActiveAccount.Value);
            if (actives.Any())
            {
                m_cbAccount.SelectedItem = actives.First();
                bindingSource.ResetBindings(false);
            }

            // Initialize link handlers.
            m_lnkGoogle.LinkClicked += (o, e) => Process.Start(GdsDefs.UrlGoogleDev);
            m_lnkHelp.LinkClicked += (o, e) => Process.Start(GdsDefs.UrlHelp);
            m_lnkHome.LinkClicked += (o, e) => Process.Start(GdsDefs.UrlHome);
            m_lnkGoogle2.LinkClicked += (o, e) => Process.Start(GdsDefs.UrlGoogleDev);
            m_lnkHelp2.LinkClicked += (o, e) => Process.Start(GdsDefs.UrlHelp);
            m_lblAttribution.Click += (o, e) => Process.Start(GdsDefs.UrlLegacyHome);

            // Manage tab changes to prevent invalid data entry.
            m_tabMain.Deselecting += HandleTabChangeValidation;

            // Don't disable all commands.
            m_chkSyncEnabled.CheckedChanged += HandleCommandDisabled;
            m_chkDownloadEnabled.CheckedChanged += HandleCommandDisabled;
            m_chkUploadEnabled.CheckedChanged += HandleCommandDisabled;

            // More oddball color picker UI handling.
            m_txtFolderDefault.Validated += HandleDefaultFolderValidated;

            // OK button handler
            m_btnOK.Click += HandleOkClicked;

            // Initialize KeePass dialog banner.
            string filePath = DatabaseFilePath;
            if (filePath.Length > 60)
            {
                m_toolTipper.SetToolTip(m_bannerImage, DatabaseFilePath);
                string fileName = System.IO.Path.GetFileName(filePath);
                if (fileName.Length > 50)
                {
                    DatabaseFilePath = "...\\" + fileName;
                }
                else
                {
                    DatabaseFilePath = filePath.Substring(0, 50-fileName.Length) + "...\\" + fileName;
                }
            }
            BannerFactory.CreateBannerEx(this, m_bannerImage,
                Resources.GetBitmap("round_settings_black_48dp"),
                Resources.GetString("Title_ConfigDialog"),
                Resources.GetFormat("Lbl_CurrentDbFmt", DatabaseFilePath));

            // Initialize tab images.
            m_imgList.Images.Add(GSigninTabIcoKey,
                Resources.GetBitmap("outline_security_black_18dp"));
            m_imgList.Images.Add(GeneralTabIcoKey,
                Resources.GetBitmap("outline_settings_black_18dp"));
            m_imgList.Images.Add(AboutIcoKey,
                Resources.GetBitmap("round_help_outline_black_18dp"));
            m_tabGSignIn.ImageKey = GSigninTabIcoKey;
            m_tabOptions.ImageKey = GeneralTabIcoKey;
            m_tabAbout.ImageKey = AboutIcoKey;

            base.OnLoad(args);
        }

        // Don't allow the last enabled command to be disabled.
        private void HandleCommandDisabled(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            Debug.Assert(cb != null, "invalid sender");
            if (cb != null && !cb.Checked)
            {
                cb.Checked = !m_chkDownloadEnabled.Checked &&
                    !m_chkUploadEnabled.Checked &&
                    !m_chkSyncEnabled.Checked;
            }
        }

        private void HandleProtectedStringParsing(object sender, ConvertEventArgs e)
        {
            Debug.Assert(e.DesiredType == typeof(ProtectedString));
            Debug.Assert(e.Value == null || e.Value.GetType() == typeof(string));
            string strVal = e.Value as string;
            e.Value = string.IsNullOrEmpty(strVal) ? null :
                new ProtectedString(true, strVal);
        }

        private void HandleProtectedStringFormatting(object sender, ConvertEventArgs e)
        {
            Debug.Assert(e.DesiredType == typeof(string));
            Debug.Assert(e.Value == null || e.Value.GetType() == typeof(ProtectedString));
            e.Value = e.Value == null ? string.Empty :
                ((ProtectedString)e.Value).ReadString();
        }

        private void HandleDefaultFolderValidated(object sender, EventArgs e)
        {
            bool bDefaultFolderSpecified = 
                !string.IsNullOrWhiteSpace(m_txtFolderDefault.Text);
            m_btnGetColors.Enabled = !m_bColorsQueried && bDefaultFolderSpecified;
            m_cbColors.Enabled = !(m_cbColors.Items.Count == 1 &&
                                    m_cbColors.SelectedItem == m_nullColor) &&
                                    bDefaultFolderSpecified;
        }

        private void HandleTabChangeValidation(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == m_tabGSignIn)
            {
                e.Cancel = !EntryClientIdStateIsValid();
            }
            else if (e.TabPage == m_tabOptions)
            {
                e.Cancel = !DefaultClientIdStateIsValid();
            }
            if (e.Cancel)
            {
                MessageBox.Show(Resources.GetFormat("Msg_OauthCredsInvalidFmt",
                                                    GdsDefs.ProductName),
                                GdsDefs.ProductName,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void HandleBoolNegation(object sender,
                                                ConvertEventArgs e)
        {
            e.Value = !((bool)e.Value);
        }

        private void HandleColorsSelectedIndexChanged(object sender,
                                                        EventArgs e)
        {
            GoogleColor item = m_cbColors.Items
                                    .Cast<GoogleColor>()
                                    .ElementAt(m_cbColors.SelectedIndex);
            m_cbColors.BackColor = item.Color;
            m_data.DefaultAppFolderColor = item == m_nullColor ? null : item;
        }

        private async void HandleColorsComboLazyLoad(object sender,
                                                        EventArgs args)
        {
            if (m_bColorsQueried)
            {
                return;
            }

            Cursor current = Cursor;
            Cursor = Cursors.WaitCursor;
            try
            {
                m_cbColors.Items.Clear();
                m_cbColors.Items.Add(m_nullColor);
                IEnumerable<Color> colors = await m_data.GetColors();
                m_cbColors.Items.AddRange(new GoogleColorCollection(colors)
                                                .Cast<object>()
                                                .ToArray());
                m_cbColors.Enabled = true;
                m_cbColors.Focus();
                m_cbColors.DroppedDown = true;

                // If the query succeeds there should be the default item and
                // the returned colors.
                m_bColorsQueried = m_cbColors.Items.Count > 1;
                m_btnGetColors.Enabled = !m_bColorsQueried;
            }
            catch (Exception e)
            {
                Debug.Fail(e.ToString());
                m_cbColors.Items.AddRange(
                    new object[] { new GoogleColor(Color.DimGray) });
            }
            finally
            {
                Cursor = current;
            }
        }

        private void HandleColorsDrawItem(object sender, DrawItemEventArgs e)
        {
            Debug.Assert(ReferenceEquals(sender, m_cbColors));

            ComboBox cb = ((ComboBox)sender);
            if (0 > e.Index || e.Index > cb.Items.Count)
            {
                return;
            }

            // Draw a combo box item using the color as a background.
            Graphics g = e.Graphics;
            Rectangle rect = e.Bounds;

            // Get the color proxy object.
            GoogleColor item = (GoogleColor)cb.Items[e.Index];

            // Background fill color painting.
            Brush brush = new SolidBrush(item.Color);
            g.FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);

            // Foreground is the color name in black.
            g.DrawString(item.ToString(), cb.Font,
                        Brushes.Black, rect.X, rect.Top);
        }

        private void HandleOkClicked(object sender, EventArgs e)
        {
            if (DialogResult != DialogResult.OK)
            {
                return;
            }

            // Validate state of dialog before closing it.
            if (m_tabMain.SelectedTab == m_tabGSignIn &&
                !EntryClientIdStateIsValid())
            {
                DialogResult = DialogResult.None;
            }
            else if (m_tabMain.SelectedTab == m_tabOptions &&
                !DefaultClientIdStateIsValid())
            {
                DialogResult = DialogResult.None;
            }
            if (DialogResult == DialogResult.None)
            {
                MessageBox.Show(
                    Resources.GetFormat("Msg_OauthCredsInvalidFmt",
                                        GdsDefs.ProductName),
                    GdsDefs.ProductName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            // In the case of changing OAuth creds with a refresh token
            // present, confirm the user's intention.
            if  (DialogResult == DialogResult.OK &&
                m_data.SelectedAccountShadow.IsModifiedOauthCreds &&
                !m_data.SelectedAccountShadow.RefreshToken.IsEmpty)
            {
                DialogResult = MessageBox.Show(
                    Resources.GetString("Msg_ChangedCredsDeletesToken"),
                    GdsDefs.ProductName,
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);
                if (DialogResult != DialogResult.OK)
                {
                    DialogResult = DialogResult.None;
                }
            }
        }

        bool EntryClientIdStateIsValid()
        {
            // Unless legacy checked, client & secret must have something.
            return m_chkLegacyClientId.Checked ||
                !string.IsNullOrEmpty(m_txtClientId.Text.Trim()) &&
                !string.IsNullOrEmpty(m_txtClientSecret.Text.Trim());
        }

        bool DefaultClientIdStateIsValid()
        {
            // Unless legacy checked, client & secret must have something.
            return m_chkDefaultLegacyClientId.Checked ||
                !string.IsNullOrEmpty(m_txtDefaultClientId.Text.Trim()) &&
                !string.IsNullOrEmpty(m_txtDefaultClientSecret.Text.Trim());
        }

        public string DatabaseFilePath { get; set; }

        void m_chkDefaultUseLegacyCreds_CheckedChanged(object sender, EventArgs e)
        {
            m_grpDriveAuthDefaults.Enabled = m_chkDefaultUseLegacyCreds.Checked;
        }

        private void m_chkUseLegacyCreds_CheckedChanged(object sender, EventArgs e)
        {
            m_grpDriveAuth.Enabled = m_chkUseLegacyCreds.Checked;
        }
    }
}