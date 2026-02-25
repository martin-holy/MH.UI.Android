using Android.Views;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Controls.Recycler;
using MH.UI.Android.Utils;
using MH.Utils.BaseClasses;

namespace MH.UI.Android.Controls.Hosts.TreeViewHost;

internal sealed class DefaultTreeItemViewHolderFactory : ITreeItemViewHolderFactory {
  public int GetViewType(FlatTreeItem item) => 0;

  public RecyclerView.ViewHolder Create(ViewGroup parent, int viewType, IAndroidTreeViewHost host) =>
    new BaseViewHolder(new FlatTreeItemV(parent.Context!, (TreeViewHost)host), new(LPU.Match, LPU.Wrap));
}