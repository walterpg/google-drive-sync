---
layout: page
title: Configuration
---

* [First Time Walkthroughs](#syncing-your-database)
* [Upgrading from *Google Sync 3.0*](#databases-configured-to-use-keepass-google-sync)
* [The Configuration Window](#configuration-window)
    * [Sync Authorization](#sync-authorization)
    * [Options and Defaults](#options-and-defaults)

To understand the plugin's settings system or to synchronize your first
database, please review the [walkthroughs](#syncing-your-database).
Users familiar with *Google Sync Plugin 3.0* should also find
this information useful.

The **Configuration** window customizes the plugin's
authorization and other settings for the current database, and sets
default configurations for all new databases that will be used with the
plugin. 

* [Database specific configuration](#sync-authorization)
* [Default configuration for new databases](#options-and-defaults)

---

### Syncing Your Database
So you just [installed](../install/normal) the plugin, and are ready to sync.
Maybe you are a veteran of the original *Google Sync 3.0*, or perhaps
this is the first time you have used a plugin to [Sync with Drive](../usage/sync).
Hopefully these short walkthroughs are helpful.

* [New or Non-configured Databases](#new-databases)
* [Databases previously used with *Google Sync 3.0*](#databases-configured-to-use-keepass-google-sync)

---

#### New Databases
The first time you sync a database, the plugin searches for an entry to
store its configuration data.  Each entry's URL field is searched
for a value containing this name: ``accounts.google.com``. If no such entry
is found, the plugin offers to create one for you.  

{:refdef: style="text-align: center;"}
![New Config](../assets/img/new-config.png)
{: refdef}

<div class="alert alert-warning text-dark" role="alert">
Unless the database contains an entry with the required
<code>accounts.google.com</code> URL value, any attempt to use the
plugin will display the above message until you (or
the plugin) have created it.
</div>

However, if the database already contains entries with the
``accounts.google.com`` URL value, those entries are searched for plugin
configuration information.  If this is a new or non-configured database, 
the information will not be found, and you will be prompted to configure
the entry of your choice:  

{:refdef: style="text-align: center;"}
![Entries Found](../assets/img/fix-config.png)
{: refdef}

This **Google Accounts Found** message briefly explains the two
alternatives to configure a plugin entry in this case.
* Click OK to [configure](#configuration-window) one of the found entries. 
* Click Cancel, then delete or modify the KeePass entries with the
``accounts.google.com`` URL field values. Then, try to sync again
and let the plugin create an entry by consenting to the former
**No Google Sync Entry Found** prompt.

<div class="alert alert-secondary" role="alert">
To get familiar with the plugin's new Google Drive authorization flow,
new users (and users with new databases) should also review the 
<a href="../usage/authorize#authorization-walkthrough">authorization walkthrough</a>.
</div>

<div class="alert alert-secondary" role="alert">
Regardless of how the plugin entry is created for new databases, it is 
highly recommended that the chosen KeePass entry include your Google Drive
user name and password, so that when the plugin needs your permission for
authorization you can access those credentials from KeePass, via the plugin's
wait window, and enter them into the Google Sign-in page.
</div>

---

#### Databases Configured to use KeePass Google Sync
If the database was previously synchronized with Google Sync 3.0,
the plugin will find the old configuration, copy it, and adopt this as its own.
If there is a *valid* [authorization token](../usage/authorize#authorization-tokens)
in the old configuration, you will be able to synchronize exactly as before
without needing to reauthorize.

If you haven't used the old plugin for some time, or if you have recently
experienced problems with the old plugin, you may be required to
authorize the new plugin.  [Please review the authorization issues topics
for guidance](../usage/authorize#authorization-issues).

<div class="alert alert-secondary" role="alert">
    <a href="../install/require#compatibility-with-google-sync-plugin">
        Compatibility
    </a>
    with Google Sync 3.0 is a primary feature of 
    the new plugin.
</div>

---

#### Configuration Window
* [Sync Authorization](#sync-authorization)
* [Options and Defaults](#options-and-defaults)

The plugin menu's **Configuration** command opens a dialog that lets you
choose various settings for the current database, as well as some global
settings affecting new databases and general plugin operation.  An **About**
tab displays the plugin version and credits.  

{:refdef: style="text-align: center;"}
![The Configuration Window](../assets/img/config-window.png)
{: refdef}

All settings available here are editable, but the current settings are not
changed until you click **OK**; clicking **Cancel** discards any changes
made in the dialog window.

##### Sync Authorization
The **Sync Authorization** tab contains the settings specific to the current
database, mostly with regard to authorizing the plugin to sync with
Google Drive.

**KeePass Entry Title** is a list box that displays the Title field of
KeePass database entries eligible to be the "active" plugin entry.  The
entry you select here is where the plugin will store the [authorization
token](../usage/authorize#authorization-tokens), and other settings.

{:refdef: style="text-align: center;"}
![Entry Selection](../assets/img/entry-title.png)
{: refdef}

Eligible entries are those where the URL field contains the name
``accounts.google.com``, as mentioned in the [New Databases](#new-databases)
section above.

<div class="alert alert-warning text-dark" role="alert">
    <div>NOTE</div>
    Changing the active entry, or any other authorization setting,
    nullifies any previously obtained consent to your Google Drive account,
    and you will be forced to authenticate and re-authorize the
    plugin the next time you run a sync command.
</div>

In the **Google Drive Features** section is the
[**Target Sync Folder**](../usage/target-folder)
feature, with which to specify a folder in Drive where the plugin
synchronizes your database files.  Leave this blank for the traditional
Google Sync 3.0 "one sync file across all of Drive" semantics.

{:refdef: style="text-align: center;"}
![Drive Feature Options](../assets/img/gdrive-opts.png)
{: refdef}

In the **Drive Authorization Token Handling** section shown below, you
may choose how the plugin handles
[authorization tokens](/usage/authorize#authorization-tokens).
The **Do not save Google Drive authorization in the database** option
allows you to avoid certain [security pitfalls](/notices/sharedsec)
associated with the storage of
[authorization tokens](/usage/authorize#authorization-tokens)
in the KeePass database.  If you wish to synchronize databases that are
shared with other users, consider enabling this option. Enabling
the option configures the plugin for session-stored authorization
tokens. This is the only way the plugin can safely synchronize databases shared with
the [Drive shared files feature](https://support.google.com/drive/answer/2494822?co=GENIE.Platform%3DDesktop&hl=en).

{:refdef: style="text-align: center;"}
![Authorization Token Options](../assets/img/auth-token-opts.png)
{: refdef}

The section shown below is where you may specify alternative app credentials
used to authorize access to Drive.  This section is generally
used only by veteran users of the legacy plugin, or those with special 
requirements.  Most users will find it much easier and safer to avoid using
the **Enable Legacy OAuth 2.0 Credentials** options.

{:refdef: style="text-align: center;"}
![Legacy Credentials Options](../assets/img/gdrive-auth.png)
{: refdef}

Checking the the **Use KeePass Google Sync 3.0 App Credentials** check box will
cause the plugin to use legacy, Google Sync 3.0 app credentials.  This is not
useful for most users, since Google has long since retired the old credentials.
If you happen to have an old database that still contains a valid
[authorization token](/usage/authorize#authorization-tokens)
for the old app credentials, and don't want to re-authorize,
then this could be the compatibility option for you.

Clearing the **Use KeePass Google Sync 3.0 App Credentials** check box
enables the **Client ID** and **Client Secret** text boxes, in which you
can enter
[your own Google issued OAuth 2.0 credentials](../usage/oauth),
if you wish to authorize the plugin that way.

The **Use limited Drive access** check box determines the
[permission level you allow the plugin when accessing Drive](../usage/x-40#limited-drive-access).

##### Options and Defaults
This **Configuration** tab contains settings pertaining to all databases
used with the plugin, and default settings for new databases.

{:refdef: style="text-align: center;"}
![Options and Defaults Tab](../assets/img/opts-and-defaults.png)
{: refdef}

[**Enabled Commands**](../usage/enabled-cmds) lets you choose the commands
you want to use with the plugin, and the commands you would rather disable.

The [**Auto Sync**](../usage/autosync) group are options to automate the [Sync with Drive](../usage/sync) command by invoking it automatically when Saving or Opening a database.  The **Resume on return from Auto-Lock or Exit** option is enabled when the **Auto Sync** on **Save** option is selected, and when selected it automatically ensures that KP ops that save-then-close are still processed by **Auto Sync** on reopen.  For more details, see the [Deferred Sync](/usage/autosync#deferred-auto-sync) topic.

The **Default Drive Authorization Token Handling** effect how the plugin
saves authorization tokens in *new* databases, and how it reacts to finding
stored authorization tokens in the database.  Enabling the 
**Do not save authorization tokens in new databases** option will enable
the analogous option in the
[**Sync Authorization**](#sync-authorization)
tab when the plugin creates plugin entries for new databases.  The
**Warn me before using a stored authorization token** option can be used by 
security-conscious users to notify them of the unexpected presence of a
[database-stored authorization token](/usage/authorize#database-stored-tokens)
when engaging a sync operation on any database.

The **Enable Legacy OAuth 2.0 Credentials** options are similar to the settings
of [Enable Legacy OAuth 2.0 Credentials](#sync-authorization) group on the 
**Sync Authorization** tab, but instead of affecting the current database,
these settings will be adopted by any [new database](#new-databases) you
use the plugin with.

The **Folder Options Defaults** settings are also similar to the **Google
Drive Options** group on the **Sync Authorization** tab, but again, 
apply to new databases created with the plugin.   

The **Default New Folder Color** option provides a way for the plugin to
colorize folders created by the plugin. For more information on this and
other folder related features, please review the
[**Designated Folder**](../usage/x-40#designated-folder-and-folder-colors)
topic.

