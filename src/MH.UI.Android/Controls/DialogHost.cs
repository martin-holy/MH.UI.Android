using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using MH.UI.Android.Dialogs;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Dialogs;
using MH.Utils;
using System;
using System.Threading.Tasks;
using Dialog = MH.UI.Controls.Dialog;

namespace MH.UI.Android.Controls;

public class DialogHost : DialogFragment {
  private static WeakReference<FragmentActivity>? _activityRef;
  private static Func<Context, Dialog, View?>? _contentViewFactory;
  private static LinearLayout? _notImplementedDialog;
  private readonly Dialog _dataContext;

  public DialogHost(Dialog dataContext) {
    _dataContext = dataContext;
    this.Bind(dataContext, x => x.Result, (_, _) => Dismiss(), false);
    Cancelable = false;
  }

  public static void Initialize(FragmentActivity activity, Func<Context, Dialog, View?>? contentViewFactory) {
    _activityRef = new WeakReference<FragmentActivity>(activity);
    _contentViewFactory = contentViewFactory;
  }

  public static Task<int> ShowAsync(Dialog dataContext) {
    if (_activityRef == null || !_activityRef.TryGetTarget(out var activity))
      throw new InvalidOperationException("DialogHost not initialized or Activity no longer valid.");

    var fragment = new DialogHost(dataContext);
    activity.RunOnUiThread(() =>
      fragment.Show(activity.SupportFragmentManager, "dialog"));

    return dataContext.TaskCompletionSource.Task;
  }

  private static View? _getDialog(Context context, Dialog dataContext) {
    View? view = dataContext switch {
      GroupByDialog gbDlg => new GroupByDialogV(context, gbDlg),
      InputDialog iDlg => new InputDialogV(context, iDlg),
      MessageDialog mDlg => new MessageDialogV(context, mDlg),
      ToggleDialog tDlg => new ToggleDialogV(context, tDlg),
      _ => null
    };

    view ??= _contentViewFactory?.Invoke(context, dataContext);
    if (view == null) return _getNotImplementedDialog(context, dataContext);

    return view;
  }

  public override global::Android.App.Dialog OnCreateDialog(Bundle? savedInstanceState) {
    var dialog = base.OnCreateDialog(savedInstanceState);

    if (dialog.Window is { } window) {
      window?.SetBackgroundDrawable(new ColorDrawable(global::Android.Graphics.Color.Transparent));
      window?.RequestFeature(WindowFeatures.NoTitle);
      window?.DecorView.SetPadding(0, 0, 0, 0);
    }

    return dialog;
  }

  public override View OnCreateView(LayoutInflater? inflater, ViewGroup? container, Bundle? savedInstanceState) {
    var context = container?.Context ?? Activity!;

    var view = new LinearLayout(context) { Orientation = Orientation.Vertical };
    view.SetBackgroundResource(Resource.Drawable.dialog_background);
    view.SetPadding(DisplayU.DpToPx(1));

    var titleBar = new LinearLayout(context) { Orientation = Orientation.Horizontal };
    titleBar.SetGravity(GravityFlags.CenterVertical);
    titleBar.SetBackgroundResource(Resource.Color.c_black2);
    titleBar.SetPadding(DimensU.Spacing);

    var titleCloseBtn = new IconButton(context).WithCommand(UI.Controls.Dialog.CloseCommand, _dataContext, false);
    titleCloseBtn.SetImageResource(Resource.Drawable.icon_x_close);

    titleBar.AddView(
      new IconView(context).Bind(_dataContext.Icon),
      new LinearLayout.LayoutParams(DimensU.IconSize, DimensU.IconSize));
    titleBar.AddView(
      new TextView(context) { Text = _dataContext.Title, TextSize = 18 },
      new LinearLayout.LayoutParams(0, LPU.Wrap, 1f));
    titleBar.AddView(titleCloseBtn);

    view.AddView(titleBar, new LinearLayout.LayoutParams(LPU.Match, LPU.Wrap));

    if (_getDialog(context, _dataContext) is { } contentView) {
      if (contentView.Parent is ViewGroup oldParent)
        oldParent.RemoveView(contentView);

      view.AddView(contentView, new LinearLayout.LayoutParams(LPU.Wrap, LPU.Wrap));
    }

    view.AddView(_createButtonsView(context, _dataContext),
      new LinearLayout.LayoutParams(LPU.Wrap, LPU.Wrap) {
        Gravity = GravityFlags.End
      });

    return view;
  }

  private static LinearLayout _createButtonsView(Context context, Dialog dataContext) {
    var margin = DimensU.Spacing * 2;
    var view = new LinearLayout(context) { Orientation = Orientation.Horizontal };

    foreach (var button in dataContext.Buttons) {
      var btn = new Button(new ContextThemeWrapper(context, Resource.Style.mh_DialogButton), null, 0)
        .WithCommand(button.Command, dataContext);

      view.AddView(btn, new LinearLayout.LayoutParams(LPU.Wrap, DisplayU.DpToPx(32))
        .WithMargin(0, margin, margin, margin));
    }

    return view;
  }

  private static LinearLayout _getNotImplementedDialog(Context context, Dialog dataContext) {
    _notImplementedDialog ??= _createNotImplementedDialog(context);
    if (_notImplementedDialog.GetChildAt(0) is TextView text)
      text.Text = $"Dialog {dataContext.GetType()} not implemented";

    return _notImplementedDialog;
  }

  private static LinearLayout _createNotImplementedDialog(Context context) {
    var container = new LinearLayout(context) { Orientation = Orientation.Vertical };
    var text = new TextView(context) { Gravity = GravityFlags.Center };
    container.AddView(text);

    return container;
  }
}