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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Color = Android.Graphics.Color;
using Dialog = MH.UI.Controls.Dialog;

namespace MH.UI.Android.Controls;

public interface IDialogHostContent {
  public View Bind(Dialog dataContext);
}

public class DialogHost : DialogFragment {
  private static FragmentActivity? _currentActivity;
  private static Func<Context, Dialog, View?>? _contentViewFactory;
  private static LinearLayout? _notImplementedDialog;
  private static readonly Dictionary<Type, IDialogHostContent> _dialogs = [];
  private readonly Dialog _dataContext;

  public DialogHost(Dialog dataContext) {
    _dataContext = dataContext;
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

    View? host = dataContext switch {
      InputDialog => new InputDialogV(context),
      _ => null
    };

    host ??= _contentViewFactory?.Invoke(context, dataContext);
    dialog = host as IDialogHostContent;
    if (dialog == null) return _getNotImplementedDialog(context, dataContext);
    _dialogs.Add(type, dialog);

    return dialog.Bind(dataContext);
  }

  public override void OnDismiss(IDialogInterface dialog) {
    base.OnDismiss(dialog);
    _dataContext.TaskCompletionSource.TrySetResult(_dataContext.Result);
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

  public override View OnCreateView(LayoutInflater? inflater, ViewGroup? container, Bundle? savedInstanceState) {
    var context = container?.Context ?? Activity!;
    var rootView = _createRootView(context);

    var titleBar = _createTitleBarView(context);
    titleBar.AddView(_createTitleIconView(context, _dataContext));
    titleBar.AddView(_createTitleTextView(context, _dataContext));
    titleBar.AddView(_createTitleCloseBtnView(context, _dataContext));
    rootView.AddView(titleBar);

    if (_getDialog(context, _dataContext) is { } contentView)
      rootView.AddView(contentView);

    rootView.AddView(_createButtonsView(context, _dataContext));

    return rootView;
  }

  private static LinearLayout _createRootView(Context context) {
    var view = new LinearLayout(context) {
      LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent),
      Orientation = Orientation.Vertical
    };
    view.SetBackgroundResource(Resource.Drawable.dialog_background);

    return view;
  }

  private static LinearLayout _createTitleBarView(Context context) {
    var view = new LinearLayout(context) {
      LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent),
      Orientation = Orientation.Horizontal
    };
    view.SetGravity(GravityFlags.CenterVertical);
    view.SetBackgroundResource(Resource.Color.c_black2);
    view.SetPadding(context.Resources!.GetDimensionPixelSize(Resource.Dimension.general_padding));

    return view;
  }

  private static ImageView _createTitleIconView(Context context, Dialog dataContext) {
    var size = context.Resources!.GetDimensionPixelSize(Resource.Dimension.icon_size);
    var view = new ImageView(context) {
      LayoutParameters = new ViewGroup.LayoutParams(size, size)
    };
    view.SetImageDrawable(Icons.GetIcon(context, dataContext.Icon));

    return view;
  }

  private static TextView _createTitleTextView(Context context, Dialog dataContext) {
    var view = new TextView(context) {
      LayoutParameters = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent) {
        Weight = 1
      },
      Text = dataContext.Title,
      TextSize = 18
    };

    return view;
  }

  private static IconButton _createTitleCloseBtnView(Context context, Dialog dataContext) {
    var view = new IconButton(context); // todo command bind
    view.SetImageResource(Resource.Drawable.icon_x_close);

    return view;
  }

  private static LinearLayout _createButtonsView(Context context, Dialog dataContext) {
    var view = new LinearLayout(context) {
      Orientation = Orientation.Horizontal
    };
    view.SetGravity(GravityFlags.End);

    // todo bind commands
    foreach (var button in dataContext.Buttons) {
      var btn = new Button(context) {
        Text = ((RelayCommandBase)button.Command).Text
      };
      btn.Click += (s, e) => button.Command?.Execute(dataContext);
      view.AddView(btn);
    }

    return view;
  }

  private static LinearLayout _getNotImplementedDialog(Context context, Dialog dataContext) {
    _notImplementedDialog ??= _createNotImplementedDialog(context);
    if (_notImplementedDialog.GetChildAt(0) is TextView text)
      text.SetText($"Dialog {dataContext.GetType()} not implemented", TextView.BufferType.Normal);

    return _notImplementedDialog;
  }

  private static LinearLayout _createNotImplementedDialog(Context context) {
    var view = new LinearLayout(context) {
      Orientation = Orientation.Vertical,
      LayoutParameters = new ViewGroup.LayoutParams(
        ViewGroup.LayoutParams.MatchParent,
        ViewGroup.LayoutParams.WrapContent)
    };

    var textView = new TextView(context) {
      Gravity = GravityFlags.Center
    };
    view.AddView(textView);

    return view;
  }
}