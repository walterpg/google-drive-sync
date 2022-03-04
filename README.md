# [KPSync for Google Drive™](https://www.kpsync.org)

[![GitHub license](https://img.shields.io/github/license/walterpg/google-drive-sync)](https://raw.githubusercontent.com/walterpg/google-drive-sync/master/LICENSE)

---
Download the current release using the link above. If you are upgrading from an Alpha release (v4.0.0 or v4.0.1), you should
[review the upgrade notes](https://www.kpsync.org/install/upgrade0).

If you are replacing Google Sync Plugin 3.0, or authorize Drive access with Google Sync Plugin 3.0's built-in
OAuth credentials, you may want to
[review the creds upgrade notes](https://www.kpsync.org/install/upgrade1).

Full documentation is available at [kpsync.org](https://www.kpsync.org) including:
* [System Requirements](https://www.kpsync.org/install/require)
* [Compatibility](https://www.kpsync.org/install/require#compatibility-with-google-sync-plugin)
* [Installation](https://www.kpsync.org/install/normal)

Visit the [CONTRIBUTING](https://github.com/walterpg/google-drive-sync/blob/master/CONTRIBUTING.md) page if you would like to get involved.

Some Project ToDo's:
* More documentation generally.
* Default option to cache Google auth tokens outside of the database.
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

---
### Acknowledgements
In addition to [KeePass](https://keepass.info), this project relies on the following support libraries, as distributed by [nuget.org](https://nuget.org).

| Project      | Package                  | License                  |
|--------------|--------------------------|--------------------------|
| [Google APIs Client Library for .NET](https://github.com/googleapis/google-api-dotnet-client)|[Google.Apis.Drive.v3](https://www.nuget.org/packages/Google.Apis.Drive.v3/)|[Apache v2.0](https://github.com/googleapis/google-api-dotnet-client/blob/master/LICENSE)|
| [Json.NET](https://github.com/JamesNK/Newtonsoft.Json) | [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/) | [X11 (MIT)](https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md)|
| [PLGX Build Tasks](https://github.com/walterpg/plgx-build-tasks)| [PlgxBuildTasks](https://www.nuget.org/packages/PlgxBuildTasks/) | [GPL v3.0](https://github.com/walterpg/plgx-build-tasks/blob/main/COPYING)|
| [Serilog](https://github.com/serilog/serilog)| [Serilog](https://www.nuget.org/packages/Serilog), [Serilog.Sinks.File](https://www.nuget.org/packages/Serilog.Sinks.File) |[Apache v2.0](https://github.com/serilog/serilog/blob/dev/LICENSE)|