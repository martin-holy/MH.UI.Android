using Android.Content;

namespace MH.UI.Android.Utils;

public static class ClipboardU {
  public static void SetText(string text) {
    if (global::Android.App.Application.Context.GetSystemService(Context.ClipboardService) is ClipboardManager clipboard)
      clipboard.PrimaryClip = ClipData.NewPlainText("text", text);
  }
}
