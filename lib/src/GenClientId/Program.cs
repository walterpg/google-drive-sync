using KeePassLib.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Diagnostics;

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
namespace GoogleDriveSync
{
    static partial class GdsDefs
    {
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
