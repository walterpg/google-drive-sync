---
layout: default
title: Download Command
description: Replace the current database with a Google Drive copy.
---

{:refdef: style="text-align: center;"}
![The Download Command](../assets/img/download.png)
{: refdef}

<div class='highlightbox'>
<div class='highlightboxtitle'>CAUTION</div>
This command <em>replaces</em> the local database with the contents of
a Drive file with the same name.  Any changes in the current database will be
lost.
</div>

## What Happens
* A Google Drive file with the same name as the local database is downloaded.
* The local database is replaced by the contents of the downloaded file.

## How it Works
* The plugin, with your [authorization](../misc/authorization), accesses
Drive and downloads a file with the same name as the current database.
* An error message is shown if the Drive file does not exist; otherwise,
the current database is closed.
* The local database file is deleted, and replaced by the downloaded file.
* The database is re-opened.