using Android.Content;
using Android.Widget;
using MH.UI.Android.Utils;
using MH.UI.Android.Extensions;
using MH.UI.Controls;
using MH.UI.Android.Controls.Hosts.TreeViewHost;

namespace MH.UI.Android.Controls.Hosts.TreeMenuHost;

public class TreeMenuHost : TreeViewHostBase<TreeView, TreeMenuHostAdapter> {  
  public TreeMenuHostSizeObserver Observer { get; }
  public PopupWindow Popup { get; }

  public TreeMenuHost(Context context, TreeView dataContext) : base(context, dataContext, null) {
    SetBackgroundResource(Resource.Drawable.view_border);
    this.SetPadding(DisplayU.DpToPx(1));
    DataContext.Host = this;
    Adapter = new TreeMenuHostAdapter(this);
    _recyclerView.SetAdapter(Adapter);
    Popup = new PopupWindow(this, LPU.Wrap, LPU.Wrap, true);
    Observer = new TreeMenuHostSizeObserver(Context!, this, Popup);
    Adapter!.RegisterAdapterDataObserver(Observer);
  }

  public void Show() {
    Adapter?.NotifyDataSetChanged();
    Popup.ShowAsDropDown(Observer.MenuAnchor);
  }

  public void Close() => Popup.Dismiss();

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      Popup.Dismiss();
      Adapter!.UnregisterAdapterDataObserver(Observer);
      Observer.Dispose();      
    }
    base.Dispose(disposing);
  }
}