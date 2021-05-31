using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Program
{
    static void Main(string[] args)
        {
            byte[] imageByteArray = { };
            int currentColorIndex = 0;
            for (int redIndex = 0; redIndex < 8; redIndex++)
            {
                for (int greenIndex = 0; greenIndex < 8; greenIndex++)
                {
                    for (int blueIndex = 0; blueIndex < 8; blueIndex++)
                    {
                        Array.Resize(ref imageByteArray, imageByteArray.Length + 3);

                        imageByteArray[currentColorIndex] = (byte)(redIndex * 34);
                        imageByteArray[currentColorIndex + 1] = (byte)(greenIndex * 34);
                        imageByteArray[currentColorIndex + 2] = (byte)(blueIndex * 34);

                        currentColorIndex+=3;
                    }
                }
            }

        //Setup our bitmaps to save later
        Bitmap stripedPalette = new Bitmap(32, 16, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
        Bitmap sortedPalette = new Bitmap(64, 8, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
        Bitmap grayPic = new Bitmap(1, 8, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

        List<Color> colorList = new List<Color>();
        //Convert byte array into a List<Color> to make things easier
        for (int i = 0; i < 512; i++)
            colorList.Add(Color.FromArgb(
                imageByteArray[i * 3], 
                imageByteArray[(i * 3) + 1], 
                imageByteArray[(i * 3) + 2]));

        //Save striped image
        int currentIndex = 0;
        for (int i = 0; i < 16; i++)
        {
            for (int t = 0; t < 32; t++)
                stripedPalette.SetPixel(t, i, colorList[t + currentIndex]);
            
            currentIndex += 32;
        }
        stripedPalette.Save(@"pal_unsorted.png");

        //Save square palette image
        for (int x = 0; x < 64; x++)
        {
            for (int y = 0; y < 8; y++)
                sortedPalette.SetPixel(x, y, colorList[(x * 8) + y]);
        }
        sortedPalette.Save(@"pal_sorted.png");


        //lets do some grayscale now!
        byte[] grayImageByteArray = {};
        for (int i = 0; i < 8; i++)
        {
            Array.Resize(ref grayImageByteArray, grayImageByteArray.Length + 3);

            grayImageByteArray[(i*3)] = (byte)(i * 34);
            grayImageByteArray[(i*3) + 1] = (byte)(i * 34);
            grayImageByteArray[(i*3) + 2] = (byte)(i * 34);
        }

        for (int i = 0; i < grayImageByteArray.Count() / 3; i++)
        {
            grayPic.SetPixel(0, i, Color.FromArgb(grayImageByteArray[i * 3], grayImageByteArray[i * 3 + 1], grayImageByteArray[i * 3 + 2]));
        }
        grayPic.Save(@"gray_image.png");


        //compile complete palette
        Bitmap finalPalette = new Bitmap(sortedPalette.Width + 1, sortedPalette.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

        Rectangle srcRect = new Rectangle(0, 0, sortedPalette.Width, sortedPalette.Height);
        Rectangle dstRect = new Rectangle(1, 0, sortedPalette.Width, sortedPalette.Height);
        CopyRegionIntoImage(sortedPalette, srcRect, ref finalPalette, dstRect);

        Rectangle srcRectGray = new Rectangle(0, 0, grayPic.Width, grayPic.Height);
        Rectangle dstRectGray = new Rectangle(0, 0, grayPic.Width, grayPic.Height);
        CopyRegionIntoImage(grayPic, srcRectGray, ref finalPalette, dstRectGray);
        
        finalPalette.Save(@"final.png");
    }

    public static void CopyRegionIntoImage(Bitmap srcBitmap, Rectangle srcRegion, ref Bitmap destBitmap, Rectangle destRegion)
    {
        using (Graphics grD = Graphics.FromImage(destBitmap))
        {
            grD.DrawImage(srcBitmap, destRegion, srcRegion, GraphicsUnit.Pixel);
        }
    }
    public static void BubbleSort(List<Color> input, out List<Color> t1)
    {
        var itemMoved = false;
        do
        {
            itemMoved = false;
            for (int i = 0; i < input.Count() - 1; i++)
            {
                int test = input[i].ToArgb();
                int test2 = input[i + 1].ToArgb();
                
                if (test > test2)
                {
                    var lowerValue = input[i + 1];
                    input[i + 1] = input[i];
                    input[i] = lowerValue;
                    itemMoved = true;
                    
                }
            }
        } while (itemMoved);

    t1 = input;
    }
    public static void InsertionSort(List<Color> input, out List<Color> t1)
    {

        for (int i = 0; i < input.Count(); i++)
        {
            var item = input[i].ToArgb();
            var currentIndex = i;

            while (currentIndex > 0 && input[currentIndex - 1].ToArgb() > item)
            {
                input[currentIndex] = input[currentIndex - 1];
                currentIndex--;
            }

            input[currentIndex] = Color.FromArgb(item);
        }
        t1 = input;
    }
    public static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
    {
        int max = Math.Max(color.R, Math.Max(color.G, color.B));
        int min = Math.Min(color.R, Math.Min(color.G, color.B));

        hue = color.GetHue();
        saturation = (max == 0) ? 0 : 1d - (1d * min / max);
        value = max / 255d;
    }
        
}

public static class ShiftHelper
{
    public static List<T> ShiftLeft<T>(this List<T> list, int shiftBy)
    {
        if (list.Count <= shiftBy)
        {
            return list;
        }

        var result = list.GetRange(shiftBy, list.Count - shiftBy);
        result.AddRange(list.GetRange(0, shiftBy));
        return result;
    }

    public static List<T> ShiftRight<T>(this List<T> list, int shiftBy)
    {
        if (list.Count <= shiftBy)
        {
            return list;
        }

        var result = list.GetRange(list.Count - shiftBy, shiftBy);
        result.AddRange(list.GetRange(0, list.Count - shiftBy));
        return result;
    }
}