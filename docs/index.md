---
layout: home
title: KPSync for Google Drive™
---

<div class="alert alert-secondary small" role="alert">
    <div><em>Did you notice something different?</em></div>
    The plugin is undergoing a name change.  "KeePass Sync for Google
    Drive", as shown in the current release and (formerly) in
    Google Sign-in consent screen, is changing to 
    "KPSync For Google Drive."  With any luck
    the change will take effect in this order:
    <li> <strike>Website</strike> (done).</li>
    <li> <strike>Google Sign-in</strike> (done).</li>
    <li> New plugin release.
    </li>
    Apologies for any confusion; watch for the rebranded plugin
    release, and the removal of this banner, coming soon.
</div>

<div class="alert alert-warning text-dark" role="alert">
    <div>
        <a href="/notices/sharedsec">SECURITY ALERT</a>
    </div>
    KeePass databases synchronized with this plugin, then
    <em>shared with partially-trusted users</em>, could present a security hazard.
    <a href="/notices/sharedsec">Please review the details</a> to
    ensure safe use of the plugin.
</div>

This [KeePass Password Safe v2](https://keepass.info) plugin provides safe,
automated synchronization of a KeePass database with a
corresponding file of the same name on your [Google Drive](https://drive.google.com). 
The plugin is based on and [compatible](../install/require#compatibility-with-google-sync-plugin)
with the original *Google Sync 3.0* plugin.

---

#### The Commands

* [Synchronize](usage/sync).  Merge changes in the currently open database with 
changes in the Google Drive file.  Optionally, sync
with Google Drive [automatically](usage/autosync) each time you open or save a database.
* [Upload](usage/upload).  Copy the current database to Google Drive to make a duplicate
of the local database.
* [Download](usage/dnload). Replace the contents of the current database with the contents
of the Google Drive file.

Beyond simple backup or online storage, the plugin leverages KeePass'
[synchronize function](https://keepass.info/help/v2/sync.html) to maintain
perfect copies of the database in the Google Drive cloud and in Windows storage for
extra security. Changes made to the database elsewhere can be accurately
merged with the local copy of the database. Users of multiple devices and
[mobile or other KeePass-compatible programs](https://keepass.info/download.html)
that synchronize with Google Drive will find this plugin quite useful.

Supports traditional KeePass v2 ["Normal"](../install/normal) and 
["Portable"](../install/portable) plugin installation.