using Android.Content;
using Android.Widget;
using MH.UI.Android.Utils;
using MH.Utils.BaseClasses;

namespace MH.UI.Android.Controls;

public class ButtonMenu : IconButton {
  private PopupWindow? _rootMenu;
  private readonly MenuItem _rootItem;

  public ButtonMenu(Context context, MenuItem root) : base(context) {
    _rootItem = root;
    SetImageDrawable(Icons.GetIcon(Context, root.Icon));
    Click += (_, _) => {
      _rootMenu ??= MenuFactory.CreateMenu(Context!, this, _rootItem);
      _rootMenu.ShowAsDropDown(this);
    };
  }

  protected override void Dispose(bool disposing) {
    if (disposing) {
      _rootMenu?.Dismiss();
      _rootMenu = null;
    }
    base.Dispose(disposing);
  }
}