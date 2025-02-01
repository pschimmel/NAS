using System;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NAS.Views.Helpers;

public static class Images
{
  public static BitmapSource LoadImage(byte[] imageData)
  {
    if (imageData == null)
    {
      return null;
    }

    using var ms = new MemoryStream(imageData);
    var decoder = BitmapDecoder.Create(ms, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
    return decoder.Frames[0];
  }

  public static BitmapSource LoadImage(string fileName)
  {
    var fi = new FileInfo(fileName);
    BitmapSource image = null;
    using (var fs = fi.OpenRead())
    {
      var decoder = BitmapDecoder.Create(fs, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
      image = decoder.Frames[0];
    }
    return image;
  }

  public enum BitmapSourceFormat { Jpeg, Bmp, Gif, Png, Tiff }

  public static byte[] SaveImage(BitmapSource bitmap, BitmapSourceFormat format)
  {
    if (bitmap == null)
    {
      return null;
    }

    using var ms = new MemoryStream();
    BitmapEncoder encoder = null;
    switch (format)
    {
      case BitmapSourceFormat.Bmp:
        encoder = new BmpBitmapEncoder();
        break;
      case BitmapSourceFormat.Gif:
        encoder = new GifBitmapEncoder();
        break;
      case BitmapSourceFormat.Jpeg:
        encoder = new JpegBitmapEncoder();
        break;
      case BitmapSourceFormat.Png:
        encoder = new PngBitmapEncoder();
        break;
      case BitmapSourceFormat.Tiff:
        encoder = new TiffBitmapEncoder();
        break;
    }
    if (encoder == null)
    {
      throw new InvalidOperationException();
    }

    var wb = new WriteableBitmap(bitmap);
    var frame = BitmapFrame.Create(wb);
    //frame.Freeze();
    encoder.Frames.Add(frame);
    encoder.Save(ms);
    return ms.GetBuffer();
  }

  public static BitmapSource ConvertImageToBitmapSource(System.Drawing.Image image, ImageFormat format)
  {
    using var stream = new MemoryStream();
    image.Save(stream, format);
    stream.Flush();
    stream.Position = 0;
    var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
    BitmapSource source = decoder.Frames[0];
    return source;
  }

  public static System.Drawing.Image ConvertBitmapSourceToImage(BitmapSource bitmap)
  {
    using var stream = new MemoryStream();
    var encoder = new BmpBitmapEncoder();
    encoder.Frames.Add(BitmapFrame.Create(bitmap));
    encoder.Save(stream);
    stream.Flush();
    stream.Position = 0;
    var img = System.Drawing.Image.FromStream(stream);
    return img;
  }

  public static BitmapSource RotateBitmap(BitmapSource bitmap, double degrees)
  {
    var transformBitmap = new TransformedBitmap(bitmap, new RotateTransform(degrees));
    return transformBitmap.Clone();
  }

  public static BitmapSource GetScreenshot(this UIElement source, double scale)
  {
    double actualHeight = source.RenderSize.Height;
    double actualWidth = source.RenderSize.Width;
    double renderHeight = actualHeight * scale;
    double renderWidth = actualWidth * scale;
    var renderTarget = new RenderTargetBitmap((int)renderWidth, (int)renderHeight, 96, 96, PixelFormats.Pbgra32);
    var sourceBrush = new VisualBrush(source);
    var drawingVisual = new DrawingVisual();
    var drawingContext = drawingVisual.RenderOpen();
    using (drawingContext)
    {
      drawingContext.PushTransform(new ScaleTransform(scale, scale));
      drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0), new Point(actualWidth, actualHeight)));
    }
    renderTarget.Render(drawingVisual);
    return BitmapFrame.Create(renderTarget);
  }

  public static void SaveCanvas(this Canvas canvas, string fileName)
  {
    SaveCanvas(canvas, fileName, 96, 96);
  }

  public static void SaveCanvas(this Canvas canvas, string fileName, double dpiX, double dpiY)
  {
    if (fileName == null)
    {
      return;
    }

    using var outStream = new FileStream(fileName, FileMode.Create);
    var format = BitmapSourceFormat.Jpeg;
    string ext = Path.GetExtension(fileName).ToLower();

    format = ext switch
    {
      ".png" => BitmapSourceFormat.Png,
      ".jpg" or ".jpeg" => BitmapSourceFormat.Jpeg,
      ".gif" => BitmapSourceFormat.Gif,
      ".tif" or ".tiff" => BitmapSourceFormat.Tiff,
      ".bmp" => BitmapSourceFormat.Bmp,
      _ => throw new NotImplementedException("No matching encoder found."),
    };

    byte[] buffer = SaveCanvas(canvas, format, dpiX, dpiY);
    File.WriteAllBytes(fileName, buffer);
  }

  public static byte[] SaveCanvas(this Canvas canvas, BitmapSourceFormat format)
  {
    return SaveCanvas(canvas, format, 96, 96);
  }

  public static byte[] SaveCanvas(this Canvas canvas, BitmapSourceFormat format, double dpiX, double dpiY)
  {
    var transform = canvas.LayoutTransform;
    canvas.LayoutTransform = null;
    if (double.IsNaN(canvas.Width) || double.IsNaN(canvas.Height))
    {
      return null;
    }

    var size = new Size(canvas.Width, canvas.Height);
    canvas.Measure(size);
    canvas.Arrange(new Rect(size));
    var renderBitmap = new RenderTargetBitmap((int)size.Width, (int)size.Height, dpiX, dpiY, PixelFormats.Pbgra32);
    renderBitmap.Render(canvas);
    byte[] result = null;
    using (var outStream = new MemoryStream())
    {
      var encoder = format switch
      {
        BitmapSourceFormat.Png => (BitmapEncoder)new PngBitmapEncoder(),
        BitmapSourceFormat.Jpeg => new JpegBitmapEncoder(),
        BitmapSourceFormat.Tiff => new TiffBitmapEncoder(),
        BitmapSourceFormat.Gif => new GifBitmapEncoder(),
        BitmapSourceFormat.Bmp => new BmpBitmapEncoder(),
        _ => throw new NotImplementedException(),
      };

      encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
      encoder.Save(outStream);
      result = outStream.ToArray();
    }
    canvas.LayoutTransform = transform;
    return result;
  }

  public static ImageSource GetIconFromFile(string fileName)
  {
    ImageSource icon = null;
    if (File.Exists(fileName))
    {
      using var sysicon = System.Drawing.Icon.ExtractAssociatedIcon(fileName);
      icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(sysicon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
    }
    return icon;
  }
}
