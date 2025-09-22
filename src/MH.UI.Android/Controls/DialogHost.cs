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
using MH.Utils.BaseClasses;
using MH.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Color = Android.Graphics.Color;
using Dialog = MH.UI.Controls.Dialog;

namespace MH.UI.Android.Controls;

public interface IDialogContentV {
  public View Bind(Dialog dataContext);
}

public class DialogHost : DialogFragment {
  private static FragmentActivity? _currentActivity;
  private static Func<Context, Dialog, View?>? _contentViewFactory;
  private static LinearLayout? _notImplementedDialog;
  private static readonly Dictionary<Type, IDialogContentV> _dialogs = [];
  private readonly Dialog _dataContext;
  private readonly List<CommandBinding> _commandBindings = [];
  private bool _disposed;

  public DialogHost(Dialog dataContext) {
    _dataContext = dataContext;
    dataContext.PropertyChanged += _onDataContextPropertyChanged;
    Cancelable = false;
  }

  public static void Initialize(FragmentActivity activity, Func<Context, Dialog, View?> contentViewFactory) {
    _currentActivity = activity;
    _contentViewFactory = contentViewFactory;
  }

  public static Task<int> ShowAsync(Dialog dataContext) {
    if (_currentActivity == null || _contentViewFactory == null)
      throw new InvalidOperationException("DialogHost not initialized");

    var fragment = new DialogHost(dataContext);
    _currentActivity.RunOnUiThread(() => {
      fragment.Show(_currentActivity.SupportFragmentManager, "dialog");
    });

    return dataContext.TaskCompletionSource.Task;
  }

  private static View? _getDialog(Context context, Dialog dataContext) {
    var type = dataContext.GetType();
    if (_dialogs.TryGetValue(type, out var dialog))
      return dialog.Bind(dataContext);

    View? view = dataContext switch {
      GroupByDialog => new GroupByDialogV(context, (GroupByDialog)dataContext),
      InputDialog => new InputDialogV(context),
      MessageDialog => new MessageDialogV(context),
      ToggleDialog => new ToggleDialogV(context),
      _ => null
    };

    view ??= _contentViewFactory?.Invoke(context, dataContext);
    dialog = view as IDialogContentV;
    if (dialog == null) return _getNotImplementedDialog(context, dataContext);
    _dialogs.Add(type, dialog);

    return dialog.Bind(dataContext);
  }

  public override global::Android.App.Dialog OnCreateDialog(Bundle? savedInstanceState) {
    var dialog = base.OnCreateDialog(savedInstanceState);

    if (dialog.Window is { } window) {
      window?.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
      window?.RequestFeature(WindowFeatures.NoTitle);
      window?.DecorView.SetPadding(0, 0, 0, 0);
    }

    return dialog;
  }

  protected override void Dispose(bool disposing) {
    if (_disposed) return;
    if (disposing) {
      _dataContext.PropertyChanged -= _onDataContextPropertyChanged;
      foreach (var cb in _commandBindings) cb.Dispose();
      _commandBindings.Clear();
    }
    _disposed = true;
    base.Dispose(disposing);
  }

  private void _onDataContextPropertyChanged(object? sender, PropertyChangedEventArgs e) {
    if (e.Is(nameof(MH.UI.Controls.Dialog.Result))) Dismiss();
  }

  public override View OnCreateView(LayoutInflater? inflater, ViewGroup? container, Bundle? savedInstanceState) {
    var context = container?.Context ?? Activity!;

    var view = new LinearLayout(context) { Orientation = Orientation.Vertical };
    view.SetBackgroundResource(Resource.Drawable.dialog_background);
    view.SetPadding(DisplayU.DpToPx(1));

    var titleBar = new LinearLayout(context) { Orientation = Orientation.Horizontal };
    titleBar.SetGravity(GravityFlags.CenterVertical);
    titleBar.SetBackgroundResource(Resource.Color.c_black2);
    titleBar.SetPadding(context.Resources!.GetDimensionPixelSize(Resource.Dimension.general_padding));

    var titleCloseBtn = new IconButton(context);
    titleCloseBtn.SetImageResource(Resource.Drawable.icon_x_close);
    _commandBindings.Add(new(titleCloseBtn, MH.UI.Controls.Dialog.CloseCommand, _dataContext));

    titleBar.AddView(new IconView(context).Bind(_dataContext.Icon));
    titleBar.AddView(
      new TextView(context) { Text = _dataContext.Title, TextSize = 18 },
      new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, 1f));
    titleBar.AddView(titleCloseBtn);

    view.AddView(titleBar, new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));

    if (_getDialog(context, _dataContext) is { } contentView) {
      if (contentView.Parent is ViewGroup oldParent)
        oldParent.RemoveView(contentView);

      view.AddView(contentView, new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
    }

    view.AddView(_createButtonsView(context, _dataContext),
      new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent) {
        Gravity = GravityFlags.End
      });

    return view;
  }

  private LinearLayout _createButtonsView(Context context, Dialog dataContext) {
    var margin = context.Resources!.GetDimensionPixelSize(Resource.Dimension.general_padding) * 2;
    var view = new LinearLayout(context) { Orientation = Orientation.Horizontal };

    foreach (var button in dataContext.Buttons) {
      var btn = new Button(new ContextThemeWrapper(context, Resource.Style.mh_DialogButton), null, 0) {
        Text = ((RelayCommandBase)button.Command).Text
      };
      _commandBindings.Add(new(btn, button.Command, dataContext));

      view.AddView(btn, new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, DisplayU.DpToPx(32))
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