using Android.OS;

namespace MH.UI.Android.Utils;

// TODO PORT
public static class Init {
  private static Handler? _handler;

  public static void SetDelegates() {
    MH.Utils.Keyboard.IsCtrlOn = () => false;
    MH.Utils.Keyboard.IsAltOn = () => false;
    MH.Utils.Keyboard.IsShiftOn = () => false;

    //MH.Utils.Clipboard.SetText = Clipboard.SetText;

    //MH.Utils.Imaging.GetBitmapHashPixels = Imaging.GetBitmapHashPixels;
    //MH.Utils.Imaging.ResizeJpg = Imaging.ResizeJpg;

    //MH.UI.Controls.Dialog.SetShowImplementation(DialogHost.Show);
    //MH.UI.Controls.Dialog.SetShowAsyncImplementation(DialogHost.ShowAsync);

    _handler = new(Looper.MainLooper!);
    MH.Utils.Tasks.Dispatch = action => _handler.Post(action);

    //CommandManager.RequerySuggested += RelayCommandBase.RaiseCanExecuteChanged;
  }
}