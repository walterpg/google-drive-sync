Googly Sync Plugin for KeePass Password Safe
============================================

Version: 2.0 (2014-11)
Authors: Danyal (DesignsInnovate), Paul Voegler
License: GPL version 3
Project: http://sourceforge.net/p/kp-googlesync


This is a Plugin for the Open Source Password Manager KeePass 2. It allows to
synchronize the currently open database with a Google Drive Account.
It supports three modes of operation:

* Synchronize: The remote version of the currently open database is first
  downloaded to a temporary file in the same directory as the current database.
  The current database is then merged with the remote version by KeePass'
  "Synchronize with File" feature. The merged (current) database is saved
  and uploaded to the Google Drive, replacing the old version. If there is no
  remote version, this works like "Upload" below.
* Upload: The current database is uploaded to the Google Drive creating a new
  file or replacing an existing version if it already exists.
* Download: The remote version of the current database is downloaded to a new
  file in the directory of the current database with a timestamp in the file
  name. The currently open database is NOT replaced.

The Synchronize mode can be configured to run automatically when the currently
open database is saved.

The Plugin matches the files by the file name of the currently open database.


Requirements
------------

You need the full .NET Framework 4.0 to use this Plugin.

The Plugin requires creating a Project with the Google Developers Console to
obtain OAuth 2.0 credentials to access your Google Drive:

* Go to the Google Developers Console in your Google Account:
  https://console.developers.google.com/project
* Create a new project called: "KeePass Google Sync Plugin"
* Enable the Google Drive API at "APIs & auth" > "APIs".
* Choose an e-mail address at "APIs & auth" > "Constent screen".
* Set the product name for the consent screen to "KeePass Google Sync Plugin".
* Create OAuth 2.0 credentials for an "Installed application" with application
  type "Other".

You should now have a "CLIENT ID" and "CLIENT SECRET".


Installation
------------

* Place the "GoogleSyncPlugin.plgx" file into the "plugins" directory of your
  KeePass installation and restart KeePass.
* Open a KeePass database.
* Make sure you have an entry for your Google Account. If that entry also has
  the URL "accounts.google.com" associated, you can skip the next step.
* Go to the "Properties" tab of that entry and copy the UUID.
* Go to "Tools" > "Google Sync Plugin" > "Configuration".
* Either select your Google Account from the drop-down list or paste the UUID
  from earlier in the "KeePass UUID" field.
* Enter the OAuth 2.0 "CLIENT ID" and "CLIENT SECRET" you created earlier.

The Plugin is now ready to work. On first use Google will ask for your
consent for the Plugin to access your Google Drive. You can manage consents
with your Google Security Settings page.

The reference to your Google Account (UUID) as well as your preference for
AutoSync are saved in your KeePass config file.
The OAuth 2.0 credentials are saved securely inside the database.
