---
layout: page
title: Drive Shortcuts
description: Redirect the sync target with the Google Drive shortcuts feature.
---

## What Happens
* The plugin can synchronize with files referenced by internal
[Drive shortcuts](https://support.google.com/drive/answer/9700156?co=GENIE.Platform%3DAndroid&hl=en).
* Shortcut support is transparent. Simply configure the target database
on Drive to be the shortcut to the actual file containing the database.
* Note that the shortcut can have a different name than the file it
points to.
* Shortcuts and the files they reference may reside anywhere in Drive,
including a configured [Target Folder](/usage/target-folder).

## How it Works
* When performing a sync op, the plugin looks for a Drive object
with the same name as the local database file, for example,
"MyDatabaseShortcut.kdbx" (for details, see the 
[Syncing Your Database](/install/config#syncing-your-database)
topic).
* If plugin determines "MyDatabaseShortcut.kdbx" is a shortcut, the
object it references is queried to determine the type of Drive object
the shortcut points to (shortcuts can reference files or folders).
* If the shortcut references a file, for example, "MyDatabaseFile.kdbx",
the plugin uses "MyDatabaseFile.kdbx" to synchronize with the current
KeePass database.

## Use with Care!
Drive shortcuts are a powerful feature for organizing your files.
When synchronizing to a shortcut, be certain the file 
being referenced is the intended target file, especially when
using the [Download](/usage/dnload) and [Upload](/usage/upload) commands.

## Shared File Restriction
It is possible to create a shortcut to a file in the
["Shared with me"](https://support.google.com/drive/answer/2375057?co=GENIE.Platform%3DDesktop&hl=en)
special folder in Drive.  This allows files to be accessed as if they 
resided in your own Drive folders.  

Due to [security issues](/notices/sharedsec)
however, the plugin will only safely synchronize such files when the
[session-stored authorization token](/usage/authorize#session-stored-tokens)
feature is enabled.