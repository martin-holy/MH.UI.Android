using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Controls;
using MH.UI.Interfaces;
using MH.Utils.Interfaces;
using System;

namespace MH.UI.Android.Controls;

public class CollectionViewHost : RelativeLayout, ICollectionViewHost, IDisposable {
  private bool _disposed;
  private readonly RecyclerView _recyclerView;
  private readonly CollectionViewHostAdapter _adapter;

  public event EventHandler<bool>? HostIsVisibleChangedEvent;

  public CollectionView DataContext { get; }
  public bool IsMultiSelectOn { get; set; }

  public Func<LinearLayout, ICollectionViewGroup, object?, View?> GetItemView { get; }

  public CollectionViewHost(Context context, CollectionView dataContext, Func<LinearLayout, ICollectionViewGroup, object?, View?> getItemView) : base(context) {
    DataContext = dataContext;
    GetItemView = getItemView;
    SetBackgroundResource(Resource.Color.c_static_ba);

    _adapter = new CollectionViewHostAdapter(Context!, this);
    _recyclerView = new(context) {
      LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent),
    };
    _recyclerView.SetLayoutManager(new LinearLayoutManager(context, LinearLayoutManager.Vertical, false));
    _recyclerView.SetAdapter(_adapter);
    AddView(_recyclerView);

    DataContext.Host = this;
    ((TreeView)DataContext).Host = this;
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      HostIsVisibleChangedEvent = null;
      DataContext.Host = null;
      ((TreeView)DataContext).Host = null;
      _adapter.Dispose();
      _recyclerView.Dispose();
    }
    _disposed = true;
    base.Dispose(disposing);
  }

  protected override void OnVisibilityChanged(View changedView, [GeneratedEnum] ViewStates visibility) {
    base.OnVisibilityChanged(changedView, visibility);
    var isVisible = visibility == ViewStates.Visible;
    HostIsVisibleChangedEvent?.Invoke(this, isVisible);
  }

  protected override void OnSizeChanged(int w, int h, int oldw, int oldh) {
    base.OnSizeChanged(w, h, oldw, oldh);

    if (Visibility == ViewStates.Visible && DataContext?.RootHolder is [ICollectionViewGroup { Width: 0 } group]) {
      group.Width = w;
      _adapter?.SetItemsSource();
    }
  }

  public void ExpandRootWhenReady(ITreeItem root) => root.IsExpanded = true;

  public void ScrollToTop() {
    // TODO PORT
  }

  public void ScrollToItems(object[] items, bool exactly) {
    // TODO PORT
  }
}