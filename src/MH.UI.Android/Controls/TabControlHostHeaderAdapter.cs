using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace MH.UI.Android.Controls;

public class TabControlHostHeaderAdapter(TabControlHost host) : RecyclerView.Adapter {
  private readonly TabControlHost _host = host;

  public override int ItemCount => _host.DataContext.Tabs.Count;

  public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) {
    var header = TabItemHeaderViewHolder.Create(parent);
    header.SelectedAction = item => _host.DataContext.Selected = item;
    return header;
  }

  public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) {
    ((TabItemHeaderViewHolder)holder).Bind(_host.DataContext.Tabs[position]);
  }
}