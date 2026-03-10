using Android.Views;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Controls.Recycler;
using MH.UI.Android.Utils;
using MH.UI.Interfaces;
using MH.Utils.Interfaces;

namespace MH.UI.Android.Controls.Hosts.TabControlHost;

public class TabControlHostHeaderAdapter(TabControlHost _tabControlHost) : RecyclerView.Adapter {
  public override int ItemCount => _tabControlHost.DataContext.Tabs.Count;

  public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) =>
    new BaseViewHolder(new TabItemHeaderV(parent.Context!, _tabControlHost), new(LPU.Wrap, LPU.Wrap));

  public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) {
    (holder.ItemView as IBindable<IListItem>)?.Rebind(_tabControlHost.DataContext.Tabs[position]);;
  }

  public override void OnViewRecycled(Java.Lang.Object holder) {
    (((RecyclerView.ViewHolder)holder).ItemView as IUnbindable)?.Unbind();
    base.OnViewRecycled(holder);
  }
}