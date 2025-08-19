using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace MH.UI.Android.Controls;

public class TreeMenuHostAdapter(Context _context, TreeMenuHost _host)
  : TreeViewHostAdapterBase(_context, _host.DataContext.RootHolder) {

  public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) =>
    new TreeMenuItemViewHolder(parent.Context!);

  public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) =>
    ((TreeMenuItemViewHolder)holder).Bind(_items[position]);
}