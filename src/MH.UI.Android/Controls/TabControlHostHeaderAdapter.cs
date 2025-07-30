using Android.Views;
using AndroidX.RecyclerView.Widget;
using MH.UI.Controls;

namespace MH.UI.Android.Controls;

public class TabControlHostHeaderAdapter(TabControl _tabControl) : RecyclerView.Adapter {
  public override int ItemCount => _tabControl.Tabs.Count;

  public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) {
    var header = TabItemHeaderViewHolder.Create(parent);
    header.SelectedAction = item => _tabControl.Selected = item;
    return header;
  }

  public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) {
    ((TabItemHeaderViewHolder)holder).Bind(_tabControl.Tabs[position], _tabControl.TabStrip.IconTextVisibility);
  }
}