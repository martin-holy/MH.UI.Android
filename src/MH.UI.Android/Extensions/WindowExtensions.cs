using Android.OS;
using Android.Views;

namespace MH.UI.Android.Extensions;

public static class WindowExtensions {
  public static void EnterFullScreen(this Window window, bool keepStatusBarOnCutoutDevices = true) {
    if (Build.VERSION.SdkInt >= BuildVersionCodes.R) {
      var hasCutout = window.DecorView.RootWindowInsets?.DisplayCutout?.SafeInsetTop > 0;
      var keepSafeInsets = keepStatusBarOnCutoutDevices && hasCutout;

      window.SetDecorFitsSystemWindows(keepSafeInsets);

      if (window.InsetsController is { } controller) {
        if (keepSafeInsets) {
          controller.Show(WindowInsets.Type.StatusBars());
          controller.Hide(WindowInsets.Type.NavigationBars());
        }
        else
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