using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Binding;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.UI.Interfaces;
using MH.Utils;
using MH.Utils.Disposables;
using MH.Utils.Interfaces;

namespace MH.UI.Android.Controls.Hosts.TabControlHost;

public class TabItemHeaderV : LinearLayout, IBindable<IListItem> {
  private readonly TabControlHost _tabControlHost;
  private readonly ImageView _icon;
  private readonly TextView _name;
  private readonly CommandBinding _selectItemCommandBinding;
  private readonly BindingScope _bindings = new();
  private bool _disposed;

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
        .WithClickAction(s => _tabControlHost.ItemMenu?.ShowItemMenu(s, DataContext))
        .WithPadding(0);
      AddView(_icon, LPU.LinearWrap());
    }
    else {
      _icon = new IconView(context);
      AddView(_icon, LPU.Linear(DimensU.IconSize, DimensU.IconSize)
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
      AddView(_name, 0, LPU.LinearWrap());
    }
    else AddView(_name, LPU.LinearWrap());

    _selectItemCommandBinding = new(this);
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
    _name.BindText(item, nameof(IListItem.Name), x => x.Name, x => x, _bindings);

    item.Bind(nameof(IListItem.IsSelected), x => x.IsSelected, x => Selected = x).DisposeWith(_bindings);
    _selectItemCommandBinding.Bind(_tabControlHost.DataContext.SelectTabCommand, item);
  }

  public void Unbind() {
    _bindings.Dispose();
    _selectItemCommandBinding.Unbind();
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      Unbind();
      _selectItemCommandBinding.Dispose();
    }
    _disposed = true;
    base.Dispose(disposing);
  }
}