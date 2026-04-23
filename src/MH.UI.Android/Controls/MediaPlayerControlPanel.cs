using Android.Content;
using Android.Views;
using Android.Widget;
using MH.UI.Android.Binding;
using MH.UI.Android.Extensions;
using MH.UI.Android.Utils;
using MH.UI.Controls;
using MH.Utils;
using MH.Utils.Disposables;
using MH.Utils.EventsArgs;
using System.Windows.Input;

namespace MH.UI.Android.Controls;

public class MediaPlayerControlPanel : LinearLayout {
  private readonly MediaPlayer _mediaPlayer;
  private readonly BindingScope _bindings = new();

  public MediaPlayerControlPanel(Context? context, MediaPlayer mediaPlayer, ICommand? playCommand = null) : base(context) {
    Orientation = Orientation.Vertical;
    _mediaPlayer = mediaPlayer;

    var trackBar = new Slider(context, new() { TickFrequency = _mediaPlayer.TimelineSmallChange })
      .BindProgress(_mediaPlayer, nameof(MediaPlayer.TimelinePosition), x => x.TimelinePosition, (s, p) => s.TimelinePosition = p, _bindings)
      .BindMaximum(_mediaPlayer, nameof(MediaPlayer.TimelineMaximum), x => x.TimelineMaximum, _bindings);

    trackBar.StartTrackingTouch += (_, _) => _mediaPlayer.TimelineSliderChangeStartedCommand.Execute(null);
    trackBar.Selector.Bind(nameof(ValueSelector.Value), x => x.Value,
      x => _mediaPlayer.TimelineSliderValueChangedCommand.Execute(new PropertyChangedEventArgs<double>(0, x)), false);
    trackBar.StopTrackingTouch += (_, _) => _mediaPlayer.TimelineSliderChangeEndedCommand.Execute(null);

    AddView(trackBar, LPU.LinearMatchWrap());
    AddView(_createNavigationButtons(playCommand), LPU.LinearWrap(GravityFlags.CenterHorizontal));
  }

  private LinearLayout _createNavigationButtons(ICommand? playCommand = null) =>
    LayoutU.Horizontal(Context)
      .Add(new IconButton(Context).WithClickCommand(_mediaPlayer.TimelineShiftBeginningCommand, _bindings), LPU.LinearWrap())
      .Add(new IconButton(Context).WithClickCommand(_mediaPlayer.TimelineShiftLargeBackCommand, _bindings), LPU.LinearWrap())
      .Add(new IconButton(Context).WithClickCommand(_mediaPlayer.TimelineShiftSmallBackCommand, _bindings), LPU.LinearWrap())
      .Add(new IconButton(Context).WithClickCommand(playCommand ?? _mediaPlayer.PlayCommand, _bindings)
        .BindVisibility(_mediaPlayer, nameof(MediaPlayer.IsPlaying), x => !x.IsPlaying, _bindings), LPU.LinearWrap())
      .Add(new IconButton(Context).WithClickCommand(_mediaPlayer.PauseCommand, _bindings)
        .BindVisibility(_mediaPlayer, nameof(MediaPlayer.IsPlaying), x => x.IsPlaying, _bindings), LPU.LinearWrap())
      .Add(new IconButton(Context).WithClickCommand(_mediaPlayer.TimelineShiftSmallForwardCommand, _bindings), LPU.LinearWrap())
      .Add(new IconButton(Context).WithClickCommand(_mediaPlayer.TimelineShiftLargeForwardCommand, _bindings), LPU.LinearWrap())
      .Add(new IconButton(Context).WithClickCommand(_mediaPlayer.TimelineShiftEndCommand, _bindings), LPU.LinearWrap());

  protected override void Dispose(bool disposing) {
    if (disposing) _bindings.Dispose();
    base.Dispose(disposing);
  }
}