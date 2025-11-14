using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Controls;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.UI.Dialogs;

namespace MH.UI.Android.Dialogs;

public sealed class GroupByDialogV : LinearLayout {
  public GroupByDialogV(Context context, GroupByDialog dataContext) : base(context) {
    Orientation = Orientation.Vertical;
    SetPadding(0, 0, 0, DisplayU.DpToPx(10));
    SetGravity(GravityFlags.CenterVertical);

    var groupMode = new RadioGroup(context) { Orientation = Orientation.Horizontal };
    groupMode.SetPadding(DimensU.Spacing);
    groupMode.AddView(new RadioButton(context) { Text = "Group by" });
    groupMode.AddView(new RadioButton(context) { Text = "Group by - Then by" });
    groupMode.BindChecked(dataContext, x => x.GroupMode, [GroupMode.GroupBy, GroupMode.ThenBy], out _);

    AddView(new TreeViewHost(context, dataContext.TreeView, null), new LayoutParams(LPU.Match, DisplayU.DpToPx(300), 1f));
    AddView(groupMode, new LayoutParams(LPU.Wrap, LPU.Wrap));
    AddView(new CheckBox(context) { Text = "Group recursive" }.BindChecked(dataContext, x => x.IsRecursive, out _));
  }
}