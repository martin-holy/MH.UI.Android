using Android.Content;
using Android.Widget;
using MH.UI.Android.Utils;
using MH.Utils.BaseClasses;

namespace MH.UI.Android.Controls;

public class ButtonMenu : IconButton {
  private readonly PopupWindow _rootMenu;

  public ButtonMenu(Context context, MenuItem root) : base(context) {
    SetImageDrawable(Icons.GetIcon(Context, root.Icon));
    _rootMenu = MenuFactory.CreateMenu(Context!, this, root);
    Click += (_, _) => _rootMenu.ShowAsDropDown(this);
  }
}