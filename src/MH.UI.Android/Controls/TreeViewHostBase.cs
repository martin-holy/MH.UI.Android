using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.Utils.BaseClasses;
using MH.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MH.UI.Android.Controls;

public interface IAndroidTreeViewHost : ITreeViewHost {
  public TreeView DataContext { get; }
  public TreeMenu? ItemMenu {  get; }
}

public abstract class TreeViewHostBase<TView, TAdapter> : RelativeLayout, IAndroidTreeViewHost
    where TView : TreeView
    where TAdapter : TreeViewHostAdapterBase {
  protected readonly RecyclerView _recyclerView;
  protected bool _disposed;

  public TView DataContext { get; }
  public TAdapter? Adapter { get; set; }
  public TreeMenu? ItemMenu { get; }
  public event EventHandler<bool>? HostIsVisibleChangedEvent;
  TreeView IAndroidTreeViewHost.DataContext => DataContext;
  double ITreeViewHost.Width => Width;

  protected TreeViewHostBase(Context context, TView dataContext, Func<object, IEnumerable<MenuItem>>? itemMenuFactory) : base(context) {
    DataContext = dataContext;
    if (itemMenuFactory != null) ItemMenu = new(context, itemMenuFactory);
    SetBackgroundResource(Resource.Color.c_static_ba);

    _recyclerView = new(context);
    _recyclerView.SetLayoutManager(new LinearLayoutManager(context, LinearLayoutManager.Vertical, false));
    AddView(_recyclerView, new LayoutParams(LPU.Match, LPU.Match));
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      DataContext.Host = null;
      _recyclerView.SetAdapter(null);
      Adapter?.Dispose();
      ItemMenu?.Dispose();
    }
    _disposed = true;
    base.Dispose(disposing);
  }

  protected override void OnVisibilityChanged(View changedView, [GeneratedEnum] ViewStates visibility) {
    base.OnVisibilityChanged(changedView, visibility);
    HostIsVisibleChangedEvent?.Invoke(this, visibility == ViewStates.Visible);
  }

  public virtual void ExpandRootWhenReady(ITreeItem root) =>
    root.IsExpanded = true;

  public virtual void ScrollToTop() =>
    _recyclerView.ScrollTo(0, 0);

  public virtual void ScrollToItems(object[] items, bool exactly) {
    var item = items[^1];
    if (Adapter?.Items.SingleOrDefault(x => ReferenceEquals(x.TreeItem, item)) is not { } flatItem) return;
    var position = Adapter.Items.ToList().IndexOf(flatItem);

    if (exactly && _recyclerView.GetLayoutManager() is LinearLayoutManager layoutManager)
      layoutManager.ScrollToPositionWithOffset(position, 0);
    else
      _recyclerView.ScrollToPosition(position);
  }
}