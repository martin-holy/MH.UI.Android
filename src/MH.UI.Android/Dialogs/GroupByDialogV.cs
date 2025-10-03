using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Controls;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Dialogs;

namespace MH.UI.Android.Dialogs;

public sealed class GroupByDialogV : LinearLayout {
  private readonly TreeViewHost _groupByItems;
  private readonly RadioButton _isGroupBy;
  private readonly RadioButton _isThenBy;
  private readonly CheckBox _isRecursive;
  private bool _disposed;

  public GroupByDialog DataContext { get; private set; }

  public GroupByDialogV(Context context, GroupByDialog dataContext) : base(context) {
    DataContext = dataContext;
    dataContext.TreeView.MultiSelect = true;

    Orientation = Orientation.Vertical;
    SetPadding(0, 0, 0, DisplayU.DpToPx(10));
    SetGravity(GravityFlags.CenterVertical);

    _groupByItems = new TreeViewHost(context, dataContext.TreeView, null);

    _isGroupBy = new RadioButton(context) { Text = "Group by", Checked = dataContext.IsGroupBy };
    _isGroupBy.CheckedChange += _isGroupByCheckedChange;

    _isThenBy = new RadioButton(context) { Text = "Group by - Then by", Checked = dataContext.IsThenBy };
    _isThenBy.CheckedChange += _isThenByCheckedChange;

    var groupMode = new RadioGroup(context) { Orientation = Orientation.Horizontal };
    groupMode.SetPadding(DimensU.Spacing);
    groupMode.AddView(_isGroupBy);
    groupMode.AddView(_isThenBy);

    _isRecursive = new CheckBox(context) { Text = "Group recursive", Checked = dataContext.IsRecursive };
    _isRecursive.CheckedChange += _isRecursiveCheckedChange;

    AddView(_groupByItems, new LayoutParams(LPU.Match, DisplayU.DpToPx(300), 1f));
    AddView(groupMode, new LayoutParams(LPU.Wrap, LPU.Wrap));
    AddView(_isRecursive);
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      _isGroupBy.CheckedChange -= _isGroupByCheckedChange;
      _isThenBy.CheckedChange -= _isThenByCheckedChange;
      _isRecursive.CheckedChange -= _isRecursiveCheckedChange;
    }
    _disposed = true;
    base.Dispose(disposing);
  }

  private void _isGroupByCheckedChange(object? sender, CompoundButton.CheckedChangeEventArgs e) {
    DataContext.IsGroupBy = e.IsChecked;
  }

  private void _isThenByCheckedChange(object? sender, CompoundButton.CheckedChangeEventArgs e) {
    DataContext.IsThenBy = e.IsChecked;
  }

  private void _isRecursiveCheckedChange(object? sender, CompoundButton.CheckedChangeEventArgs e) {
    DataContext.IsRecursive = e.IsChecked;
  }
}