namespace KPSyncForDrive
{
    partial class AuthWaitOrCancel
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.lnkHelp = new System.Windows.Forms.LinkLabel();
            this.lblSubTitle = new System.Windows.Forms.Label();
            this.btnCopyUser = new System.Windows.Forms.Button();
            this.btnCopyPassword = new System.Windows.Forms.Button();
            this.m_bannerImage = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(377, 290);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 15;
            this.btnCancel.Text = "Btn_DlgCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // lblMessage
            // 
            this.lblMessage.Location = new System.Drawing.Point(12, 106);
            this.lblMessage.MaximumSize = new System.Drawing.Size(440, 152);
            this.lblMessage.MinimumSize = new System.Drawing.Size(440, 152);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(440, 152);
            this.lblMessage.TabIndex = 16;
            this.lblMessage.Text = "Msg_AuthDialog";
            // 
            // lnkHelp
            // 
            this.lnkHelp.AutoSize = true;
            this.lnkHelp.Location = new System.Drawing.Point(12, 264);
            this.lnkHelp.Name = "lnkHelp";
            this.lnkHelp.Size = new System.Drawing.Size(83, 13);
            this.lnkHelp.TabIndex = 17;
            this.lnkHelp.TabStop = true;
            this.lnkHelp.Text = "Lnk_SignInHelp";
            this.lnkHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkHelp_LinkClicked);
            // 
            // lblSubTitle
            // 
            this.lblSubTitle.AutoSize = true;
            this.lblSubTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubTitle.Location = new System.Drawing.Point(12, 78);
            this.lblSubTitle.Name = "lblSubTitle";
            this.lblSubTitle.Size = new System.Drawing.Size(138, 15);
            this.lblSubTitle.TabIndex = 18;
            this.lblSubTitle.Text = "Title_AuthDialogSub";
            // 
            // btnCopyUser
            // 
            this.btnCopyUser.Location = new System.Drawing.Point(12, 290);
            this.btnCopyUser.Name = "btnCopyUser";
            this.btnCopyUser.Size = new System.Drawing.Size(103, 23);
            this.btnCopyUser.TabIndex = 19;
            this.btnCopyUser.Text = "Btn_CopyUser";
            this.btnCopyUser.UseVisualStyleBackColor = true;
            this.btnCopyUser.Click += new System.EventHandler(this.btnCopyUser_Click);
            // 
            // btnCopyPassword
            // 
            this.btnCopyPassword.Location = new System.Drawing.Point(121, 290);
            this.btnCopyPassword.Name = "btnCopyPassword";
            this.btnCopyPassword.Size = new System.Drawing.Size(103, 23);
            this.btnCopyPassword.TabIndex = 20;
            this.btnCopyPassword.Text = "Btn_CopyPassword";
            this.btnCopyPassword.UseVisualStyleBackColor = true;
            this.btnCopyPassword.Click += new System.EventHandler(this.btnCopyPassword_Click);
            // 
            // m_bannerImage
            // 
            this.m_bannerImage.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_bannerImage.Location = new System.Drawing.Point(0, 0);
            this.m_bannerImage.Name = "m_bannerImage";
            this.m_bannerImage.Size = new System.Drawing.Size(464, 60);
            this.m_bannerImage.TabIndex = 21;
            this.m_bannerImage.TabStop = false;
            // 
            // AuthWaitOrCancel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(464, 326);
            this.Controls.Add(this.m_bannerImage);
            this.Controls.Add(this.btnCopyPassword);
            this.Controls.Add(this.btnCopyUser);
            this.Controls.Add(this.lblSubTitle);
            this.Controls.Add(this.lnkHelp);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AuthWaitOrCancel";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Google Sync Plugin";
            ((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.LinkLabel lnkHelp;
        private System.Windows.Forms.Label lblSubTitle;
        private System.Windows.Forms.Button btnCopyUser;
        private System.Windows.Forms.Button btnCopyPassword;
        private System.Windows.Forms.PictureBox m_bannerImage;
    }
}
