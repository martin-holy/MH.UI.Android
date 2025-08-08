using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.Utils.BaseClasses;
using System;
using System.Threading.Tasks;
using Color = Android.Graphics.Color;
using Dialog = MH.UI.Controls.Dialog;

namespace MH.UI.Android.Controls;

public class DialogHost : DialogFragment {
  private static FragmentActivity? _currentActivity;
  private static Func<Context, Dialog, View?>? _contentViewFactory;
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

    if (_contentViewFactory?.Invoke(context, _dataContext) is { } contentView)
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
    view.SetImageDrawable(Icons.GetIcon(context, dataContext.Icon)); // todo X icon

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
}