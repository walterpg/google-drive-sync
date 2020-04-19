# Building and Releasing
### Build Tools
* Visual Studio for Windows (currently releases are built with
[Visual Studio Community](https://visualstudio.microsoft.com/vs/community),
but any updated Edition of Visual Studio >= 2017 should suffice).
* [nuget.exe](https://www.nuget.org/downloads/) command-line tool in the current PATH.
* [Zip](https://en.wikipedia.org/wiki/ZIP_(file_format)) command-line tool
([7zip](https://www.7-zip.org/) recommended) in the current PATH.

### Setup
Review these ``set`` commands in ``BuildMe.bat``:

```
set sevenzip="C:\Program Files (x86)\7-Zip\7z.exe"
set msbuildcmd="C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\Tools\VsMSBuildCmd.bat"
```

Modify them if necessary to match the paths to the ``VsMSBuildCmd.bat`` and zip
tool.

### Build
Run ``BuildMe.bat``.

### Products
If successful, the ``build`` directory contains a log, and two directories.
* ``bin``: "raw" build output, including pdbs, etc.
* ``dist``: release PLGX, ZIP, and documentation files.

### Preparing for Release
Update the release tag version strings:
* ``src\Properties\AssemblyInfo.cs``
* ``BuildMe.bat``

For example, if the version of the current development cycle is
``0.1.0-unstable``, the release tag [could be](http://semver.org/)
``0.1.0-alpha``.

Document the release in ``ChangeLog.md``.

For general "production" releases (*not* pre-release alpha, beta, etc.,
tagging), update ``current_version_manifest.txt`` also.

Push and tag the release.  

After the release is tagged (and before release binaries, if any, are posted),
push the ``AssemblyInfo.cs`` version strings again, to reflect the next
development cycle. For example, ``0.1.0-alpha`` => ``0.1.0-unstable``.


 