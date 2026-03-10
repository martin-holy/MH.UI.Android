using Android.Content;
using MH.UI.Android.Binding;
using MH.Utils.BaseClasses;

namespace MH.UI.Android.Controls.Hosts.TreeViewHost;

public class FlatTreeItemV : FlatTreeItemVBase {
  private readonly CommandBinding _selectItemCommandBinding;
  private bool _disposed;

  public FlatTreeItemV(Context context, IAndroidTreeViewHost treeViewHost) : base(context, treeViewHost) {
    SetBackgroundResource(Resource.Drawable.selectable_item);
    _selectItemCommandBinding = new(this);
  }

  public override void Bind(FlatTreeItem? item) {
    base.Bind(item);
    if (item == null) return;
    _selectItemCommandBinding.Bind(_treeViewHost.DataContext.SelectItemCommand, item.TreeItem);
    BindIsSelected(item);
  }

  public override void Unbind() {
    base.Unbind();
    _selectItemCommandBinding.Unbind();
  }

  public void BindIsSelected(FlatTreeItem item) {
    if (_treeViewHost.DataContext.ShowTreeItemSelection)
      Selected = item.TreeItem.IsSelected;
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      _selectItemCommandBinding.Dispose();
    }
    _disposed = true;
    base.Dispose(disposing);
  }
}