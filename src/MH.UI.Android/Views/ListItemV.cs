using Android.Content;
using Android.Views;
using MH.UI.Android.Controls;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Interfaces;
using MH.Utils.Interfaces;

namespace MH.UI.Android.Views;

public class ListItemV : IconTextView, IBindable<IListItem> {
  public ListItemV(Context context) : base(context) {
    SetBackgroundResource(Resource.Drawable.selectable_item);
    SetGravity(GravityFlags.CenterVertical);
    this.SetPadding(DimensU.Spacing);
    Clickable = true;
    Focusable = true;
  }

  public void Bind(IListItem item) {
    BindIcon(item.Icon).BindText(item.Name);
  }

  public void Unbind() { }
}