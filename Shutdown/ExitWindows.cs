using System;
using System.Runtime.InteropServices;

// Not currently in use, but added in preparation of moving away from current shutdown method.

// See: https://docs.microsoft.com/en-gb/windows/win32/api/winuser/nf-winuser-exitwindowsex
//      https://docs.microsoft.com/en-us/windows/win32/shutdown/system-shutdown-reason-codes
//      http://www.blackwasp.co.uk/ExitLogoffWindows.aspx
//      https://stackoverflow.com/questions/24726116/when-using-exitwindowsex-logoff-works-but-shutdown-and-restart-do-not

namespace Shutdown
{
  class ExitWindows
  {

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TokPriv1Luid
    {
      public int Count;
      public long Luid;
      public int Attr;
    }

    [DllImport("kernel32.dll", ExactSpelling = true)]
    internal static extern IntPtr GetCurrentProcess();

    [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
    internal static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);

    [DllImport("advapi32.dll", SetLastError = true)]
    internal static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);

    [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
    internal static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall, ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);

    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    internal static extern bool ExitWindowsEx(uint flag, uint reason);

    internal const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";

    internal const int SE_PRIVILEGE_ENABLED = 0x00000002;
    internal const int TOKEN_QUERY = 0x00000008;
    internal const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;

    // The shutdown type.
    internal const uint EWX_HYBRID_SHUTDOWN = 0x00400000;
    internal const uint EWX_LOGOFF = 0x00000000;
    internal const uint EWX_POWEROFF = 0x00000008;    
    internal const uint EWX_REBOOT = 0x00000002;
    internal const uint EWX_RESTARTAPPS = 0x00000040;  
    internal const uint EWX_SHUTDOWN = 0x00000001;

    // The shutdown type can optionally include one of the following values.
    internal const uint EWX_FORCE = 0x00000004;
    internal const uint EWX_FORCEIFHUNG = 0x00000010;

    enum Reason : uint
    {
      ApplicationIssue = 0x00040000,
      HardwareIssue = 0x00010000,
      SoftwareIssue = 0x00030000,
      PlannedShutdown = 0x80000000
    }


    public static void PerformHybridShutdown() => DoExitWin(EWX_SHUTDOWN | EWX_HYBRID_SHUTDOWN);

    public static void PerformRestart() => DoExitWin(EWX_REBOOT);


    /// <summary>
    /// Method to shutdown / reboot windows
    /// </summary>
    /// <param name="flg"></param>
    private static void DoExitWin(uint flg)
    {
      // NB: Does not check any return codes assigned to 'ok'

      bool ok;
      TokPriv1Luid tp;
      IntPtr hproc = GetCurrentProcess();
      IntPtr htok = IntPtr.Zero;
      ok = OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok);
      tp.Count = 1;
      tp.Luid = 0;
      tp.Attr = SE_PRIVILEGE_ENABLED;
      ok = LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, ref tp.Luid);
      ok = AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
      ok = ExitWindowsEx(flg, (uint)Reason.PlannedShutdown); // <-- MSDN says do not use zero for second parameter!
    }
  }
}
