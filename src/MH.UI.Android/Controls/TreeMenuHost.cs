using Android.Content;
using MH.UI.Controls;

namespace MH.UI.Android.Controls;

public class TreeMenuHost : TreeViewHostBase<TreeView, TreeViewHostAdapterBase> {
  public TreeMenuHost(Context context, TreeView dataContext) : base(context, dataContext, null) {
    DataContext.Host = this;
    Adapter = new TreeMenuHostAdapter(context, this);
    _recyclerView.SetAdapter(Adapter);
  }
}