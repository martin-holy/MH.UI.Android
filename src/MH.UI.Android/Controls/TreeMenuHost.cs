using Android.Content;
using Android.Widget;
using MH.UI.Android.Utils;
using MH.UI.Android.Extensions;
using MH.UI.Controls;

namespace MH.UI.Android.Controls;

public class TreeMenuHost : TreeViewHostBase<TreeView, TreeViewHostAdapterBase> {  
  public TreeMenuHostSizeObserver Observer { get; }
  public PopupWindow Popup { get; }

  public TreeMenuHost(Context context, TreeView dataContext) : base(context, dataContext, null) {
    SetBackgroundResource(Resource.Drawable.view_border);
    this.SetPadding(DisplayU.DpToPx(1));
    DataContext.Host = this;
    Adapter = new TreeMenuHostAdapter(context, this);
    _recyclerView.SetAdapter(Adapter);
    Popup = new PopupWindow(this, LPU.Wrap, LPU.Wrap, true);
    Observer = new TreeMenuHostSizeObserver(Context!, this, Popup);
    Adapter!.RegisterAdapterDataObserver(Observer);
  }

  public void RefreshMenu() {
    Post(() => Adapter?.NotifyDataSetChanged());
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