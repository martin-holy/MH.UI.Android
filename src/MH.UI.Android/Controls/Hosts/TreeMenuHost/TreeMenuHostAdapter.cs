using Android.Views;
using AndroidX.RecyclerView.Widget;
using MH.UI.Android.Controls.Hosts.TreeViewHost;
using MH.UI.Android.Controls.Recycler;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Interfaces;
using MH.Utils.BaseClasses;
using System.Collections.Generic;

namespace MH.UI.Android.Controls.Hosts.TreeMenuHost;

public class TreeMenuHostAdapter(TreeMenuHost _host) : TreeViewHostAdapterBase(_host.DataContext.RootHolder) {

  public override int GetItemViewType(int position) =>
    Items[position].TreeItem is MenuItemSeparator ? 1 : 0;

  public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) {
    if (viewType == 1)
      return new BaseViewHolder(new TreeMenuSeparatorV(parent.Context!),
        new RecyclerView.LayoutParams(LPU.Match, DimensU.MenuItemSeparatorHeight).WithDpMargin(4, 2, 4, 2));

    return new BaseViewHolder(new TreeMenuItemV(parent.Context!, _host.Close), new(LPU.Match, LPU.Wrap));
  }

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