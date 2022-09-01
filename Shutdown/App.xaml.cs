#nullable enable

using System.Windows;
using static Shutdown.SpecialKeys;
using static Shutdown.WindowsShutdown;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("UnitTestProject")]

namespace Shutdown
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);
      Current.DispatcherUnhandledException += DispatcherUnhandledExceptionHandler;

      // If Ctrl-key is down during launch then perform immediate shutdown of the system.
      if (CtrlKeyIsDown)
      {
        ShutdownWindows();
        return;
      }

      // Ctrl-key was not down, so show the main window.

      var overlayWindows = new OverlayWindow();

      var mainWindow = new MainWindow();
      Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
      Current.MainWindow = mainWindow;
#if !DEBUG
      overlayWindows.Show();
#endif
      mainWindow.ShowDialog();
    }


    // Catch unhandled exceptions on the UI thread.
    // See: http://www.abhisheksur.com/2010/07/unhandled-exception-handler-for-wpf.html
    // Other Threads' exceptions will not be caught by the DispatcherUnhandledException. 
    // This event hooks only the UI thread, and only be called when the UI thread gets an exception which crashes the program. 
    // If you need to handle the Exception occurring from Non UI Thread, you can handle the UnhandledException that comes with AppDomain.
    private void DispatcherUnhandledExceptionHandler(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
      // Prevent the app from 'crashing', most of the time ;)
      e.Handled = true;

      // Inform the user
      _ = MessageBox.Show(e.Exception.Message, "Unhandled Exception", MessageBoxButton.OK, MessageBoxImage.Error);

      // Manually exit application
      Current.Shutdown();
    }

    // Whether the windows should shutdown or restart
    internal static WindowsShutdownMode CurrentShutdownMode =>
      ShutdownModeToggleKeyIsDown
      ? WindowsShutdownMode.Restart
      : WindowsShutdownMode.Shutdown;

    // Displays the supplied error message to the user
    public void DisplayError(string message) =>
      MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

    /// <summary>
    /// Shuts down windows. 
    /// </summary>
    public void ShutdownWindows()
    {
      //PerformShutdown(CurrentShutdownMode); // <-- Previous (working method)

      if (CurrentShutdownMode == WindowsShutdownMode.Shutdown) ExitWindows.PerformHybridShutdown();
      else ExitWindows.PerformRestart();

      Current.Shutdown();
    }



  }
}
