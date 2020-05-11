# KeePass Google Drive Sync

[![GitHub license](https://img.shields.io/github/license/walterpg/google-drive-sync)](https://raw.githubusercontent.com/walterpg/google-drive-sync/master/LICENSE)

##### A [KeePass Password Safe](https://keepass.info) Plugin and fork of the original [KeePass Google Sync](http://sourceforge.net/projects/kp-googlesync/) Plugin

---
Google Drive Sync helps maintain synchronized KeePass databases accessible to your KeePass-compatible apps via Google Drive.

[Documentation](https://github.com/walterpg/google-drive-sync/blob/master/doc).

[PLGX Download](https://github.com/walterpg/google-drive-sync/releases/download/4.0.0-alpha.1/GoogleDriveSync-4.0.0-alpha.1.plgx).

[Zip Download](https://github.com/walterpg/google-drive-sync/releases/download/4.0.0-alpha.1/GoogleDriveSync-4.0.0-alpha.1.zip).

Some Features:
* Feature-compatible with KeePass Google Sync Plugin: manual upload/download, sync, and auto-sync.
* Browser-based OAuth 2.0 authentication flow enhanced with KeePass clipboard clickies.
* Default user-defined ClientId config and automated database initialization.
* Deeper integration with Google Drive, including folders, scopes, etc.
* Simplified UI with KeePass theme integration and material.io icons.
* Updated Google libraries and new KeePass database format support.
* Localized language support[*](#localization).

Some ToDo's:
* More documentation generally.
* Submit application to Google for verification (required for replacing the built-in credential authorization in new databases).
* Secure storage of default user-specified OAuth ClientId/Secret.
* Specify sign-in browser of choice.
* Language translations.
* Enhancement requests.

## Requirements
* KeePass 2.35 or later.
* .NET Framework 4.5 or later.
* Default Windows web browser capable of Google Sign-In (almost any modern browser will do).
* Any version of Windows that works with all of the above.

## Compatibility
* User-specified OAuth ClientId credentials or the v2.1 "built-in" credentials.
* Current, non-expired authorization tokens saved in the database will continue to work.
* Forward-compatibility: changes to a database configuration such as ClientId/Secret will not be visible to KeePass Google Sync.
* Limited-scope workaround for use with the expired "built-in" ClientId, or as a security feature.
* Recognizes the credential KeePass entries with the "accounts.google.com" URL field.

## *Localization
The plugin supports natural languages with standard .NET/Visual 
Studio resource files.  No *genuine* translations are available - yet.
  
There is a 
[demo resource](https://github.com/walterpg/google-drive-sync/blob/master/src/Strings.es.resx)
that will load when the user loads the 
[KeePass translation files](https://keepass.info/translations.html) for Spanish.
The strings in that resource were machine translated - if you are able and willing to translate
Spanish or any other KeePass-supported language please get in touch or submit PRs.
