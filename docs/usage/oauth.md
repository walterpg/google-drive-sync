---
layout: page
title: DEPRECATED Personal OAuth 2.0 Credential Support
---

<div class="alert alert-secondary" role="alert">
This feature is retained for compatibility only.
The use of legacy authorization methods, employing either personal or
<em>Google Sync Plugin 3.0</em> app credentials, should no 
longer be necessary for most users.
</div>

This workaround was devised in the
[legacy plugin](https://sourceforge.net/projects/kp-googlesync)
to allow you to use your own, Google provided
[OAuth 2.0](https://oauth.net/2/) client
credentials.  With the prior, Google unverified plugin there were
legitimate reasons for doing this:

* If you don't share the credentials with anyone, then no non-mechanical
third party can use them - they are a secret between you, Google, and the
KeePass program running on your PC.
* Though reasonably protected, and only useful with your expressed permission,
the built-in credentials could be compromised by unintended neglect or
nefarious means, and used to spoof the plugin's request for Drive account
authorization.
* [OAuth 2.0](https://oauth.net/2/), and Internet protocols in general,
are *never* impervious to attack by determined third parties.  Some users
may find some (likely displaced) solace by supplying, their own
"personal" OAuth 2.0 credentials.

To transparently support users of the legacy plugin there is a **very**
valid reason for doing this:

* *Google Sync Plugin 3.0* plugin was not verified by Google, and so
it has exceeded its validation limits.

If you have OAuth 2.0 [client credentials](https://www.oauth.com/oauth2-servers/access-tokens/client-credentials/)
in the form of a `client_id` and `client_secret`, you can configure the
plugin to use these to authorize plugin commands with the current database
by entering them in the appropriate text boxes of the **Sync Authorization**
tab of the Configuration dialog.  Ensure that the **Enable Legacy OAuth
2.0 Credentials** check box is checked, and the **Use KeePass Google Sync
3.0 App Credentials** checkbox is unchecked to enable the text boxes.

<div class="alert alert-warning text-dark" role="alert">
    <div>WARNING</div>
    When you change OAuth 2.0 credentials, the next time you use a
    plugin command you will be forced to reauthorize, by authenticating
    your Google account.
</div>

{:refdef: style="text-align: center;"}
![Entering personal OAuth 2.0 credentials](../assets/img/oauth-config.png)
{: refdef}

If you wish, you may also enter the credentials in the similar text boxes
of the **Options and Defaults** tab.  In this configuration, the plugin will
use the supplied credentials in all new databases.

<div class="alert alert-secondary" role="alert">
The personal OAuth 2.0 app credentials are not related in any way to your
Drive account credentials (user id/password, etc.).  App credentials only
serve to identify the application as your personal agent when
authorizing access to your Drive account.  Authorization can only be
validated by authenticating your Drive account credentials via Google
Sign-in.
</div>

---
#### Obtaining Personal OAuth 2.0 Credentials
Google currently allows any user to [obtain OAuth 2.0 client credentials](https://developers.google.com/identity/protocols/oauth2)
for access to its Drive and other APIs.  The process for doing so is designed
for developer use, and it changes periodically.  

Google could discontinue access to personal OAuth 2.0 credentials in the future.
But as of this writing, the procedure generally it goes like this:

1. Log on to the [Google Cloud API Console](https://console.developers.google.com/).
2. Use the "Create a Project" link to create a space for your credentials.  
Name it something and click CREATE.
3. Click the "Credentials" link, then "CREATE CREDENTIALS".  A menu or 
screen appears.  Select "OAuth client ID" or something similar.
4. Click through various forms, including a configuration of the consent screen
that Google will display when you use the credentials.  This will help you
identify the use of the credentials later. 
5. Again, click "Credentials", then "CREATE CREDENTIALS". You may have to
enter yet more info, particularly "Application type", which you should indicate
as "other" or "native" (not "web" or "mobile").  Finally, click CREATE.
6. A screen showing your new credentials (the text named "client ID" and "client
secret") should appear.  Copy the credential somewhere safe (hi there KeePass!).

---
#### Shared File Workaround
<div class="alert alert-warning text-dark" role="alert">
    <div>
        <a href="../notices/sharedsec">SECURITY ALERT</a>
    </div>
    KeePass databases synchronized with this plugin, then
    <em>shared with partially-trusted users</em>, could present a security hazard.
    <a href="../notices/sharedsec">Please review the details</a> to
    ensure safe use of the plugin.
</div>

To accommodate legacy use cases, the plugin enables sync ops to Drive files
[shared with other Drive accounts](https://support.google.com/drive/answer/2494822?co=GENIE.Platform%3DDesktop&hl=en) by default when configured for personal OAuth 2.0
app credentials.
This type of usage is highly discouraged due to [security issues](/notices/sharedsec).
However since this is the behavior of the legacy plugin, and it does
not endanger the integrity of the plugin's built-in app credentials,
it is enabled in this case.

This workaround is the **only** way to synchronize a database that uses
[database-stored authorization tokens](/usage/authorize#database-stored-tokens)
and is shared with other users via the Drive shared file feature.

Note that using the workaround is a hazardous compromise of a
legacy user's security purely for the sake of compatibility. The user
bears all responsibility for the risks of using this workaround. 
Because it involves deprecated functionality this workaround may
be removed in a future release. Please review the
[security bulletin](/notices/sharedsec)
for more information regarding safe use of shared databases.

<div class="alert alert-secondary" role="alert">
<div>NOTE</div>
The safety of the workaround is slightly enhanced when the
<a href="../install/config#sync-authorization">Use limited Drive access</a>
option is enabled, which effectively limits the Drive account's
exposure to plugin-synchronized files only.
</div>
