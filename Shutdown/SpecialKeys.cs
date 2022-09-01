#nullable enable

using System.Windows.Input;

namespace Shutdown
{
  internal static class SpecialKeys
  {
    internal static bool CtrlKeyIsDown => Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
    //internal static bool IsShutdownModeToggleKey(Key key) => (key == Key.LeftAlt) || (key == Key.RightAlt) || (key==Keys.m);
    internal static bool ShutdownModeToggleKeyIsDown => Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);


    /// <summary>
    /// Returns true if the key is either the left or right Alt key. Otherwise returns false.
    /// </summary>
    public static bool IsAlt(this Key key) =>
      key == System.Windows.Input.Key.LeftAlt || key == System.Windows.Input.Key.RightAlt ? true : false;

  }
}
