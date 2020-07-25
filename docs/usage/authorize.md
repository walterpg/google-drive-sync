---
layout: page
title: Authorization Flow
---

One way in which Google have improved the security of Google Drive
APIs in the last few years is by eliminating support for
older browsers and authentication mechanisms. For this reason, the
plugin is enhanced with approved techniques using
Google Sign-in.

Successful authorization results in Google issuing an authorization
token - thus you usually only need to authorize a new database once.

* [Authorization Walkthrough](#authorization-walkthrough)
* [Authorization Issues](#authorization-issues)
* [Authorization Tokens](#authorization-tokens)

---

#### Authorization Walkthrough

When you invoke a sync command from the plugin menu, the plugin
will determine if first you need to authorize.  If so, it will
display this prompt window:

{:refdef: style="text-align: center;"}
![Authorization Prompt](../assets/img/auth-dialog.png)
{: refdef}

This prompt notifies you that your consent is needed to operate
the plugin. Because the plugin also opens the Windows-default
web browser application, the prompt and KeePass may become hidden
behind the Google Sign-in page display. The image below shows
the plugin waiting for the user enter credentials in the 
Google Sign-in page.  The Google Sign-in page may have a different
appearance if the browser is already signed on to Google.

{:refdef: style="text-align: center;"}
![Prompt and Google Sign-in Page](../assets/img/auth.png)
{: refdef}

<div class="alert alert-secondary" role="alert">
    Note that the plugin prompt dialog provides <b>Copy User</b> and
    <b>Copy Password</b> buttons.  If the
    <a href="../install/config">configured</a>
    plugin entry contains your Google credentials, these buttons
    are enabled. Clicking them invokes the KeePass clipboard
    copy function, to facilitate data entry in the Sign-in page.
</div>

After you successfully sign on in the browser, the Google authentication
service may ask you to confirm you consent, showing a page similar
to the image below:

{:refdef: style="text-align: center;"}
![Google Authorization Confirmation](../assets/img/auth-requested.png)
{: refdef}

After you click **Allow**, the browser will show a "return to KeePass"
message.  The plugin will automatically close the prompt dialog, and
begin processing the command that initiated the authorization sequence.
If the command was [Sync with Drive](sync), KeePass will show
a message in its status bar after the command completes successfully,
such as shown below.

{:refdef: style="text-align: center;"}
![Sync complete status](../assets/img/sync-complete-status.png)
{: refdef}

---

#### Authorization Issues

If Google does not recognize the [OAuth 2.0 credentials](../usage/oauth)
that the plugin is configured for, the browser may display one of various
error pages.  For example, the plugin currently uses the legacy
KeePass Google Sync 3.0 plugin's OAuth 2.0 credentials; unless the
[**Use Limited Drive Access**](x-40#limited-drive-access) option
is in use, the browser may display this message after sign on:

{:refdef: style="text-align: center;"}
![KGS 3.0 credentials retired](../assets/img/app-denied.png)
{: refdef}

If you see this message, you will likely either have to enable
the **Use Limited Drive Access** option, or supply [your
own OAuth 2.0 credentials](../usage/oauth) before attempting to authorize
again.

<div class="alert alert-secondary" role="alert">
    The legacy plugin credentials currently used by the plugin will
    be replaced in a future release, making obsolete the <b>Use Limited
    Drive Access</b> option and personal OAuth 2.0 credentials.
</div>

---

#### Authorization Tokens
When you successfully authorize, the Google Sign-in service issues
an authorization token which the plugin saves to the database.
This token is proof of your consent, and 
the plugin must send it in each request to use the Google Drive API.

Because the token is securely saved in the database, you can use
plugin commands without re-authorizing each KeePass session.

Occasionally however, re-authorization will be required. There are
basically only two conditions which require the plugin to obtain a new
authorization token:

* Syncing a new database.
* Expired or Google-retired authorization tokens saved in an existing
database.

The plugin initiates the authorization sequence any time a command
requires a new or refreshed token.
