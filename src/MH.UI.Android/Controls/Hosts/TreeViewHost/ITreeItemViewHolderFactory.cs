using Android.Views;
using AndroidX.RecyclerView.Widget;
using MH.Utils.Tree;

namespace MH.UI.Android.Controls.Hosts.TreeViewHost;

public interface ITreeItemViewHolderFactory {
  public int GetViewType(FlatTreeItem item);
  public RecyclerView.ViewHolder Create(ViewGroup parent, int viewType, IAndroidTreeViewHost host);
}