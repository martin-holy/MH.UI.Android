using Android.Content;
using MH.UI.Android.Utils;
using MH.UI.Controls;

namespace MH.UI.Android.Controls;

public class ButtonMenu : IconButton {
  private TreeMenuHost? _treeMenuHost;
  private readonly TreeView _dataContext;
  private bool _disposed;

  public ButtonMenu(Context context, TreeView dataContext, string? iconName) : base(context) {
    _dataContext = dataContext;
    SetImageDrawable(Icons.GetIcon(Context, iconName));
    Click += _onClick;
  }

  private void _onClick(object? sender, System.EventArgs e) {
    if (_treeMenuHost == null) {
      _treeMenuHost = new TreeMenuHost(Context!, _dataContext);
      _treeMenuHost.Observer.MenuAnchor = this;
    }
    else
      _treeMenuHost.RefreshMenu();

    _treeMenuHost.Popup.ShowAsDropDown(this);
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      _treeMenuHost?.Dispose();
      Click -= _onClick;
    }
    _disposed = true;
    base.Dispose(disposing);
  }
}