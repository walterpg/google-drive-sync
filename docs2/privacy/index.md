---
layout: page
title: Privacy Statements
---

The content authors associated with this site are solely focused on
publishing useful, free software. Your fundamental human right to 
privacy is carefully protected by both this site and the sofware.
The software made available here neither collects nor abuses your
personal information.

Please review the [Site Privacy](#site-privacy) section below
for information regarding website privacy, and in particular, the site
hosting service provider's policies.

---


### Plugin Privacy
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

When you authorize access, Google Drive gives the plugin
a digital token representing your consent, granted only to the plugin.
The token is saved to the currently open
KeePass database, and subsequently used in plugin commands accessing
Google Drive. The token is only valid for use by the plugin, and may be
[invalidated by you](https://myaccount.google.com/security) or Google at
any time. Since it is saved in the KeePass database, it is as secure as
all other personal information there. 

Please note: by authorizing the plugin, Google implicitly grants access
to some identifying details of your account, such as your email address.
The plugin never requests this access, *and in no way exercises this 
trusted privelege*. Though Google unavoidably grants the extraneous
permission, the plugin ignores it and operates perfectly without it.

The surest guard of your digital privacy is you.  And one of the great
benefits of using open source software is your right to critically
inspect [the code](https://github.com/walterpg/google-drive-sync).
The authors enthusiastically invite this criticizm to help improve
the software and hopefully gain your trust. 

---

### Site Privacy
This privacy statement applies only to this site. Please be aware when moving to
another site, and read the privacy statement of any other site(s) which may
collect personal information about you.

When visiting this site, "cookies" may be placed on your device (computer,
mobile, or tablet). A cookie is a small text file that is placed on
your device by websites you visit. The only purpose of cookies on this
site is "technical", that is, to provide form and function to your site
browsing experience. 

Some sites may use cookies to "personalize" or
"analyze" your usage; this site does not use cookies in this way.
Except for cookies that record your preferences or consent, all 
cookies issued by the site are created with "session lifetime".  That
is, the browser should delete them when your visit to the site ends.

To enhance responsiveness and navigation of content, this site uses
JavaScript. JavaScript is a programming language that runs programs, or
"scripts"; in this case, in your web browser.  The scripts
used by this site take advantage of a browser feature called "local 
storage" or "cache", which allows scripts to maintain the status of your
browser session, such as the currently displayed page.  Local storage
exists on your device, much like a cookie. The data placed in local storage
by this site includes no identifying personal information. Browsers may have
various features and settings that allow some non-standard control of
local storage, including the saving of local storage data to secondary,
or persistent storage on your device after the session ends. The
scripts on this site only require local storage to exist for as
long as you are browsing the site. 

Most web browsers offer settings allowing you to opt-out of the use of
cookies and/or JavaScript, and some browser plugins may allow you to
review and block some or all cookies and scripts offered by a
site.  Note that blocking the functional cookies and scripts of
*this* site can negatively effect its ability to provide information
adequately.

Here is the current list of cookies in use on the site:

<table class="table table-bordered">
  <thead>
    <tr>
      <th scope="col">Name</th>
      <th scope="col">Realm</th>
      <th scope="col">Purpose</th>
      <th scope="col">Lifetime</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>EU_COOKIE_LAW_CONSENT</td>
      <td>Technical</td>
      <td>A record of the user’s consent to accept cookies in compliance with 
      <a href="https://eur-lex.europa.eu/legal-content/EN/TXT/?uri=CELEX:02009L0136-20091219">
        European Union Directive 2009/136/EC.
        </a></td>
      <td>30 days</td>
    </tr>
  </tbody>
</table>

>Note that the above list does not include cookies that the site's
hosting provider may issue, and over which the content authors have no
control.

This site is hosted by [GitHub Pages](https://pages.github.com/), and
so is subject to GitHub [terms](https://help.github.com/en/github/site-policy/github-terms-of-service)
for hosted content, and the GitHub [privacy statement](https://help.github.com/en/github/site-policy/github-privacy-statement).  Please review GitHub's
own cookie [policy statement](https://docs.github.com/en/github/site-policy/github-privacy-statement#our-use-of-cookies-and-tracking) there.

---

#### OAuth 2.0 

As with Google Sync Plugin 3.0, this release offers flexible options for 
securing Google Drive account access.  You may use the plugin's
"built-in" OAuth 2.0 credentials, or [your own personal credentials](#obtaining-custom-oauth-20-credentials),
with the assurance that, from the perspective of KeePass and the
plugin, no personal information is logged, shared, or used
for any purpose other than to read and write the KeePass database to and
from Google Drive.  
