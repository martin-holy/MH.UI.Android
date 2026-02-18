using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Interfaces;
using MH.Utils.BaseClasses;

namespace MH.UI.Android.Controls.Hosts.TreeViewHost;

public abstract class FlatTreeItemVBase : LinearLayout, IBindable<FlatTreeItem> {
  protected readonly IAndroidTreeViewHost _treeViewHost;
  protected virtual bool _showIcon => true;
  protected virtual bool _showName => true;
  private readonly ImageView _expandedIcon;
  private readonly IconButton? _icon;
  private readonly TextView? _name;

  public FlatTreeItem? DataContext { get; private set; }

  public FlatTreeItemVBase(Context context, IAndroidTreeViewHost treeViewHost) : base(context) {
    Orientation = Orientation.Horizontal;
    Clickable = true;
    Focusable = true;
    SetGravity(GravityFlags.CenterVertical);
    this.SetPadding(DimensU.Spacing);
    SetBackgroundResource(Resource.Color.c_static_ba);

    _treeViewHost = treeViewHost;
    _expandedIcon = _createTreeItemExpandIconView(context)
      .WithClickAction(this, (o, _) => {
        if (o.DataContext == null) return;
        o.DataContext.TreeItem.IsExpanded = !o.DataContext.TreeItem.IsExpanded;
      });

    AddView(_expandedIcon, new LayoutParams(DimensU.IconButtonSize, DimensU.IconButtonSize)
      .WithMargin(DimensU.Spacing, 0, DimensU.Spacing, 0));

    if (_showIcon) {
      _icon = new IconButton(context)
        .WithClickAction(this, (o, s) => o._treeViewHost.ItemMenu?.ShowItemMenu(s, o.DataContext?.TreeItem));
      AddView(_icon);
    }

    if (_showName) {
      _name = new TextView(context);
      AddView(_name);
    }
  }

  public virtual void Bind(FlatTreeItem? item) {
    DataContext = item;
    if (item == null) return;

    int indent = item.Level * DimensU.FlatTreeItemIndentSize;
    SetPadding(indent, PaddingTop, PaddingRight, PaddingBottom);

    _expandedIcon.Visibility = item.TreeItem.Items.Count > 0 ? ViewStates.Visible : ViewStates.Invisible;
    _expandedIcon.Activated = item.TreeItem.IsExpanded;

    if (_icon != null)
      _icon.SetImageDrawable(Icons.GetIcon(Context, item.TreeItem.Icon));

    if (_name != null)
      _name.Text = item.TreeItem.Name;
  }

  public virtual void Unbind() { }

  private static ImageView _createTreeItemExpandIconView(Context context) {
    var icon = new ImageView(context) {
      Clickable = true,
      Focusable = true
    };
    icon.SetScaleType(ImageView.ScaleType.Center);
    icon.SetImageResource(Resource.Drawable.tree_item_expanded_selector);

    return icon;
  }
}