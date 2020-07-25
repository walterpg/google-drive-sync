---
layout: page
title: Requirements and Compatibility
---

Supported installations of the plugin require the following components.  

* [KeePass](https://keepass.info) version 2.35 or later.
* [.NET Framework](https://dotnet.microsoft.com) version 4.5 or later.
* A [Microsoft-supported Windows OS](https://www.microsoft.com/en-us/windows)
running the above.

Note that these requirements are more restrictive than the requirements for
running KeePass alone, due to Google API requirements.  Also, unlike KeePass,
successful installation of the plugin requires *you* to ensure these
requirements are met.

See also:
* [Installation (normal)](normal)
* [Installation (portable)](portable)
* [Configuration](config)

---

### Compatibility with Google Sync Plugin
This is the 4th generation of the plugin, with updated Google Drive APIs,
modern KeePass ingegration, and updated .NET Framework security.  Every
effort is made to preserve functional compatibility with
[Google Sync Plugin 3.0](https://sourceforge.net/projects/kp-googlesync/),
while also fixing issues with shifting Google authorization requirements.  

<div class="alert alert-secondary" role="alert">
    Compatibility is an important objective for this plugin. If you
    suspect a compatibility problem, 
    <a class="alert-link"
     href="https://github.com/walterpg/google-drive-sync/issues">
        please report the issue
    </a>.
</div>

If the old plugin currently synchronizes your databases successfully, this
release will be able to synchronize those databases as well.  Additionally,
you will not be required to [authenticate and reauthorize](../usage/authorize)
the plugin, at least as long as Google continues to respect the
[authorization token](../usage/authorize#authorization-tokens)
granted to the old plugin.  

However, if you want to synchronize a new database,
or if Google retires the token saved in an existing database, this
plugin may be the only way to continue synchronizing KeePass in a customary
way.

If you have configured [custom OAuth 2.0 credentials](../usage/oauth) for
your database with the old plugin, these should continue to work as before.

#### Settings Migration
Compatibility is achieved through a one-way "migration" of the settings
of the old plugin. Full, or forward compatibility with the old plugin is
not supported. That is, changes you make to the sync configuration of the
new plugin will not be reflected in the settings of the old plugin, and
conversely, changes to the old plugin configuration will not be reflected
in an already-existing new plugin configuration.

The first time the new plugin is used on a database
configured to sync with the old plugin, the settings from the old plugin are
copied and migrated, one time, to the new settings format.


#### Side-by-Side Compatibility
Configuration settings of the old plugin are not modified by the new
plugin.  Therefore, though not recommended, the new plugin may be safely
installed side-by-side with Google Sync Plugin 3.0.