using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.UI.Interfaces;
using MH.Utils.Interfaces;
using System;

namespace MH.UI.Android.Controls;

public class CollectionViewHost : RelativeLayout, ICollectionViewHost {
  private RecyclerView _recyclerView = null!;
  private CollectionViewHostAdapter? _adapter;

  public event EventHandler<bool>? HostIsVisibleChangedEvent;

  public CollectionView? ViewModel { get; private set; }

  public Func<LinearLayout, ICollectionViewGroup, object?, View?> GetItemView { get; set; } =
    (container, group, item) => throw new NotImplementedException();

  public CollectionViewHost(Context context) : base(context) => _initialize(context);
  public CollectionViewHost(Context context, IAttributeSet attrs) : base(context, attrs) => _initialize(context);
  protected CollectionViewHost(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

  private void _initialize(Context context) {
    LayoutInflater.From(context)!.Inflate(Resource.Layout.collection_view_host, this, true);
    _recyclerView = FindViewById<RecyclerView>(Resource.Id.tree_recycler_view)!;
    _recyclerView.SetLayoutManager(new LinearLayoutManager(context));
  }

  public CollectionViewHost Bind(CollectionView? viewModel) {
    ViewModel = viewModel;
    if (ViewModel == null) return this;
    ViewModel.Host = this;
    ((TreeView)ViewModel).Host = this;
    _adapter = new CollectionViewHostAdapter(Context!, this);
    _recyclerView.SetAdapter(_adapter);
    return this;
  }

  protected override void OnVisibilityChanged(View changedView, [GeneratedEnum] ViewStates visibility) {
    base.OnVisibilityChanged(changedView, visibility);
    var isVisible = visibility == ViewStates.Visible;
    HostIsVisibleChangedEvent?.Invoke(this, isVisible);

    if (isVisible && Parent is View { Width: > 0 } parent && ViewModel?.RootHolder is [ICollectionViewGroup { Width: 0 } group]) {
      group.Width = parent.Width / DisplayU.Metrics.Density;
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