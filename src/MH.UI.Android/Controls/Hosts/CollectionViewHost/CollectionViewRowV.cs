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

public class CollectionViewRowV : RelativeLayout, IBindable<FlatTreeItem> {
  private bool _disposed;
  private ICollectionViewRow? _row;
  private readonly RecyclerView _items;
  private readonly CollectionViewItemAdapter _adapter;
  private int _rowHeight;

  public FlatTreeItem? DataContext { get; private set; }

  public CollectionViewRowV(Context context, CollectionViewHost cvHost) : base(context) {
    _adapter = new CollectionViewItemAdapter(
      item => cvHost.HandleItemClick(_row!, item),
      item => cvHost.HandleItemLongClick(_row!, item),
      cvHost.CreateItemContent);

    _items = new RecyclerView(context) { HasFixedSize = false };
    _items.SetRecycledViewPool(cvHost.ItemViewPool);
    _items.SetItemAnimator(null);
    _items.SetLayoutManager(new LinearLayoutManager(context, LinearLayoutManager.Horizontal, false));
    _items.AddItemDecoration(new VerticalCenterItemDecoration(() => _rowHeight));
    _items.SetAdapter(_adapter);

    AddView(_items, new RelativeLayout.LayoutParams(LPU.Wrap, LPU.Wrap));
  }

  public void Bind(FlatTreeItem item) {
    DataContext = item;
    BindItems(item);
  }

  public void BindItems(FlatTreeItem item) {
    _adapter.Submit(null, Array.Empty<ISelectable>());
    _row = item.TreeItem as ICollectionViewRow;
    if (_row == null || _dataContext?.TreeItem is not ITreeItem { Parent: ICollectionViewGroup group }) return;

    var items = _row.Leaves.ToArray();
    _rowHeight = items.Length == 0 ? 0 : items.Max(x => group.GetItemSize(x, false)) + CollectionView.ItemBorderSize * 2;
    _items.LayoutParameters!.Height = _rowHeight;

    _adapter.Submit(group, items);
  }

  public void Unbind() { }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      _items.SetAdapter(null);
    }
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