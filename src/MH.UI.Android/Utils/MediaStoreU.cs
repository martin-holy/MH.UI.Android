using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Provider;
using Android.Webkit;
using AndroidX.Core.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Path = System.IO.Path;

namespace MH.UI.Android.Utils;

public static class MediaStoreU {
  public static Task<Bitmap?> GetImageThumbnail(string filePath, Context context, int targetSize = 512) =>
    _getThumbnail(filePath, context, targetSize, _getImageId, MediaStore.Images.Media.ExternalContentUri,
      _getLegacyImageThumb, _createLegacyImageValues);

  public static Task<Bitmap?> GetVideoThumbnail(string filePath, Context context, int targetSize = 512) =>
    _getThumbnail(filePath, context, targetSize, _getVideoId, MediaStore.Video.Media.ExternalContentUri,
      _getLegacyVideoThumb, _createLegacyVideoValues);

  private static Bitmap? _getLegacyImageThumb(ContentResolver resolver, long id) =>
    MediaStore.Images.Thumbnails.GetThumbnail(resolver, id, ThumbnailKind.MiniKind, new() { InSampleSize = 1 });

  private static Bitmap? _getLegacyVideoThumb(ContentResolver resolver, long id) =>
    MediaStore.Video.Thumbnails.GetThumbnail(resolver, id, VideoThumbnailKind.MiniKind, new() { InSampleSize = 1 });

  private static async Task<Bitmap?> _getThumbnail(string filePath, Context context, int targetSize,
    Func<string, Context, long> getMediaId, global::Android.Net.Uri? collectionUri,
    Func<ContentResolver, long, Bitmap?> legacyGetThumb,
    Func<string, ContentValues> createLegacyMediaValues) {

    if (collectionUri == null) return null;

    try {
      var mediaId = getMediaId(filePath, context);

      // API 29+ path
      if (Build.VERSION.SdkInt >= BuildVersionCodes.Q) {
        var size = new global::Android.Util.Size(targetSize, targetSize);
        var uri = mediaId == -1
          ? global::Android.Net.Uri.FromFile(new Java.IO.File(filePath))
          : ContentUris.WithAppendedId(collectionUri, mediaId);

        try {
          if (context.ContentResolver?.LoadThumbnail(uri, size, null) is { } bmp)
            return bmp;
        }
        catch (Exception ex) {
          MH.Utils.Log.Error(ex, $"LoadThumbnail uri failed: {ex.Message}");
        }

        try {
          if (await ScanFileAsync(context, filePath) is { } scanUri
            && context.ContentResolver?.LoadThumbnail(scanUri, size, null) is { } bmp)
            return bmp;
        }
        catch (Exception ex) {
          MH.Utils.Log.Error(ex, $"ScanFile failed: {ex.Message}");
        }
      }
      else {
        // API < 29 - legacy flow
        if (mediaId == -1) {
          var values = createLegacyMediaValues(filePath);
          try {
            context.ContentResolver?.Insert(collectionUri, values);
          }
          catch (Exception ex) {
            MH.Utils.Log.Error(ex, $"Insert into MediaStore (legacy) failed: {ex.Message}");
          }

          mediaId = getMediaId(filePath, context);
        }

        if (mediaId != -1) {
          try {
            return legacyGetThumb(context.ContentResolver!, mediaId);
          }
          catch (Exception ex) {
            MH.Utils.Log.Error(ex, $"GetThumbnail (legacy) failed: {ex.Message}");
          }
        }
      }
    }
    catch (Exception ex) {
      MH.Utils.Log.Error(ex, $"GetThumbnailBitmap error: {ex}");
    }

    return null;
  }

  public static Task<global::Android.Net.Uri?> ScanFileAsync(Context context, string filePath) {
    var tcs = new TaskCompletionSource<global::Android.Net.Uri?>();

    MediaScannerConnection.ScanFile(
      context,
      [filePath],
      null,
      new ScanFileCompletedListener(uri => tcs.TrySetResult(uri))
    );

    return tcs.Task;
  }

  private static ContentValues _createLegacyImageValues(string filePath) {
    var values = new ContentValues();
    values.Put(MediaStore.Images.Media.InterfaceConsts.Data, filePath);
    values.Put(MediaStore.Images.Media.InterfaceConsts.MimeType, _getMimeType(filePath) ?? "image/*");
    var dt = _getDateTakenMillisSafe(filePath);
    if (dt.HasValue) values.Put(MediaStore.Images.Media.InterfaceConsts.DateTaken, dt.Value);
    var fi = new FileInfo(filePath);
    values.Put(MediaStore.Images.Media.InterfaceConsts.DateModified, (long)(fi.LastWriteTimeUtc.Subtract(new DateTime(1970, 1, 1)).TotalSeconds));
    values.Put(MediaStore.Images.Media.InterfaceConsts.Size, fi.Exists ? fi.Length : 0);
    values.Put(MediaStore.Images.Media.InterfaceConsts.DisplayName, Path.GetFileName(filePath));
    return values;
  }

