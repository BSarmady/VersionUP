using System;
using System.IO;
using System.Text;

namespace VersionUp {
    class Program {

        private static string GetNewVersion(string CurrVersion) {

            string[] tmpVersionInfo = CurrVersion.Split('.');
            int[] VersionInfo = new int[4] { 0, 0, 0, 0 };

            // Use major and minor as is
            for (int i = 0; i < 4; i++) {
                if (tmpVersionInfo.Length > i) {
                    try {
                        VersionInfo[i] = Convert.ToInt32(tmpVersionInfo[i]);
                    } catch { }
                }
            }

            DateTime now = DateTime.Now;
            // date part is YYDDD
            VersionInfo[2] = Convert.ToInt32(now.Year - 2000 + now.DayOfYear.ToString("000"));

            // make sure build number doesn't go beyond 65535 (max capacity)
            VersionInfo[3]++;
            if (VersionInfo[3] == 0xFFFF) {
                VersionInfo[1]++;
                VersionInfo[3] = 0;
            }
            return string.Join(".", VersionInfo);
        }

        private static int ShowHelp() {
            WriteMessage(
                "    Invalid use. Correct call format is:\n" +
                "        VersionUp.exe \"$(ProjectDir)\" \"$(TargetPath)\"\n" +
                "        $(ProjectDir) and $(TargetPath) are visual studio macros."
            );
            return 1;
        }

        private static void WriteMessage(string inStr) {
            Console.WriteLine("\n" +
                "=== VersionUp ========================================================\n" +
                inStr + "\n" +
                "======================================================================");
        }

        private static int Main(string[] args) {
            try {
                //Console.WriteLine(args.Length);
                //Console.WriteLine("->" + args[0]);
                //Console.WriteLine("->" + args[1]);


                args = CreateArgs(Environment.CommandLine);
                if (args.Length != 3) {
                    return ShowHelp();
                }

                // args[0] is name of this executable

                string ProjectDir = args[1].Trim('"'); // args[1] is $(ProjectDir) in quotes
                ProjectDir = ProjectDir.TrimEnd('\\') + "\\";

                // Looking for AssemblyInfo.cs in project root then in Properties folder
                string VersionFile = ProjectDir + "AssemblyInfo.cs";
                if (!File.Exists(VersionFile)) {
                    VersionFile = ProjectDir + "Properties\\AssemblyInfo.cs";
                }
                if (!File.Exists(VersionFile)) {
                    WriteMessage("    Failed: Cannot find AssemblyInfo.cs expecting to be in root of project or Properties folder");
                    return 1;
                }

                string TargetPath = args[2]; // args[2] is $(TargetPath) in quotes
                if (File.Exists(TargetPath)) {
                    // if project output is newer than assemblyfile.cs we assume project is not changed
                    FileInfo infoOutput = new FileInfo(TargetPath);
                    FileInfo infoProp = new FileInfo(VersionFile);
                    if (infoProp.LastWriteTime > infoOutput.LastWriteTime) {
                        WriteMessage("    Project " + Path.GetFileNameWithoutExtension(TargetPath) + " is up to date");
                        return 0;
                    }
                }

                string[] Lines = File.ReadAllLines(VersionFile);
                StringBuilder sb = new StringBuilder();
                string prevVersion = "";
                string currVersion = "";
                foreach (string line in Lines) {
                    if (line.IndexOf("[assembly: AssemblyVersion(") == 0) {
                        prevVersion = line.Replace("[assembly: AssemblyVersion(\"", "").Replace("\")]", "");
                        currVersion = GetNewVersion(prevVersion);
                        sb.AppendLine("[assembly: AssemblyVersion(\"" + currVersion + "\")]");
                    } else if (line.IndexOf("[assembly: AssemblyFileVersion(") == 0) {
                        prevVersion = line.Replace("[assembly: AssemblyFileVersion(\"", "").Replace("\")]", "");
                        currVersion = GetNewVersion(prevVersion);
                        sb.AppendLine("[assembly: AssemblyFileVersion(\"" + currVersion + "\")]");
                    } else {
                        sb.AppendLine(line);
                    }
                }
                File.WriteAllText(VersionFile, sb.ToString());

                WriteMessage(
                    "    Project " + Path.GetFileNameWithoutExtension(TargetPath) + "\n" +
                    "    Current : " + prevVersion + "\n" +
                    "    New     : " + currVersion
                );

                return 0;

            } catch (Exception ex) {
                WriteMessage("    Failed : " + ex.Message);
            }
            return 1;
        }
        private static string[] CreateArgs(string commandLine) {
            StringBuilder sb = new StringBuilder(commandLine);

            bool in_quote = false;
            for (int i = 0; i < sb.Length; i++) {
                if (sb[i] == '"')
                    in_quote = !in_quote;
                if (sb[i] == ' ' && !in_quote)
                    sb[i] = '\n';
            }
            string[] array = sb.ToString().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int j = 0; j < array.Length; j++) {
                array[j] = array[j].Replace("\"", "");
            }
            return array;
        }
    }
}