using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ValheimSaveEditor
{
    class Utility
    {
        public static bool IsGameRunning()
        {
            Process[] processes = Process.GetProcessesByName("valheim");
            return processes.Length != 0;
        }
    }
}
