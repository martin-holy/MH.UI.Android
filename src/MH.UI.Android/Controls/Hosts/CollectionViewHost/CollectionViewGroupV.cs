using Android.Content;
using Android.Widget;
using MH.UI.Android.Controls.Hosts.TreeViewHost;
using MH.UI.Android.Utils;
using MH.UI.Interfaces;
using MH.Utils.BaseClasses;

namespace MH.UI.Android.Controls.Hosts.CollectionViewHost;

public class CollectionViewGroupV : FlatTreeItemVBase {
  private readonly TextView _sourceCount;

  public CollectionViewGroupV(Context context, IAndroidTreeViewHost treeViewHost) : base(context, treeViewHost) {
    _sourceCount = new TextView(context);
    AddView(_sourceCount);
    Background = BackgroundFactory.Dark();
  }

  public override void Bind(FlatTreeItem? item) {
    base.Bind(item);
    if (item is not { TreeItem: ICollectionViewGroup group }) return;

    _sourceCount.Text = group.SourceCount.ToString();
  }
}