using Android.Content;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Interfaces;
using MH.Utils.BaseClasses;

namespace MH.UI.Android.Controls.Hosts.CollectionViewHost;

public class CollectionViewGroupViewHolder : FlatTreeItemViewHolderBase {
  private readonly TextView _sourceCount;

  public CollectionViewGroupViewHolder(Context context, IAndroidTreeViewHost treeViewHost) : base(context, treeViewHost) {
    _sourceCount = new TextView(context);
    _container.AddView(_sourceCount);    
    _container.Background = BackgroundFactory.Dark();
    if (_container.LayoutParameters is RecyclerView.LayoutParams lp)
      _container.LayoutParameters = lp.WithDpMargin(2);
  }

  public override void Bind(FlatTreeItem? item) {
    base.Bind(item);
    if (item is not { TreeItem: ICollectionViewGroup group}) return;

    _sourceCount.Text = group.SourceCount.ToString();
  }
}