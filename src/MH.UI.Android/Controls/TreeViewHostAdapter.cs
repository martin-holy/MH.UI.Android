using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace MH.UI.Android.Controls;

public class TreeViewHostAdapter(Context _context, TreeViewHost _host)
  : TreeViewHostAdapterBase(_context, _host.DataContext.RootHolder) {

  public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) =>
    new FlatTreeItemViewHolder(parent.Context!, _host);

  public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) =>
    ((FlatTreeItemViewHolder)holder).Bind(_items[position]);
}