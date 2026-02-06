using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.UI.Interfaces;
using MH.Utils.BaseClasses;
using MH.Utils.Interfaces;
using System;
using System.Linq;

namespace MH.UI.Android.Controls.Hosts.CollectionViewHost;

public class CollectionViewRowViewHolder : RecyclerView.ViewHolder, IDisposable {
  private bool _disposed;
  private ICollectionViewRow? _dataContext;
  private readonly RecyclerView _items;
  private readonly CollectionViewItemAdapter _adapter;
  private int _rowHeight;

  public CollectionViewRowViewHolder(Context context, CollectionViewHost cvHost) : base(new RelativeLayout(context)) {
    _adapter = new CollectionViewItemAdapter(
      item => cvHost.HandleItemClick(_dataContext!, item),
      item => cvHost.HandleItemLongClick(_dataContext!, item),
      cvHost.CreateItemContent);

    _items = new RecyclerView(context) { HasFixedSize = false };
    _items.SetRecycledViewPool(cvHost.ItemViewPool);
    _items.SetItemAnimator(null);
    _items.SetLayoutManager(new LinearLayoutManager(context, LinearLayoutManager.Horizontal, false));
    _items.AddItemDecoration(new VerticalCenterItemDecoration(() => _rowHeight));
    _items.SetAdapter(_adapter);

    ((RelativeLayout)ItemView).AddView(_items, new RelativeLayout.LayoutParams(LPU.Wrap, LPU.Wrap));
  }

  public void Bind(FlatTreeItem item) {
    _adapter.Submit(null, Array.Empty<ISelectable>());
    _dataContext = item.TreeItem as ICollectionViewRow;
    if (_dataContext == null || _dataContext is not ITreeItem { Parent: ICollectionViewGroup group }) return;

    var items = _dataContext.Leaves.ToArray();
    _rowHeight = items.Max(x => group.GetItemSize(x, false)) + CollectionView.ItemBorderSize * 2;
    _items.LayoutParameters!.Height = _rowHeight;

    _adapter.Submit(group, items);
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing)       _items.SetAdapter(null);
    _disposed = true;
    base.Dispose(disposing);
  }

  private class VerticalCenterItemDecoration : RecyclerView.ItemDecoration {
    private readonly Func<int> _rowHeightProvider;

    public VerticalCenterItemDecoration(Func<int> rowHeightProvider) {
      _rowHeightProvider = rowHeightProvider;
    }

    public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state) {
      if (view is not CollectionViewItemShell shell) return;
      var top = (_rowHeightProvider() - shell.ShellHeight) / 2;
      if (top > 0) outRect.Top = top;
    }
  }
}