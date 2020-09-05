# 4.0.2-beta
* Ship Google-verified OAuth 2.0 creds (woo-hooo!!).
* Add upgrade prompt, shown once for those using the legacy creds.
* Rebranding as required by the service provider.
* Fix satellite resource dll deployment.
* Change binary distribution blob names.
* Migrate to proposed OAuth 2.0 client_id.
* Add "limited access" detail docs.
* Publish [https://kpsync.org](https://kpsync.org) (nee gdrivesync.org).
* Warn user when changing auth method if there is a refresh_token
present.
* Add "issue" templates, CONTRIBUTING, and CODE_OF_CONDUCT.
* Beautify "about" tab.

##### Release Note
New builtin OAuth 2.0 creds are here, and about time too!
These are freshly minted, fully Google-approved, and project
(re)branded for your security and simple pleasure!  Full 
compatibility with prior creds is retained (both old-plugin builtin,
and user customized), but the new creds will be the default option
going forward.  NO MORE scary sign-in screens! (Unless that's what
you prefer!)

Unfortunately, rebranding required for verification changed
some names and such.  Please review the UPGRADE docs for more
info.

Also, everything worth documenting is now found at
[kpsync.org](https://kpsync.org).  As always, we want to know
how the plugin works for **you**, whether bad or good (please
submit an "issue").

---
## 4.0.1-alpha.2
* Fix [MRU bug](https://github.com/walterpg/google-drive-sync/issues/2).
* Fix broken links, typos.  Some doc changes.
* Implement selective command disabling feature.
* Fix [drive scope](https://github.com/walterpg/google-drive-sync/issues/3)
selection and related configuration issues.

##### Release Note
Many thanks to those who have started using the new plugin.  Your
feedback is always valuable and welcome.

Google Drive OAuth 2.0 authorization has become tricky business for Google-unverified
applications such as this.  Even using your own OAuth credentials may 
result in scary-looking warnings when authenticating your account, especially
in the Chrome browser.  And the built-in OAuth credentials may not work at all,
or only in a limited scope.  Please see the
[Default OAuth](https://github.com/walterpg/google-drive-sync/issues/3#issuecomment-637851334)
topic for more info, and bear with us as we prepare to apply for verification.

---
## 4.0.0-alpha.1
* Prepare to go public.
* Reasonable first draft of documentation.
* Release build tool for signing the KeePass version manifest.
* Release build tool for managing "built-in" credentials.
* Fix global config bug.

---
## 4.0.0-alpha
First alpha.