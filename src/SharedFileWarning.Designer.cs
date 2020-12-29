namespace KeePassSyncForDrive
{
    partial class SharedFileWarning
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.cbDontShowAgain = new System.Windows.Forms.CheckBox();
            this.lnkPersonalOauth = new System.Windows.Forms.LinkLabel();
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
            this.lblSubTitle.Text = "SubTitle_SharedFileWarning";
            this.lblSubTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblMessage
            // 
            this.lblMessage.Location = new System.Drawing.Point(12, 116);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(440, 133);
            this.lblMessage.TabIndex = 24;
            this.lblMessage.Text = "Msg_SharedFileWarning";
            // 
            // lnkHelp
            // 
            this.lnkHelp.AutoSize = true;
            this.lnkHelp.Location = new System.Drawing.Point(12, 264);
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
            this.btnOK.Location = new System.Drawing.Point(296, 307);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 31;
            this.btnOK.Text = "Btn_DlgOK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(377, 307);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 30;
            this.btnCancel.Text = "Btn_DlgCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // cbDontShowAgain
            // 
            this.cbDontShowAgain.Location = new System.Drawing.Point(15, 291);
            this.cbDontShowAgain.Name = "cbDontShowAgain";
            this.cbDontShowAgain.Size = new System.Drawing.Size(275, 39);
            this.cbDontShowAgain.TabIndex = 32;
            this.cbDontShowAgain.Text = "Lbl_DontWarnAboutSharedFileAgain";
            this.cbDontShowAgain.UseVisualStyleBackColor = true;
            // 
            // lnkPersonalOauth
            // 
            this.lnkPersonalOauth.AutoSize = true;
            this.lnkPersonalOauth.Location = new System.Drawing.Point(141, 264);
            this.lnkPersonalOauth.Name = "lnkPersonalOauth";
            this.lnkPersonalOauth.Size = new System.Drawing.Size(101, 13);
            this.lnkPersonalOauth.TabIndex = 33;
            this.lnkPersonalOauth.TabStop = true;
            this.lnkPersonalOauth.Text = "Lnk_PersonalOauth";
            this.lnkPersonalOauth.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkPersonalOauth_LinkClicked);
            // 
            // SharedFileWarning
            // 
            this.AcceptButton = this.btnCancel;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 342);
            this.Controls.Add(this.lnkPersonalOauth);
            this.Controls.Add(this.cbDontShowAgain);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lnkHelp);
            this.Controls.Add(this.lblSubTitle);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.m_bannerImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SharedFileWarning";
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
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox cbDontShowAgain;
        private System.Windows.Forms.LinkLabel lnkPersonalOauth;
    }
}