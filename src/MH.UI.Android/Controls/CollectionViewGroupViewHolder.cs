using Android.Content;
using Android.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Interfaces;
using MH.Utils.BaseClasses;

namespace MH.UI.Android.Controls;

public class CollectionViewGroupViewHolder : FlatTreeItemViewHolderBase {
  private readonly TextView _sourceCount;

  public CollectionViewGroupViewHolder(Context context, IAndroidTreeViewHost treeViewHost) : base(context, treeViewHost) {
    _sourceCount = new TextView(context);
    _container.AddView(_sourceCount);

    var gp = context.Resources!.GetDimensionPixelSize(Resource.Dimension.general_padding);
    _container.SetMargin(gp, gp, gp, 0);
    _container.Background = BackgroundFactory.Dark();
  }

  public override void Bind(FlatTreeItem? item) {
    base.Bind(item);
    if (item is not { TreeItem: ICollectionViewGroup group}) return;

    _sourceCount.Text = group.SourceCount.ToString();
  }
}