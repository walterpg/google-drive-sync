namespace KeePassSyncForDrive
{
    partial class AppCredsUpgrade
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
            this.rbNewBuiltIn = new System.Windows.Forms.RadioButton();
            this.rbNoCredsUpgrade = new System.Windows.Forms.RadioButton();
            this.lblMessage = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lnkMoreInfo = new System.Windows.Forms.LinkLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lnkPrivacy = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).BeginInit();
            this.groupBox1.SuspendLayout();
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
            // rbNewBuiltIn
            // 
            this.rbNewBuiltIn.AutoSize = true;
            this.rbNewBuiltIn.Location = new System.Drawing.Point(33, 76);
            this.rbNewBuiltIn.Name = "rbNewBuiltIn";
            this.rbNewBuiltIn.Size = new System.Drawing.Size(156, 17);
            this.rbNewBuiltIn.TabIndex = 24;
            this.rbNewBuiltIn.TabStop = true;
            this.rbNewBuiltIn.Text = "Btn_UseNewCredsUpgrade";
            this.rbNewBuiltIn.UseVisualStyleBackColor = true;
            // 
            // rbNoCredsUpgrade
            // 
            this.rbNoCredsUpgrade.AutoSize = true;
            this.rbNoCredsUpgrade.Location = new System.Drawing.Point(33, 105);
            this.rbNoCredsUpgrade.Name = "rbNoCredsUpgrade";
            this.rbNoCredsUpgrade.Size = new System.Drawing.Size(183, 17);
            this.rbNoCredsUpgrade.TabIndex = 25;
            this.rbNoCredsUpgrade.TabStop = true;
            this.rbNoCredsUpgrade.Text = "Btn_UseLegacyCredsDowngrade";
            this.rbNoCredsUpgrade.UseVisualStyleBackColor = true;
            // 
            // lblMessage
            // 
            this.lblMessage.Location = new System.Drawing.Point(6, 16);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(425, 116);
            this.lblMessage.TabIndex = 26;
            this.lblMessage.Text = "Msg_AppCredsUpgrade";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(377, 320);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 28;
            this.btnCancel.Text = "Btn_DlgCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(296, 320);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 29;
            this.btnOK.Text = "Btn_DlgOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lnkMoreInfo
            // 
            this.lnkMoreInfo.AutoSize = true;
            this.lnkMoreInfo.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lnkMoreInfo.Location = new System.Drawing.Point(6, 166);
            this.lnkMoreInfo.Name = "lnkMoreInfo";
            this.lnkMoreInfo.Size = new System.Drawing.Size(140, 13);
            this.lnkMoreInfo.TabIndex = 30;
            this.lnkMoreInfo.TabStop = true;
            this.lnkMoreInfo.Text = "Lnk_HelpAppCredsUpgrade";
            this.lnkMoreInfo.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkMoreInfo_LinkClicked);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lnkPrivacy);
            this.groupBox1.Controls.Add(this.lnkMoreInfo);
            this.groupBox1.Controls.Add(this.lblMessage);
            this.groupBox1.Location = new System.Drawing.Point(15, 128);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(437, 186);
            this.groupBox1.TabIndex = 31;
            this.groupBox1.TabStop = false;
            // 
            // lnkPrivacy
            // 
            this.lnkPrivacy.AutoSize = true;
            this.lnkPrivacy.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lnkPrivacy.Location = new System.Drawing.Point(6, 141);
            this.lnkPrivacy.Name = "lnkPrivacy";
            this.lnkPrivacy.Size = new System.Drawing.Size(134, 13);
            this.lnkPrivacy.TabIndex = 31;
            this.lnkPrivacy.TabStop = true;
            this.lnkPrivacy.Text = "Lnk_HelpAppCredsPrivacy";
            this.lnkPrivacy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkPrivacy_LinkClicked);
            // 
            // AppCredsUpgrade
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(464, 351);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.rbNoCredsUpgrade);
            this.Controls.Add(this.rbNewBuiltIn);
            this.Controls.Add(this.m_bannerImage);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AppCredsUpgrade";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Google Sync Plugin";
            ((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox m_bannerImage;
        private System.Windows.Forms.RadioButton rbNewBuiltIn;
        private System.Windows.Forms.RadioButton rbNoCredsUpgrade;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.LinkLabel lnkMoreInfo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.LinkLabel lnkPrivacy;
    }
}