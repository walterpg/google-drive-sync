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
            this.components = new System.ComponentModel.Container();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_tabOptions = new System.Windows.Forms.TabPage();
            this.m_grpDriveAuthDefaults = new System.Windows.Forms.GroupBox();
            this.m_lnkGoogle2 = new System.Windows.Forms.LinkLabel();
            this.m_lnkHelp2 = new System.Windows.Forms.LinkLabel();
            this.m_chkDefaultLegacyClientId = new System.Windows.Forms.CheckBox();
            this.m_chkDefaultDriveScope = new System.Windows.Forms.CheckBox();
            this.m_lblDefaultClientSecret = new System.Windows.Forms.Label();
            this.m_lblDefaultClientId = new System.Windows.Forms.Label();
            this.m_txtDefaultClientSecret = new System.Windows.Forms.TextBox();
            this.m_txtDefaultClientId = new System.Windows.Forms.TextBox();
            this.m_grpFolderDefaults = new System.Windows.Forms.GroupBox();
            this.m_btnGetColors = new System.Windows.Forms.Button();
            this.m_cbColors = new System.Windows.Forms.ComboBox();
            this.m_lblHintDefaultFolder = new System.Windows.Forms.Label();
            this.m_txtFolderDefault = new System.Windows.Forms.TextBox();
            this.m_lblDefaultFolderLabel = new System.Windows.Forms.Label();
            this.m_lblDefFolderColor = new System.Windows.Forms.Label();
            this.m_grpAutoSync = new System.Windows.Forms.GroupBox();
            this.m_chkSyncOnSave = new System.Windows.Forms.CheckBox();
            this.m_chkSyncOnOpen = new System.Windows.Forms.CheckBox();
            this.m_tabGSignIn = new System.Windows.Forms.TabPage();
            this.m_grpDriveAuth = new System.Windows.Forms.GroupBox();
            this.m_chkDriveScope = new System.Windows.Forms.CheckBox();
            this.m_lnkGoogle = new System.Windows.Forms.LinkLabel();
            this.m_lnkHelp = new System.Windows.Forms.LinkLabel();
            this.m_lblClientSecret = new System.Windows.Forms.Label();
            this.m_chkLegacyClientId = new System.Windows.Forms.CheckBox();
            this.m_lblClientId = new System.Windows.Forms.Label();
            this.m_txtClientSecret = new System.Windows.Forms.TextBox();
            this.m_txtClientId = new System.Windows.Forms.TextBox();
            this.m_grpEntry = new System.Windows.Forms.GroupBox();
            this.m_cbAccount = new System.Windows.Forms.ComboBox();
            this.m_lblAccount = new System.Windows.Forms.Label();
            this.m_grpDriveOptions = new System.Windows.Forms.GroupBox();
            this.m_lblHintFolder = new System.Windows.Forms.Label();
            this.m_txtFolder = new System.Windows.Forms.TextBox();
            this.m_lblFolder = new System.Windows.Forms.Label();
            this.m_tabMain = new System.Windows.Forms.TabControl();
            this.m_tabAbout = new System.Windows.Forms.TabPage();
            this.m_lnkHome = new System.Windows.Forms.LinkLabel();
            this.m_lblAboutVer = new System.Windows.Forms.Label();
            this.m_lblAboutProd = new System.Windows.Forms.Label();
            this.m_aboutPic = new System.Windows.Forms.PictureBox();
            this.m_imgList = new System.Windows.Forms.ImageList(this.components);
            this.m_toolTipper = new System.Windows.Forms.ToolTip(this.components);
            this.m_bannerImage = new System.Windows.Forms.PictureBox();
            this.m_lblAttribution = new System.Windows.Forms.Label();
            this.m_tabOptions.SuspendLayout();
            this.m_grpDriveAuthDefaults.SuspendLayout();
            this.m_grpFolderDefaults.SuspendLayout();
            this.m_grpAutoSync.SuspendLayout();
            this.m_tabGSignIn.SuspendLayout();
            this.m_grpDriveAuth.SuspendLayout();
            this.m_grpEntry.SuspendLayout();
            this.m_grpDriveOptions.SuspendLayout();
            this.m_tabMain.SuspendLayout();
            this.m_tabAbout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_aboutPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).BeginInit();
            this.SuspendLayout();
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(497, 436);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 102;
            this.m_btnCancel.Text = "Btn_DlgCancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            // 
            // m_btnOK
            // 
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(416, 436);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 100;
            this.m_btnOK.Text = "Btn_DlgOK";
            this.m_btnOK.UseVisualStyleBackColor = true;
            // 
            // m_tabOptions
            // 
            this.m_tabOptions.Controls.Add(this.m_grpDriveAuthDefaults);
            this.m_tabOptions.Controls.Add(this.m_grpFolderDefaults);
            this.m_tabOptions.Controls.Add(this.m_grpAutoSync);
            this.m_tabOptions.Location = new System.Drawing.Point(4, 23);
            this.m_tabOptions.Name = "m_tabOptions";
            this.m_tabOptions.Size = new System.Drawing.Size(552, 337);
            this.m_tabOptions.TabIndex = 3;
            this.m_tabOptions.Text = "Title_DefaultsTab";
            this.m_tabOptions.UseVisualStyleBackColor = true;
            // 
            // m_grpDriveAuthDefaults
            // 
            this.m_grpDriveAuthDefaults.Controls.Add(this.m_lnkGoogle2);
            this.m_grpDriveAuthDefaults.Controls.Add(this.m_lnkHelp2);
            this.m_grpDriveAuthDefaults.Controls.Add(this.m_chkDefaultLegacyClientId);
            this.m_grpDriveAuthDefaults.Controls.Add(this.m_chkDefaultDriveScope);
            this.m_grpDriveAuthDefaults.Controls.Add(this.m_lblDefaultClientSecret);
            this.m_grpDriveAuthDefaults.Controls.Add(this.m_lblDefaultClientId);
            this.m_grpDriveAuthDefaults.Controls.Add(this.m_txtDefaultClientSecret);
            this.m_grpDriveAuthDefaults.Controls.Add(this.m_txtDefaultClientId);
            this.m_grpDriveAuthDefaults.Location = new System.Drawing.Point(6, 86);
            this.m_grpDriveAuthDefaults.Name = "m_grpDriveAuthDefaults";
            this.m_grpDriveAuthDefaults.Size = new System.Drawing.Size(538, 156);
            this.m_grpDriveAuthDefaults.TabIndex = 31;
            this.m_grpDriveAuthDefaults.TabStop = false;
            this.m_grpDriveAuthDefaults.Text = "Group_AuthDefaults";
            // 
            // m_lnkGoogle2
            // 
            this.m_lnkGoogle2.AutoSize = true;
            this.m_lnkGoogle2.Location = new System.Drawing.Point(199, 129);
            this.m_lnkGoogle2.Name = "m_lnkGoogle2";
            this.m_lnkGoogle2.Size = new System.Drawing.Size(85, 13);
            this.m_lnkGoogle2.TabIndex = 25;
            this.m_lnkGoogle2.TabStop = true;
            this.m_lnkGoogle2.Text = "Lnk_GoogleDev";
            // 
            // m_lnkHelp2
            // 
            this.m_lnkHelp2.AutoSize = true;
            this.m_lnkHelp2.Location = new System.Drawing.Point(140, 129);
            this.m_lnkHelp2.Name = "m_lnkHelp2";
            this.m_lnkHelp2.Size = new System.Drawing.Size(53, 13);
            this.m_lnkHelp2.TabIndex = 24;
            this.m_lnkHelp2.TabStop = true;
            this.m_lnkHelp2.Text = "Lnk_Help";
            // 
            // m_chkDefaultLegacyClientId
            // 
            this.m_chkDefaultLegacyClientId.AutoSize = true;
            this.m_chkDefaultLegacyClientId.Location = new System.Drawing.Point(136, 18);
            this.m_chkDefaultLegacyClientId.Name = "m_chkDefaultLegacyClientId";
            this.m_chkDefaultLegacyClientId.Size = new System.Drawing.Size(137, 17);
            this.m_chkDefaultLegacyClientId.TabIndex = 18;
            this.m_chkDefaultLegacyClientId.Text = "Lbl_UseLegacyClientID";
            this.m_chkDefaultLegacyClientId.UseVisualStyleBackColor = true;
            // 
            // m_chkDefaultDriveScope
            // 
            this.m_chkDefaultDriveScope.AutoSize = true;
            this.m_chkDefaultDriveScope.Location = new System.Drawing.Point(136, 41);
            this.m_chkDefaultDriveScope.Name = "m_chkDefaultDriveScope";
            this.m_chkDefaultDriveScope.Size = new System.Drawing.Size(146, 17);
            this.m_chkDefaultDriveScope.TabIndex = 15;
            this.m_chkDefaultDriveScope.Text = "Lbl_UseAppFileApiScope";
            this.m_chkDefaultDriveScope.UseVisualStyleBackColor = true;
            // 
            // m_lblDefaultClientSecret
            // 
            this.m_lblDefaultClientSecret.Location = new System.Drawing.Point(12, 98);
            this.m_lblDefaultClientSecret.Name = "m_lblDefaultClientSecret";
            this.m_lblDefaultClientSecret.Size = new System.Drawing.Size(115, 13);
            this.m_lblDefaultClientSecret.TabIndex = 17;
            this.m_lblDefaultClientSecret.Text = "Lbl_DefaultClientSecret";
            this.m_lblDefaultClientSecret.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // m_lblDefaultClientId
            // 
            this.m_lblDefaultClientId.Location = new System.Drawing.Point(12, 72);
            this.m_lblDefaultClientId.Name = "m_lblDefaultClientId";
            this.m_lblDefaultClientId.Size = new System.Drawing.Size(115, 13);
            this.m_lblDefaultClientId.TabIndex = 16;
            this.m_lblDefaultClientId.Text = "Lbl_DefaultClientID";
            this.m_lblDefaultClientId.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // m_txtDefaultClientSecret
            // 
            this.m_txtDefaultClientSecret.Location = new System.Drawing.Point(136, 95);
            this.m_txtDefaultClientSecret.Name = "m_txtDefaultClientSecret";
            this.m_txtDefaultClientSecret.Size = new System.Drawing.Size(391, 20);
            this.m_txtDefaultClientSecret.TabIndex = 14;
            this.m_txtDefaultClientSecret.UseSystemPasswordChar = true;
            // 
            // m_txtDefaultClientId
            // 
            this.m_txtDefaultClientId.Location = new System.Drawing.Point(136, 69);
            this.m_txtDefaultClientId.Name = "m_txtDefaultClientId";
            this.m_txtDefaultClientId.Size = new System.Drawing.Size(391, 20);
            this.m_txtDefaultClientId.TabIndex = 13;
            // 
            // m_grpFolderDefaults
            // 
            this.m_grpFolderDefaults.Controls.Add(this.m_btnGetColors);
            this.m_grpFolderDefaults.Controls.Add(this.m_cbColors);
            this.m_grpFolderDefaults.Controls.Add(this.m_lblHintDefaultFolder);
            this.m_grpFolderDefaults.Controls.Add(this.m_txtFolderDefault);
            this.m_grpFolderDefaults.Controls.Add(this.m_lblDefaultFolderLabel);
            this.m_grpFolderDefaults.Controls.Add(this.m_lblDefFolderColor);
            this.m_grpFolderDefaults.Location = new System.Drawing.Point(6, 248);
            this.m_grpFolderDefaults.Name = "m_grpFolderDefaults";
            this.m_grpFolderDefaults.Size = new System.Drawing.Size(538, 86);
            this.m_grpFolderDefaults.TabIndex = 32;
            this.m_grpFolderDefaults.TabStop = false;
            this.m_grpFolderDefaults.Text = "Group_FolderDefaults";
            // 
            // m_btnGetColors
            // 
            this.m_btnGetColors.Location = new System.Drawing.Point(311, 49);
            this.m_btnGetColors.Name = "m_btnGetColors";
            this.m_btnGetColors.Size = new System.Drawing.Size(167, 23);
            this.m_btnGetColors.TabIndex = 31;
            this.m_btnGetColors.Text = "Btn_GetColors";
            this.m_btnGetColors.UseVisualStyleBackColor = true;
            // 
            // m_cbColors
            // 
            this.m_cbColors.FormattingEnabled = true;
            this.m_cbColors.Location = new System.Drawing.Point(183, 50);
            this.m_cbColors.Name = "m_cbColors";
            this.m_cbColors.Size = new System.Drawing.Size(122, 21);
            this.m_cbColors.TabIndex = 30;
            // 
            // m_lblHintDefaultFolder
            // 
            this.m_lblHintDefaultFolder.AutoSize = true;
            this.m_lblHintDefaultFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblHintDefaultFolder.Location = new System.Drawing.Point(187, 10);
            this.m_lblHintDefaultFolder.Name = "m_lblHintDefaultFolder";
            this.m_lblHintDefaultFolder.Size = new System.Drawing.Size(140, 13);
            this.m_lblHintDefaultFolder.TabIndex = 29;
            this.m_lblHintDefaultFolder.Text = "Lbl_DefaultTargetFolderHint";
            // 
            // m_txtFolderDefault
            // 
            this.m_txtFolderDefault.Location = new System.Drawing.Point(184, 24);
            this.m_txtFolderDefault.Name = "m_txtFolderDefault";
            this.m_txtFolderDefault.Size = new System.Drawing.Size(294, 20);
            this.m_txtFolderDefault.TabIndex = 25;
            // 
            // m_lblDefaultFolderLabel
            // 
            this.m_lblDefaultFolderLabel.Location = new System.Drawing.Point(12, 27);
            this.m_lblDefaultFolderLabel.Name = "m_lblDefaultFolderLabel";
            this.m_lblDefaultFolderLabel.Size = new System.Drawing.Size(165, 13);
            this.m_lblDefaultFolderLabel.TabIndex = 24;
            this.m_lblDefaultFolderLabel.Text = "Lbl_DefaultTargetFolder";
            this.m_lblDefaultFolderLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // m_lblDefFolderColor
            // 
            this.m_lblDefFolderColor.Location = new System.Drawing.Point(15, 53);
            this.m_lblDefFolderColor.Name = "m_lblDefFolderColor";
            this.m_lblDefFolderColor.Size = new System.Drawing.Size(162, 13);
            this.m_lblDefFolderColor.TabIndex = 26;
            this.m_lblDefFolderColor.Text = "Lbl_DefaultTgtFolderColor";
            this.m_lblDefFolderColor.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // m_grpAutoSync
            // 
            this.m_grpAutoSync.Controls.Add(this.m_chkSyncOnSave);
            this.m_grpAutoSync.Controls.Add(this.m_chkSyncOnOpen);
            this.m_grpAutoSync.Location = new System.Drawing.Point(6, 6);
            this.m_grpAutoSync.Name = "m_grpAutoSync";
            this.m_grpAutoSync.Size = new System.Drawing.Size(538, 74);
            this.m_grpAutoSync.TabIndex = 29;
            this.m_grpAutoSync.TabStop = false;
            this.m_grpAutoSync.Text = "Group_AutoSync";
            // 
            // m_chkSyncOnSave
            // 
            this.m_chkSyncOnSave.AutoSize = true;
            this.m_chkSyncOnSave.Location = new System.Drawing.Point(136, 41);
            this.m_chkSyncOnSave.Name = "m_chkSyncOnSave";
            this.m_chkSyncOnSave.Size = new System.Drawing.Size(131, 17);
            this.m_chkSyncOnSave.TabIndex = 2;
            this.m_chkSyncOnSave.Text = "Lbl_AutoSyncOnSave";
            this.m_chkSyncOnSave.UseVisualStyleBackColor = true;
            // 
            // m_chkSyncOnOpen
            // 
            this.m_chkSyncOnOpen.AutoSize = true;
            this.m_chkSyncOnOpen.Location = new System.Drawing.Point(136, 18);
            this.m_chkSyncOnOpen.Name = "m_chkSyncOnOpen";
            this.m_chkSyncOnOpen.Size = new System.Drawing.Size(132, 17);
            this.m_chkSyncOnOpen.TabIndex = 1;
            this.m_chkSyncOnOpen.Text = "Lbl_AutoSyncOnOpen";
            this.m_chkSyncOnOpen.UseVisualStyleBackColor = true;
            // 
            // m_tabGSignIn
            // 
            this.m_tabGSignIn.Controls.Add(this.m_grpDriveAuth);
            this.m_tabGSignIn.Controls.Add(this.m_grpEntry);
            this.m_tabGSignIn.Controls.Add(this.m_grpDriveOptions);
            this.m_tabGSignIn.Location = new System.Drawing.Point(4, 23);
            this.m_tabGSignIn.Name = "m_tabGSignIn";
            this.m_tabGSignIn.Padding = new System.Windows.Forms.Padding(3);
            this.m_tabGSignIn.Size = new System.Drawing.Size(552, 337);
            this.m_tabGSignIn.TabIndex = 0;
            this.m_tabGSignIn.Text = "Title_SyncAuthTab";
            this.m_tabGSignIn.UseVisualStyleBackColor = true;
            // 
            // m_grpDriveAuth
            // 
            this.m_grpDriveAuth.Controls.Add(this.m_chkDriveScope);
            this.m_grpDriveAuth.Controls.Add(this.m_lnkGoogle);
            this.m_grpDriveAuth.Controls.Add(this.m_lnkHelp);
            this.m_grpDriveAuth.Controls.Add(this.m_lblClientSecret);
            this.m_grpDriveAuth.Controls.Add(this.m_chkLegacyClientId);
            this.m_grpDriveAuth.Controls.Add(this.m_lblClientId);
            this.m_grpDriveAuth.Controls.Add(this.m_txtClientSecret);
            this.m_grpDriveAuth.Controls.Add(this.m_txtClientId);
            this.m_grpDriveAuth.Location = new System.Drawing.Point(6, 65);
            this.m_grpDriveAuth.Name = "m_grpDriveAuth";
            this.m_grpDriveAuth.Size = new System.Drawing.Size(542, 179);
            this.m_grpDriveAuth.TabIndex = 6;
            this.m_grpDriveAuth.TabStop = false;
            this.m_grpDriveAuth.Text = "Group_DriveAuth";
            // 
            // m_chkDriveScope
            // 
            this.m_chkDriveScope.AutoSize = true;
            this.m_chkDriveScope.Location = new System.Drawing.Point(131, 47);
            this.m_chkDriveScope.Name = "m_chkDriveScope";
            this.m_chkDriveScope.Size = new System.Drawing.Size(146, 17);
            this.m_chkDriveScope.TabIndex = 10;
            this.m_chkDriveScope.Text = "Lbl_UseAppFileApiScope";
            this.m_chkDriveScope.UseVisualStyleBackColor = true;
            // 
            // m_lnkGoogle
            // 
            this.m_lnkGoogle.AutoSize = true;
            this.m_lnkGoogle.Location = new System.Drawing.Point(192, 147);
            this.m_lnkGoogle.Name = "m_lnkGoogle";
            this.m_lnkGoogle.Size = new System.Drawing.Size(85, 13);
            this.m_lnkGoogle.TabIndex = 22;
            this.m_lnkGoogle.TabStop = true;
            this.m_lnkGoogle.Text = "Lnk_GoogleDev";
            // 
            // m_lnkHelp
            // 
            this.m_lnkHelp.AutoSize = true;
            this.m_lnkHelp.Location = new System.Drawing.Point(133, 147);
            this.m_lnkHelp.Name = "m_lnkHelp";
            this.m_lnkHelp.Size = new System.Drawing.Size(53, 13);
            this.m_lnkHelp.TabIndex = 20;
            this.m_lnkHelp.TabStop = true;
            this.m_lnkHelp.Text = "Lnk_Help";
            // 
            // m_lblClientSecret
            // 
            this.m_lblClientSecret.Location = new System.Drawing.Point(18, 106);
            this.m_lblClientSecret.Name = "m_lblClientSecret";
            this.m_lblClientSecret.Size = new System.Drawing.Size(104, 13);
            this.m_lblClientSecret.TabIndex = 12;
            this.m_lblClientSecret.Text = "Lbl_ClientSecret";
            this.m_lblClientSecret.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // m_chkLegacyClientId
            // 
            this.m_chkLegacyClientId.AutoSize = true;
            this.m_chkLegacyClientId.Location = new System.Drawing.Point(131, 24);
            this.m_chkLegacyClientId.Name = "m_chkLegacyClientId";
            this.m_chkLegacyClientId.Size = new System.Drawing.Size(137, 17);
            this.m_chkLegacyClientId.TabIndex = 12;
            this.m_chkLegacyClientId.Text = "Lbl_UseLegacyClientID";
            this.m_chkLegacyClientId.UseVisualStyleBackColor = true;
            // 
            // m_lblClientId
            // 
            this.m_lblClientId.Location = new System.Drawing.Point(18, 80);
            this.m_lblClientId.Name = "m_lblClientId";
            this.m_lblClientId.Size = new System.Drawing.Size(104, 13);
            this.m_lblClientId.TabIndex = 10;
            this.m_lblClientId.Text = "Lbl_ClientID";
            this.m_lblClientId.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // m_txtClientSecret
            // 
            this.m_txtClientSecret.Location = new System.Drawing.Point(131, 103);
            this.m_txtClientSecret.Name = "m_txtClientSecret";
            this.m_txtClientSecret.Size = new System.Drawing.Size(391, 20);
            this.m_txtClientSecret.TabIndex = 8;
            this.m_txtClientSecret.UseSystemPasswordChar = true;
            // 
            // m_txtClientId
            // 
            this.m_txtClientId.Location = new System.Drawing.Point(131, 77);
            this.m_txtClientId.Name = "m_txtClientId";
            this.m_txtClientId.Size = new System.Drawing.Size(391, 20);
            this.m_txtClientId.TabIndex = 6;
            // 
            // m_grpEntry
            // 
            this.m_grpEntry.Controls.Add(this.m_cbAccount);
            this.m_grpEntry.Controls.Add(this.m_lblAccount);
            this.m_grpEntry.Location = new System.Drawing.Point(6, 6);
            this.m_grpEntry.Name = "m_grpEntry";
            this.m_grpEntry.Size = new System.Drawing.Size(540, 53);
            this.m_grpEntry.TabIndex = 9;
            this.m_grpEntry.TabStop = false;
            this.m_grpEntry.Text = "Group_Entry";
            // 
            // m_cbAccount
            // 
            this.m_cbAccount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_cbAccount.FormattingEnabled = true;
            this.m_cbAccount.Location = new System.Drawing.Point(131, 19);
            this.m_cbAccount.Name = "m_cbAccount";
            this.m_cbAccount.Size = new System.Drawing.Size(391, 21);
            this.m_cbAccount.TabIndex = 4;
            // 
            // m_lblAccount
            // 
            this.m_lblAccount.Location = new System.Drawing.Point(18, 22);
            this.m_lblAccount.Name = "m_lblAccount";
            this.m_lblAccount.Size = new System.Drawing.Size(104, 13);
            this.m_lblAccount.TabIndex = 5;
            this.m_lblAccount.Text = "Lbl_AuthPwEntry";
            this.m_lblAccount.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // m_grpDriveOptions
            // 
            this.m_grpDriveOptions.Controls.Add(this.m_lblHintFolder);
            this.m_grpDriveOptions.Controls.Add(this.m_txtFolder);
            this.m_grpDriveOptions.Controls.Add(this.m_lblFolder);
            this.m_grpDriveOptions.Location = new System.Drawing.Point(6, 250);
            this.m_grpDriveOptions.Name = "m_grpDriveOptions";
            this.m_grpDriveOptions.Size = new System.Drawing.Size(542, 82);
            this.m_grpDriveOptions.TabIndex = 8;
            this.m_grpDriveOptions.TabStop = false;
            this.m_grpDriveOptions.Text = "Group_DriveOptions";
            // 
            // m_lblHintFolder
            // 
            this.m_lblHintFolder.AutoSize = true;
            this.m_lblHintFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblHintFolder.Location = new System.Drawing.Point(128, 19);
            this.m_lblHintFolder.Name = "m_lblHintFolder";
            this.m_lblHintFolder.Size = new System.Drawing.Size(140, 13);
            this.m_lblHintFolder.TabIndex = 30;
            this.m_lblHintFolder.Text = "Lbl_DefaultTargetFolderHint";
            // 
            // m_txtFolder
            // 
            this.m_txtFolder.Location = new System.Drawing.Point(131, 35);
            this.m_txtFolder.Name = "m_txtFolder";
            this.m_txtFolder.Size = new System.Drawing.Size(289, 20);
            this.m_txtFolder.TabIndex = 30;
            // 
            // m_lblFolder
            // 
            this.m_lblFolder.Location = new System.Drawing.Point(18, 38);
            this.m_lblFolder.Name = "m_lblFolder";
            this.m_lblFolder.Size = new System.Drawing.Size(104, 13);
            this.m_lblFolder.TabIndex = 22;
            this.m_lblFolder.Text = "Lbl_TargetFolder";
            this.m_lblFolder.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // m_tabMain
            // 
            this.m_tabMain.Controls.Add(this.m_tabGSignIn);
            this.m_tabMain.Controls.Add(this.m_tabOptions);
            this.m_tabMain.Controls.Add(this.m_tabAbout);
            this.m_tabMain.ImageList = this.m_imgList;
            this.m_tabMain.Location = new System.Drawing.Point(12, 66);
            this.m_tabMain.Name = "m_tabMain";
            this.m_tabMain.SelectedIndex = 0;
            this.m_tabMain.Size = new System.Drawing.Size(560, 364);
            this.m_tabMain.TabIndex = 1;
            // 
            // m_tabAbout
            // 
            this.m_tabAbout.Controls.Add(this.m_lblAttribution);
            this.m_tabAbout.Controls.Add(this.m_lnkHome);
            this.m_tabAbout.Controls.Add(this.m_lblAboutVer);
            this.m_tabAbout.Controls.Add(this.m_lblAboutProd);
            this.m_tabAbout.Controls.Add(this.m_aboutPic);
            this.m_tabAbout.Location = new System.Drawing.Point(4, 23);
            this.m_tabAbout.Name = "m_tabAbout";
            this.m_tabAbout.Size = new System.Drawing.Size(552, 337);
            this.m_tabAbout.TabIndex = 4;
            this.m_tabAbout.Text = "Title_AboutTab";
            this.m_tabAbout.UseVisualStyleBackColor = true;
            // 
            // m_lnkHome
            // 
            this.m_lnkHome.AutoSize = true;
            this.m_lnkHome.Location = new System.Drawing.Point(326, 184);
            this.m_lnkHome.Name = "m_lnkHome";
            this.m_lnkHome.Size = new System.Drawing.Size(59, 13);
            this.m_lnkHome.TabIndex = 19;
            this.m_lnkHome.TabStop = true;
            this.m_lnkHome.Text = "Lnk_Home";
            // 
            // m_lblAboutVer
            // 
            this.m_lblAboutVer.AutoSize = true;
            this.m_lblAboutVer.Location = new System.Drawing.Point(326, 148);
            this.m_lblAboutVer.Name = "m_lblAboutVer";
            this.m_lblAboutVer.Size = new System.Drawing.Size(50, 13);
            this.m_lblAboutVer.TabIndex = 2;
            this.m_lblAboutVer.Text = "vX.X.X.X";
            // 
            // m_lblAboutProd
            // 
            this.m_lblAboutProd.AutoSize = true;
            this.m_lblAboutProd.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblAboutProd.Location = new System.Drawing.Point(325, 110);
            this.m_lblAboutProd.Name = "m_lblAboutProd";
            this.m_lblAboutProd.Size = new System.Drawing.Size(147, 20);
            this.m_lblAboutProd.TabIndex = 1;
            this.m_lblAboutProd.Text = "Google Sync Plugin";
            // 
            // m_aboutPic
            // 
            this.m_aboutPic.Image = global::GoogleSyncPlugin.Images.gdsync;
            this.m_aboutPic.InitialImage = null;
            this.m_aboutPic.Location = new System.Drawing.Point(39, 40);
            this.m_aboutPic.Name = "m_aboutPic";
            this.m_aboutPic.Size = new System.Drawing.Size(255, 255);
            this.m_aboutPic.TabIndex = 0;
            this.m_aboutPic.TabStop = false;
            // 
            // m_imgList
            // 
            this.m_imgList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.m_imgList.ImageSize = new System.Drawing.Size(16, 16);
            this.m_imgList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // m_bannerImage
            // 
            this.m_bannerImage.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_bannerImage.Location = new System.Drawing.Point(0, 0);
            this.m_bannerImage.Name = "m_bannerImage";
            this.m_bannerImage.Size = new System.Drawing.Size(580, 60);
            this.m_bannerImage.TabIndex = 1;
            this.m_bannerImage.TabStop = false;
            // 
            // m_lblAttribution
            // 
            this.m_lblAttribution.Cursor = System.Windows.Forms.Cursors.Hand;
            this.m_lblAttribution.Location = new System.Drawing.Point(326, 241);
            this.m_lblAttribution.Name = "m_lblAttribution";
            this.m_lblAttribution.Size = new System.Drawing.Size(215, 54);
            this.m_lblAttribution.TabIndex = 20;
            this.m_lblAttribution.Text = "Lbl_LegacyAttribution";
            // 
            // ConfigurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(580, 468);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOK);
            this.Controls.Add(this.m_tabMain);
            this.Controls.Add(this.m_bannerImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigurationForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Google Sync Plugin";
            this.m_tabOptions.ResumeLayout(false);
            this.m_grpDriveAuthDefaults.ResumeLayout(false);
            this.m_grpDriveAuthDefaults.PerformLayout();
            this.m_grpFolderDefaults.ResumeLayout(false);
            this.m_grpFolderDefaults.PerformLayout();
            this.m_grpAutoSync.ResumeLayout(false);
            this.m_grpAutoSync.PerformLayout();
            this.m_tabGSignIn.ResumeLayout(false);
            this.m_grpDriveAuth.ResumeLayout(false);
            this.m_grpDriveAuth.PerformLayout();
            this.m_grpEntry.ResumeLayout(false);
            this.m_grpDriveOptions.ResumeLayout(false);
            this.m_grpDriveOptions.PerformLayout();
            this.m_tabMain.ResumeLayout(false);
            this.m_tabAbout.ResumeLayout(false);
            this.m_tabAbout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_aboutPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox m_bannerImage;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Button m_btnOK;
        private System.Windows.Forms.TabPage m_tabOptions;
        private System.Windows.Forms.TabPage m_tabGSignIn;
        private System.Windows.Forms.TabControl m_tabMain;
        private System.Windows.Forms.ComboBox m_cbAccount;
        private System.Windows.Forms.GroupBox m_grpDriveAuth;
        private System.Windows.Forms.Label m_lblAccount;
        private System.Windows.Forms.CheckBox m_chkLegacyClientId;
        private System.Windows.Forms.Label m_lblClientSecret;
        private System.Windows.Forms.Label m_lblClientId;
        private System.Windows.Forms.TextBox m_txtClientSecret;
        private System.Windows.Forms.TextBox m_txtClientId;
        private System.Windows.Forms.GroupBox m_grpDriveOptions;
        private System.Windows.Forms.LinkLabel m_lnkHelp;
        private System.Windows.Forms.LinkLabel m_lnkGoogle;
        private System.Windows.Forms.TextBox m_txtFolder;
        private System.Windows.Forms.Label m_lblFolder;
        private System.Windows.Forms.Label m_lblDefFolderColor;
        private System.Windows.Forms.Label m_lblDefaultFolderLabel;
        private System.Windows.Forms.TextBox m_txtFolderDefault;
        private System.Windows.Forms.GroupBox m_grpAutoSync;
        private System.Windows.Forms.GroupBox m_grpDriveAuthDefaults;
        private System.Windows.Forms.GroupBox m_grpFolderDefaults;
        private System.Windows.Forms.Label m_lblHintDefaultFolder;
        private System.Windows.Forms.CheckBox m_chkSyncOnSave;
        private System.Windows.Forms.CheckBox m_chkSyncOnOpen;
        private System.Windows.Forms.Label m_lblHintFolder;
        private System.Windows.Forms.ImageList m_imgList;
        private System.Windows.Forms.CheckBox m_chkDriveScope;
        private System.Windows.Forms.ComboBox m_cbColors;
        private System.Windows.Forms.Button m_btnGetColors;
        private System.Windows.Forms.CheckBox m_chkDefaultDriveScope;
        private System.Windows.Forms.Label m_lblDefaultClientSecret;
        private System.Windows.Forms.CheckBox m_chkDefaultLegacyClientId;
        private System.Windows.Forms.Label m_lblDefaultClientId;
        private System.Windows.Forms.TextBox m_txtDefaultClientSecret;
        private System.Windows.Forms.TextBox m_txtDefaultClientId;
        private System.Windows.Forms.LinkLabel m_lnkGoogle2;
        private System.Windows.Forms.LinkLabel m_lnkHelp2;
        private System.Windows.Forms.ToolTip m_toolTipper;
        private System.Windows.Forms.GroupBox m_grpEntry;
        private System.Windows.Forms.TabPage m_tabAbout;
        private System.Windows.Forms.PictureBox m_aboutPic;
        private System.Windows.Forms.LinkLabel m_lnkHome;
        private System.Windows.Forms.Label m_lblAboutVer;
        private System.Windows.Forms.Label m_lblAboutProd;
        private System.Windows.Forms.Label m_lblAttribution;
    }
}