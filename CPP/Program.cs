/***************************************************************************************************
  Description   : 
  Created       : 2015-03-11 Bob Sarmady
  Modified      : 2014-11-06 Bob Sarmady
***************************************************************************************************/
using System;
using System.IO;
using System.Text;

namespace VersionUpRC {
    class Program {
        static string Version = "";

        static string GetNewVersion(string CurrVersion) {
            if (Version != "") {
                return Version;
            }

            Console.WriteLine("    Increasing Version Number ...");
            Console.WriteLine("    Current : " + CurrVersion);
            string[] tmpVersionInfo = CurrVersion.Split(new char[] { ',' });
            int[] VersionInfo = new int[4] { 0, 0, 0, 0 };

            for (int i = 0; i < 4; i++) {
                if (tmpVersionInfo.Length > i) {
                    try {
                        VersionInfo[i] = Convert.ToInt32(tmpVersionInfo[i]);
                    } catch { }
                }
            }

            DateTime now = DateTime.Now;
            VersionInfo[2] = Convert.ToInt32((now.Year - 2000) + now.DayOfYear.ToString("000"));

            VersionInfo[3]++;
            if (VersionInfo[3] == 0xFFFF) {
                VersionInfo[1]++;
                VersionInfo[3] = 0;
            }



            CurrVersion = String.Join(",", VersionInfo);
            Console.WriteLine("    New     : " + CurrVersion);
            Version = CurrVersion;
            return CurrVersion;
        }

        static int Main(string[] args) {
            try {
                Console.WriteLine("");
                Console.WriteLine("========== VersionUp ========================================");

                if (args.Length < 1) {

                    Console.WriteLine("");
                    Console.WriteLine("    Failed : Version resource file is required:");
                    Console.WriteLine("        VersionUpRC.exe \"{Path to version resource file e.g. VER.RC}\"");
                    Console.WriteLine("======================================================================");
                    return 1;
                }
                string VersionFile = args[0].Replace("\"", "");
                if (!File.Exists(VersionFile)) {

                    Console.WriteLine("");
                    Console.WriteLine("    Failed : Version Resource file does not exists");
                    Console.WriteLine("        \""+VersionFile+"\"");
                    Console.WriteLine("======================================================================");
                    return 1;
                }
                Console.WriteLine("    Reading file : \"" + VersionFile + "\"");

                string VersionFileText = File.ReadAllText(VersionFile);
                string[] Lines = VersionFileText.Split(new char[] { '\n' });
                StringBuilder sb = new StringBuilder();
                bool VersionChanged = false;
                foreach (string line in Lines) {
                    if (line.IndexOf("FILEVERSION") > -1) {
                        string Version = line.Replace("FILEVERSION", "").Trim();
                        File.WriteAllText(VersionFile, VersionFileText.Replace(Version, GetNewVersion(Version)).Replace(Version.Replace(",", "."), GetNewVersion(Version).Replace(",", ".")));

                        VersionChanged = true;
                    }
                }
                if (!VersionChanged) {
                    Console.WriteLine("    No Version information found in file.");
                }
                Console.WriteLine("======================================================================");
                Console.WriteLine("");
                return 0;


            } catch (Exception ex) {
                Console.WriteLine("    Failed : " + ex.Message);
            }
            Console.WriteLine("======================================================================");
            Console.WriteLine("");
            return 1;
        }
    }
}
