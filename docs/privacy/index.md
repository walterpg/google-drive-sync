---
layout: plain
title: Privacy Statements
---

---
The only concern of the content authors of this site is the publication
of useful, free software. There is no interest here in breaching the
fundamental human right of privacy, nor in the collection and abuse of
personal information.

---

## Site Privacy
This privacy statement applies only to this site. Please be aware when moving to
another site, and read the privacy statement of any other site(s) which may
collect personal information about you.

This site is hosted by [GitHub Pages](https://pages.github.com/), and
so is subject to GitHub [terms](https://help.github.com/en/github/site-policy/github-terms-of-service)
for hosted content, and the GitHub [privacy statement](https://help.github.com/en/github/site-policy/github-privacy-statement).

---


## Plugin Privacy
The plugin software uses Google-provided application programming
interfaces that allow you to access your Google Drive account within
KeePass.  It does so through the use of [open-standard authentication
protocols and frameworks](https://tools.ietf.org/html/rfc6749). 

Your login details, e.g., Google user name and password, are never 
accessed by the plugin software nor its component libraries.
The plugin works cooperatively with the web browser of your choice
and the [Google Sign-in](https://accounts.google.com/Login)
authentication service to explicitly obtain your consent to access 
Google Drive.  The plugin communicates with no other online services
or software.

When you authorize Google Drive access, the plugin is given
a digital token representing your consent, granted only to the plugin.
The token is saved to the currently open
KeePass database, and subsequent used in plugin commands accessing
Google Drive. The token is only valid for use by the plugin, and may be
[invalidated by you](https://myaccount.google.com/security) or Google at
any time. Since it is saved in the KeePass database, it is secure as all
other personal information saved there. 

Please note: authorizing the plugin implicitly grants access to some
identifying details of your Google account, such as your email address.
The plugin never requests such access, *and in no way exercises this 
trusted privelege*. Though Google unavoidably grants the extraneous
permission, the plugin ignores it and operates perfectly without it.

The surest guard of your digital privacy is one of the great benefits
of using open source software. If you have trouble trusting what the
plugin does with your personal information, [you've got the code](https://github.com/walterpg/google-drive-sync),
check it out!

#### OAuth 2.0 

As with Google Sync Plugin 3.0, this release offers flexible options for 
securing Google Drive account access.  You may use the plugin's
"built-in" OAuth 2.0 credentials, or [your own personal credentials](#obtaining-custom-oauth-20-credentials),
with the assurance that, from the perspective of KeePass and the
plugin, no personal information is logged, shared, or used
for any purpose other than to read and write the KeePass database to and
from Google Drive.  
