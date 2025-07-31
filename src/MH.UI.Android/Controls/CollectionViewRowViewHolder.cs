using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Interfaces;
using MH.Utils.BaseClasses;
using MH.Utils.Interfaces;

namespace MH.UI.Android.Controls;

public class CollectionViewRowViewHolder : RecyclerView.ViewHolder {
  private readonly LinearLayout _container;

  public CollectionViewRowViewHolder(Context context) : base(_createContainerView(context)) {
    _container = (LinearLayout)ItemView;
  }

  public void Bind(FlatTreeItem item, CollectionViewHost cvHost) {
    _container.RemoveAllViews();

    if (item.TreeItem is not ICollectionViewRow row || row is not ITreeItem { Parent: ICollectionViewGroup group }) return;

    foreach (var rowItem in row.Leaves)
      if (cvHost.GetItemView(_container, group, rowItem) is { } view)
        _container.AddView(new CollectionViewItem(_container.Context!).Bind(cvHost, row, rowItem, view));
  }

  private static LinearLayout _createContainerView(Context context) {
    var container = new LinearLayout(context) {
      LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent),
      Orientation = Orientation.Horizontal
    };
    container.SetGravity(GravityFlags.CenterVertical);
    container.SetPadding(context.Resources!.GetDimensionPixelSize(Resource.Dimension.general_padding));

    return container;
  }
}