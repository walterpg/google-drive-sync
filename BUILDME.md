# Building and Releasing
### Build Tools
* Visual Studio for Windows. Currently, releases are built with
[Visual Studio Community](https://visualstudio.microsoft.com/vs/community),
but any updated version/edition of Visual Studio 2017 or later should suffice.
* [nuget.exe](https://www.nuget.org/downloads/) command-line tool in the current PATH.
* [Zip](https://en.wikipedia.org/wiki/ZIP_(file_format)) command-line tool
([7zip](https://www.7-zip.org/) recommended).
* (optional) [Pandoc](https://pandoc.org/) command-line tool

### Setup
Review these ``set`` commands in ``BuildMe.bat``:

```
set sevenzip="C:\Program Files (x86)\7-Zip\7z.exe"
set msbuildcmd="C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\Tools\VsMSBuildCmd.bat"
set pandoc="%LOCALAPPDATA%\Pandoc\pandoc.exe"
```

Modify them if necessary to match the paths to the ``VsMSBuildCmd.bat`` and zip
tool.  Note that ``pandoc`` is used to generate an "ascii" version of README.md
and is not required to successfully build the project.

### Build
Run ``BuildMe.bat``.

### Products
If successful, the ``build`` directory contains three subdirectories:
* ``bin``: "raw" build output, including pdbs, etc.
* ``dist``: release PLGX, ZIP, and text documentation files.
* ``log``: contains build status logging.

As a convenience, the build script also updates the version manifest
file ``kpsync_version_4.txt``.  See
[Posting a Release](#posting-a-release) below for details.

### Preparing for Release
Release versioning should be clear and consistent, and is critical for
supporting the "Check for Updates" command in KeePass.

Update the release tag version strings with [SemVer](https://semver.org)-compatible version strings:
* ``set version=`` at the top of ``BuildMe.bat``
* ``src\Properties\AssemblyInfo.cs``: all ``a.b.c`` version values should be the same,
and the ``.d`` suffix, *if* specified, should always be 0.  For example, ``0.1.0.0``. 

Suppose the version of the current development cycle is
``0.1.0-unstable``.  Thus the release tag could be
``0.1.0-alpha``, which will be used in the following examples.

The release tag string must be passed to the attribute constructor of
``AssemblyInformationalVersion`` in ``AssemblyInfo.cs``:

```
[assembly: AssemblyInformationalVersion("0.1.0-alpha")]
```

The ``AssemblyVersion`` and ``AssemblyFileVersion`` attributes should only
be assigned ``a.b.c`` or preferrably ``a.b.c.d`` version numbers.  They should
also "match" the release tag string:

```
[assembly: AssemblyVersion("0.1.0.0")]
[assembly: AssemblyFileVersion("0.1.0.0")]
```

Finally, the same release tag string must be used to update ``BuildMe.bat``.
Using the same example above:

```
set version=0.1.0-alpha
```

### Tagging a Release

Document the release in ``ChangeLog.md``.

Commit ``AssemblyInfo.cs``, ``ChangeLog.md``, and any remaining 
release changes.  Only commit ``BuildMe.bat`` if something else besides the version
number changed.  

*DO NOT* commit changes, if any, to the following files:
```
kpsync_version_4.txt
src/GdsDefs.OAuthCreds.txt
```

You may eventually commit ``kpsync_version_4.txt``,
if you [post a release](#posting-a-release), **but the other file
must never be committed to remote**. And it should remain in the repo
as-is.

Ensure there are no build errors by running ``BuildMe.bat``. 

Tag the release with the release tag string used to set the version in
the [previous section](#preparing-for-release).

Before running the "final" release build, edit ``GdsDefs.OAuthCreds.txt``
to insert the valid "built-in" credentials. Again, never commit this file to
remote, lest you post your clear-text OAuth 2.0 credentials to the world.

Produce the release binaries with ``BuildMe.bat``.

### Posting a Release

"Draft" a new release on github with the tag created in the [previous
section](#tagging-a-release).

Try to do all of the following the steps, after the first, in a single
commit/push (the first step doesn't require a commit). This helps
ensure that new releases are not detected by the public or KeePass
before they are actually available.

1. Upload the binaries.  At the moment these are simply appended as
"assets" to the github tag/release post.
2. Update the links to the new binaries in the repo README and website.
3. Ensure ``kpsync_version_4.txt`` is correct before committing it.
It is mechanically updated by ``BuildMe.bat``, and if it doesn't match the
tagged release version, there is something wrong in ``AssemblyInfo.cs``; fix
it, run ``BuildMe.bat`` again, and start again at step 1.
4. Update ``AssemblyInformationalVersion`` in ``AssemblyInfo.cs`` 
a final time, to reflect the next, post-release development cycle. For
example, ``0.1.0-alpha`` &#x21D2; ``0.1.0-unstable``.

Commit and push the above changes.  If this is a versioned release,
verify that KeePass detects it via the "Check for Updates" command.
 