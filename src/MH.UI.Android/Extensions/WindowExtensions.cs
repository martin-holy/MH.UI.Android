using Android.OS;
using Android.Views;

namespace MH.UI.Android.Extensions;

public static class WindowExtensions {
  public static void EnterFullScreen(this Window window) {
    if (Build.VERSION.SdkInt >= BuildVersionCodes.R) {
      window.SetDecorFitsSystemWindows(false);
      if (window.InsetsController is { } controller) {
        controller.Hide(WindowInsets.Type.SystemBars());
        controller.SystemBarsBehavior = (int)WindowInsetsControllerBehavior.ShowTransientBarsBySwipe;
      }
    }
    else {
      window.AddFlags(WindowManagerFlags.Fullscreen);
      window.DecorView.SystemUiVisibility = (StatusBarVisibility)(
        SystemUiFlags.ImmersiveSticky |
        SystemUiFlags.HideNavigation |
        SystemUiFlags.Fullscreen |
        SystemUiFlags.LayoutFullscreen |
        SystemUiFlags.LayoutHideNavigation |
        SystemUiFlags.LayoutStable);
    }
  }

  public static void ExitFullScreen(this Window window) {
    if (Build.VERSION.SdkInt >= BuildVersionCodes.R) {
      window.InsetsController?.Show(WindowInsets.Type.SystemBars());
      window.SetDecorFitsSystemWindows(true);
    }
    else {
      window.ClearFlags(WindowManagerFlags.Fullscreen);
      window.DecorView.SystemUiVisibility = StatusBarVisibility.Visible;
    }
  }
}