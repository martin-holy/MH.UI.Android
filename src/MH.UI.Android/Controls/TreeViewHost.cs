using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Controls;
using MH.Utils.Interfaces;
using System;

namespace MH.UI.Android.Controls;

public class TreeViewHost : RelativeLayout, ITreeViewHost {
  private RecyclerView _recyclerView = null!;
  private TreeViewHostAdapter? _adapter;

  public event EventHandler<bool>? HostIsVisibleChangedEvent;

  public TreeView? ViewModel { get; private set; }

  public TreeViewHost(Context context) : base(context) => _initialize(context);
  public TreeViewHost(Context context, IAttributeSet attrs) : base(context, attrs) => _initialize(context);
  protected TreeViewHost(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

  private void _initialize(Context context) {
    SetBackgroundResource(Resource.Color.c_static_ba);
    _recyclerView = new(context) {
      LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent),
    };
    _recyclerView.SetLayoutManager(new LinearLayoutManager(context, LinearLayoutManager.Vertical, false));
    AddView(_recyclerView);
  }

  public TreeViewHost Bind(TreeView? treeView) {
    ViewModel = treeView;
    if (ViewModel == null) return this;
    ViewModel.Host = this;
    _adapter = new TreeViewHostAdapter(Context!, ViewModel);
    _recyclerView.SetAdapter(_adapter);
    return this;
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