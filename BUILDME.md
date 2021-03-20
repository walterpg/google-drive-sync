# Building and Releasing
### Build Tools
* Visual Studio for Windows. Currently, releases are built with [Visual Studio Community](https://visualstudio.microsoft.com/vs/community), but any updated version/edition of Visual Studio 2017 or later should suffice.  A migration to `dotnet` CLI is in work, but it will likely require a higher target framework dependency for the plugin.
* [Zip](https://en.wikipedia.org/wiki/ZIP_(file_format)) command-line tool ([7zip](https://www.7-zip.org/) recommended).
* (optional) [Pandoc](https://pandoc.org/) command-line tool

### Setup
Review these ``set`` commands in ``BuildMe.bat``:

```
set sevenzip="C:\Program Files (x86)\7-Zip\7z.exe"
set msbuildcmd="C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\Tools\VsMSBuildCmd.bat"
set pandoc="%LOCALAPPDATA%\Pandoc\pandoc.exe"
```

Modify them if necessary to match the paths to the ``VsMSBuildCmd.bat`` and zip tool.  Note that ``pandoc`` is used to generate a plain-text version of README.md for a software release, and is not required to successfully build the project.

### Build
Run ``BuildMe.bat``.

### Products
If successful, the ``build`` directory contains three subdirectories:
* ``bin``: "raw" build output, including pdbs, etc.
* ``dist``: release PLGX, ZIP, and text documentation files.
* ``log``: contains build status logging.

As a convenience, the build script also updates the version manifest file ``kpsync_final.txt``.  See [Posting a Release](#posting-a-release) below for details.

### Preparing for Release
Release version notation should be clear and consistent.  This is critical for supporting the "Check for Updates" command in KeePass.  KeePass looks for the `[AssemblyVersion]` attribute for the update check using the `v.r.u` three-integer scheme.  The `[AssemblyInformationalVersion]` attribute should be used to reflect the *matching* git tag for the release.

Create tag version strings with [SemVer](https://semver.org)-compatible strings:
* ``set versionPrefix=`` and ``set versionSuffix=`` at the top of ``BuildMe.bat`` to reflect the tagged version ID, e.g., "4.0.6-beta".
* ``versionPrefix`` will reflect the "check for updates" version. 

For example, if the tag version should be "4.0.6-beta" edit ``BuildMe.bat`` to modify the ``set`` commands as shown:
```
set versionPrefix=4.0.6
set versionSuffix=beta
```

### Tagging a Release

Document the release in ``ChangeLog.md``.

Commit ``ChangeLog.md``, and any remaining release changes.  Only commit ``BuildMe.bat`` if something else besides the version number changed.  

*DO NOT* commit changes, if any, to the following files:
```
kpsync_final.txt
src/GdsDefs.OAuthCreds.txt
```

You may eventually commit ``kpsync_final.txt``, if you [post a release](#posting-a-release), **but `src/GdsDefs.OAuthCreds.txt` must never be committed to remote**. And it should remain in the repo as-is.

>###### *Hint*
>The `GdsDefs.OAuthCreds.txt` file is a special case in source code control; it contains "placeholder" content that *usually* must be changed before a development activity but never committed. Ordinarily, leaving the file "untracked" and using the  `.gitignore` file is a good way to avoid committing sensitive data. The subtlety here is that the originally committed, tracked file contains "fake data" - useful for, say, a CI test build of a freshly cloned branch, but not for developing or releasing software. If the file were untracked and specified in `.gitignore`, such a build would fail without intervention between cloning the branch and building.  Also, in this case, the fake data includes information on its usage which should be retained. There is no single, perfect solution here known to this author. Below is *one* approach to the problem.
>
>Use the following [command](https://www.git-scm.com/docs/git-update-index#Documentation/git-update-index.txt---no-assume-unchanged) to set the "assume unchanged bit" on the file: 
>
> ```sh
>git update-index --assume-unchanged src/GdsDefs.OAuthCreds.txt`
>``` 
>This forces git to ignore changes to the file in the **local repo**, even though it is still "tracked" in the index and remote branch. This way, local changes to the file do not show in `git status`, VisualStudio, etc., helping to prevent accidental commits. This technique is particularly useful when preparing commits for a posted release.
> 
>Note however, the assume-unchanged bit may (safely) change the behavior of other git commands and tools. In the event of a commit of the file in the remote repo, errors will occur in `git pull`, et.al., performed on a local repo. In such cases, reset the bit (and for safety stash the local copy) then deal with the changed remote file:
>
> ```sh
>git update-index --no-assume-unchanged src/GdsDefs.OAuthCreds.txt
># Optionally stash the local changes.
>git stash push src/GdsDefs.OauthCreds.Txt
># Do the git op that failed before flipping the bit.
>git pull
># In the case of pull, etc., get local changes back to resolve differences.
>git stash pop
># Optionally set the bit again.
>git update-index --assume-unchanged src/GdsDefs.OAuthCreds.txt
>```

Ensure there are no build errors by running ``BuildMe.bat``. 

Tag the release with the release tag string used to set the version in the [previous section](#preparing-for-release).

Before running the "final" release build, edit ``GdsDefs.OAuthCreds.txt`` to insert the valid "built-in" credentials. Again, never commit this file to remote, lest you post your clear-text OAuth 2.0 credentials to the world.

Produce the release binaries with ``BuildMe.bat``.

### Posting a Release

"Draft" a new release on Github with the tag created in the [previous section](#tagging-a-release).

Try to do all of the following the steps, after the first, in a single commit/push (the first step doesn't require a commit). This helps
ensure that new releases are not detected by the public or KeePass before they are actually available.

1. Upload the binaries.  At the moment these are simply appended as "assets" to the Github tag/release post.
2. Ensure ``kpsync_final.txt`` is correct before committing it. It is mechanically updated by ``BuildMe.bat``, and if it doesn't match the tagged release version, there is something wrong with ``versionPrefix``; fix it, run ``BuildMe.bat`` again, and start again at step 1.
3. Update ``<VersionPrefix>`` in ``KPSyncForDrive.csproj``, to reflect the next, post-release development cycle. For
example, ``0.1.0-alpha`` &#x21D2; ``0.1.0-unstable``.

Commit and push the above changes.  If this is a generally-available release, install and verify that KeePass detects it via the "Check for Updates" command.
 