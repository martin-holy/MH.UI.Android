using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Utils;
using MH.Utils;
using MH.Utils.Interfaces;
using System;

namespace MH.UI.Android.Controls;

public class SelectionBar : LinearLayout {
  private readonly Selecting<ITreeItem> _selecting;
  private readonly TextView _selectedCount;
  private readonly IconButton _clearSelectionBtn;

  public SelectionBar(Context context, Selecting<ITreeItem> selecting) : base(context) {
    _selecting = selecting;
    selecting.ItemsChangedEvent += _onSelectedTreeItemsChanged;
    selecting.AllDeselectedEvent += _onSelectedTreeItemsAllDeselected;

    _selectedCount = new TextView(context);

    _clearSelectionBtn = new IconButton(context);
    _clearSelectionBtn.SetImageResource(Resource.Drawable.icon_x_close);
    _clearSelectionBtn.Click += _onClearSelectionBtnClick;

    var gp = context.Resources!.GetDimensionPixelSize(Resource.Dimension.general_padding);
    Orientation = global::Android.Widget.Orientation.Horizontal;
    Visibility = ViewStates.Gone;
    Background = BackgroundFactory.RoundSolidDark();
    SetGravity(GravityFlags.CenterVertical);
    SetPadding(gp, 0, gp, 0);
    AddView(_selectedCount);
    AddView(_clearSelectionBtn);
  }

  private void _onSelectedTreeItemsChanged(object? sender, ITreeItem[] e) =>
    _updateSelectionBar(e);

  private void _onSelectedTreeItemsAllDeselected(object? sender, EventArgs e) =>
    _updateSelectionBar(null);

  private void _updateSelectionBar(ITreeItem[]? items) {
    if (items == null || items.Length < 2) {
      Visibility = ViewStates.Gone;
      return;
    }

    Visibility = ViewStates.Visible;
    _selectedCount.Text = items.Length.ToString();
  }

  private void _onClearSelectionBtnClick(object? sender, EventArgs e) =>
    _selecting.DeselectAll();

  protected override void Dispose(bool disposing) {
    if (disposing) {
      _selecting.ItemsChangedEvent -= _onSelectedTreeItemsChanged;
      _selecting.AllDeselectedEvent -= _onSelectedTreeItemsAllDeselected;
      _clearSelectionBtn.Click -= _onClearSelectionBtnClick;
    }
    base.Dispose(disposing);
  }
}