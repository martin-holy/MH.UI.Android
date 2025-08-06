using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Controls;
using MH.Utils.Interfaces;
using System;

namespace MH.UI.Android.Controls;

public interface IAndroidTreeViewHost : ITreeViewHost {
  public Func<View, object?, PopupWindow?> ItemMenuFactory { get; }
}

public abstract class BaseTreeViewHost<TView, TAdapter> : RelativeLayout, IAndroidTreeViewHost
    where TView : TreeView
    where TAdapter : BaseTreeViewHostAdapter {
  protected readonly RecyclerView _recyclerView;
  protected TAdapter? _adapter;
  protected bool _disposed;

  public TView DataContext { get; }
  public Func<View, object?, PopupWindow?> ItemMenuFactory { get; }
  public event EventHandler<bool>? HostIsVisibleChangedEvent;

  protected BaseTreeViewHost(Context context, TView dataContext, Func<View, object?, PopupWindow?> itemMenuFactory) : base(context) {
    DataContext = dataContext;
    ItemMenuFactory = itemMenuFactory;
    SetBackgroundResource(Resource.Color.c_static_ba);

    _recyclerView = new(context) {
      LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent),
    };
    _recyclerView.SetLayoutManager(new LinearLayoutManager(context, LinearLayoutManager.Vertical, false));
    AddView(_recyclerView);
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      HostIsVisibleChangedEvent = null;
      DataContext.Host = null;
      _adapter?.Dispose();
      _recyclerView.Dispose();
    }
    _disposed = true;
    base.Dispose(disposing);
  }

  protected override void OnVisibilityChanged(View changedView, [GeneratedEnum] ViewStates visibility) {
    base.OnVisibilityChanged(changedView, visibility);
    HostIsVisibleChangedEvent?.Invoke(this, visibility == ViewStates.Visible);
  }

  public virtual void ExpandRootWhenReady(ITreeItem root) => root.IsExpanded = true;

  public virtual void ScrollToTop() { /* TODO PORT */ }

  public virtual void ScrollToItems(object[] items, bool exactly) { /* TODO PORT */ }
}