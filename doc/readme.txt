Google Sync Plugin for KeePass Password Safe
============================================

Version: 3.1.0 (October 2016)
Authors: Danyal (DesignsInnovate), Paul Voegler, Walter Goodwin
License: GPL version 3 (see gpl.txt)
Project: http://sourceforge.net/p/kp-googlesync


This is a Plugin for KeePass Password Safe >= v2.35. It allows to synchronize
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

* .NET Framework 4.5 (the .NET Client Profile does not suffice).
* KeePass v2.35 or later.

OAuth 2.0 credentials are required to access the Google Drive API.  Sinc v2.1
the plugin has provided it's own built-in credentials, but still allows you
to provide your own as with previous versions.  For reasons explained below,
using your own credentials is the preferred (and possibly required) option.

To provide your own OAuth 2.0 credentials, you can create a Project with the
Google Developers Console:

1. Go to the Google Developers Console and login to your Google Account:
   https://console.developers.google.com/start
2. Select "New Project".
3. Choose a Project name like e.g.: "KeePass Google Sync Plugin".
4. From the Dashboard, select "Enable APIs and Services".  
5. In the Search box, enter "Drive", or browse the page for "Google Drive API".
6. Choose an e-mail address at "APIs & auth" > "Consent screen".
7. Set the product name for the consent screen to e.g.: "KeePass Google Sync Plugin".
8. Click "Credentials" > "Create Credentials".
7. Create a new OAuth 2.0 Client ID for an "Installed application" with
   application type "Other" at "APIs & auth" > "Credentials".

You should now have a "CLIENT ID" and "CLIENT SECRET".  Save and use these in the 
Configuration steps below.


Installation
------------

1. Exit and Close KeePass. Make sure it is not running at all (in case it is
   minimized to tray instead of taskbar).
2. Place the "GoogleSyncPlugin.plgx" file from the downloaded .zip into your
   KeePass installation directory.
   e.g. C:\Program Files (x86)\KeePass Password Safe 2
3. Run KeePass and open your KeePass database.  The plugin cannot be configured
   unless a database is opened.


Sync Password Entry
-------------------

The KeePass database you open must have a password entry to save the sync
configuration information.  Databases previously used with v2.x of the plugin
will have such an entry configured, and the plugin will find and use this.

For new or never-synchronized databases you have two options:

* Let the plugin automatically create a suitable password entry (you will be
  prompted to allow this the first time you sync or config the plugin).

* Manually create or modify a suitable password entry.

To take advantage of all features, you should modify the automatically-created
entry or your own manually-created entry with the following information:

* The URL field of the entry should contain the string "accounts.google.com",
  for example: "https://accounts.google.com".  Automatically-created entries
  will set this properly.  THIS FIELD IS REQUIRED FOR PLUGIN FUNCTIONALITY.

* The User and Password fields of the entry should contain the credentials of
  the Google Drive account with which you wish to sync the database.  This
  simplifies the plugin's authorization when required, but is optional.


Plugin Configuration
---------------------

Sync Authorization Tab:

* Optionally supply OAuth 2.0 Client ID credentials for secure, recommended
  authorization to Google Drive.

* Specify the permissions the plugin will request when accessing Google Drive
  (see Access Permissions below for recommendations).

* Optionally specify a target top-level folder location in Google Drive for the
  database file.  If this folder does not exist, the plugin can create it.

Other Options Tab:

* Specify desired Auto-Sync options.

* Specify other global, default configuration details used when when other
  databases are configured for the first time.  For example, if you have
  a single set of OAuth 2.0 credentials to use for every database you will
  ever create and sync in the future, save them here so that they will be
  used to automatically create the plugin's password entry.

The Plugin is now ready to work. On first use Google will ask for your consent
to access your Google Drive via the Windows default browser.

Certain configuration options are provided to workaround issues with Google 
Sign-In authorization and access to Drive.  You may wish to review the 
sections below to understand, and learn to successfully navigate these.


