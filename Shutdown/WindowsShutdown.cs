#nullable enable

using System.Diagnostics;

// This is probably the least portable code in the World, ever! 

namespace Shutdown
{
  internal static class WindowsShutdown
  {
    public enum WindowsShutdownMode { Shutdown, Restart }

    public static void PerformShutdown(WindowsShutdownMode option) =>
      Process.Start(CreateShutdownProcessStartInfo(option));

    private static ProcessStartInfo CreateShutdownProcessStartInfo(WindowsShutdownMode option)
    {
      // HACK: Hardcode path: c:\windows\system32\
      return new ProcessStartInfo(@"c:\windows\system32\shutdown.exe", $@"{option.ToCmdLineArg()} /t 0")
      {
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true
      };

    }
      
   

    private static string ToCmdLineArg(this WindowsShutdownMode mode) => mode == WindowsShutdownMode.Shutdown ? "/s" : "/r";

  }
}
