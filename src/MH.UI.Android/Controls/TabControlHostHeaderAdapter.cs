using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace MH.UI.Android.Controls;

public class TabControlHostHeaderAdapter(TabControlHost _tabControlHost) : RecyclerView.Adapter {
  public override int ItemCount => _tabControlHost.DataContext.Tabs.Count;

  public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) =>
    new TabItemHeaderViewHolder(parent.Context!, _tabControlHost);

  public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) {
    ((TabItemHeaderViewHolder)holder)
      .Bind(_tabControlHost.DataContext.Tabs[position], _tabControlHost.DataContext.TabStrip.IconTextVisibility);
  }
}