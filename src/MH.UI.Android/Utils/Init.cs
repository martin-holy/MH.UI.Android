namespace MH.UI.Android.Utils;

// TODO PORT
public static class Init {
  public static void SetDelegates() {
    MH.Utils.Keyboard.IsCtrlOn = () => false;
    MH.Utils.Keyboard.IsAltOn = () => false;
    MH.Utils.Keyboard.IsShiftOn = () => false;

    //MH.Utils.Clipboard.SetText = Clipboard.SetText;

    //MH.Utils.Imaging.GetBitmapHashPixels = Imaging.GetBitmapHashPixels;
    //MH.Utils.Imaging.ResizeJpg = Imaging.ResizeJpg;

    //MH.UI.Controls.Dialog.SetShowImplementation(DialogHost.Show);
    //MH.UI.Controls.Dialog.SetShowAsyncImplementation(DialogHost.ShowAsync);

    //MH.Utils.Tasks.Dispatch = action => Application.Current.Dispatcher.Invoke(DispatcherPriority.Render, action);

    //CommandManager.RequerySuggested += RelayCommandBase.RaiseCanExecuteChanged;
  }
}