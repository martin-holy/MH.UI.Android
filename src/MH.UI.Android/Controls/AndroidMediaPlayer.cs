using Android.Media;
using Android.Views;
using MH.UI.Interfaces;
using System;
using static Android.Media.MediaPlayer;

namespace MH.UI.Android.Controls;

public class AndroidMediaPlayer : Java.Lang.Object, IPlatformSpecificUiMediaPlayer {
  private readonly MediaPlayer _player = new();

  private Surface? _surface;
  private string? _source;
  private bool _isPrepared;
  private int? _pendingPositionMs;

  public MH.UI.Controls.MediaPlayer? ViewModel { get; set; }

  public double SpeedRatio {
    get => _player.PlaybackParams?.Speed ?? 1.0;
    set {
      // BUG Java.Lang.IllegalStateException
      //if (global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.M) {
      //  var p = PlaybackParams ?? new global::Android.Media.PlaybackParams();
      //  p.SetSpeed((float)value);
      //  PlaybackParams = p;
      //}
    }
  }

  public double Volume {
    get => 1.0; // MediaPlayer doesn't expose getter
    set => _player.SetVolume((float)value, (float)value);
  }

  public bool IsMuted {
    get => false;
    set => _player.SetVolume(value ? 0f : 1f, value ? 0f : 1f);
  }

  public TimeSpan Position {
    get => TimeSpan.FromMilliseconds(_player.CurrentPosition);
    set {
      var ms = (int)value.TotalMilliseconds;

      if (_isPrepared)
        _player.SeekTo(ms);
      else
        _pendingPositionMs = ms;
    }
  }

  public Uri? Source {
    get => _source == null ? null : new Uri(_source);
    set {
      Stop();
      _source = value?.ToString();
      _isPrepared = false;
    }
  }

  public event EventHandler<(int Width, int Height)>? VideoSizeChanged;

  public AndroidMediaPlayer() {
    _player.Prepared += _onPrepared;
    _player.VideoSizeChanged += _onVideoSizeChanged;
  }

  public void SetSurface(Surface? surface) {
    _surface = surface;
    _player.SetSurface(surface);
  }

  private void _tryPrepare() {
    if (_surface == null || string.IsNullOrEmpty(_source)) return;

    try {
      _player.Reset();

      _player.SetSurface(_surface);
      _player.SetDataSource(_source);

      _isPrepared = false;
      _player.PrepareAsync();
    }
    catch (Exception ex) {
      MH.Utils.Log.Error(ex);
    }
  }

  // TODO OnMediaEnded
  private void _onPrepared(object? sender, EventArgs e) {
    _isPrepared = true;

    if (_pendingPositionMs.HasValue) {
      _player.SeekTo(_pendingPositionMs.Value);
      _pendingPositionMs = null;
    }

    ViewModel?.OnMediaOpened(_player.Duration);
    _player.Start();
  }

  private void _onVideoSizeChanged(object? sender, VideoSizeChangedEventArgs e) {
    VideoSizeChanged?.Invoke(this, (e.Width, e.Height));
  }

  public void Play() {
    if (_isPrepared) {
      _player.Start();
      return;
    }

    _tryPrepare();
  }

  public void Pause() {
    if (_player.IsPlaying)
      _player.Pause();
  }

  public void Stop() {
    try {
      _player.Stop();
    }
    catch { }
  }

  protected override void Dispose(bool disposing) {
    if (disposing) {
      try {
        _player.Release();
      }
      catch { }
    }

    base.Dispose(disposing);
  }
}