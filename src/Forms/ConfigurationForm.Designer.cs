namespace GoogleSyncPlugin
{
	partial class ConfigurationForm
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
			this.txtClientId = new System.Windows.Forms.TextBox();
			this.lnkGoogle = new System.Windows.Forms.LinkLabel();
			this.lblTitle = new System.Windows.Forms.Label();
			this.txtClientSecret = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.btnOk = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.txtUuid = new System.Windows.Forms.TextBox();
			this.cbAccount = new System.Windows.Forms.ComboBox();
			this.chkAutoSync = new System.Windows.Forms.CheckBox();
			this.lnkHelp = new System.Windows.Forms.LinkLabel();
			this.lblVersion = new System.Windows.Forms.Label();
			this.lnkHome = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			//
			// txtClientId
			//
			this.txtClientId.Location = new System.Drawing.Point(112, 102);
			this.txtClientId.Name = "txtClientId";
			this.txtClientId.Size = new System.Drawing.Size(340, 20);
			this.txtClientId.TabIndex = 7;
			//
			// lnkGoogle
			//
			this.lnkGoogle.AutoSize = true;
			this.lnkGoogle.Location = new System.Drawing.Point(88, 192);
			this.lnkGoogle.Name = "lnkGoogle";
			this.lnkGoogle.Size = new System.Drawing.Size(134, 13);
			this.lnkGoogle.TabIndex = 13;
			this.lnkGoogle.TabStop = true;
			this.lnkGoogle.Text = "Google Developer Console";
			this.lnkGoogle.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkGoogle_LinkClicked);
			//
			// lblTitle
			//
			this.lblTitle.AutoSize = true;
			this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblTitle.Location = new System.Drawing.Point(12, 13);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(239, 16);
			this.lblTitle.TabIndex = 0;
			this.lblTitle.Text = "Google Sync Plugin Configuration";
			//
			// txtClientSecret
			//
			this.txtClientSecret.Location = new System.Drawing.Point(112, 128);
			this.txtClientSecret.Name = "txtClientSecret";
			this.txtClientSecret.Size = new System.Drawing.Size(340, 20);
			this.txtClientSecret.TabIndex = 9;
			//
			// label2
			//
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 105);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(62, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "CLIENT ID:";
			//
			// label3
			//
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 131);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(94, 13);
			this.label3.TabIndex = 8;
			this.label3.Text = "CLIENT SECRET:";
			//
			// btnOk
			//
			this.btnOk.Location = new System.Drawing.Point(377, 187);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 11;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			//
			// label4
			//
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 52);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(62, 13);
			this.label4.TabIndex = 2;
			this.label4.Text = "ACCOUNT:";
			//
			// btnCancel
			//
			this.btnCancel.Location = new System.Drawing.Point(296, 187);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 12;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			//
			// label5
			//
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 79);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(82, 13);
			this.label5.TabIndex = 4;
			this.label5.Text = "KeePass UUID:";
			//
			// txtUuid
			//
			this.txtUuid.Location = new System.Drawing.Point(112, 76);
			this.txtUuid.Name = "txtUuid";
			this.txtUuid.Size = new System.Drawing.Size(340, 20);
			this.txtUuid.TabIndex = 5;
			//
			// cbAccount
			//
			this.cbAccount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbAccount.FormattingEnabled = true;
			this.cbAccount.Location = new System.Drawing.Point(112, 49);
			this.cbAccount.Name = "cbAccount";
			this.cbAccount.Size = new System.Drawing.Size(340, 21);
			this.cbAccount.TabIndex = 3;
			this.cbAccount.SelectedIndexChanged += new System.EventHandler(this.cbAccount_SelectedIndexChanged);
			//
			// chkAutoSync
			//
			this.chkAutoSync.AutoSize = true;
			this.chkAutoSync.Location = new System.Drawing.Point(112, 154);
			this.chkAutoSync.Name = "chkAutoSync";
			this.chkAutoSync.Size = new System.Drawing.Size(118, 17);
			this.chkAutoSync.TabIndex = 10;
			this.chkAutoSync.Text = "Auto Sync on Save";
			this.chkAutoSync.UseVisualStyleBackColor = true;
			//
			// lnkHelp
			//
			this.lnkHelp.AutoSize = true;
			this.lnkHelp.Location = new System.Drawing.Point(53, 192);
			this.lnkHelp.Name = "lnkHelp";
			this.lnkHelp.Size = new System.Drawing.Size(29, 13);
			this.lnkHelp.TabIndex = 14;
			this.lnkHelp.TabStop = true;
			this.lnkHelp.Text = "Help";
			this.lnkHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkHelp_LinkClicked);
			//
			// lblVersion
			//
			this.lblVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblVersion.Location = new System.Drawing.Point(377, 13);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(75, 16);
			this.lblVersion.TabIndex = 1;
			this.lblVersion.Text = "v2.0";
			this.lblVersion.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.lblVersion.DoubleClick += new System.EventHandler(this.lblVersion_DoubleClick);
			//
			// lnkHome
			//
			this.lnkHome.AutoSize = true;
			this.lnkHome.Location = new System.Drawing.Point(12, 192);
			this.lnkHome.Name = "lnkHome";
			this.lnkHome.Size = new System.Drawing.Size(35, 13);
			this.lnkHome.TabIndex = 15;
			this.lnkHome.TabStop = true;
			this.lnkHome.Text = "Home";
			this.lnkHome.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkHome_LinkClicked);
			//
			// ConfigurationForm
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(464, 222);
			this.Controls.Add(this.lnkHome);
			this.Controls.Add(this.lblVersion);
			this.Controls.Add(this.lnkHelp);
			this.Controls.Add(this.chkAutoSync);
			this.Controls.Add(this.cbAccount);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.txtUuid);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtClientSecret);
			this.Controls.Add(this.lblTitle);
			this.Controls.Add(this.lnkGoogle);
			this.Controls.Add(this.txtClientId);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "ConfigurationForm";
			this.ShowIcon = false;
			this.Text = "Google Sync Plugin";
			this.Load += new System.EventHandler(this.GoogleOAuthCredentialsForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtClientId;
		private System.Windows.Forms.LinkLabel lnkGoogle;
		private System.Windows.Forms.Label lblTitle;
		private System.Windows.Forms.TextBox txtClientSecret;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtUuid;
		private System.Windows.Forms.ComboBox cbAccount;
		private System.Windows.Forms.CheckBox chkAutoSync;
		private System.Windows.Forms.LinkLabel lnkHelp;
		private System.Windows.Forms.Label lblVersion;
		private System.Windows.Forms.LinkLabel lnkHome;
	}
}