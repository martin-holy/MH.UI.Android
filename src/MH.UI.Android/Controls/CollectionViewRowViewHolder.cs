using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MH.UI.Interfaces;
using MH.Utils.BaseClasses;
using MH.Utils.Interfaces;

namespace MH.UI.Android.Controls;

public class CollectionViewRowViewHolder(View itemView, CollectionViewHost host) : RecyclerView.ViewHolder(itemView) {
  private readonly CollectionViewHost _host = host;
  private readonly LinearLayout _container = itemView.FindViewById<LinearLayout>(Resource.Id.row_container)!;

  public FlatTreeItem? Item { get; private set; }

  public static CollectionViewRowViewHolder Create(ViewGroup parent, CollectionViewHost host) =>
    new(LayoutInflater.From(parent.Context)!.Inflate(Resource.Layout.collection_view_row, parent, false)!, host);

  public void Bind(FlatTreeItem? item) {
    Item = item;
    _container.RemoveAllViews();

    if (item?.TreeItem is not ICollectionViewRow row || row is not ITreeItem { Parent: ICollectionViewGroup group }) return;

    foreach (var rowItem in row.Leaves) {
      if (_host.GetItemView(_container, group, rowItem) is not { } view) continue;
      view.Clickable = true;
      view.Click += (_, _) => {
        if (_host.ViewModel is not { } vm) return;        
        if (vm.CanSelect) vm.SelectItem(row, rowItem, false, false);
        if (vm.CanOpen) vm.OpenItem(rowItem);
      };
      _container.AddView(view);
    }
  }
}