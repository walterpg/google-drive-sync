---
layout: page
title: Sync with Drive
description: Simplified, secure KeePass Synchronize with Drive files.
---

{:refdef: style="text-align: center;"}
![The Synchronize Command](../assets/img/sync.png)
{: refdef}

KeePass' own built-in [Synchronize](https://keepass.info/help/v2/sync.html) 
command is a powerful feature, but too generic to use safely and
efficiently with Google Drive files.  The plugin command reduces
several steps to a single click.

## What Happens
* The most recent changes of both databases (on Google Drive and the PC)
are intelligently merged with the KeePass [Synchronize](https://keepass.info/help/v2/sync.html)
function.
* Optionally the plugin can [automatically invoke](./autosync) the
**Sync with Drive** command after invoking the KeePass **Save** or
**Open** commands, to ensure the database always has the current updates.


## How it Works
* The plugin, with your [authorization](authorize), accesses your Drive
to find a file with the same name as the currently open database.
* If the file exists on Drive it is downloaded to a temporary file on
your PC (if a Drive copy doesn't exist, the database is simply uploaded).
* KeePass [Synchronize](https://keepass.info/help/v2/sync.html) merges the
contents of the temp file with the local database.
* To summarize [Synchronize](https://keepass.info/help/v2/sync.html), KeePass
uses modification times of both entries and database "save" operations to
save only the most recent changes (new entries, updated passwords, etc.), and
discard older changes.
* If the sync operation modifies the database, it's uploaded to Drive.
* The databases on your PC and Google Drive are now perfectly synchronized.