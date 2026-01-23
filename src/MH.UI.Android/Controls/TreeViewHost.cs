using Android.Content;
using Android.Widget;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.Utils.BaseClasses;
using System;
using System.Collections.Generic;

namespace MH.UI.Android.Controls;

public class TreeViewHost : TreeViewHostBase<TreeView, TreeViewHostAdapter>, ITreeViewHost {
  public TreeViewHost(
    Context context,
    TreeView dataContext,
    Func<object, IEnumerable<MenuItem>>? itemMenuFactory,
    ITreeItemViewHolderFactory? itemFactory = null)
    : base(context, dataContext, itemMenuFactory) {

    DataContext.Host = this;
    Adapter = new TreeViewHostAdapter(context, this, itemFactory);
    _recyclerView.SetAdapter(Adapter);

    AddView(new SelectionBar(context, DataContext.SelectedTreeItems), new LayoutParams(LPU.Wrap, LPU.Wrap)
      .WithMargin(DimensU.Spacing)
      .WithRule(LayoutRules.AlignParentRight));
  }
}