Why Use Custom OAuth 2.0 Credentials?
-------------------------------------
Since v2.1, the plugin has provided the option of using plugin-default "legacy"
OAuth 2.0 credentials.  If you currently sync your database using this option,
great!  However, sometime in the last year or so Google disabled new full-
access authorizations with these credentials.  For this and other reasons, it
is recommended that you obtain and configure your own OAuth 2.0 credentials, as
described above.

Issues with the default Legacy credentials:
* New (unauthorized) databases cannot be sync'ed using the "unrestricted" 
  access option (See Access Permission below).
* Even if you currently use the legacy credentials, Google may eventually
  revoke the existing authorization, forcing you to re-configure.
* The current owners of the legacy credentials may be forced to surrender
  them for various reasons, which would revoke existing authorizations.

Legacy credential workarounds:
* Use your own OAuth 2.0 credentials.  Google doesn't make it particularly 
  simple for non-developers to do so, but it can be done.  Once registered, the
  credentials will be valid for all your database sync'ing needs.  And then
  the registration is a matter between you and Google and no one else.
* Continue using the legacy credentials, but configure the "restricted" access
  option (see Access Permission), which currently Google will still authorize.


Access Permissions
------------------
Prior releases of the plugin always requested "full" access to your Google 
Drive assets, which, if authorized, provides the ability to create, read,
write, and delete ANYTHING in your Google Drive.  For obvious reasons, Google
considers this access request as "restricted", and currently it is only
granted to "verified" apps.

Apps are identified by their OAuth 2.0 credentials, and the legacy credentials
in the plugin have been tagged by Google as non-verified app identifiers.  This
likely is due to the popularity of the plugin; apps' usages are metered, and a
threshold number of authorizations trigger the non-verified restrictions, and
remain enforced until the app is certified by Google.

It is difficult (if not a violation of Google TOS) for Windows open-source 
applications such as this plugin to attain "verified" status.  However, Google
allows any account holder to register their own OAuth 2.0 credentials. Since it
is unlikely that a single user will ever trigger restriction thresholds, it is 
highly recommended that users of the plugin obtain and configure their own
OAuth 2.0 credentials.

But wait!  You say you are using the legacy credentials and can still sync your
database without any problems.  Before Google enforced the restriction on the
legacy credentials, many plugin users were granted access to sync their 
databases with "full" access mode, and since the plugin saves the "access token"
representing the grant, and Google still honors it, many of those users can
still sync their databases.  Eventually, Google may revoke the token, and then
you may be stuck.  At that point, you have two options: get OAuth 2.0 
credentials or change the plugin access mode.

This release allows you to specify the "recommended" access mode, which 
provides the authorized plugin the ability to read, write, and delete only the
Drive files that it "creates and uses".  This access mode can even be
authorized with the legacy credential.

One drawback of the "recommended" access is that the plugin cannot sync with a
copy of the file that may have been created on Drive by other means, such as 
Drive Backup or web interface upload.  A simple workaround for this is to
ensure that the file is initially created on Drive by the plugin with the
upload or sync commands.  After the file has been created by the plugin, other
verified apps (such as Keepass2Android, Drive Backup, etc.) can also sync with
the file.

If however, 
* "Recommended" access is configured,
* The database was initially copied to Drive by something other than the
  plugin,
* The plugin then tries to sync the database,

..the sync will actually upload a new file with the SAME NAME.  If this occurs,
you may be able to reconcile things by examining the modification dates of the
duplicated files in the Drive web interface.  Again, this may be avoided by
ensuring that the one, true copy of the file is initially uploaded by the
plugin.

Another plugin option to help avoid the duplicate file name problem is the
target folder option.  You still should ensure that this folder has a unique
name among the top-level folders in Drive (to avoid a duplicate folder
problem!).


Portable Mode
-------------

Your preference for AutoSync, default sync authorization and folder defaults
are saved in your KeePass config file.  Per-database OAuth 2.0 credentials are
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
The Plugin has only been tested with the .NET 4.5 Framework for Windows, and
with the Chrome, Edge, and Internet Explorer browsers.  It should work with
any modern, non-embedded Windows browser.

Linux and Mac OS X are not supported.  However, prior versions been
successfully used with limited functionality in Mono for Linux:
http://sourceforge.net/p/kp-googlesync/discussion/general/thread/19cca399/#4df1
