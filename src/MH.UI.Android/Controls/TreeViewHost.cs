using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Controls;

namespace MH.UI.Android.Controls;

public class TreeViewHost : FrameLayout {
  private RecyclerView _recyclerView;
  private TreeViewHostAdapter _adapter;
  private TreeView _viewModel;

  public TreeView ViewModel { get => _viewModel; private set => _viewModel = value; }

  public TreeViewHost(Context context, TreeView viewModel) : base(context) => _initialize(context, viewModel);
  public TreeViewHost(Context context, IAttributeSet attrs, TreeView viewModel) : base(context, attrs) => _initialize(context, viewModel);
  protected TreeViewHost(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

  private void _initialize(Context context, TreeView viewModel) {
    LayoutInflater.From(context).Inflate(Resource.Layout.tree_view_host, this, true);
    _adapter = new TreeViewHostAdapter(Context, viewModel.RootHolder);
    _recyclerView = FindViewById<RecyclerView>(Resource.Id.tree_recycler_view);
    _recyclerView.SetLayoutManager(new LinearLayoutManager(context));
    _recyclerView.SetAdapter(_adapter);
    ViewModel = viewModel;
  }
}