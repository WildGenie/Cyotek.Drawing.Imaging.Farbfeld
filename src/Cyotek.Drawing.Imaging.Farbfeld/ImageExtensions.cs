﻿using System.Drawing;
using System.Drawing.Imaging;

namespace Cyotek.Drawing.Imaging
{
  internal static class ImageExtensions
  {
    #region Static Methods

    public static Bitmap Copy(this Image image)
    {
      return Copy(image, Color.Transparent);
    }

    public static Bitmap Copy(this Image image, Color transparentColor)
    {
      Bitmap copy;

      copy = new Bitmap(image.Size.Width, image.Size.Height, PixelFormat.Format32bppArgb);

      using (Graphics g = Graphics.FromImage(copy))
      {
        g.Clear(transparentColor);
        g.PageUnit = GraphicsUnit.Pixel;
        g.DrawImage(image, new Rectangle(Point.Empty, image.Size));
      }

      return copy;
    }

    public static ArgbColor[] GetPixels(this Bitmap bitmap)
    {
      ArgbColor[] results;

      if (bitmap.PixelFormat == PixelFormat.Format32bppArgb)
      {
        results = bitmap.Get32BppArgbPixels();
      }
      else
      {
        // HACK: Need dedicated bpp methods

        using (Bitmap copy = bitmap.Copy())
        {
          results = copy.Get32BppArgbPixels();
        }
      }

      return results;
    }

    private static ArgbColor[] Get32BppArgbPixels(this Bitmap bitmap)
    {
      int width;
      int height;
      BitmapData bitmapData;
      ArgbColor[] results;

      width = bitmap.Width;
      height = bitmap.Height;
      results = new ArgbColor[width * height];
      bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
                                   PixelFormat.Format32bppArgb);

      unsafe
      {
        ArgbColor* pixel;

        pixel = (ArgbColor*)bitmapData.Scan0;

        for (int row = 0; row < height; row++)
        {
          for (int col = 0; col < width; col++)
          {
            results[row * width + col] = *pixel;

            pixel++;
          }
        }
      }

      bitmap.UnlockBits(bitmapData);

      return results;
    }

    #endregion
  }
}
