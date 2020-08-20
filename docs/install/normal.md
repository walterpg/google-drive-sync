---
layout: page
title: Normal Installation
---

* [Installation](#installation-steps)
* [Upgrade](#upgrading)

Normal installation is recommended to those who like to run KeePass like any
other Windows program on their computer, and is highly recommended when KeePass
has been [installed with the KeePass installer](https://keepass.info/help/v2/setup.html#installer).

Alternatively, some may prefer the ["portable" installation method](portable).

---

#### Installation Steps
Please review the general [KeePass instructions for installing plugins](https://keepass.info/help/v2/plugins.html)
before proceeding.  Also, ensure that all [system requirements](require) are met.

Use the [Download .PLGX button](/) to obtain a file named
-x.x.x.plgx, where 'x.x.x' is the release version.  For
example "KeePassSyncForDrive-4.0.1.plgx" or "KeePassSyncForDrive-4.0.1-beta.plgx".

1. Copy the downloaded .plgx file to the KeePass installation directory, for
example "C:\Program Files (x86)\KeePass Password Safe 2".  You probably need to
be signed on with an account in the [Windows "local Administrators group"](https://docs.microsoft.com/en-us/windows/security/identity-protection/access-control/local-accounts#sec-administrator).
Otherwise Windows may prevent the copy with "Access Denied" or similar messages.
2. Restart KeePass.  Please ensure that KeePass is fully shut down and not simply
minimized to the task bar or [notification tray](https://docs.microsoft.com/en-us/windows/win32/shell/notification-area) before restarting.

When KeePass starts, a short delay occurs to initialize new plugin(s)
you have installed.  Verify that the plugin is installed by examining the
KeePass Tools menu:

{:refdef: style="text-align: center;"}
![KeePass Tools menu with Plugin submenu](../assets/img/tools-menu.png)
{: refdef}

To ensure you have the most current version of the plugin, click
**Check for Updates** on the KeePass Help menu:

{:refdef: style="text-align: center;"}
![Update Check Tool](../assets/img/update-check.png)
{: refdef}

----

#### Upgrading
To install a new version of the plugin to a normal-style installation, follow
these steps.

Use the [Download .PLGX](/) button to obtain the latest release' .zip archive.

1. Delete the old version's .plgx file from the KeePass installation
directory, for example "C:\Program Files (x86)\KeePass Password Safe 2".
Again, you probably need to be signed on with an account in the
[Windows "local Administrators group"](https://docs.microsoft.com/en-us/windows/security/identity-protection/access-control/local-accounts#sec-administrator).
Otherwise Windows may prevent the deletion with "Access Denied" or similar
messages.
2. Copy the newly downloaded .plgx file to the KeePass installation directory.
3. Restart KeePass.  Ensure that KeePass is fully shut down and not simply
minimized to the task bar or [notification tray](https://docs.microsoft.com/en-us/windows/win32/shell/notification-area) before restarting.
4. Verify the installation of the new version in the **Check For Updates**
window shown above.
