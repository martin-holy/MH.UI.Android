using Android.Content;

namespace MH.UI.Android.Controls;

public class CollectionViewGroupViewHolder : FlatTreeItemViewHolderBase {
  public CollectionViewGroupViewHolder(Context context, IAndroidTreeViewHost treeViewHost) : base(context, treeViewHost) {
    _container.SetBackgroundResource(Resource.Color.gray1);
  }
}