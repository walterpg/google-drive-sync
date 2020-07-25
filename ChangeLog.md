

# 4.0.1-alpha.2
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