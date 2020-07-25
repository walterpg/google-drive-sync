# Contributing

Contributions that significantly improve the plugin are always welcome. Especially welcome are [C# and .NET Framework coding](#code-contributions), and [translations](#translation-contributions).  Here are a few guidelines for those kind enough to consider making the effort. 

### Code Contributions

Code contributions to the master branch generally fall into two categories, [bugfixes](#bugfixes) and [feature enhancements](feature-enhancements). In each case, some guidelines are offered below in order to prevent duplicate or otherwise unnecessary efforts.

A contributor must *minimally* ensure that any code contribution passes the following "smoke tests" before submission:

* The contribution must not disable or negatively effect the performance of any existing feature, unless the community has pre-approved such a submission.
* The contribution should be locally branched or merged from the current tip of the remote master branch.
* The merge must build without error using the development release build script.
* The products of an error-free release build must include the .PLGX and .ZIP binary archives.
* The products must install and run with the oldest currently-supported versions of KeePass and .NET Framework.
* The .PLGX product must install and run using the "standard" installed plugin scheme.
* The .ZIP product must install and run using the "standalone" installed plugin scheme.

#### Bugfixes

Before undertaking a bugfix submission:
* Ensure proper preparation and ability to submit a contribution that passes the smoke tests mentioned above.
* Thoroughly search the database of issues, open and closed, to ensure that the bug has not already been reported and tasked.
* If no matching issue is found, report the bug in a new issue.
* If a bug report is untasked, demonstrate, via post in the issue thread, knowledge of the existing code and outline a proposed remedy suitable for review by the community.
* At the risk of quick rejection, simultaneous issue and bugfix pull request submissions are allowed.
* Otherwise, allow the maintainers a reasonable period to review and the assign the issue.
* Always ensure smoke tests pass before submitting the pull request.

#### Feature Enhancements

New features *should* meet with the approval of the community, and pull requests *must* be approved by the maintainers. In reviewing feature requests and submissions, maintainers will ensure that the feature is feasible in terms of

1. The scope of plugin's intended functionality.
2. The capabilities of the currently supported platforms.
3. Community support for the feature, actual or percieved.

Before undertaking a feature submission, please note:
* Contributors are *urged* though not *required* to raise a new issue explaining the feature/enhancement.
* The issue content should indicate contributor ability and willingness to work on the issue.
* Allow the community a reasonable period to review, and the maintainers to assign, the task.
* Pull requests that do not have the approval of the community may be delayed or rejected.
* Approval of a large or sensitive feature submissions can be an iterative process, requiring a contributor to consider modifications requested by the maintainers and/or community.
* Unapproved or very large submissions may require separate development on another branch or repo.

### Translation Contributions

Text translations of plugin strings should be submitted as pull requests.  Translations are *greatly* valued, but for the purposes of cultural sensitivity, initial pull requests should indicate the contributor's ability to express reasonably grammatically-correct English language, and ideally should cite, publicly or privately, verifiable references to prior open source translation works.  

The default natural language for the plugin is culturally-neutral English ('en'), thus any translation effort must be based on the contents of the source file Strings.resx.  Pull requests for new translations must be submitted as a new file, named Strings.xx.resx, where xx is the two-letter [ISO 639-1](https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes) code for the target language.  KeePass requires the ISO 639-1 standard for translations.  Pull requests for existing translation modifications should justify, in English and the target language, significant reasons for the request.

Once a translation pull request has been merged, the contributor may volunteer (and will likely be asked!) to become a maintainer of the translation.
