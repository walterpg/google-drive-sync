Googly Sync Plugin for KeePass Password Safe
============================================

Version: 2.1.2 (August 2015)
Authors: Danyal (DesignsInnovate), Paul Voegler
License: GPL version 3 (see gpl.txt)
Project: http://sourceforge.net/p/kp-googlesync


This is a Plugin for KeePass Password Safe >= v2.18. It allows to synchronize
the currently open database with a Google Drive Account.

It supports three modes of operation:

* Synchronize: The remote version of the open database is downloaded to a
  temporary file in the same directory as the open database. The remote version
  is then merged into the open database by KeePass' "Synchronize with File"
  feature. The updated database is saved and uploaded to the Google Drive,
  replacing the old version.
* Upload: The open database is uploaded to the Google Drive creating a new file
  or replacing an existing version.
* Download: The remote version of the open database is downloaded replacing the
  open database.

The "Synchronize" mode can be configured to run automatically when an open
database is saved, a database is opened or both. Databases which have no
matching configuration will be ignored.

The automatic syncing can be suppressed by holding the Shift-key.

The Plugin matches the remote file by the file name of the open database.
The remote file can be moved to any folder within the Google Drive, but the
file name must be unique within the Drive and match the local file name.


Upgrade from v1.x
-----------------

Version 2.x of this Plugin is not compatible with the configuration of previous
1.x versions.

Discussion:
http://sourceforge.net/p/kp-googlesync/discussion/general/thread/81f69f55

When upgrading from version 1.x you should first remove all old configuration
settings and revoke the API access:

1. Exit and Close KeePass. Make sure it is not running at all (in case it is
	minimized to tray instead of taskbar).
2. Locate your keepass.config.xml.
   * e.g. C:\Users[User]\AppData\Roaming\KeePass
   * For portable versions, look in the same directory as the KeePass.exe
     executable
3. Edit your keepass.config.xml file and remove all <Item> entries in the
   <Custom> group for the old Plugin. Those are items with <Key>
   "EnableAutoSync", "GoogleSyncKeePassUID", "GoogleSyncClientID"
   and "GoogleSyncClientSecret". Delete from <Item> to </Item> (including).
4. Save your changes to keepass.config.xml and close the file.
5. Go to https://security.google.com/settings/security/permissions
6. Select your KeePass Google Sync Plugin from the list of Account Permissions.
   It may be called "KeePassPlugin" or whatever you named it initially.
7. Click on "Revoke access".


Requirements
------------

The Plugin requires the .NET Framework 4.0
(the .NET Client Profile does not suffice).

The Plugin requires OAuth 2.0 credentials to access the Google Drive API.
Starting with v2.1 it has it's own credentials built in, but still allows you
to provide your own as with previous versions.

In case you would like to provide your own OAuth 2.0 credentials, you can
create a Project with the Google Developers Console:

1. Go to the Google Developers Console in your Google Account:
   https://console.developers.google.com/start
2. Select "Enable Google APIs for use in your apps".
3. Choose a Project name like e.g.: "KeePass Google Sync Plugin".
4. Enable the "Drive API" at "APIs & auth" > "APIs".
5. Choose an e-mail address at "APIs & auth" > "Consent screen".
6. Set the product name for the consent screen to e.g.: "KeePass Google Sync Plugin".
7. Create a new OAuth 2.0 Client ID for an "Installed application" with
   application type "Other" at "APIs & auth" > "Credentials".

You should now have a "CLIENT ID" and "CLIENT SECRET".


Installation
------------

1. Exit and Close KeePass. Make sure it is not running at all (in case it is
   minimized to tray instead of taskbar).
2. Place the "GoogleSyncPlugin.plgx" file from the downloaded .zip into your
   KeePass installation directory.
   e.g. C:\Program Files (x86)\KeePass Password Safe 2
3. Run KeePass and open your KeePass database.
4. Make sure you have a password entry for your Google Account in your
   database. If that entry has the URL "accounts.google.com" associated, you
   can skip the next step.
5. Go to the "Properties" tab of you Google Account password entry and copy the
   UUID at the bottom to the clipboard.
6. Go to: Tools > Google Sync Plugin > Configuration
7. Either select your Google Account from the drop-down list or select
   "Custom KeePass UUID" and paste the UUID you copied to the clipboard into
   the "KeePass UUID" field.
8. Optional Step: Provide the OAuth 2.0 "CLIENT ID" and "CLIENT SECRET" you
   created following the steps in the Requirements section after enabling the
   "Custom OAuth 2.0 Credentials" checkbox.

The Plugin is now ready to work. On first use Google will ask for your consent
to access your Google Drive.


Portable Mode
-------------

The reference to your Google Account (UUID) as well as your preference for
AutoSync are saved in your KeePass config file. The OAuth 2.0 credentials are
saved securely inside the encrypted database.

You can use the compiled version of the Plugin instead of the .plgx file which
creates temporary files on the host system.

1. You should clear KeePass' Plugin Cache first: Tools > Plugins > Clear
   and then restart KeePass with the .plgx Plugin installed.
2. The compiled version can then be found inside KeePass' Plugin Cache folder.
   e.g.: C:\Users[User]\AppData\Local\KeePass\PluginCache
3. Inside one of the cryptically named folders is a file named
   GoogleSyncPlugin.dll. Copy that folder with all its files into the KeePass
   folder of your Portable KeePass installation.
   You may rename the cryptic folder name to "GoogleSyncPlugin".
4. Delete the GoogleSyncPlugin.plgx file from your Portable KeePass folder if
   present.


Compatibility
-------------

The Plugin has only been tested with the .NET 4.0 Framework for Windows.
Linux and Mac OS X are not supported.
However, the Plugin does seem to work limited with Linux:
http://sourceforge.net/p/kp-googlesync/discussion/general/thread/19cca399/#4df1
