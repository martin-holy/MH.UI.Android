using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
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
  public bool IsMultiSelectOn { get; set; }

  public Func<LinearLayout, ICollectionViewGroup, object?, View?> GetItemView { get; set; } =
    (container, group, item) => throw new NotImplementedException();

  public CollectionViewHost(Context context) : base(context) => _initialize(context);
  public CollectionViewHost(Context context, IAttributeSet attrs) : base(context, attrs) => _initialize(context);
  protected CollectionViewHost(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

  private void _initialize(Context context) {
    SetBackgroundResource(Resource.Color.c_static_ba);
    _recyclerView = new(context) {
      LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent),
    };
    _recyclerView.SetLayoutManager(new LinearLayoutManager(context, LinearLayoutManager.Vertical, false));
    AddView(_recyclerView);
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
  }

  protected override void OnSizeChanged(int w, int h, int oldw, int oldh) {
    base.OnSizeChanged(w, h, oldw, oldh);

    if (Visibility == ViewStates.Visible && ViewModel?.RootHolder is [ICollectionViewGroup { Width: 0 } group]) {
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