  private static ContentValues _createLegacyVideoValues(string filePath) {
    var values = new ContentValues();
    values.Put(MediaStore.Video.Media.InterfaceConsts.Data, filePath);
    values.Put(MediaStore.Video.Media.InterfaceConsts.MimeType, _getMimeType(filePath) ?? "video/*");
    var dt = _getDateTakenMillisSafe(filePath);
    if (dt.HasValue) values.Put(MediaStore.Video.Media.InterfaceConsts.DateTaken, dt.Value);
    var fi = new FileInfo(filePath);
    values.Put(MediaStore.Video.Media.InterfaceConsts.DateModified, (long)(fi.LastWriteTimeUtc.Subtract(new DateTime(1970, 1, 1)).TotalSeconds));
    values.Put(MediaStore.Video.Media.InterfaceConsts.Size, fi.Exists ? fi.Length : 0);
    values.Put(MediaStore.Video.Media.InterfaceConsts.DisplayName, Path.GetFileName(filePath));
    return values;
  }

  private static long _getImageId(string filePath, Context context) =>
  _getMediaId(
    filePath,
    context,
    MediaStore.Images.Media.ExternalContentUri,
    MediaStore.Images.Media.InterfaceConsts.Id,
    MediaStore.Images.Media.InterfaceConsts.Data,
    MediaStore.Images.Media.InterfaceConsts.DisplayName,
    Build.VERSION.SdkInt >= BuildVersionCodes.Q ? MediaStore.Images.Media.InterfaceConsts.RelativePath : null);

  private static long _getVideoId(string filePath, Context context) =>
    _getMediaId(
      filePath,
      context,
      MediaStore.Video.Media.ExternalContentUri,
      MediaStore.Video.Media.InterfaceConsts.Id,
      MediaStore.Video.Media.InterfaceConsts.Data,
      MediaStore.Video.Media.InterfaceConsts.DisplayName,
      Build.VERSION.SdkInt >= BuildVersionCodes.Q ? MediaStore.Video.Media.InterfaceConsts.RelativePath : null);

  private static long _getMediaId(string filePath, Context context, global::Android.Net.Uri? collectionUri,
    string idColumn, string dataColumn, string displayNameColumn, string? relativePathColumn) {

    if (context.ContentResolver is not { } resolver || collectionUri is null) return -1;

    try {
      // 1) try DATA (works pre-29 and often on 29 if provider exposes it)
      using var dataCursor = resolver.Query(collectionUri, [idColumn], $"{dataColumn}=?", [filePath], null);

      if (dataCursor != null && dataCursor.MoveToFirst())
        return dataCursor.GetLong(0);

      // 2) Try by DISPLAY_NAME + RELATIVE_PATH on API29+ (only possible if file is in shared storage)
      if (Build.VERSION.SdkInt >= BuildVersionCodes.Q) {
        if (_computeRelativePathUnderExternalStorage(filePath) is { } relativePath) {
          using var pathCursor = resolver.Query(
            collectionUri,
            [idColumn],
            $"{displayNameColumn}=? AND {relativePathColumn}=?",
            [Path.GetFileName(filePath), relativePath],
            null);
          if (pathCursor != null && pathCursor.MoveToFirst())
            return pathCursor.GetLong(0);
        }
      }
    }
    catch (Exception ex) {
      MH.Utils.Log.Error(ex, $"GetMediaId query failed: {ex.Message}");
    }

    return -1;
  }

  private static string? _computeRelativePathUnderExternalStorage(string filePath) {
    try {
      var extRoot = global::Android.OS.Environment.ExternalStorageDirectory!.AbsolutePath.TrimEnd(Path.DirectorySeparatorChar);
      var normalized = Path.GetFullPath(filePath).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
      if (!normalized.StartsWith(extRoot, StringComparison.OrdinalIgnoreCase)) return null;

      var relative = normalized[extRoot.Length..].TrimStart(Path.DirectorySeparatorChar);
      if (string.IsNullOrEmpty(relative)) return null;

      var dir = Path.GetDirectoryName(relative)?.Replace(Path.DirectorySeparatorChar, '/');
      if (string.IsNullOrEmpty(dir)) return null;
      if (!dir.EndsWith('/')) dir += "/";
      return dir;
    }
    catch {
      return null;
    }
  }

  private static string? _getMimeType(string filePath) =>
    MimeTypeMap.Singleton?.GetMimeTypeFromExtension(Path.GetExtension(filePath)?.TrimStart('.').ToLowerInvariant());

