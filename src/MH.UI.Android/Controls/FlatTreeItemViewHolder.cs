using Android.Content;
using MH.UI.Android.Utils;
using MH.Utils.BaseClasses;

namespace MH.UI.Android.Controls;

public class FlatTreeItemViewHolder : FlatTreeItemViewHolderBase {
  private readonly CommandBinding _selectItemCommandBinding;

  public FlatTreeItemViewHolder(Context context, TreeViewHost treeViewHost) : base(context, treeViewHost) {
    _selectItemCommandBinding = new CommandBinding(_container, treeViewHost.DataContext.SelectItemCommand);
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      _selectItemCommandBinding.Dispose();
    }
    base.Dispose(disposing);
  }

  public override void Bind(FlatTreeItem? item) {
    base.Bind(item);
    _selectItemCommandBinding.Parameter = item?.TreeItem;
  }
}