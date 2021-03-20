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

namespace KPSyncForDrive
{
    partial class SharedFileError
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
            this.lblSubTitle = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.lnkHelp = new System.Windows.Forms.LinkLabel();
            this.btnOK = new System.Windows.Forms.Button();
            this.lnkPersonalOauth = new System.Windows.Forms.LinkLabel();
            this.lnkSessionAuthTokens = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).BeginInit();
            this.SuspendLayout();
            // 
            // m_bannerImage
            // 
            this.m_bannerImage.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_bannerImage.Location = new System.Drawing.Point(0, 0);
            this.m_bannerImage.Name = "m_bannerImage";
            this.m_bannerImage.Size = new System.Drawing.Size(464, 60);
            this.m_bannerImage.TabIndex = 23;
            this.m_bannerImage.TabStop = false;
            // 
            // lblSubTitle
            // 
            this.lblSubTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubTitle.Location = new System.Drawing.Point(12, 63);
            this.lblSubTitle.Name = "lblSubTitle";
            this.lblSubTitle.Size = new System.Drawing.Size(440, 41);
            this.lblSubTitle.TabIndex = 25;
            this.lblSubTitle.Text = "SubTitle_SharedFileError";
            this.lblSubTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblMessage
            // 
            this.lblMessage.Location = new System.Drawing.Point(12, 104);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(440, 132);
            this.lblMessage.TabIndex = 24;
            this.lblMessage.Text = "Msg_SharedFileError";
            // 
            // lnkHelp
            // 
            this.lnkHelp.AutoSize = true;
            this.lnkHelp.Location = new System.Drawing.Point(12, 289);
            this.lnkHelp.Name = "lnkHelp";
            this.lnkHelp.Size = new System.Drawing.Size(103, 13);
            this.lnkHelp.TabIndex = 26;
            this.lnkHelp.TabStop = true;
            this.lnkHelp.Text = "Lnk_SharedFileHelp";
            this.lnkHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkHelp_LinkClicked);
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(377, 290);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 31;
            this.btnOK.Text = "Btn_DlgOK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // lnkPersonalOauth
            // 
            this.lnkPersonalOauth.AutoSize = true;
            this.lnkPersonalOauth.Location = new System.Drawing.Point(12, 268);
            this.lnkPersonalOauth.Name = "lnkPersonalOauth";
            this.lnkPersonalOauth.Size = new System.Drawing.Size(101, 13);
            this.lnkPersonalOauth.TabIndex = 33;
            this.lnkPersonalOauth.TabStop = true;
            this.lnkPersonalOauth.Text = "Lnk_PersonalOauth";
            this.lnkPersonalOauth.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkPersonalOauth_LinkClicked);
            // 
            // lnkSessionAuthTokens
            // 
            this.lnkSessionAuthTokens.AutoSize = true;
            this.lnkSessionAuthTokens.Location = new System.Drawing.Point(12, 247);
            this.lnkSessionAuthTokens.Name = "lnkSessionAuthTokens";
            this.lnkSessionAuthTokens.Size = new System.Drawing.Size(126, 13);
            this.lnkSessionAuthTokens.TabIndex = 34;
            this.lnkSessionAuthTokens.TabStop = true;
            this.lnkSessionAuthTokens.Text = "Lnk_SessionAuthTokens";
            this.lnkSessionAuthTokens.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkSessionAuthTokens_LinkClicked);
            // 
            // SharedFileError
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 323);
            this.Controls.Add(this.lnkSessionAuthTokens);
            this.Controls.Add(this.lnkPersonalOauth);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lnkHelp);
            this.Controls.Add(this.lblSubTitle);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.m_bannerImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SharedFileError";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Google Sync Plugin";
            ((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox m_bannerImage;
        private System.Windows.Forms.Label lblSubTitle;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.LinkLabel lnkHelp;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.LinkLabel lnkPersonalOauth;
        private System.Windows.Forms.LinkLabel lnkSessionAuthTokens;
    }
}
