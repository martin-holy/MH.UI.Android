using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Utils;
using MH.UI.Android.Extensions;
using MH.UI.Controls;

namespace MH.UI.Android.Controls;

public class TreeMenuHost : TreeViewHostBase<TreeView, TreeViewHostAdapterBase> {  
  public TreeMenuHostSizeObserver Observer { get; }
  public PopupWindow Popup { get; }

  public TreeMenuHost(Context context, TreeView dataContext, View anchor) : base(context, dataContext, null) {
    SetBackgroundResource(Resource.Drawable.view_border);
    this.SetPadding(DisplayU.DpToPx(1));
    DataContext.Host = this;
    Adapter = new TreeMenuHostAdapter(context, this);
    _recyclerView.SetAdapter(Adapter);
    Popup = new PopupWindow(this, ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent, true);
    Observer = new TreeMenuHostSizeObserver(Context!, this, Popup, anchor);
    Adapter!.RegisterAdapterDataObserver(Observer);
  }

  public void RefreshMenu() {
    Post(() => Adapter?.NotifyDataSetChanged());
  }
}