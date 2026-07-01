using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Controls.Hosts.TreeMenuHost;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.Utils.Interfaces;
using System;
using System.Collections.Generic;

namespace MH.UI.Android.Controls.Hosts.TreeViewHost;

public abstract class TreeViewHostBase<TView, TAdapter> : RelativeLayout, IAndroidTreeViewHost
    where TView : TreeView
    where TAdapter : TreeViewHostAdapterBase {
  protected readonly RecyclerView _recyclerView;
  protected bool _disposed;

  public TView DataContext { get; }
  public TAdapter? Adapter { get; set; }
  public TreeMenu? ItemMenu { get; }
  TreeView IAndroidTreeViewHost.DataContext => DataContext;

  protected TreeViewHostBase(Context context, TView dataContext, Func<object, IEnumerable<ITreeItem>>? itemMenuFactory) : base(context) {
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
    DataContext.SetVisible(visibility == ViewStates.Visible);
  }

  public virtual void ScrollToTop() =>
    Post(() => _recyclerView.ScrollToPosition(0));

  public virtual void ScrollTo(ITreeItem item, bool exactly) {
    Post(() => _scrollTo(item, exactly));
  }

  private void _scrollTo(ITreeItem item, bool exactly) {
    var position = DataContext.FlatTree.IndexOf(item);
    if (position < 0) return;

    if (exactly && _recyclerView.GetLayoutManager() is LinearLayoutManager layoutManager)
      layoutManager.ScrollToPositionWithOffset(position, 0);
    else
      _recyclerView.ScrollToPosition(position);
  }
}