  private static long? _getDateTakenMillisSafe(string filePath) {
    try {
      // prefer EXIF DateTimeOriginal if image file supports it
      try {
        var exif = new ExifInterface(filePath);
        var dtStr = exif.GetAttribute(ExifInterface.TagDatetimeOriginal) ??
                    exif.GetAttribute(ExifInterface.TagDatetime);
        if (!string.IsNullOrEmpty(dtStr)) {
          // EXIF format: "yyyy:MM:dd HH:mm:ss"
          if (DateTime.TryParseExact(dtStr, ["yyyy:MM:dd HH:mm:ss", "yyyy:MM:dd"], null, System.Globalization.DateTimeStyles.AssumeLocal, out var dt))
            return (long)(dt.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }
      }
      catch (Exception ex) {
        // ignore exif errors and fallback to file times
        MH.Utils.Log.Error(ex, $"Exif read failed: {ex.Message}");
      }

      var fi = new FileInfo(filePath);
      if (fi.Exists)
        return (long)(fi.CreationTimeUtc - new DateTime(1970, 1, 1)).TotalMilliseconds;
    }
    catch (Exception ex) {
      MH.Utils.Log.Error(ex, $"GetDateTakenMillisSafe error: {ex}");
    }

    return null;
  }

#pragma warning disable CS0618 // DATA is deprecated but required for path-based deletion
  private const string _dataColumn = MediaStore.Files.FileColumns.Data;
#pragma warning restore CS0618

  public static void DeleteFiles(IList<string> filePaths, Context context) {
    if (context.ContentResolver is not { } resolver ||
      MediaStore.Files.GetContentUri("external") is not { } uri) return;

    for (int i = 0; i < filePaths.Count; i++) {
      var path = filePaths[i];

      if (string.IsNullOrEmpty(path)) continue;

      try {
        if (!File.Exists(path)) continue;

        int rows = 0;

        try {
          rows = resolver.Delete(uri, _dataColumn + "=?", new[] { path });
        }
        catch (Exception ex) {
          MH.Utils.Log.Error(ex, $"MediaStore delete failed: {path}");
        }

        if (rows > 0) continue;

        try {
          File.Delete(path);
        }
        catch (Exception ex) {
          MH.Utils.Log.Error(ex, $"File delete failed: {path}");
        }
      }
      catch (Exception ex) {
        MH.Utils.Log.Error(ex, $"Delete failed: {path}");
      }
    }

    try {
      MediaScannerConnection.ScanFile(context, filePaths.ToArray(), null, null);
    }
    catch (Exception ex) {
      MH.Utils.Log.Error(ex, "Scan failed");
    }
  }

  private static HashSet<string> _mimeTypes = ["audio", "image", "text", "video"];

  public static void ShareFiles(Context context, IEnumerable<string> filePaths) {
    try {
      var uris = new List<global::Android.Net.Uri>();
      var mimeTypes = new HashSet<string>();

      foreach (var path in filePaths) {
        var file = new Java.IO.File(path);
        if (!file.Exists()) continue;

        var mime = _getMimeType(path);
        if (!string.IsNullOrEmpty(mime))
          mimeTypes.Add(mime.Split('/')[0]);
        else
          mimeTypes.Add("unknown");

        if (FileProvider.GetUriForFile(context, context.PackageName + ".fileprovider", file) is { } uri)
          uris.Add(uri);
      }

      if (uris.Count == 0) return;

      var intentType = "*/*";
      if (mimeTypes.Count == 1) {
        var mainType = mimeTypes.First();

        if (_mimeTypes.Contains(mainType))
          intentType = $"{mainType}/*";
      }

      Intent intent;
      if (uris.Count == 1) {
        intent = new Intent(Intent.ActionSend);
        intent.PutExtra(Intent.ExtraStream, uris[0]);
      }
      else {
        intent = new Intent(Intent.ActionSendMultiple);
        intent.PutParcelableArrayListExtra(Intent.ExtraStream, [.. uris.Cast<IParcelable>()]);
      }
      intent.SetType(intentType);
      intent.AddFlags(ActivityFlags.GrantReadUriPermission);

      if (Intent.CreateChooser(intent, "Share via") is { } chooser) {
        chooser.AddFlags(ActivityFlags.NewTask);
        context.StartActivity(chooser);
      }
    }
    catch (Exception ex) {
      MH.Utils.Log.Error(ex, "Failed to share files");
    }
  }
}

public class ScanFileCompletedListener(Action<global::Android.Net.Uri?> onCompleted) : Java.Lang.Object, MediaScannerConnection.IOnScanCompletedListener {
  public void OnScanCompleted(string? path, global::Android.Net.Uri? uri) =>
    onCompleted?.Invoke(uri);
}