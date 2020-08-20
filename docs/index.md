---
layout: home
title: KeePass Sync for Google Drive™
---

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
extra security. Changes made to the database by another user or on another
device can be accurately merged with the local copy of the database. Handy for
users of [mobile or other KeePass-compatible programs](https://keepass.info/download.html)
that synchronize with Google Drive, it can also simplify distribution of
group shared password archives.  

Supports traditional KeePass v2 ["Normal"](../install/normal) and 
["Portable"](../install/portable) plugin installation.