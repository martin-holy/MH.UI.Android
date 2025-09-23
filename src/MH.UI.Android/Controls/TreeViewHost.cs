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
  private readonly SelectionBar _selectionBar;

  public TreeViewHost(Context context, TreeView dataContext, Func<object, IEnumerable<MenuItem>>? itemMenuFactory)
    : base(context, dataContext, itemMenuFactory) {
    DataContext.Host = this;
    Adapter = new TreeViewHostAdapter(context, this);
    _recyclerView.SetAdapter(Adapter);
    _selectionBar = new SelectionBar(context, DataContext.SelectedTreeItems);

    AddView(_selectionBar, new LayoutParams(LPU.Wrap, LPU.Wrap)
      .WithMargin(DimensU.Spacing)
      .WithRule(LayoutRules.AlignParentRight));
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      _selectionBar.Dispose();
    }
    base.Dispose(disposing);
  }
}