---
layout: post
title: KeePass v2.48 Save Command Failure
subtitle: Risk of Data Loss When Using KeePass 2.48 with the Plugin
author: walterpg
date: May 9, 2021
update: Jan 29, 2022
---

Users are advised to **avoid using KeePass v2.48**.  If you have KeePass v2.48 installed, we recommend upgrading to [v2.48.1](https://keepass.info/news/n210507_2.48.html) (or [later](https://keepass.info/download.html)) immediately.

If you [experience the problem](#detecting-the-problem) outlined in this document, please try the [workaround](#possible-workaround) given below, to possibly recover your database changes.  Before using the plugin again, [upgrade or downgrade KeePass from v2.48](#use-a-different-keepass-release).  If that is not currently a viable option, discontinue using the plugin.

## The Issue

KeePass v2.48 contains a [software bug](https://sourceforge.net/p/keepass/bugs/2048/) that can cause database **Save** command failures after syncing with the plugin.  Since the database Save command can fail, there is a potential risk of data loss.  

If you experience this problem, you may be able to recover the database by following the [steps outlined below](#possible-workaround).

## Use a Different KeePass Release

Users of KeePass v2.48 should upgrade to [KeePass v2.48.1 or later immediately](https://keepass.info/download.html) to continue using the plugin safely.  Though not recommended, you may also consider installing an [earlier or later](https://sourceforge.net/projects/keepass/files/KeePass%202.x/) KeePass release.  

The defect affects all plugin releases from v4.0.5-beta.  As of the 4.1 release, the plugin supports KeePass releases v2.35 through v2.50.  Though not recommended, you may also consider installing a plugin release prior to v4.0.5-beta.


## Detecting the Problem

You may encounter a KeePass error message similar to the following after a Save operation.  

{:refdef: style="text-align: center;"}
![KP Save Error Message](/assets/img/kp2-48-error.png)
{: refdef}

Note that any outstanding changes to the database are *not saved*, and cannot be saved [without intervention](#possible-workaround). Further, note that Save may be invoked implicitly in KeePass composite operations such as "auto lock" and "save on exit".

## Possible Workaround

If you see the error message above after using the plugin, please try the following steps to avoid losing your changes.

1. Click OK on the error message box, as shown above. **DO NOT EXIT KEEPASS**.  Doing so could cause unrecoverable data loss.
2. On the **Tools** menu, select **Database Tools** ⇒ **Database Maintenance**.
3. The **Database Maintenance** dialog is displayed as shown below.

    ![KP Database Maintenance](/assets/img/kp2-48-dbmaint.png)

4. Select the **Plugin Data** tab.  In the **Name**/**Value** list, select the item highlighted below, and click **Delete**.

    ![KP DB Plugin Data](/assets/img/kp2-48-pidata.png)

5. Close the dialog, and attempt to use **Save** again.

---
[View all Security Bulletins and Known Issues](/notices)