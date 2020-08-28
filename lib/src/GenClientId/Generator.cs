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

using KeePassLib.Security;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GenClientId
{
    public class Generator
    {
        static string GenSrc(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                if (0 == i % 8)
                {
                    sb.Append("            ");
                }
                sb.AppendFormat("0x{0:x2},", bytes[i]);
                if (0 != (i + 1) % 8 &&
                    (i + 1) < bytes.Length)
                {
                    sb.Append(' ');
                }
                else
                {
                    sb.Append(Environment.NewLine);
                }
            }
            return sb.ToString();
        }

        public Generator()
        {
        }

        public string SourceFilePath { get; set; }

        public string LegacyIdCsharpConstant { get; set; }
        public string LegacyIdPadCsharpConstant { get; set; }
        public string LegacySecretCsharpConstant { get; set; }
        public string LegacySecretPadCsharpConstant { get; set; }
        public string ClientIdCsharpConstant { get; set; }
        public string ClientIdPadCsharpConstant { get; set; }
        public string ClientSecretCsharpConstant { get; set; }
        public string ClientSecretPadCsharpConstant { get; set; }

        byte[] LegacyIdBytes { get; set; }
        byte[] LegacyIdPadBytes { get; set; }
        byte[] LegacySecretBytes { get; set; }
        byte[] LegacySecretPadBytes { get; set; }
        byte[] ClientIdBytes { get; set; }
        byte[] ClientIdPadBytes { get; set; }
        byte[] ClientSecretBytes { get; set; }
        byte[] ClientSecretPadBytes { get; set; }

        public void Run()
        {
            using (StreamReader reader = File.OpenText(SourceFilePath))
            {
                LegacyIdBytes = Encoding.UTF8.GetBytes(
                                    reader.ReadLine().Trim());
                LegacySecretBytes = Encoding.UTF8.GetBytes(
                                    reader.ReadLine().Trim());
                ClientIdBytes = Encoding.UTF8.GetBytes(
                                    reader.ReadLine().Trim());
                ClientSecretBytes = Encoding.UTF8.GetBytes(
                                    reader.ReadLine().Trim());
                LegacyIdPadBytes = new byte[LegacyIdBytes.Length];
                LegacySecretPadBytes = new byte[LegacySecretBytes.Length];
                ClientIdPadBytes = new byte[ClientIdBytes.Length];
                ClientSecretPadBytes = new byte[ClientSecretBytes.Length];

                using (RandomNumberGenerator rng = 
                            RandomNumberGenerator.Create())
                {
                    rng.GetNonZeroBytes(LegacyIdPadBytes);
                    rng.GetNonZeroBytes(LegacySecretPadBytes);
                    rng.GetNonZeroBytes(ClientIdPadBytes);
                    rng.GetNonZeroBytes(ClientSecretPadBytes);
                }

                XorredBuffer clientIdBuf = new XorredBuffer(LegacyIdBytes,
                                                            LegacyIdPadBytes);
                XorredBuffer secretBuf = new XorredBuffer(LegacySecretBytes,
                                                        LegacySecretPadBytes);

                LegacyIdCsharpConstant = GenSrc(clientIdBuf.ReadPlainText());
                LegacyIdPadCsharpConstant = GenSrc(LegacyIdPadBytes);
                LegacySecretCsharpConstant = GenSrc(secretBuf.ReadPlainText());
                LegacySecretPadCsharpConstant = GenSrc(LegacySecretPadBytes);

                clientIdBuf = new XorredBuffer(ClientIdBytes,
                                                ClientIdPadBytes);
                secretBuf = new XorredBuffer(ClientSecretBytes,
                                                ClientSecretPadBytes);

                ClientIdCsharpConstant = GenSrc(clientIdBuf.ReadPlainText());
                ClientIdPadCsharpConstant = GenSrc(ClientIdPadBytes);
                ClientSecretCsharpConstant = GenSrc(secretBuf.ReadPlainText());
                ClientSecretPadCsharpConstant = GenSrc(ClientSecretPadBytes);
            }
        }
    }
}
