#nullable enable

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using static Shutdown.GeneralExtensions;

namespace Shutdown
{
  public partial class MainWindow : Window
  {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public MainWindow() => InitializeComponent();

    /// <summary>
    /// Timer which triggers the Windows shutdown.
    /// </summary>
    private DispatcherTimer? ShutdownTimer { get; set; } = null;

    /// <summary>
    /// Timer delay before shutting down Windows.
    /// </summary>
    private int ShutdownDelay { get; set; } = 10;

    /// <summary>
    /// Returns the current <see cref="App"/>
    /// </summary>
    private App ThisApp { get; } = (App)Application.Current;



    // Enables HandleIfAltKey() to avoid running more than once for the same key state 
    private KeyState _altKeyState = KeyState.Up;

    /// <summary>
    /// Handles button-click which initiates the Windows shutdown
    /// </summary>
    private void ShutdownNowButton_Click(object sender, RoutedEventArgs e) => ThisApp.ShutdownWindows();

    /// <summary>
    /// Handles button-click which cancels the Windows shutdown
    /// </summary>
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
      KillTimer();
      Environment.Exit(0); // Added to replace Close()
      //Close(); // This was causing 'TaskCanceledException', at least in the IDE.
    }


    /// <summary>
    /// Sets control states and initiates timed Windows shutdown.
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      SetControlTextsForAltState(KeyState.Up);

      try
      {
        InitTimer();
      }
      catch (Exception ex)
      {
        ThisApp.DisplayError(ex.Message);
      }

    }

    /// <summary>
    /// Initializes the shutdown timer.
    /// </summary>
    private void InitTimer()
    {
      Debug.Assert(ShutdownTimer is null, "Timer has already been created.");
      
      ShutdownTimer = new DispatcherTimer();
      ShutdownTimer.Tick += DispatcherTimer_Tick;
      ShutdownTimer.Interval = new TimeSpan(0, 0, 1);
      ShutdownTimer.Start();
    }

    /// <summary>
    /// Performs shutdown timer tear-down.
    /// </summary>
    private void KillTimer()
    {
      Debug.Assert(ShutdownTimer != null, "Timer has not been created.");
      if (ShutdownTimer is null) return;

      ShutdownTimer.Stop();
      ShutdownTimer.Tick -= DispatcherTimer_Tick;
      ShutdownTimer = null;

    }

    /// <summary>
    /// Handles the Tick event for the shutdown timer
    /// </summary>
    private void DispatcherTimer_Tick(object sender, EventArgs e)
    {
      if (1 > --ShutdownDelay)
      {
        try
        {
          KillTimer();
          ThisApp.ShutdownWindows();

        }
        catch (Exception ex)
        {
          ThisApp.DisplayError(ex.Message);
        }
      }
      else
      {
        ShutdownDelayTextBlock.Text = ShutdownDelay.ToString();// + " second" + (_shutdownDelay > 1 ? "s" : "");
      }
    }

    /// <summary>
    /// Sets text on controls which are effected by the state of the Alt key
    /// </summary>
    private void SetControlTextsForAltState(KeyState keyState)
    {
      ShutdownNowButton.Content = keyState == KeyState.Down ? "Restart Now" : "Shutdown Now";
      MainMessageTextBlock.Text = keyState == KeyState.Down ? "Restarting in" : "Shutting down in";
    }

    /// <summary>
    ///  Handles state for the Alt key and triggers updating of associated controls
    /// </summary>
    /// <returns>True if <paramref name="key"/> was either Alt key, otherwise false.</returns>
    private bool HandleAltKey(Key key, KeyState keyState)
    {
      if (!key.IsAlt()) return false;
      if (SetIfNotEqual(ref _altKeyState, keyState)) SetControlTextsForAltState(keyState);
      return true;
    }

    /// <summary>
    /// Triggers processing of Alt key press.
    /// </summary>
    private void Window_KeyDown(object sender, KeyEventArgs e) => e.Handled = HandleAltKey(e.SystemKey, KeyState.Down);

    /// <summary>
    /// Triggers processing of Alt key release.
    /// </summary>
    private void Window_KeyUp(object sender, KeyEventArgs e) => e.Handled = HandleAltKey(e.SystemKey, KeyState.Up);


    /// <summary>
    /// Performs timer tear-down.
    /// </summary>
    private void Window_Closed(object sender, EventArgs e)
    {
      if (ShutdownTimer != null)
      {
        try
        {
          KillTimer();
        }
        catch (Exception ex)
        {
          ThisApp.DisplayError(ex.Message);
        }
      }
    }
  }
}
