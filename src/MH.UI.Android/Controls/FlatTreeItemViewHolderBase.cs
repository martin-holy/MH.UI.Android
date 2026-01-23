using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.Utils.BaseClasses;

namespace MH.UI.Android.Controls;

public abstract class FlatTreeItemViewHolderBase : RecyclerView.ViewHolder {
  protected readonly IAndroidTreeViewHost _treeViewHost;
  protected readonly LinearLayout _container;
  protected virtual bool _showIcon => true;
  protected virtual bool _showName => true;
  private readonly ImageView _expandedIcon;
  private readonly IconButton? _icon;
  private readonly TextView? _name;

  public FlatTreeItem? DataContext { get; private set; }

  public FlatTreeItemViewHolderBase(Context context, IAndroidTreeViewHost treeViewHost) : base(_createContainerView(context)) {
    _treeViewHost = treeViewHost;
    _expandedIcon = _createTreeItemExpandIconView(context)
      .WithClickAction(this, (o, _) => {
        if (o.DataContext == null) return;
        o.DataContext.TreeItem.IsExpanded = !o.DataContext.TreeItem.IsExpanded;
      });

    _container = (LinearLayout)ItemView;
    _container.AddView(_expandedIcon, new LinearLayout.LayoutParams(DimensU.IconButtonSize, DimensU.IconButtonSize)
      .WithMargin(DimensU.Spacing, 0, DimensU.Spacing, 0));

    if (_showIcon) {
      _icon = new IconButton(context)
        .WithClickAction(this, (o, s) => o._treeViewHost.ItemMenu?.ShowItemMenu(s, o.DataContext?.TreeItem));
      _container.AddView(_icon);
    }

    if (_showName) {
      _name = new TextView(context);
      _container.AddView(_name);
    }
  }

  public virtual void Bind(FlatTreeItem? item) {
    DataContext = item;
    if (item == null) return;

    int indent = item.Level * DimensU.FlatTreeItemIndentSize;
    ItemView.SetPadding(indent, ItemView.PaddingTop, ItemView.PaddingRight, ItemView.PaddingBottom);

    _expandedIcon.Visibility = item.TreeItem.Items.Count > 0 ? ViewStates.Visible : ViewStates.Invisible;
    _expandedIcon.Activated = item.TreeItem.IsExpanded;

    if (_icon != null)
      _icon.SetImageDrawable(Icons.GetIcon(ItemView.Context, item.TreeItem.Icon));

    if (_name != null)
      _name.Text = item.TreeItem.Name;
  }

  private static LinearLayout _createContainerView(Context context) {
    var container = new LinearLayout(context) {
      Orientation = Orientation.Horizontal,
      LayoutParameters = new RecyclerView.LayoutParams(LPU.Match, LPU.Wrap),
      Clickable = true,
      Focusable = true
    };
    container.SetGravity(GravityFlags.CenterVertical);
    container.SetPadding(DimensU.Spacing);
    container.SetBackgroundResource(Resource.Color.c_static_ba);

    return container;
  }

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