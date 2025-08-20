using Android.Content;
using Android.Widget;
using MH.UI.Android.Utils;
using MH.UI.Controls;

namespace MH.UI.Android.Controls;

public class ButtonMenu : IconButton {
  private PopupWindow? _menu;

  public ButtonMenu(Context context, TreeView dataContext, string? iconName) : base(context) {
    SetImageDrawable(Icons.GetIcon(Context, iconName));
    Click += (_, _) => {
      if (_menu == null)
        _menu = TreeMenuFactory.CreateMenu(Context!, dataContext, this);
      else if (_menu.ContentView is TreeMenuHost { Adapter: { } adapter })
        _menu.ContentView.Post(adapter.NotifyDataSetChanged);

      _menu.ShowAsDropDown(this);
    };
  }

  protected override void Dispose(bool disposing) {
    if (disposing) {
      _menu?.Dismiss();
      _menu = null;
    }
    base.Dispose(disposing);
  }
}