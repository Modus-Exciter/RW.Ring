using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Notung.Feuerzauber.Converters
{
  [ValueConversion(typeof(Image), typeof(BitmapSource))]
  public class ImageToBitmapSourceConverter : IValueConverter
  {
    private readonly ConditionalWeakTable<Image, BitmapSource> m_cache = new ConditionalWeakTable<Image, BitmapSource>();
    private static readonly ConditionalWeakTable<Image, BitmapSource>.CreateValueCallback _converter = PerformConvert;

    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DeleteObject(IntPtr value);

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      Image myImage = value as Image;

      //ensure provided value is valid image.
      if (myImage == null)
        return null;

      //GetHbitmap will fail if either dimension is larger than max short value.
      //Throwing here to reduce cpu and resource usage when error can be detected early.
      if (myImage.Height > Int16.MaxValue || myImage.Width > Int16.MaxValue)
        throw new ArgumentOutOfRangeException("Size",
          string.Format("Image size must not be greater than {0}", Int16.MaxValue));

      return m_cache.GetValue(myImage, _converter);
    }

    private static BitmapSource PerformConvert(Image myImage)
    {
      using (Bitmap bitmap = new Bitmap(myImage))
      {
        //ensure Bitmap is disposed of after usefulness is fulfilled.
        IntPtr bmpPt = bitmap.GetHbitmap();
        try
        {
          BitmapSource bitmapSource =
           System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                 bmpPt,
                 IntPtr.Zero,
                 Int32Rect.Empty,
                 BitmapSizeOptions.FromEmptyOptions());

          //freeze bitmapSource and clear memory to avoid memory leaks
          bitmapSource.Freeze();

          return bitmapSource;
        }
        finally
        {
          //done in a finally block to ensure this memory is not leaked regardless of exceptions.
          DeleteObject(bmpPt);
        }
      }
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
