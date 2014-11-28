Googly Sync Plugin for KeePass Password Safe
============================================

Version: 2.1 (December 2014)
Authors: Danyal (DesignsInnovate), Paul Voegler
License: GPL version 3 (see gpl.txt)
Project: http://sourceforge.net/p/kp-googlesync


This is a Plugin for KeePass Password Safe >= v2.18. It allows to synchronize
the currently open database with a Google Drive Account.
It supports three modes of operation:

* Synchronize: The remote version of the open database is downloaded to a
  temporary file in the same directory as the open database.
  The remote version is then merged into the open database by KeePass'
  "Synchronize with File" feature. The updated database is saved and uploaded
  to the Google Drive, replacing the old version.
* Upload: The open database is uploaded to the Google Drive creating a new
  file or replacing an existing version.
* Download: The remote version of the open database is downloaded replacing
  the open database.

The "Synchronize" mode can be configured to run automatically when an open
database is saved, a database is opened or both. Databases which have no
matching configuration will be ignored.

The Plugin matches the remote file by the file name of the open database. The
remote file can be moved to any folder within the Google Drive, but the file
name must be unique within the Drive and match the local file name.


Upgrade from v1.x
-----------------

Version 2 of this Plugin is not compatible with the configuration of previous
versions.

Discussion:
http://sourceforge.net/p/kp-googlesync/discussion/general/thread/81f69f55

When upgrading from version 1.x you should first remove all old configuration
settings and revoke the API access:

* Make sure KeePass it is not running (check taskbar and tray).
* Locate your keepass.config.xml. e.g.:
  * C:\Users\[User]\AppData\Roaming\KeePass (local config)
  * KeePass.exe directory (portable, global config)
* Edit your keepass.config.xml file and remove all <Item> entries in the
  <Custom> group for the old Plugin. Those are items with <Key>
  "EnableAutoSync", "GoogleSyncKeePassUID", "GoogleSyncClientID"
  and "GoogleSyncClientSecret". Delete from <Item> to </Item> (including).
* Save your changes to keepass.config.xml and close the file.
* Go to https://security.google.com/settings/security/permissions
* Select your KeePass Google Sync Plugin from the list of Account Permissions.
  It may be called "KeePassPlugin" or whatever you named it initially.
* Click on "Revoke access".
* Continue with the installation instructions below. You may skip the part to
  create a new Google Project.


Requirements
------------

The Plugin requires the .NET Framework 4.0 or newer.
(The .NET Client Profile does not suffice)

The Plugin requires OAuth 2.0 credentials to access the Google Drive API.
Starting with v2.1 it has it's own credentials built in, but still allows you
to provide your own as with previous versions.

In case you would like to provide your own OAuth 2.0 credentials, you can
create a Project with the Google Developers Console:

* Go to the Google Developers Console in your Google Account:
  https://console.developers.google.com/project
* Create a new project called: "KeePass Google Sync Plugin"
* Enable the Google Drive API at "APIs & auth" > "APIs".
* Choose an e-mail address at "APIs & auth" > "Consent screen".
* Set the product name for the consent screen to "KeePass Google Sync Plugin".
* Create OAuth 2.0 credentials for an "Installed application" with application
  type "Other". ("APIs & auth" > "Credentials")

You now have the "CLIENT ID" and "CLIENT SECRET" you may use below.


Installation
------------

* Make sure KeePass it is not running (check taskbar and tray).
* Place the "GoogleSyncPlugin.plgx" file into the "plugins" folder of your
  KeePass installation.
* Open a KeePass database.
* Make sure you have a password entry for your Google Account. You may leave
  the password field blank if you wish. If that entry has the URL
  "accounts.google.com" associated, you can skip the next step.
* Go to the "Properties" tab of that entry and copy the UUID at the bottom.
* Go to "Tools" > "Google Sync Plugin" > "Configuration".
* Either select your Google Account from the drop-down list or paste the UUID
  into the "KeePass UUID" field.
* If you want to provide your own OAuth 2.0 credentials select "Custom OAuth
  2.0 Credentials" and provide the "CLIENT ID" and "CLIENT SECRET".

The Plugin is now ready to work. On first use Google will ask for your
consent to access your Google Drive. Make sure you authenticate with the
correct account in case you use multiple Google Accounts.


Portable Mode
-------------

The reference to your Google Account (UUID) as well as your preference for
AutoSync are saved in your KeePass config file.
The OAuth 2.0 credentials are saved securely inside the encrypted database.
You can use the compiled version of the Plugin instead of the .plgx file which
creates temporary files on the host system.
You should clear KeePass' Plugin Cache first ("Tools" > "Plugins" > "Clear")
and then restart KeePass with the .plgx Plugin installed.
The compiled version can then be found inside KePass' Plugin Cache folder.
e.g.: C:\Users\[User]\AppData\Local\KeePass\PluginCache
Inside one of the cryptically named folders is a file named
"GoogleSyncPlugin.dll". Copy that folder with all it's files into the "plugins"
folder of your Portable KeePass installation. You may rename the cryptic folder
name to "GoogleSyncPlugin". Delete the GoogleSyncPlugin.plgx file from your
Portable KeePass installation if present.


Compatibility
-------------

The Plugin has only been tested with the .NET 4.0 Framework for Windows.
Due to compatibility issues with the included Google API libraries, the Plugin
unfortunately does not work with Mono for Linux.
Compatibility with Mono for other platforms is unknown.
