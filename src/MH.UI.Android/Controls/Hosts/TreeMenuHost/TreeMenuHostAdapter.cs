using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Controls.Hosts.TreeViewHost;
using MH.UI.Android.Controls.Recycler;
using MH.UI.Android.Utils;
using MH.UI.Interfaces;
using MH.Utils.BaseClasses;
using System.Collections.Generic;

namespace MH.UI.Android.Controls.Hosts.TreeMenuHost;

public class TreeMenuHostAdapter(Context _context, TreeMenuHost _host)
  : TreeViewHostAdapterBase(_context, _host.DataContext.RootHolder) {

  public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) =>
    new BaseViewHolder(new TreeMenuItemV(parent.Context!, _host.Close), new(LPU.Match, LPU.Wrap));

  public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) =>
    (holder.ItemView as IBindable<FlatTreeItem>)?.Bind(Items[position]);

  public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position, IList<Java.Lang.Object> payloads) {
    base.OnBindViewHolder(holder, position, payloads);
    if (payloads.Count == 0) return;

    if (holder.ItemView is not TreeMenuItemV view) return;
    var item = Items[position];

    foreach (var payload in payloads)
      if (ReferenceEquals(payload, _isExpandedPayload))
        view.BindIsExpanded(item);
      else if (ReferenceEquals(payload, _isExpandedVisiblePayload))
        view.BindIsExpandedVisible(item);
  }
}