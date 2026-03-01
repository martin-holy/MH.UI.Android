using Android.Content;
using Android.Views;
using MH.UI.Android.Utils;

namespace MH.UI.Android.Controls.Hosts.TreeMenuHost;

public class TreeMenuSeparatorV : View {
  public TreeMenuSeparatorV(Context context) : base(context) {
    SetBackgroundResource(Resource.Color.c_static_bo);
  }
}