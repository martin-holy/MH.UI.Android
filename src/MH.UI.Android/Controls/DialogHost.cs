using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
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

  public override global::Android.App.Dialog OnCreateDialog(Bundle savedInstanceState) {
    var dialog = base.OnCreateDialog(savedInstanceState);
    dialog.Window?.RequestFeature(WindowFeatures.NoTitle);
    return dialog;
  }

  public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
    var context = Activity ?? throw new InvalidOperationException("Activity is null");
    var rootLayout = new LinearLayout(context) {
      Orientation = Orientation.Vertical,
      LayoutParameters = new ViewGroup.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent)
    };

    // Title bar
    var titleLayout = new LinearLayout(context) {
      Orientation = Orientation.Horizontal,
      Background = new global::Android.Graphics.Drawables.ColorDrawable(Color.DarkGray)
    };

    var titleText = new TextView(context) {
      Text = _dataContext.Title,
      Gravity = GravityFlags.CenterVertical,
      TextSize = 18
    };
    titleLayout.AddView(titleText);
    rootLayout.AddView(titleLayout);

    // Content
    var contentView = _contentViewFactory?.Invoke(context, _dataContext);
    if (contentView != null) rootLayout.AddView(contentView);

    // Buttons
    var buttonsLayout = new LinearLayout(context) {
      Orientation = Orientation.Horizontal
    };
    buttonsLayout.SetGravity(GravityFlags.End);

    foreach (var button in _dataContext.Buttons) {
      var btn = new Button(context) {
        Text = ((RelayCommandBase)button.Command).Text
      };
      btn.Click += (s, e) => button.Command?.Execute(_dataContext);
      buttonsLayout.AddView(btn);
    }

    rootLayout.AddView(buttonsLayout);
    return rootLayout;
  }
}