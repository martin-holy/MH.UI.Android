using Android.Content;
using Android.Views;
using MH.UI.Controls;
using MH.Utils.BaseClasses;
using System;
using System.Collections.Generic;

namespace MH.UI.Android.Controls;

public class TreeMenu(Context _context, Func<object, IEnumerable<MenuItem>> _itemMenuFactory) {
  private TreeView? _itemMenuVM;
  private TreeMenuHost? _itemMenuV;

  public void ShowItemMenu(View anchor, object? item) {
    if (item == null) return;
    if (_itemMenuVM == null) {
      _itemMenuVM = new();
      _itemMenuV = new TreeMenuHost(_context, _itemMenuVM);
    }

    if (_itemMenuFactory(item) is not { } menuItems) return;

    _itemMenuVM.RootHolder.Execute(items => {
      items.Clear();
      foreach (var menuItem in menuItems)
        items.Add(menuItem);
    });

    if (_itemMenuV == null || _itemMenuVM.RootHolder.Count == 0) return;
    _itemMenuV.Observer.MenuAnchor = anchor;
    _itemMenuV.Observer.UpdatePopupSize();
    _itemMenuV.Popup.ShowAsDropDown(anchor);
  }
}