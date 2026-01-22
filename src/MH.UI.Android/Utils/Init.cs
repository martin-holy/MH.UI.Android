using Android.Content;
using Android.OS;
using MH.UI.Android.Controls;

namespace MH.UI.Android.Utils;

public static class Init {
  private static Handler? _handler;

  public static void Utils(Context context) {
    DimensU.Init(context);
    DisplayU.Init(context.Resources!.DisplayMetrics!);
  }

  public static void SetDelegates() {
    // TODO for users with HW keyboard
    MH.Utils.Keyboard.IsCtrlOn = () => false;
    MH.Utils.Keyboard.IsAltOn = () => false;
    MH.Utils.Keyboard.IsShiftOn = () => false;

    MH.Utils.Clipboard.SetText = ClipboardU.SetText;

    //MH.Utils.Imaging.GetBitmapHashPixels = Imaging.GetBitmapHashPixels;
    MH.Utils.Imaging.ResizeJpg = _resizeJpg;

    MH.UI.Controls.Dialog.SetShowAsyncImplementation(DialogHost.ShowAsync);

    _handler = new(Looper.MainLooper!);
    MH.Utils.Tasks.Dispatch = action => _handler.Post(action);
  }

  private static void _resizeJpg(string src, string dest, int px, bool withMetadata, bool withThumbnail, int quality) {
    ImagingU.ResizeJpg(src, dest, px, withMetadata, quality);
  }
}