using Android.Util;

namespace MH.UI.Android.Utils;

public class DisplayU {
  public static DisplayMetrics Metrics { get; private set; }

  public static void Init(DisplayMetrics displayMetrics) {
    Metrics = displayMetrics;
  }
}