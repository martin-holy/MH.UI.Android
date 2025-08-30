using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Interfaces;
using MH.Utils.BaseClasses;
using MH.Utils.Interfaces;
using System;

namespace MH.UI.Android.Controls;

public class CollectionViewRowViewHolder : RecyclerView.ViewHolder, IDisposable {
  private bool _disposed;
  private readonly CollectionViewHost _cvHost;
  private readonly LinearLayout _container;
  private ICollectionViewRow? _dataContext;

  public CollectionViewRowViewHolder(Context context, CollectionViewHost cvHost) : base(_createContainerView(context)) {
    _cvHost = cvHost;
    _container = (LinearLayout)ItemView;
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) _unbind();
    _disposed = true;
    base.Dispose(disposing);
  }

  public void Bind(FlatTreeItem item) {
    _unbind();
    _dataContext = item.TreeItem as ICollectionViewRow;
    if (_dataContext == null || _dataContext is not ITreeItem { Parent: ICollectionViewGroup group }) return;

    foreach (var rowItem in _dataContext.Leaves)
      if (_cvHost.GetItemView(_container, group, rowItem) is { } view) {
        var itemWidth = group.GetItemSize(rowItem, true);
        var itemHeight = group.GetItemSize(rowItem, false);
        var itemView = new CollectionViewItem(_container.Context!).Bind(rowItem, view, itemWidth, itemHeight);
        itemView.Click += _onItemClick;
        itemView.LongClick += _onItemLongClick;
        _container.AddView(itemView);
      }
  }

  private void _unbind() {
    for (int i = 0; i < _container.ChildCount; i++)
      if (_container.GetChildAt(i) is View childView) {
        childView.Click -= _onItemClick;
        childView.LongClick -= _onItemLongClick;
      }

    _container.RemoveAllViews();
  }

  private void _onItemClick(object? sender, EventArgs e) =>
    _cvHost.HandleItemClick(_dataContext!, sender as CollectionViewItem);

  private void _onItemLongClick(object? sender, View.LongClickEventArgs e) =>
    _cvHost.HandleItemLongClick(_dataContext!, sender as CollectionViewItem);

  private static LinearLayout _createContainerView(Context context) {
    var container = new LinearLayout(context) {
      LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent),
      Orientation = Orientation.Horizontal
    };
    container.SetGravity(GravityFlags.CenterVertical);
    container.SetPadding(0);

    return container;
  }
}