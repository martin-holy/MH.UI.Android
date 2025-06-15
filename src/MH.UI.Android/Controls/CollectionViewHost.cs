using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Controls;

namespace MH.UI.Android.Controls;

public class CollectionViewHost : RelativeLayout {
  private RecyclerView _recyclerView = null!;
  private CollectionViewHostAdapter _adapter = null!;
  private CollectionView _viewModel = null!;

  public CollectionView ViewModel {
    get => _viewModel;
    set {
      _viewModel = value;
      _adapter = new CollectionViewHostAdapter(Context!, _viewModel);
      _recyclerView.SetAdapter(_adapter);
    }
  }

  public CollectionViewHost(Context context) : base(context) => _initialize(context);
  public CollectionViewHost(Context context, IAttributeSet attrs) : base(context, attrs) => _initialize(context);
  protected CollectionViewHost(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

  private void _initialize(Context context) {
    LayoutInflater.From(context)!.Inflate(Resource.Layout.collection_view_host, this, true);
    _recyclerView = FindViewById<RecyclerView>(Resource.Id.tree_recycler_view)!;
    _recyclerView.SetLayoutManager(new LinearLayoutManager(context));
  }
}