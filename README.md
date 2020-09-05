# [KeePass Sync for Google Drive™](https://kpsync.org)

[![GitHub license](https://img.shields.io/github/license/walterpg/google-drive-sync)](https://raw.githubusercontent.com/walterpg/google-drive-sync/master/LICENSE)

---
Beta release now available! If you currently have an Alpha release
installed, you should
[review the upgrade notes](https://kpsync.org/install/upgrade0).

If you currently authorize Drive access with the prior plugin's built-in
OAuth credentials, you may want to
[review the creds upgrade notes](https://kpsync.org/install/upgrade1).

Current release downloads and full docs on [kpsync.org](https://kpsync.org) including:
* [System Requirements](https://kpsync.org/install/require)
* [Compatibility](https://kpsync.org/install/require#compatibility-with-google-sync-plugin)
* [Installation](https://kpsync.org/install/normal)

Visit the [CONTRIBUTING](https://github.com/walterpg/google-drive-sync/blob/master/CONTRIBUTING.md) page if you would like to get involved.

Some Project ToDo's:
* More documentation generally.
* ~~Submit application to Google for verification (required for replacing the built-in credential authorization in new databases).~~
* Secure storage of default user-specified OAuth ClientId/Secret.
* Specify sign-in browser of choice.
* Language translations[*](#localization).
* [Bugfixes and enhancement requests](https://github.com/walterpg/google-drive-sync/issues).

---
## Localization
The plugin supports natural languages with standard .NET/Visual 
Studio resource files.  No *genuine* translations are available - yet.
  
There is a 
[demo resource](https://github.com/walterpg/google-drive-sync/blob/master/src/Strings.es.resx)
that will load when the user loads the 
[KeePass translation files](https://keepass.info/translations.html) for Spanish.
The strings in that resource were machine translated - if you are able and willing to translate
Spanish or any other KeePass-supported language please get in touch or submit PRs.
