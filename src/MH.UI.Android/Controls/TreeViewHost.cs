using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Controls;

namespace MH.UI.Android.Controls;

public class TreeViewHost : RelativeLayout {
  private RecyclerView _recyclerView = null!;
  private TreeViewHostAdapter _adapter = null!;
  private TreeView _viewModel = null!;

  public TreeView ViewModel {
    get => _viewModel;
    set {
      _viewModel = value;
      _adapter = new TreeViewHostAdapter(Context!, _viewModel);
      _recyclerView.SetAdapter(_adapter);
    }
  }

  public TreeViewHost(Context context) : base(context) => _initialize(context);
  public TreeViewHost(Context context, IAttributeSet attrs) : base(context, attrs) => _initialize(context);
  protected TreeViewHost(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

  private void _initialize(Context context) {
    LayoutInflater.From(context)!.Inflate(Resource.Layout.tree_view_host, this, true);
    _recyclerView = FindViewById<RecyclerView>(Resource.Id.tree_recycler_view)!;
    _recyclerView.SetLayoutManager(new LinearLayoutManager(context));
  }
}