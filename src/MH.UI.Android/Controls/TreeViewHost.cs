using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Controls;
using MH.Utils;

namespace MH.UI.Android.Controls;

public class TreeViewHost : FrameLayout {
  private RecyclerView _recyclerView;
  private TreeViewHostAdapter _adapter;
  private TreeView? _viewModel;

  public TreeViewHost(Context context, TreeView? viewModel) : base(context) => _initialize(context, viewModel);
  public TreeViewHost(Context context, IAttributeSet attrs, TreeView? viewModel) : base(context, attrs) => _initialize(context, viewModel);
  protected TreeViewHost(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

  private void _initialize(Context context, TreeView? viewModel) {
    LayoutInflater.From(context).Inflate(Resource.Layout.tree_view_host, this, true);
    _adapter = new TreeViewHostAdapter(Context);
    _recyclerView = FindViewById<RecyclerView>(Resource.Id.tree_recycler_view);
    _recyclerView.SetLayoutManager(new LinearLayoutManager(context));
    _recyclerView.SetAdapter(_adapter);
    ViewModel = viewModel;
  }

  public TreeView? ViewModel {
    get => _viewModel;
    set {
      _viewModel = value;
      _setItemsSource();
    }
  }

  private void _setItemsSource() {
    if (ViewModel == null) return;
    var newFlatItems = Tree.ToFlatTreeItems(ViewModel.RootHolder);
    //_updateTreeItemSubscriptions(ItemsSource as IEnumerable<FlatTreeItem>, newFlatItems);
    _adapter.UpdateItems(newFlatItems);
  }
}