using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.Utils.BaseClasses;

namespace MH.UI.Android.Controls;

public class FlatTreeItemViewHolder : RecyclerView.ViewHolder {
  private readonly TreeView _vmParent;
  private readonly LinearLayout _container;
  private readonly ImageView _expandedIcon;
  private readonly ImageView _icon;
  private readonly TextView _name;

  public FlatTreeItem? DataContext { get; private set; }

  public FlatTreeItemViewHolder(Context context, TreeView vmParent) : base(_createContainerView(context)) {
    _vmParent = vmParent;

    ItemView.Click += _onContainerClick;

    _expandedIcon = ViewBuilder.CreateTreeItemExpandIconView(context);
    _expandedIcon.Click += _onExpandedChanged;

    _icon = _createIconView(context);
    _name = _createNameView(context);
    _container = (LinearLayout)ItemView;
    _container.AddView(_expandedIcon);
    _container.AddView(_icon);
    _container.AddView(_name);
  }

  public void Bind(FlatTreeItem? item) {
    DataContext = item;
    if (item == null) return;

    int indent = item.Level * ItemView.Resources!.GetDimensionPixelSize(Resource.Dimension.flat_tree_item_indent_size);
    ItemView.SetPadding(indent, ItemView.PaddingTop, ItemView.PaddingRight, ItemView.PaddingBottom);

    _expandedIcon.Visibility = item.TreeItem.Items.Count > 0 ? ViewStates.Visible : ViewStates.Invisible;
    _expandedIcon.Selected = item.TreeItem.IsExpanded;

    _icon.SetImageDrawable(Icons.GetIcon(ItemView.Context, item.TreeItem.Icon));

    _name.SetText(item.TreeItem.Name, TextView.BufferType.Normal);
  }

  private void _onExpandedChanged(object? sender, System.EventArgs e) {
    if (DataContext == null) return;
    DataContext.TreeItem.IsExpanded = !DataContext.TreeItem.IsExpanded;
  }

  private void _onContainerClick(object? sender, System.EventArgs e) {
    if (DataContext != null && _vmParent.SelectItemCommand.CanExecute(DataContext.TreeItem))
      _vmParent.SelectItemCommand.Execute(DataContext.TreeItem);
  }

  private static LinearLayout _createContainerView(Context context) {
    var container = new LinearLayout(context) {
      LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent),
      Orientation = Orientation.Horizontal,
      Clickable = true,
      Focusable = true
    };
    container.SetGravity(GravityFlags.CenterVertical);
    container.SetPadding(context.Resources!.GetDimensionPixelSize(Resource.Dimension.general_padding));
    container.SetBackgroundResource(Resource.Color.c_static_ba);

    return container;
  }

  private static ImageView _createIconView(Context context) =>
    new(context) {
      LayoutParameters = new LinearLayout.LayoutParams(DisplayU.DpToPx(24), DisplayU.DpToPx(24)) {
        MarginStart = DisplayU.DpToPx(8)
      }
    };

  private static TextView _createNameView(Context context) {
    var textView = new TextView(context) {
      LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent) {
        MarginStart = DisplayU.DpToPx(8)
      }
    };
    textView.SetTextColor(new Color(context.Resources!.GetColor(Resource.Color.c_static_fo, context.Theme)));

    return textView;
  }
}