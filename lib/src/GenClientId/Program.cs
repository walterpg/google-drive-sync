/**
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

using System.Diagnostics;
using System.IO;
using System.Text;

namespace GenClientId
{
    class Program
    {
        static void Main(string[] args)
        {
            // FOR TESTING PURPOSES ONLY.

            // Two arguments:
            // 1. The path of a source file.
            // 2. The path of a target file.
            //
            // The UTF8-encoded source file must contain two lines of text:
            // 1. An OAuth2.0 client ID.
            // 2. The cleartext secret associated with the client ID
            //

            Generator gen = new Generator()
            {
                SourceFilePath = args[0]
            };
            gen.Run();

            StringBuilder src = new StringBuilder(@"
namespace KeePassSyncForDrive
{
    static partial class GdsDefs
    {
        static readonly byte[] s_legacyClientIdBytes = new byte[]
        {
");
            src.Append(gen.LegacyIdCsharpConstant);
            src.Append(@"
        };
        static readonly byte[] s_legacyClientIdPad = new byte[]
        {
");
            src.Append(gen.LegacyIdPadCsharpConstant);
            src.Append(@"
        };
        static readonly byte[] s_legacyClientSecretBytes = new byte[]
		{
");
            src.Append(gen.LegacySecretCsharpConstant);
            src.Append(@"
		};
		static readonly byte[] s_legacyClientSecretPad = new byte[]
		{
");
            src.Append(gen.LegacySecretPadCsharpConstant);
            src.Append(@"
		};
        static readonly byte[] s_clientIdBytes = new byte[]
        {
");
            src.Append(gen.ClientIdCsharpConstant);
            src.Append(@"
        };
        static readonly byte[] s_clientIdPad = new byte[]
        {
");
            src.Append(gen.ClientIdPadCsharpConstant);
            src.Append(@"
        };
        static readonly byte[] s_clientSecretBytes = new byte[]
		{
");
            src.Append(gen.ClientSecretCsharpConstant);
            src.Append(@"
		};
		static readonly byte[] s_clientSecretPad = new byte[]
		{
");
            src.Append(gen.ClientSecretPadCsharpConstant);
            src.Append(@"
		};

	}
}");

            Debug.WriteLine(src.ToString());

            using (StreamWriter sw = File.CreateText(args[1]))
            {
                sw.Write(src.ToString());
            }
        }
    }
}
