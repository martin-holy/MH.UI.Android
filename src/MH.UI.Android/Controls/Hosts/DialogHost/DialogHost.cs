using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using MH.UI.Android.Binding;
using MH.UI.Android.Dialogs;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Dialogs;
using MH.Utils;
using MH.Utils.Disposables;
using System;
using System.Threading.Tasks;
using Dialog = MH.UI.Controls.Dialog;

namespace MH.UI.Android.Controls.Hosts.DialogHost;

public class DialogHost : DialogFragment {
  private static WeakReference<FragmentActivity>? _activityRef;
  private static Func<Context, Dialog, BindingScope, View?>? _contentViewFactory;
  private readonly Dialog _dataContext;
  private readonly BindingScope _bindings = new();

  public DialogHost(Dialog dataContext) {
    _dataContext = dataContext;
    dataContext.Bind(nameof(UI.Controls.Dialog.Result), x => x.Result, _ => Dismiss(), false).DisposeWith(_bindings);
    Cancelable = false;
  }

  public static void Initialize(FragmentActivity activity, Func<Context, Dialog, BindingScope, View?>? contentViewFactory) {
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

  private static View? _getDialog(Context context, Dialog dataContext, BindingScope bindings) {
    View? view = dataContext switch {
      GroupByDialog gbDlg => new GroupByDialogV(context, gbDlg, bindings),
      InputDialog iDlg => new InputDialogV(context, iDlg, bindings),
      MessageDialog mDlg => new MessageDialogV(context, mDlg),
      SelectFromListDialog sflDlg => new SelectFromListDialogV(context, sflDlg),
      ToggleDialog tDlg => new ToggleDialogV(context, tDlg),
      _ => null
    };

    view ??= _contentViewFactory?.Invoke(context, dataContext, bindings);
    if (view == null) return _createNotImplementedDialog(context, dataContext);

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

    var view = LayoutU.Vertical(context);
    view.SetBackgroundResource(Resource.Drawable.dialog_background);
    view.SetPadding(DisplayU.DpToPx(1));
    view.AddView(_titleBarLayout(context), LPU.LinearMatchWrap());

    if (_getDialog(context, _dataContext, _bindings) is { } contentView) {
      if (contentView.Parent is ViewGroup oldParent)
        oldParent.RemoveView(contentView);

      if (contentView.LayoutParameters?.Width == LPU.Match && Dialog?.Window is { } window)
        window.SetLayout(LPU.Match, LPU.Wrap);

      view.AddView(contentView, contentView.LayoutParameters ?? LPU.LinearWrap());
    }

    view.AddView(_buttonsLayout(context), LPU.Linear(LPU.Wrap, LPU.Wrap, GravityFlags.End));

    return view;
  }

  public override void OnDestroyView() {
    _bindings.Dispose();
    base.OnDestroyView();
  }

  private LinearLayout _titleBarLayout(Context context) {
    var icon = new IconView(context, _dataContext.Icon);
    var title = new TextView(context) { Text = _dataContext.Title, TextSize = 18 };
    var closeBtn = new IconButton(context).WithClickCommand(UI.Controls.Dialog.CloseCommand, _bindings, _dataContext, false);
    closeBtn.SetImageResource(Resource.Drawable.icon_x_close);

    var bar = LayoutU.Horizontal(context);
    bar.SetGravity(GravityFlags.CenterVertical);
    bar.SetBackgroundResource(Resource.Color.c_black2);
    bar.SetPadding(DimensU.Spacing);

    bar.AddView(icon, LPU.Linear(DimensU.IconSize, DimensU.IconSize));
    bar.AddView(title, LPU.Linear(0, LPU.Wrap, 1f));
    bar.AddView(closeBtn, LPU.LinearWrap());

    return bar;
  }

  private LinearLayout _buttonsLayout(Context context) {
    var margin = DimensU.Spacing * 2;
    var view = LayoutU.Horizontal(context);

    foreach (var button in _dataContext.Buttons) {
      var btn = new Button(new ContextThemeWrapper(context, Resource.Style.mh_DialogButton), null, 0)
        .WithClickCommand(button.Command, _bindings, _dataContext);

      view.AddView(btn, LPU.Linear(LPU.Wrap, DisplayU.DpToPx(32))
        .WithMargin(0, margin, margin, margin));
    }

    return view;
  }

  private static FrameLayout _createNotImplementedDialog(Context context, Dialog dataContext) =>
    new FrameLayout(context)
      .Add(new TextView(context) {
        Text = $"{dataContext.GetType()}\nnot implemented",
        Gravity = GravityFlags.Center
      }, LPU.FrameMatch());
}