/**
 * Google Sync Plugin for KeePass Password Safe
 * Copyright © 2012-2016  DesignsInnovate
 * Copyright © 2014-2016  Paul Voegler
 * 
 * KeePass Sync for Google Drive
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

namespace KeePassSyncForDrive
{
    partial class SavedAuthWarning
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.m_bannerImage = new System.Windows.Forms.PictureBox();
            this.m_lblMessage = new System.Windows.Forms.Label();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_cbDontShowAgain = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).BeginInit();
            this.SuspendLayout();
            // 
            // m_bannerImage
            // 
            this.m_bannerImage.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_bannerImage.Location = new System.Drawing.Point(0, 0);
            this.m_bannerImage.Name = "m_bannerImage";
            this.m_bannerImage.Size = new System.Drawing.Size(464, 60);
            this.m_bannerImage.TabIndex = 22;
            this.m_bannerImage.TabStop = false;
            // 
            // m_lblMessage
            // 
            this.m_lblMessage.Location = new System.Drawing.Point(12, 74);
            this.m_lblMessage.Name = "m_lblMessage";
            this.m_lblMessage.Size = new System.Drawing.Size(440, 162);
            this.m_lblMessage.TabIndex = 26;
            this.m_lblMessage.Text = "Msg_SavedAuthWarning";
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(377, 250);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 28;
            this.m_btnCancel.Text = "Btn_DlgCancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            // 
            // m_btnOK
            // 
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(296, 250);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 29;
            this.m_btnOK.Text = "Btn_DlgOK";
            this.m_btnOK.UseVisualStyleBackColor = true;
            // 
            // m_cbDontShowAgain
            // 
            this.m_cbDontShowAgain.Location = new System.Drawing.Point(12, 239);
            this.m_cbDontShowAgain.Name = "m_cbDontShowAgain";
            this.m_cbDontShowAgain.Size = new System.Drawing.Size(269, 34);
            this.m_cbDontShowAgain.TabIndex = 32;
            this.m_cbDontShowAgain.Text = "Lbl_DontShowSavedAuthWarning";
            this.m_cbDontShowAgain.UseVisualStyleBackColor = true;
            // 
            // SavedAuthWarning
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(464, 284);
            this.Controls.Add(this.m_lblMessage);
            this.Controls.Add(this.m_cbDontShowAgain);
            this.Controls.Add(this.m_btnOK);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_bannerImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SavedAuthWarning";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Google Sync Plugin";
            ((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox m_bannerImage;
        private System.Windows.Forms.Label m_lblMessage;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Button m_btnOK;
        private System.Windows.Forms.CheckBox m_cbDontShowAgain;
    }
}