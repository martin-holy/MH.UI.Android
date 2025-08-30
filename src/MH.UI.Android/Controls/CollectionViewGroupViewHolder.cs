using Android.Content;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;

namespace MH.UI.Android.Controls;

public class CollectionViewGroupViewHolder : FlatTreeItemViewHolderBase {
  public CollectionViewGroupViewHolder(Context context, IAndroidTreeViewHost treeViewHost) : base(context, treeViewHost) {
    var gp = context.Resources!.GetDimensionPixelSize(Resource.Dimension.general_padding);
    _container.SetMargin(gp, gp, gp, 0);
    _container.Background = BackgroundFactory.Dark();
  }
}