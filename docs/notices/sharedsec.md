---
layout: post
title: SECURITY BULLETIN
subtitle: Shared Databases May Allow Unauthorized Access to Partially-Trusted Users
author: walterpg
date: Dec 30 2020
---

This documents a potential security threat to your personal data.
When KeePass Sync for Google Drive plugin is used to synchronize a 
KeePass database **shared with partially-trusted users**, Google Drive
data files can be compromised.  

This is a Day One defect for all releases of the plugin, including
Google Sync Plugin 3.0.

Please review the following to determine if you are affected by this
issue, and if so how to measure the potential threat and remediate the
problem.

* [What are the risks?](#exposure-risk)
* [Who is affected?](#who-is-affected)
* [Who is NOT affected?](#who-is-not-affected)
* [How can I prevent the threats?](#remediation)
* [Is there a permanent solution, or workarounds?](#fixes-and-workarounds)

## Exposure Risk
If you are affected, any user in a group you share a database and
its master password with may gain access to all Drive
files, instead of only the synchronized KeePass file.  Unless
everyone in the group is already granted full-access privileges
to the Google Drive account used for synchronization, the Drive
data files could be at risk. A software
exploit, leveraging a flaw in the plugin's design and public Google
APIs, can be crafted to expose, delete, and/or disfigure all data
files in the Drive account.

## Who is Affected
You may be exposed if:
* The database is configured for
[database-stored authorization tokens](/usage/authorize#database-stored-tokens)
(the traditional, default mode), AND
* The plugin-synchronized database and its master
password credentials are shared with users who are not otherwise
granted access to the Drive account used for synchronization.

<div class="alert alert-secondary" role="alert">
<div>EXAMPLE</div>
A typical problematic scenario is an office of workers who 
share a KeePass database for daily use.  Managers who share
the database might use the plugin to provide synchronization using
a Drive account reserved for management-only tasks, rather than
general shared use.
</div>

<div class="alert alert-secondary" role="alert">
<div>NOTE</div>
At the time of this writing, Google's position on the subject of
multiple users
<a href="https://support.google.com/a/answer/33330?hl=en">
sharing a single Google Drive account</a>
could be considered
<a href="https://support.google.com/a/answer/6309862?hl=en">ambivalent</a>.
</div>

## Who is NOT Affected
* If you *do not* share your KeePass database with any other users,
you are *not affected*.
* If you share your KeePass database only with users who *do not*
have access to its master password credentials, you are *not affected*.
* If you share your KeePass database and credentials only with
*fully-trusted* users who also are explicitly granted full access to the Google
Drive account used with the plugin, you are *not affected*.
* If the database is configured for
[session-stored authorization tokens](/usage/authorize#session-stored-tokens),
you are *not affected*.

## Remediation
First, ensure you are affected and at risk of harm as outlined above.
If you are at risk, you should neutralize threats posed by the copies
of the synchronized database distributed to the members of the group.
Do the following to revoke the plugin's authorizations:

* Sign on to the Google Drive account used for synchronization.
* Go to Drive settings, and select **Manage Apps**.
* In the list of applications, find "KeePass Sync for Google Drive".
* Select the option **Disconnect from Drive** for that entry.
* Do not synchronize the database again until you determine it is
safe to do so as outlined below.

To continue using the plugin safely, you must:

* Stop sharing synchronized database files with other users, or...
* If you must share a database, configure it for 
[session-stored authorization tokens](/usage/authorize#session-stored-tokens),
otherwise...
* If you must share, only share with users you would trust to have
full access to the Drive account.
* If you must share, consider using KeePass multi-factor
credentials and secure hardware.  Maintain strict control over
the hardware and at least one factor of the credentials.

<div class="alert alert-secondary" role="alert">
<div>Recommended</div>
If you are considering configuring your shared database to sync with
<a href="/usage/authorize#session-stored-tokens">session-stored tokens</a>,
also consider enabling the
<em><a href="/install/config#options-and-defaults">Warn me before using
a stored token</a></em>
option, so that sync operations are protected from inadvertent
misconfigurations.
</div>

If none of the above remediations suit your use-case and
sharing with partially-trusted users is unavoidable,
you should stop using the plugin until this issue is 
generally resolved in a future release.

## Fixes and Workarounds
By default, the version 4.0.4-beta release of the plugin blocks
synchronization with files configured with 
[Drive's shared file feature](https://support.google.com/drive/answer/2494822?co=GENIE.Platform%3DDesktop&hl=en).
Note however this does not prevent other, less simple file sharing
techniques, and so the general security issue remains unresolved. 
The (hazardous) legacy behavior of allowing sync under these conditions is 
retained only when the database is configured with
[personal OAuth 2.0 credentials](/usage/oauth#shared-file-workaround). 

Further, version 4.0.4-beta implements the 
[session-stored authorization token](/usage/authorize#session-stored-tokens)
feature, which addresses the issue by preventing the storage of
authorization tokens in the database. This solution is not ideal
however, because it requires the user to re-authorize the plugin
each time KeePass is restarted.

The current design of the plugin is not conducive to a simple
solution to the underlying problem of secure storage of
authorization tokens in a shared database scenario.
[Please see Github issue #21](https://github.com/walterpg/google-drive-sync/issues/21)
for a discussion of the portion of the issue centered on the
Drive shared file feature, and join in the research for a permanent
solution. 

---
[View all Security Bulletins](/notices)