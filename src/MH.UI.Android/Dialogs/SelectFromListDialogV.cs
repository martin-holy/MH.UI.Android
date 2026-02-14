using Android.Content;
using MH.UI.Android.Controls.Items;
using MH.UI.Android.Utils;
using MH.UI.Android.Views;
using MH.UI.Dialogs;
using MH.Utils.Interfaces;

namespace MH.UI.Android.Dialogs;

public class SelectFromListDialogV : SelectableItemsView<IListItem> {
  private readonly SelectFromListDialog _dataContext;

  public SelectFromListDialogV(Context context, SelectFromListDialog dataContext) : base(context, x => new ListItemV(x)) {
    SetMinimumWidth(DisplayU.DpToPx(300));
    BindItems(dataContext.Items);
    _dataContext = dataContext;
    Selection.ItemSelectionChanged += _onItemSelectionChanged;
  }

  private void _onItemSelectionChanged(IListItem item, bool selected) {
    _dataContext.SelectedItem = selected ? item : null;
  }
}