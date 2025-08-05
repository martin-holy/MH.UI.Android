using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Controls;
using MH.Utils.Interfaces;
using System;

namespace MH.UI.Android.Controls;

public class TreeViewHost : RelativeLayout, ITreeViewHost {
  private readonly RecyclerView _recyclerView;
  private readonly TreeViewHostAdapter _adapter;
  private bool _disposed;

  public TreeView DataContext { get; }
  public Func<View, object?, PopupWindow?> ItemMenuFactory { get; }

  public event EventHandler<bool>? HostIsVisibleChangedEvent;

  public TreeViewHost(Context context, TreeView dataContext, Func<View, object?, PopupWindow?> itemMenuFactory) : base(context) {
    DataContext = dataContext;
    ItemMenuFactory = itemMenuFactory;
    SetBackgroundResource(Resource.Color.c_static_ba);
    DataContext.Host = this;

    _adapter = new TreeViewHostAdapter(Context!, this);
    
    _recyclerView = new(context) {
      LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent),
    };
    _recyclerView.SetLayoutManager(new LinearLayoutManager(context, LinearLayoutManager.Vertical, false));
    _recyclerView.SetAdapter(_adapter);
    AddView(_recyclerView);
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      _adapter.Dispose();
    }
    _disposed = true;
    base.Dispose(disposing);
  }

  protected override void OnVisibilityChanged(View changedView, [GeneratedEnum] ViewStates visibility) {
    base.OnVisibilityChanged(changedView, visibility);
    HostIsVisibleChangedEvent?.Invoke(this, visibility == ViewStates.Visible);
  }

  public void ExpandRootWhenReady(ITreeItem root) => root.IsExpanded = true;

  public void ScrollToTop() {
    // TODO PORT
  }

  public void ScrollToItems(object[] items, bool exactly) {
    // TODO PORT
  }
}