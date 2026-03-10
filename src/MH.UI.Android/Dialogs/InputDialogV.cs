using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.Core.Content;
using MH.UI.Android.Binding;
using MH.UI.Android.Controls;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Dialogs;
using MH.Utils;
using MH.Utils.Disposables;

namespace MH.UI.Android.Dialogs;

public sealed class InputDialogV : LinearLayout {
  public InputDialogV(Context context, InputDialog dataContext, BindingScope bindings) : base(context) {
    Orientation = Orientation.Horizontal;
    SetMinimumWidth(DisplayU.DpToPx(300));
    SetPadding(0, DisplayU.DpToPx(10), 0, DisplayU.DpToPx(10));
    SetGravity(GravityFlags.CenterVertical);

    var icon = new IconView(context, dataContext.Icon);
    var message = new TextView(context) { Text = dataContext.Message };
    var answer = new EditText(context).BindText(dataContext, nameof(InputDialog.Answer), x => x.Answer, (s, v) => s.Answer = v, bindings);

    dataContext.Bind(nameof(InputDialog.Error), x => x.Error, error => {
      answer.Hint = dataContext.ErrorMessage; // TODO Find out other way to show error message.
      if (answer.Background == null) return;
      if (error)
        answer.Background!.SetColorFilter(new PorterDuffColorFilter(
          new Color(ContextCompat.GetColor(Context, Resource.Color.c_input_error)),
          PorterDuff.Mode.SrcAtop!));
      else
        answer.Background!.ClearColorFilter();
    }).DisposeWith(bindings);

    var messageAndAnswer = new LinearLayout(context) { Orientation = Orientation.Vertical };
    messageAndAnswer.AddView(message, new LayoutParams(LPU.Wrap, LPU.Wrap).WithDpMargin(5));
    messageAndAnswer.AddView(answer, new LayoutParams(LPU.Match, LPU.Wrap).WithDpMargin(5, 5, 10, 5));

    AddView(icon, new LayoutParams(DisplayU.DpToPx(32), DisplayU.DpToPx(32)).WithDpMargin(10));
    AddView(messageAndAnswer, new LayoutParams(LPU.Wrap, LPU.Wrap, 1f));

    Post(() => {
      answer.RequestFocus();
      if (Context?.GetSystemService(Context.InputMethodService) is InputMethodManager imm)
        imm.ShowSoftInput(answer, ShowFlags.Implicit);
    });
  }
}