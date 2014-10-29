Googly Sync Plugin for KeePass Password Safe
============================================

Version: 2.0 (2014-11)
Authors: Danyal (DesignsInnovate), Paul Voegler
License: GPL version 3
Project: http://sourceforge.net/p/kp-googlesync


This is a Plugin for KeePass Password Safe >= v2.09. It allows to synchronize
the currently open database with a Google Drive Account.
It supports three modes of operation:

* Synchronize: The remote version of the open database downloaded to a
  temporary file in the same directory as the open database.
  The remote version is then merged into the open database by KeePass'
  "Synchronize with File" feature. The updated database is saved and uploaded
  to the Google Drive, replacing the old version.
* Upload: The open database is uploaded to the Google Drive creating a new
  file or replacing an existing version.
* Download: The remote version of the open database is downloaded replacing
  the open database.

The Synchronize mode can be configured to run automatically when the open
database is saved.

The Plugin matches remote file by the file name of the open database. The
remote file can be moved to any folder within the Google Drive, but the file
name must be unique within the drive and match the local file name.


Requirements
------------

The Plugin requires the full .NET Framework 4.0 on the system.

The Plugin requires creating a Project with the Google Developers Console to
obtain OAuth 2.0 credentials to access the Google Drive:

* Go to the Google Developers Console in your Google Account:
  https://console.developers.google.com/project
* Create a new project called: "KeePass Google Sync Plugin"
* Enable the Google Drive API at "APIs & auth" > "APIs".
* Choose an e-mail address at "APIs & auth" > "Consent screen".
* Set the product name for the consent screen to "KeePass Google Sync Plugin".
* Create OAuth 2.0 credentials for an "Installed application" with application
  type "Other".

You should now have a "CLIENT ID" and "CLIENT SECRET".


Installation
------------

* Place the "GoogleSyncPlugin.plgx" file into the "plugins" directory of your
  KeePass installation.
* Open a KeePass database.
* Make sure you have a password entry for your Google Account. If that entry
  has the URL "accounts.google.com" associated, you can skip the next step.
* Go to the "Properties" tab of that entry and copy the UUID at the bottom.
* Go to "Tools" > "Google Sync Plugin" > "Configuration".
* Either select your Google Account from the drop-down list or paste the UUID
  into the "KeePass UUID" field.
* Provide the OAuth 2.0 "CLIENT ID" and "CLIENT SECRET" you created earlier.

The Plugin is now ready to work. On first use Google will ask for your
consent to access your Google Drive.

The reference to your Google Account (UUID) as well as your preference for
AutoSync are saved in your KeePass config file.
The OAuth 2.0 credentials are saved securely inside the database.


Upgrade from v1.x
-----------------

Version 2 of this Plugin is not compatible with the configuration of previous
versions. That is mainly because previous versions saved the Google Drive API
credentials in a way an attacker might gain access to the Google Drive when
the Plugin was used on public / untrusted / shared systems.

Discussion:
http://sourceforge.net/p/kp-googlesync/discussion/general/thread/81f69f55

When upgrading from version 1.x you should first remove all old configuration
settings and revoke the API access.

* Locate your keepass.config.xml. e.g. C:\Users\[User]\AppData\Roaming\KeePass
* Edit your keepass.config.xml file and remove all <Item> entries in the
  <Custom> group for the old Plugin. Those are items with <Key>
  "EnableAutoSync", "GoogleSyncKeePassUID", "GoogleSyncClientID"
  and "GoogleSyncClientSecret". Delete from <Item> to </Item> (including).
* Go to https://security.google.com/settings/security/permissions
* Select your KeePass Google Sync Plugin from the list of Account Permissions.
  It may be called "KeePassPlugin" or whatever you named it initially.
* Click on "Revoke access".

Now install and configure the new Plugin like a new installation skipping
the part to create a new Project with Google and instead using your existing
Project.

Although it is not necessary because the refresh token to access the
Google Drive API is now invalid, you may want to remove the old configuration
regardless. In that case go to the discussion forum linked above for
detailed instructions.