using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Controls;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.UI.Dialogs;
using System;

namespace MH.UI.Android.Dialogs;

public class GroupByDialogV : LinearLayout, IDialogContentV {
  private readonly TreeViewHost _treeViewHost;
  private readonly RadioButton _isGroupBy;
  private readonly RadioButton _isThenBy;
  private readonly CheckBox _isRecursive;
  private bool _disposed;

  public GroupByDialog? DataContext { get; private set; }

  public GroupByDialogV(Context context, GroupByDialog dataContext) : base(context) {
    DataContext = dataContext;
    _createThisView();

    _treeViewHost = new(context, dataContext.TreeView, null) {
      LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.MatchParent, DisplayU.DpToPx(300), 1f)
    };

    _isGroupBy = new(context) { Text = "Group by" };
    _isGroupBy.CheckedChange += _isGroupByCheckedChange;

    _isThenBy = new(context) { Text = "Group by - Then by" };
    _isThenBy.CheckedChange += _isThenByCheckedChange;

    var groupMode = new LinearLayout(context) {
      Orientation = Orientation.Horizontal,
    };
    groupMode.AddView(_isGroupBy);
    groupMode.AddView(_isThenBy);

    _isRecursive = new(context) { Text = "Group recursive" };
    _isRecursive.CheckedChange += _isRecursiveCheckedChange;

    AddView(_treeViewHost);
    AddView(groupMode);
    AddView(_isRecursive);
  }
  
  public View Bind(Dialog dataContext) {
    if (dataContext is not GroupByDialog vm) throw new InvalidOperationException();
    DataContext = vm;
    _isGroupBy.Checked = vm.IsGroupBy;
    _isThenBy.Checked = vm.IsThenBy;
    _isRecursive.Checked = vm.IsRecursive;

    return this;
  }

  private void _createThisView() {
    Orientation = Orientation.Vertical;
    LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent);

    SetPadding(0, 0, 0, DisplayU.DpToPx(10));
    SetGravity(GravityFlags.CenterVertical);
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
    if (DataContext == null) return;
    DataContext.IsGroupBy = e.IsChecked;
  }

  private void _isThenByCheckedChange(object? sender, CompoundButton.CheckedChangeEventArgs e) {
    if (DataContext == null) return;
    DataContext.IsThenBy = e.IsChecked;
  }

  private void _isRecursiveCheckedChange(object? sender, CompoundButton.CheckedChangeEventArgs e) {
    if (DataContext == null) return;
    DataContext.IsRecursive = e.IsChecked;
  }
}