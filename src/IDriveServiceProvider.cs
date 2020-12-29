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
using System;
using System.Threading.Tasks;

namespace KeePassSyncForDrive
{
    public interface IDriveServiceProvider
    {
        /// <summary>
		/// Authenticate and authorize the drive service and invoke a user
		/// function. Obtain and update authorization details from/to the
		/// current configuration.
        /// </summary>
        /// <param name="use">Delegate is passed two parameters:
        /// DriveService: The authorized Google Drive API service object.
        /// SyncConfiguration: The user's configuration for the current op.
        /// The delegate returns a final, localized status message, or null.</param>
        /// <returns>Error status string or null.</returns>
        Task<string> ConfigAndUseDriveService(
            Func<DriveService, SyncConfiguration, Task<string>> use);

        /// <summary>
		/// Authenticate and authorize the drive service with given 
        /// authorization data, and invoke a user function.  If 
        /// authorization produces a drive service refresh token, the token
        /// property is updated in the caller's authorization data structure.
        /// </summary>
        /// <param name="use">Delegate is passed two parameters:
        /// DriveService: The authorized Google Drive API service object.
        /// SyncConfiguration: The user's configuration for the current op.
        /// The delegate returns a final, localized status message, or null.</param>
        /// <returns>Error status string or null.</returns>
        Task<string> UseDriveService(SyncConfiguration authData,
            Func<DriveService, SyncConfiguration, Task<string>> use);
    }
}
