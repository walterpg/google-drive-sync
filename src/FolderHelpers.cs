/**
 * Google Sync Plugin for KeePass Password Safe
 * Copyright(C) 2012-2016  DesignsInnovate
 * Copyright(C) 2014-2016  Paul Voegler
 * 
 * KeePass Sync for Google Drive
 * Copyright(C) 2020       Walter Goodwin
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
**/

using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDriveFile = Google.Apis.Drive.v3.Data.File;


namespace KeePassSyncForDrive
{
    static class FileAndFolderExtensions
    {
        public static string QueryGdriveObjectName(this string This)
        {
            if (This == null)
            {
                throw new ArgumentNullException("This");
            }
            StringBuilder sb = new StringBuilder(This.Length * 2);
            foreach (char c in This)
            {
                switch (c)
                {
                    case FolderName.EscapeChar:
                    case '\'':
                        sb.Append(FolderName.EscapeChar);
                        break;
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        public static string GetDisplayableGdriveObjectName(this string This)
        {
            if (This == null)
            {
                throw new ArgumentNullException("This");
            }
            StringBuilder sb = new StringBuilder(This.Length * 2);
            foreach (char c in This)
            {
                if (c == FolderName.EscapeChar)
                {
                    sb.Append(c);
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        public static string TrimLeadingAndTrailingSeparators(this string This)
        {
            if (This == null)
            {
                throw new ArgumentNullException("This");
            }
            int iBegin = 0;
            while (iBegin < This.Length)
            {
                if (!FolderName.FolderSeparators.Contains(This[iBegin]))
                {
                    break;
                }
                if (This[iBegin] == FolderName.EscapeChar &&
                    iBegin+1 < This.Length &&
                    FolderName.FolderSeparators.Contains(This[iBegin + 1]))
                {
                    break;
                }
                iBegin++;
            }
            int iEnd = This.Length;
            while (iEnd > iBegin)
            {
                if (!FolderName.FolderSeparators.Contains(This[iEnd - 1]))
                {
                    break;
                }
                if (iBegin < iEnd-1 &&
                    This[iEnd - 2] == FolderName.EscapeChar)
                {
                    break;
                }
                iEnd--;
            }
            return iBegin < This.Length ?
                This.Substring(iBegin, iEnd - iBegin) :
                string.Empty;
        }
    }

    // Folders and their names are an odd, many-to-one affair.  That is,
    // a single folder, including root, may have multiple files and folders
    // with the same name.  So the classes below model the notion of a 
    // folder hierarchy "path", that may consist of several complete and/or
    // incomplete sets of folders. This allows the user to inspect the 
    // hierarchies in Drive for existence and uniqueness, with an option to
    // create a new, complete hierarchy if desired.

    // A FolderName instance is one link in a "path" of folder names, doubly-
    // linked to its parent and child.  The first link (with no
    // ParentFolderName) is a root-level folder name.  The Folders list
    // contains the folders with the current link's name, each situated in
    // a unique hierarchy of folders whose names correspond to a list of
    // names beginning with the first link.  A "complete" hierarchy is one
    // whose last folder is in the Folder list of the last FolderName link.
    class FolderName
    {
        public const char EscapeChar = '\\';
        public static char[] FolderSeparators = new []{
            Path.AltDirectorySeparatorChar,
            Path.DirectorySeparatorChar
        };

        // Produce ordered list of folder names.  Folder names can
        // contain file separator chars if escaped with '\'.
        // Ignore leading and trailing separators.  Return the top-level.
        public static FolderName GetFolderNameLinks(string path)
        {
            FolderName currentFolder = null;
            StringBuilder sb = new StringBuilder();
            int iPath = 0;
            while (iPath < path.Length)
            {
                char c = path[iPath];
                iPath++;
                if (c == EscapeChar &&
                    iPath < path.Length &&
                    FolderSeparators.Contains(path[iPath]))
                {
                    sb.Append(path[iPath]);
                    iPath++;
                }
                else if (!FolderSeparators.Contains(c))
                {
                    sb.Append(c);
                }
                else
                {
                    if (sb.Length > 0)
                    {
                        FolderName fn = new FolderName(sb.ToString(),
                                                        currentFolder);
                        if (currentFolder != null)
                        {
                            currentFolder.ChildFolderName = fn;
                        }
                        currentFolder = fn;
                    }
                    sb.Clear();
                }
            }
            if (sb.Length > 0)
            {
                FolderName fn = new FolderName(sb.ToString(),
                                                currentFolder);
                if (currentFolder != null)
                {
                    currentFolder.ChildFolderName = fn;
                }
                currentFolder = fn;
            }
            while (currentFolder != null &&
                currentFolder.ParentFolderName != null)
            {
                currentFolder = currentFolder.ParentFolderName;
            }
            return currentFolder;
        }


        readonly string m_name;
        readonly FolderName m_parentName;
        readonly List<Folder> m_folders;

        public FolderName(string name, FolderName parentName)
        {
            m_name = name;
            m_parentName = parentName;
            m_folders = new List<Folder>();
            ChildFolderName = null;
        }

        public override string ToString()
        {
            return m_name == null ?
                string.Empty :
                m_name.GetDisplayableGdriveObjectName();
        }

        public FolderName ParentFolderName
        {
            get
            {
                return m_parentName;
            }
        }

        public FolderName ChildFolderName { get; set; }

        public List<Folder> Folders
        {
            get
            {
                return m_folders;
            }
        }

        // Create a new folder hierarchy, parented by the first Folder in
        // the ParentFolderName link (or root if this is the top link).
        // The new hierarchy will be a set of nested folders, as modeled by
        // the "path" of the FolderName chain.
        // The top-most folder in the hierarchy will take the FolderName of
        // this link.  Return the "leaf" Folder of the hierarchy.
        public async Task<Folder> CreateNewFolderPathFrom(Folder parent,
            DriveService service, Action<string> feedback
            )
        {
            if (feedback != null)
            {
                string status = Resources.GetFormat(
                                    "Msg_CreatingFolderFmt",
                                    m_name);
                feedback(status);
            }

            GDriveFile folderMetadata = new GDriveFile()
            {
                Name = m_name,
                MimeType = GdsDefs.FolderMimeType
            };
            if (parent != null)
            {
                folderMetadata.Parents = new List<string>()
                {
                    parent.DriveEntry.Id
                };
            }
            if (PluginConfig.Default.FolderColor != null)
            {
                folderMetadata.FolderColorRgb =
                    PluginConfig.Default.FolderColor.HtmlHexString;
            }
            FilesResource.CreateRequest folderCreate;
            folderCreate = service.Files.Create(folderMetadata);
            folderCreate.Fields = "id";

            Folder newFolder = new Folder(this, parent,
                await folderCreate.ExecuteAsync());
            Folders.Add(newFolder);
            if (ChildFolderName != null)
            {
                return await ChildFolderName
                    .CreateNewFolderPathFrom(newFolder, service, feedback);
            }
            return newFolder;
        }

        // Return the set of existing folders found at the end of the
        // chain of the "path" represented by this FolderName.  If no such
        // folders exist, return an empty list.
        public async Task<List<Folder>> ResolveLeafFolders(
            DriveService service, Action<string> feedback)
        {
            // Find all folders named m_name, whose parents are in the 
            // ParentFolderName list.  For each folder found, add a folder
            // object to the list, then invoke this method on ChildFolderName.
            Folders.Clear();
            List<Folder> parentFolders;
            if (ParentFolderName != null)
            {
                parentFolders = ParentFolderName.Folders;
            }
            else
            {
                parentFolders = new List<Folder>()
                {
                    Folder.RootFolder
                };
            }

            if (feedback != null)
            {
                string status = Resources.GetFormat(
                                    "Msg_RetrievingFolderFmt",
                                    m_name);
                feedback(status);
            }

            foreach (Folder parent in parentFolders)
            {
                FilesResource.ListRequest req = service.Files.List();
                req.Q = "mimeType='" + GdsDefs.FolderMimeType +
                        "' and name='" +
                        m_name.QueryGdriveObjectName() +
                        "' and '" +
                        parent.DriveEntry.Id +
                        "' in parents and trashed=false";
                FileList appFolders = await req.ExecuteAsync();
                foreach (GDriveFile entry in appFolders.Files)
                {
                    Folders.Add(new Folder(this, parent, entry));
                }
            }
            if (Folders.Any() && ChildFolderName != null)
            {
                return await ChildFolderName.ResolveLeafFolders(service,
                                                            feedback);
            }
            return Folders;
        }

        // Return this FolderName or the first of its successors that does not
        // have folders. Used to find an existing partial path after 
        // ResolveLeafFolders returns an empty list. Assuming 
        // ResolveLeafFolders has been called, the return value's
        // ParentFolderName property value is either null (root), or an
        // instance whose Folders property contains at least one Folder.
        public FolderName FirstFolderNameWithoutFolders()
        {
            if (Folders.Any())
            {
                if (ChildFolderName != null)
                {
                    return ChildFolderName.FirstFolderNameWithoutFolders();
                }
                Debug.Fail(
                    "All FolderNames have folders; ResolveLeafFolders failed");
                throw new ApplicationException(
                    "Expected empty folders in Target Folder chain.");
            }
            return this;
        }
    }

    class Folder
    {
        public static readonly Folder RootFolder = new Folder(null, null,
            new GDriveFile() { Id = "root" });

        readonly FolderName m_name;
        readonly Folder m_parent;
        readonly GDriveFile m_entry;

        public Folder(FolderName name, Folder parent, GDriveFile entry)
        {
            m_entry = entry;
            m_name = name;
            m_parent = parent;
        }

        // Returns an escaped path to this folder.
        public override string ToString()
        {
            if (m_name == null || m_parent == null)
            {
                return string.Empty;
            }
            return m_parent.ToString() + '/' + m_name.ToString();
        }

        public GDriveFile DriveEntry
        {
            get
            {
                return m_entry;
            }
        }
    }
}
