using Android.Content;
using MH.UI.Android.Utils;
using MH.Utils.BaseClasses;

namespace MH.UI.Android.Controls.Hosts.TreeViewHost;

public class FlatTreeItemV : FlatTreeItemVBase {
  private readonly CommandBinding _selectItemCommandBinding;

  public FlatTreeItemV(Context context, IAndroidTreeViewHost treeViewHost) : base(context, treeViewHost) {
    SetBackgroundResource(Resource.Drawable.selectable_item);
    _selectItemCommandBinding = this.Bind(treeViewHost.DataContext.SelectItemCommand);
  }

  public override void Bind(FlatTreeItem? item) {
    base.Bind(item);
    if (item == null) return;
    _selectItemCommandBinding.Parameter = item.TreeItem;
    BindIsSelected(item);
  }

  public void BindIsSelected(FlatTreeItem item) {
    if (_treeViewHost.DataContext.ShowTreeItemSelection)
      Selected = item.TreeItem.IsSelected;
  }
}