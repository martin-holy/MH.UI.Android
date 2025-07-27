using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Controls;
using MH.UI.Interfaces;

namespace MH.UI.Android.Controls;

public class CollectionViewItem : FrameLayout {
  private View _border = null!;
  private CollectionViewHost _host = null!;
  private ICollectionViewRow _row = null!;
  private object _item = null!;

  public CollectionViewItem(Context context) : base(context) => _initialize(context);

  private void _initialize(Context context) {
    LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
    Clickable = true;
    Focusable = true;
    Click += _onClick;
    LongClick += _onLongClick;

    _border = new View(context);
    _border.SetBackgroundResource(Resource.Drawable.collection_view_item_selector);
  }

  public CollectionViewItem Bind(CollectionViewHost host, ICollectionViewRow row, object item, View itemView) {
    _host = host;
    _row = row;
    _item = item;
    RemoveAllViews();

    if (row.Parent is not ICollectionViewGroup { } group) return this;

    var itemWidth = group.GetItemSize(item, true);
    var itemHeight = group.GetItemSize(item, false);
    itemView.LayoutParameters = new MarginLayoutParams(itemWidth, itemHeight);
    itemView.SetMarginUnified(CollectionView.ItemBorderSize);

    _border.LayoutParameters = new LayoutParams(
      itemWidth + (CollectionView.ItemBorderSize * 2),
      itemHeight + (CollectionView.ItemBorderSize * 2));

    AddView(itemView);
    AddView(_border);
    return this;
  }

  private void _onClick(object? sender, System.EventArgs e) {
    if (_host.ViewModel is not { } vm) return;
    if (vm.CanSelect) vm.SelectItem(_row, _item, false, false);
    if (vm.CanOpen) vm.OpenItem(_item);
  }

  private void _onLongClick(object? sender, LongClickEventArgs e) {
    if (_host.ViewModel is not { } vm) return;
    if (vm.CanSelect) vm.SelectItem(_row, _item, false, false);
    _border.Selected = true; // TODO just for test
    _border.Invalidate();
  }
}