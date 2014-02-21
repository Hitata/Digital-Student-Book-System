using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ImageProcess.ImageProcessing
{
        
        using System;
        using System.Collections.Generic;
        using System.Drawing;
        using System.Drawing.Imaging;
        using System.IO;
        using System.Runtime.InteropServices;
        using AForge;
        using AForge.Imaging;
        public class ImageGroup
        {
            public static Bitmap Clone(Bitmap source)
            {
                BitmapData sourceData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, source.PixelFormat);
                Bitmap bitmap = Clone(sourceData);
                source.UnlockBits(sourceData);
                if (((source.PixelFormat == PixelFormat.Format1bppIndexed) || (source.PixelFormat == PixelFormat.Format4bppIndexed)) || ((source.PixelFormat == PixelFormat.Format8bppIndexed) || (source.PixelFormat == PixelFormat.Indexed)))
                {
                    ColorPalette palette = source.Palette;
                    ColorPalette palette2 = bitmap.Palette;
                    int length = palette.Entries.Length;
                    for (int i = 0; i < length; i++)
                    {
                        palette2.Entries[i] = palette.Entries[i];
                    }
                    bitmap.Palette = palette2;
                }
                return bitmap;
            }

            public static Bitmap Clone(BitmapData sourceData)
            {
                int width = sourceData.Width;
                int height = sourceData.Height;
                Bitmap bitmap = new Bitmap(width, height, sourceData.PixelFormat);
                BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
                SystemTools.CopyUnmanagedMemory(bitmapdata.Scan0, sourceData.Scan0, height * sourceData.Stride);
                bitmap.UnlockBits(bitmapdata);
                return bitmap;
            }

            public static Bitmap Clone(Bitmap source, PixelFormat format)
            {
                if (source.PixelFormat == format)
                {
                    return Clone(source);
                }
                int width = source.Width;
                int height = source.Height;
                Bitmap image = new Bitmap(width, height, format);
                Graphics graphics = Graphics.FromImage(image);
                graphics.DrawImage(source, 0, 0, width, height);
                graphics.Dispose();
                return image;
            }

            //public static unsafe Bitmap Convert16bppTo8bpp(Bitmap bimap)
            //{
            //    Bitmap bitmap = null;
            //    int num = 0;
            //    int width = bimap.Width;
            //    int height = bimap.Height;
            //    switch (bimap.PixelFormat)
            //    {
            //        case PixelFormat.Format64bppPArgb:
            //            bitmap = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
            //            num = 4;
            //            break;

            //        case PixelFormat.Format64bppArgb:
            //            bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            //            num = 4;
            //            break;

            //        case PixelFormat.Format16bppGrayScale:
            //            bitmap = CreateGrayscaleImage(width, height);
            //            num = 1;
            //            break;

            //        case PixelFormat.Format48bppRgb:
            //            bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            //            num = 3;
            //            break;

            //        default:
            //            throw new UnsupportedImageFormatException("Invalid pixel format of the source image.");
            //    }
            //    BitmapData bitmapdata = bimap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, bimap.PixelFormat);
            //    BitmapData data2 = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            //    int num4 = (int)bitmapdata.Scan0.ToPointer();
            //    int num5 = (int)data2.Scan0.ToPointer();
            //    int stride = bitmapdata.Stride;
            //    int num7 = data2.Stride;
            //    for (int i = 0; i < height; i++)
            //    {
            //        ushort* numPtr = (ushort*)(num4 + (i * stride));
            //        byte* numPtr2 = (byte*)(num5 + (i * num7));
            //        int num9 = 0;
            //        int num10 = width * num;
            //        while (num9 < num10)
            //        {
            //            numPtr2[0] = (byte)(numPtr[0] >> 8);
            //            num9++;
            //            numPtr++;
            //            numPtr2++;
            //        }
            //    }
            //    bimap.UnlockBits(bitmapdata);
            //    bitmap.UnlockBits(data2);
            //    return bitmap;
            //}

            //public static unsafe Bitmap Convert8bppTo16bpp(Bitmap bimap)
            //{
            //    Bitmap bitmap = null;
            //    int num = 0;
            //    int width = bimap.Width;
            //    int height = bimap.Height;
            //    switch (bimap.PixelFormat)
            //    {
            //        case PixelFormat.Format32bppPArgb:
            //            bitmap = new Bitmap(width, height, PixelFormat.Format64bppPArgb);
            //            num = 4;
            //            break;

            //        case PixelFormat.Format32bppArgb:
            //            bitmap = new Bitmap(width, height, PixelFormat.Format64bppArgb);
            //            num = 4;
            //            break;

            //        case PixelFormat.Format24bppRgb:
            //            bitmap = new Bitmap(width, height, PixelFormat.Format48bppRgb);
            //            num = 3;
            //            break;

            //        case PixelFormat.Format8bppIndexed:
            //            bitmap = new Bitmap(width, height, PixelFormat.Format16bppGrayScale);
            //            num = 1;
            //            break;

            //        default:
            //            throw new UnsupportedImageFormatException("Invalid pixel format of the source image.");
            //    }
            //    BitmapData bitmapdata = bimap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, bimap.PixelFormat);
            //    BitmapData data2 = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            //    int num4 = (int)bitmapdata.Scan0.ToPointer();
            //    int num5 = (int)data2.Scan0.ToPointer();
            //    int stride = bitmapdata.Stride;
            //    int num7 = data2.Stride;
            //    for (int i = 0; i < height; i++)
            //    {
            //        byte* numPtr = (byte*)(num4 + (i * stride));
            //        ushort* numPtr2 = (ushort*)(num5 + (i * num7));
            //        int num9 = 0;
            //        int num10 = width * num;
            //        while (num9 < num10)
            //        {
            //            numPtr2[0] = (ushort)(numPtr[0] << 8);
            //            num9++;
            //            numPtr++;
            //            numPtr2++;
            //        }
            //    }
            //    bimap.UnlockBits(bitmapdata);
            //    bitmap.UnlockBits(data2);
            //    return bitmap;
            //}

            public static Bitmap CreateColorPad(Bitmap original, Color color, int padWidth, int padHeight)
            {
                Bitmap image = CreateNoneIndexedImage(original);
                int width = image.Width + (2 * padWidth);
                int height = image.Height + (2 * padHeight);
                Bitmap bitmap2 = new Bitmap(width, height, image.PixelFormat);
                Graphics graphics = Graphics.FromImage(bitmap2);
                Brush brush = new SolidBrush(color);
                graphics.FillRectangle(brush, new Rectangle(0, 0, width, height));
                graphics.DrawImage(image, padWidth, padHeight);
                graphics.Dispose();
                return bitmap2;
            }

            public static Bitmap CreateGrayscaleImage(int width, int height)
            {
                Bitmap image = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
                SetGrayscalePalette(image);
                return image;
            }

            public static unsafe Bitmap CreateIndexedGrayScaleBitmap(Bitmap original)
            {
                Bitmap image = (Bitmap)original.Clone();
                if (IsGrayscale(original))
                {
                    return original;
                }
                FormatImage(ref image);
                try
                {
                    Bitmap bitmap2 = MakeGrayscale(image);
                    Bitmap bitmap3 = CreateGrayscaleImage(image.Width, image.Height);
                    BitmapData bitmapdata = bitmap2.LockBits(new Rectangle(0, 0, bitmap2.Width, bitmap2.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                    BitmapData data2 = bitmap3.LockBits(new Rectangle(0, 0, bitmap3.Width, bitmap3.Height), ImageLockMode.ReadWrite, bitmap3.PixelFormat);
                    int num = 3;
                    for (int i = 0; i < bitmap2.Height; i++)
                    {
                        byte* numPtr = (byte*)((bitmapdata.Scan0) + (i * bitmapdata.Stride));
                        byte* numPtr2 = (byte*)((data2.Scan0) + (i * data2.Stride));
                        for (int j = 0; j < original.Width; j++)
                        {
                            numPtr2[j] = Convert.ToByte((int)(((numPtr[j * num] + numPtr[(j * num) + 1]) + numPtr[(j * num) + 2]) / 3));
                        }
                    }
                    bitmap2.UnlockBits(bitmapdata);
                    bitmap3.UnlockBits(data2);
                    return bitmap3;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public static Bitmap CreateNoneIndexedImage(Bitmap original)
            {
                Bitmap image = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);
                using (Graphics graphics = Graphics.FromImage(image))
                {
                    graphics.DrawImage(original, 0, 0);
                }
                return image;
            }

            public static Bitmap FillColor(Bitmap original, Rectangle rect, int colorIndex)
            {
                Bitmap bitmap;
                if (IsGrayscale(original))
                {
                    bitmap = original;
                }
                else
                {
                    bitmap = CreateIndexedGrayScaleBitmap(CreateNoneIndexedImage(original));
                }
                BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                IntPtr source = bitmapdata.Scan0;
                int length = Math.Abs(bitmapdata.Stride) * bitmap.Height;
                byte[] destination = new byte[length];
                Marshal.Copy(source, destination, 0, length);
                if ((rect.X + rect.Width) >= bitmap.Width)
                {
                    rect.Width = (bitmap.Width - rect.X) - 1;
                }
                if ((rect.Y + rect.Height) >= bitmap.Height)
                {
                    rect.Height = (bitmap.Height - rect.Y) - 1;
                }
                for (int i = rect.X; i <= (rect.Width + rect.X); i++)
                {
                    for (int j = rect.Y; j <= (rect.Height + rect.Y); j++)
                    {
                        if (destination[(j * bitmapdata.Stride) + i] != colorIndex)
                        {
                            destination[(j * bitmapdata.Stride) + i] = Convert.ToByte(colorIndex);
                        }
                    }
                }
                Marshal.Copy(destination, 0, source, length);
                bitmap.UnlockBits(bitmapdata);
                return bitmap;
            }

            public static void FormatImage(ref Bitmap image)
            {
                if ((((image.PixelFormat != PixelFormat.Format24bppRgb) && (image.PixelFormat != PixelFormat.Format32bppRgb)) && ((image.PixelFormat != PixelFormat.Format32bppArgb) && (image.PixelFormat != PixelFormat.Format48bppRgb))) && (((image.PixelFormat != PixelFormat.Format64bppArgb) && (image.PixelFormat != PixelFormat.Format16bppGrayScale)) && !IsGrayscale(image)))
                {
                    Bitmap source = image;
                    image = Clone(source, PixelFormat.Format24bppRgb);
                    source.Dispose();
                }
            }

            public static Bitmap FromFile(string fileName)
            {
                Bitmap bitmap = null;
                FileStream stream = null;
                try
                {
                    stream = File.OpenRead(fileName);
                    MemoryStream stream2 = new MemoryStream();
                    byte[] buffer = new byte[0x2710];
                    while (true)
                    {
                        int count = stream.Read(buffer, 0, 0x2710);
                        if (count == 0)
                        {
                            break;
                        }
                        stream2.Write(buffer, 0, count);
                    }
                    bitmap = (Bitmap)System.Drawing.Image.FromStream(stream2);
                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Close();
                        stream.Dispose();
                    }
                }
                return bitmap;
            }

            public static Color Get8bppIndexedPixel(Bitmap bmp, int x, int y)
            {
                BitmapData bitmapdata = bmp.LockBits(new Rectangle(x, y, 1, 1), ImageLockMode.ReadOnly, bmp.PixelFormat);
                Color color = bmp.Palette.Entries[Marshal.ReadByte(bitmapdata.Scan0)];
                bmp.UnlockBits(bitmapdata);
                return color;
            }

            public static Rectangle GetPatternRectangeBoundary(Bitmap original, int colorIndex, int hStep, int vStep, bool bTopStart)
            {
                if (hStep <= 0)
                {
                    hStep = 1;
                }
                if (vStep <= 0)
                {
                    vStep = 1;
                }
                Rectangle rectangle = new Rectangle(0, 0, 0, 0);
                Bitmap bitmap = CreateIndexedGrayScaleBitmap(original);
                BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                IntPtr source = bitmapdata.Scan0;
                int length = Math.Abs(bitmapdata.Stride) * bitmap.Height;
                byte[] destination = new byte[length];
                Marshal.Copy(source, destination, 0, length);
                bool flag = false;
                if (!bTopStart)
                {
                    for (int i = 0; i < bitmap.Width; i++)
                    {
                        for (int j = 0; j < bitmap.Height; j++)
                        {
                            if (destination[(j * bitmapdata.Stride) + i] != colorIndex)
                            {
                                rectangle.X = i;
                                rectangle.Y = j;
                                rectangle.Width = 1;
                                rectangle.Height = 1;
                                flag = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    for (int k = 0; k < bitmap.Height; k++)
                    {
                        for (int m = 0; m < bitmap.Width; m++)
                        {
                            if (destination[(k * bitmapdata.Stride) + m] != colorIndex)
                            {
                                rectangle.X = m;
                                rectangle.Y = k;
                                rectangle.Width = 1;
                                rectangle.Height = 1;
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (flag)
                    {
                        bool flag2 = false;
                        while (!flag2)
                        {
                            flag2 = true;
                            for (int n = 1; n <= vStep; n++)
                            {
                                for (int x = rectangle.X; x < (rectangle.X + rectangle.Width); x++)
                                {
                                    int num6 = (rectangle.Y + rectangle.Height) + n;
                                    if ((num6 < bitmap.Height) && (destination[(num6 * bitmapdata.Stride) + x] != colorIndex))
                                    {
                                        rectangle.Height += n;
                                        flag2 = false;
                                        x = rectangle.X;
                                        n = 1;
                                    }
                                }
                            }
                            for (int num7 = 1; num7 <= hStep; num7++)
                            {
                                for (int y = rectangle.Y; y < (rectangle.Y + rectangle.Height); y++)
                                {
                                    int num9 = rectangle.X - num7;
                                    if ((num9 >= 0) && (destination[(y * bitmapdata.Stride) + num9] != colorIndex))
                                    {
                                        rectangle.X -= num7;
                                        rectangle.Width += num7;
                                        flag2 = false;
                                        y = rectangle.Y;
                                        num7 = 1;
                                    }
                                }
                            }
                            for (int num10 = 1; num10 <= hStep; num10++)
                            {
                                for (int num11 = rectangle.Y; num11 < (rectangle.Y + rectangle.Height); num11++)
                                {
                                    int num12 = (rectangle.X + rectangle.Width) + num10;
                                    if ((num12 < bitmap.Width) && (destination[(num11 * bitmapdata.Stride) + num12] != colorIndex))
                                    {
                                        rectangle.Width += num10;
                                        flag2 = false;
                                        num11 = rectangle.Y;
                                        num10 = 1;
                                    }
                                }
                            }
                        }
                    }
                    goto Label_0495;
                }
                if (flag)
                {
                    bool flag3 = false;
                    while (!flag3)
                    {
                        flag3 = true;
                        for (int num15 = 1; num15 <= hStep; num15++)
                        {
                            for (int num16 = rectangle.Y; num16 < (rectangle.Y + rectangle.Height); num16++)
                            {
                                int num17 = (rectangle.X + rectangle.Width) + num15;
                                if ((num17 < bitmap.Width) && (destination[(num16 * bitmapdata.Stride) + num17] != colorIndex))
                                {
                                    rectangle.Width += num15;
                                    flag3 = false;
                                    num16 = rectangle.Y;
                                    num15 = 1;
                                }
                            }
                        }
                        for (int num18 = 1; num18 <= vStep; num18++)
                        {
                            for (int num19 = rectangle.X; num19 < (rectangle.X + rectangle.Width); num19++)
                            {
                                int num20 = (rectangle.Y + rectangle.Height) + num18;
                                if ((num20 < bitmap.Height) && (destination[(num20 * bitmapdata.Stride) + num19] != colorIndex))
                                {
                                    rectangle.Height += num18;
                                    flag3 = false;
                                    num19 = rectangle.X;
                                    num18 = 1;
                                }
                            }
                        }
                        for (int num21 = 1; num21 <= vStep; num21++)
                        {
                            for (int num22 = rectangle.X; num22 < (rectangle.X + rectangle.Width); num22++)
                            {
                                int num23 = rectangle.Y - num21;
                                if ((num23 >= 0) && (destination[(num23 * bitmapdata.Stride) + num22] != colorIndex))
                                {
                                    rectangle.Y -= num21;
                                    rectangle.Height += num21;
                                    flag3 = false;
                                    num22 = rectangle.X;
                                    num21 = 1;
                                }
                            }
                        }
                    }
                }
            Label_0495:
                bitmap.UnlockBits(bitmapdata);
                return rectangle;
            }

            public static byte[] GrayscaletoBytes(Bitmap original)
            {
                Bitmap bitmap = CreateIndexedGrayScaleBitmap(original);
                byte[] buffer = new byte[bitmap.Width * bitmap.Height];
                BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                int length = Math.Abs(bitmapdata.Stride) * bitmap.Height;
                byte[] destination = new byte[length];
                Marshal.Copy(bitmapdata.Scan0, destination, 0, length);
                bitmap.UnlockBits(bitmapdata);
                for (int i = 0; i < bitmap.Height; i++)
                {
                    for (int j = 0; j < bitmap.Width; j++)
                    {
                        buffer[(j * bitmap.Height) + i] = destination[(i * bitmapdata.Stride) + j];
                    }
                }
                return buffer;
            }

            public static bool IsGrayscale(Bitmap image)
            {
                bool flag = false;
                if (image.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    flag = true;
                    ColorPalette palette = image.Palette;
                    for (int i = 0; i < 0x100; i++)
                    {
                        Color color = palette.Entries[i];
                        if (((color.R != i) || (color.G != i)) || (color.B != i))
                        {
                            return false;
                        }
                    }
                }
                return flag;
            }

            public static Bitmap MakeGrayscale(Bitmap original)
            {
                Bitmap image = (Bitmap)original.Clone();
                if (IsGrayscale(original))
                {
                    return original;
                }
                FormatImage(ref image);
                try
                {
                    Bitmap bitmap2 = new Bitmap(image.Width, image.Height);
                    Graphics graphics = Graphics.FromImage(bitmap2);
                    float[][] newColorMatrix = new float[5][];
                    newColorMatrix[0] = new float[] { 0.3f, 0.3f, 0.3f, 0f, 0f };
                    newColorMatrix[1] = new float[] { 0.59f, 0.59f, 0.59f, 0f, 0f };
                    newColorMatrix[2] = new float[] { 0.11f, 0.11f, 0.11f, 0f, 0f };
                    float[] numArray2 = new float[5];
                    numArray2[3] = 1f;
                    newColorMatrix[3] = numArray2;
                    float[] numArray3 = new float[5];
                    numArray3[4] = 1f;
                    newColorMatrix[4] = numArray3;
                    ColorMatrix matrix = new ColorMatrix(newColorMatrix);
                    using (ImageAttributes attributes = new ImageAttributes())
                    {
                        attributes.SetColorMatrix(matrix);
                        graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                    }
                    graphics.Dispose();
                    return bitmap2;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            //public static unsafe Bitmap MakeGrayscale1(Bitmap original)
            //{
            //    Bitmap bitmap = new Bitmap(original.Width, original.Height);
            //    BitmapData bitmapdata = original.LockBits(new Rectangle(0, 0, original.Width, original.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            //    BitmapData data2 = bitmap.LockBits(new Rectangle(0, 0, original.Width, original.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            //    int num = 3;
            //    for (int i = 0; i < original.Height; i++)
            //    {
            //        byte* numPtr = (byte*)(((void*)bitmapdata.Scan0) + (i * bitmapdata.Stride));
            //        byte* numPtr2 = (byte*)(((void*)data2.Scan0) + (i * data2.Stride));
            //        for (int j = 0; j < original.Width; j++)
            //        {
            //            byte num4 = (byte)(((numPtr[j * num] * 0.11) + (numPtr[(j * num) + 1] * 0.59)) + (numPtr[(j * num) + 2] * 0.3));
            //            numPtr2[j * num] = num4;
            //            numPtr2[(j * num) + 1] = num4;
            //            numPtr2[(j * num) + 2] = num4;
            //        }
            //    }
            //    bitmap.UnlockBits(data2);
            //    original.UnlockBits(bitmapdata);
            //    return bitmap;
            //}

            public static List<Rectangle> PatternRectangeBoundaryList(Bitmap original, int colorIndex, int hStep, int vStep, bool bTopStart, int widthMin, int heightMin)
            {
                Bitmap bitmap = CreateNoneIndexedImage(original);
                List<Rectangle> list = new List<Rectangle>();
                while (true)
                {
                    Rectangle rect = GetPatternRectangeBoundary(bitmap, colorIndex, hStep, vStep, bTopStart);
                    bitmap = FillColor(bitmap, rect, colorIndex);
                    if ((rect.Width < widthMin) || (rect.Height < heightMin))
                    {
                        return list;
                    }
                    list.Add(rect);
                }
            }

            public static Rectangle RectangeBoundarybyColor(Bitmap original, int colorIndex)
            {
                Bitmap bitmap;
                Rectangle rectangle = new Rectangle(0, 0, 0, 0);
                if (!IsGrayscale(original))
                {
                    bitmap = CreateIndexedGrayScaleBitmap(original);
                }
                else
                {
                    bitmap = original;
                }
                BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                IntPtr source = bitmapdata.Scan0;
                int length = Math.Abs(bitmapdata.Stride) * bitmap.Height;
                byte[] destination = new byte[length];
                Marshal.Copy(source, destination, 0, length);
                for (int i = 0; i < bitmap.Height; i++)
                {
                    for (int n = 0; n < bitmap.Width; n++)
                    {
                        if (destination[(i * bitmapdata.Stride) + n] != colorIndex)
                        {
                            rectangle.Y = i;
                            break;
                        }
                    }
                }
                for (int j = 0; j < bitmap.Width; j++)
                {
                    for (int num5 = 0; num5 < bitmap.Height; num5++)
                    {
                        if (destination[(num5 * bitmapdata.Stride) + j] != colorIndex)
                        {
                            rectangle.X = j;
                            break;
                        }
                    }
                }
                for (int k = bitmap.Height - 1; k > 0; k--)
                {
                    for (int num7 = bitmap.Width - 1; num7 > 0; num7--)
                    {
                        if (destination[(k * bitmapdata.Stride) + num7] != colorIndex)
                        {
                            rectangle.Height = k - rectangle.Y;
                            break;
                        }
                    }
                }
                for (int m = bitmap.Width - 1; m > 0; m--)
                {
                    for (int num9 = bitmap.Height - 1; num9 > 0; num9--)
                    {
                        if (destination[(num9 * bitmapdata.Stride) + m] != colorIndex)
                        {
                            rectangle.Width = m - rectangle.X;
                            break;
                        }
                    }
                }
                bitmap.UnlockBits(bitmapdata);
                bitmap.Dispose();
                return rectangle;
            }

            public static void Set8bppIndexedPixel(Bitmap bmp, int x, int y, Color c)
            {
                int num2 = 0x7fffffff;
                int num3 = 0;
                int index = 0;
                while (index < 0x100)
                {
                    Color color = bmp.Palette.Entries[index];
                    int num4 = c.R - color.R;
                    int num5 = c.G - color.G;
                    int num6 = c.B - color.B;
                    int num7 = ((num4 * num4) + (num5 * num5)) + (num6 * num6);
                    if (num7 == 0)
                    {
                        break;
                    }
                    if (num7 < num2)
                    {
                        num2 = num7;
                        num3 = index;
                    }
                    index++;
                }
                if (index == 0x100)
                {
                    index = num3;
                }
                BitmapData bitmapdata = bmp.LockBits(new Rectangle(x, y, 1, 1), ImageLockMode.ReadOnly, bmp.PixelFormat);
                Marshal.WriteByte(bitmapdata.Scan0, (byte)index);
                bmp.UnlockBits(bitmapdata);
            }

            public static void SetGrayscalePalette(Bitmap image)
            {
                if (image.PixelFormat != PixelFormat.Format8bppIndexed)
                {
                    throw new UnsupportedImageFormatException("Source image is not 8 bpp image.");
                }
                ColorPalette palette = image.Palette;
                for (int i = 0; i < 0x100; i++)
                {
                    palette.Entries[i] = Color.FromArgb(i, i, i);
                }
                image.Palette = palette;
            }
        }
}