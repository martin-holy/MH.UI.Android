using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.UI.Interfaces;
using MH.Utils;
using MH.Utils.Interfaces;
using System;

namespace MH.UI.Android.Controls.Hosts.TabControlHost;

public class TabItemHeaderV : LinearLayout, IBindable<IListItem> {
  private readonly TabControlHost _tabControlHost;
  private readonly ImageView _icon;
  private readonly TextView _name;
  private readonly CommandBinding _selectItemCommandBinding;
  private IDisposable? _nameBinding;

  public IListItem? DataContext { get; private set; }

  public TabItemHeaderV(Context context, TabControlHost tabControlHost) : base(context) {
    _tabControlHost = tabControlHost;
    Orientation = Orientation.Horizontal;
    Clickable = true;
    Focusable = true;
    SetGravity(GravityFlags.CenterVertical);
    SetPadding(0, 0, DimensU.Spacing, 0);
    SetBackgroundResource(Resource.Drawable.tab_item_header_background);

    if (tabControlHost.DataContext.TabStrip.IconTextVisibility.HasFlag(IconTextVisibility.Both)) {
      _icon = new IconButton(context)
        .WithClickAction(this, (o, s) => o._tabControlHost.ItemMenu?.ShowItemMenu(s, o.DataContext));
      _icon.SetPadding(0);
      AddView(_icon);
    }
    else {
      _icon = new IconView(context);
      AddView(_icon, new LayoutParams(DimensU.IconSize, DimensU.IconSize)
        .WithMargin(DimensU.Spacing, DimensU.Spacing, 0, DimensU.Spacing));
    }

    // TODO BUG text is clipped when rotated 90 or 270
    _name = new TextView(context);
    var rotationAngle = tabControlHost.DataContext.TabStrip.RotationAngle;
    if (rotationAngle is 90 or 270) {
      _name.Rotation = rotationAngle;
      _name.SetSingleLine(true);
      _name.Ellipsize = null;

      Orientation = Orientation.Vertical;
      SetPadding(0, DimensU.Spacing, 0, 0);
      AddView(_name, 0);
    }
    else AddView(_name);

    _selectItemCommandBinding = this.Bind(tabControlHost.DataContext.SelectTabCommand);
  }

  public void Bind(IListItem? item) {
    DataContext = item;
    if (item == null) return;

    var itv = _tabControlHost.DataContext.TabStrip.IconTextVisibility;
    var isIconVisible = itv.HasFlag(IconTextVisibility.Icon);
    var isTextVisible = itv.HasFlag(IconTextVisibility.Text) && !string.IsNullOrEmpty(item.Name);

    _icon.Visibility = isIconVisible ? ViewStates.Visible : ViewStates.Gone;
    _icon.SetImageDrawable(isIconVisible ? IconU.GetIcon(_icon.Context, item.Icon) : null);

    _name.Visibility = isTextVisible ? ViewStates.Visible : ViewStates.Gone;
    _name.SetPadding(isIconVisible ? DimensU.Spacing : 0);
    _nameBinding?.Dispose();
    _name.BindText(item, nameof(IListItem.Name), x => x.Name, x => x, out _nameBinding);

    Selected = item.IsSelected;
    _selectItemCommandBinding.Parameter = item;
  }

  public void Unbind() { }
}