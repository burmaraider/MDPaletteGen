using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class MDPaletteGen
{
    private byte[] imageByteArray = { };
    private List<Color> colorList = new List<Color>();

    Bitmap stripedPalette = new Bitmap(32, 16, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
    Bitmap sortedPalette = new Bitmap(64, 8, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
    Bitmap grayPic = new Bitmap(1, 8, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

    public MDPaletteGen()
    {
        GenerateBasePalette();
        ConvertColorArrayToColorList(imageByteArray);
    }

    /// <summary>
    /// This generates a base palette with 512 colors that the megadrive can use
    /// </summary>
    private void GenerateBasePalette()
    {
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

                    currentColorIndex += 3;
                }
            }
        }
    }

    /// <summary>
    /// Converts the byte array into colors to make it easier to draw with
    /// </summary>
    /// <param name="inputBytes">Byte array R G B</param>
    private void ConvertColorArrayToColorList(byte[] inputBytes)
    {
        //Convert byte array into a List<Color> to make things easier
        for (int i = 0; i < 512; i++)
            colorList.Add(Color.FromArgb(
                inputBytes[i * 3],
                inputBytes[(i * 3) + 1],
                inputBytes[(i * 3) + 2]));
    }

    /// <summary>
    /// Generates a palette in a stripe
    /// </summary>
    /// <param name="szFileName">File name prefix</param>
    /// <param name="save">Do you want to save this palette to a png?</param>
    public void SaveStripedImage(string szFileName, bool save)
    {
        int currentIndex = 0;
        for (int i = 0; i < 16; i++)
        {
            for (int t = 0; t < 32; t++)
                stripedPalette.SetPixel(t, i, colorList[t + currentIndex]);

            currentIndex += 32;
        }
        if (save)
            stripedPalette.Save(@"" + szFileName + ".png");
    }

    /// <summary>
    /// Generates a square palette in an easy to see png
    /// </summary>
    /// <param name="szFileName">File name prefix</param>
    /// <param name="save">Do you want to save this palette to a png?</param>
    public void SaveSquarePalette (string szFileName, bool save)
    {
        for (int x = 0; x < 64; x++)
        {
            for (int y = 0; y < 8; y++)
                sortedPalette.SetPixel(x, y, colorList[(x * 8) + y]);
        }
        if (save)
            sortedPalette.Save(@"" + szFileName + "_sorted.png");
    }

    /// <summary>
    /// Generates a grayscale palette, 8 values
    /// </summary>
    /// <param name="szFileName">File name prefix</param>
    /// <param name="save">Do you want to save this palette to a png?</param>
    public void SaveGrayScalePalette(string szFileName, bool save)
    {
        byte[] grayImageByteArray = { };
        for (int i = 0; i < 8; i++)
        {
            Array.Resize(ref grayImageByteArray, grayImageByteArray.Length + 3);

            grayImageByteArray[(i * 3)] = (byte)(i * 34);
            grayImageByteArray[(i * 3) + 1] = (byte)(i * 34);
            grayImageByteArray[(i * 3) + 2] = (byte)(i * 34);
        }

        for (int i = 0; i < grayImageByteArray.Count() / 3; i++)
        {
            grayPic.SetPixel(0, i, Color.FromArgb(grayImageByteArray[i * 3], grayImageByteArray[i * 3 + 1], grayImageByteArray[i * 3 + 2]));
        }
        if(save)
            grayPic.Save(@"" + szFileName + "_gray.png");
    }

    /// <summary>
    /// Saves the complete 9bit palette to a file, grayscale will be added on the left of the png
    /// </summary>
    /// <param name="szFileName"></param>
    public void SaveFullPalette(string szFileName)
    {
        //Make sure these are generated... No need to save them
        SaveStripedImage(szFileName, false);
        SaveSquarePalette(szFileName, false);
        SaveGrayScalePalette(szFileName, false);

        //compile complete palette
        Bitmap finalPalette = new Bitmap(sortedPalette.Width + 1, sortedPalette.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

        Rectangle srcRect = new Rectangle(0, 0, sortedPalette.Width, sortedPalette.Height);
        Rectangle dstRect = new Rectangle(1, 0, sortedPalette.Width, sortedPalette.Height);
        CopyRegionIntoImage(sortedPalette, srcRect, ref finalPalette, dstRect);

        Rectangle srcRectGray = new Rectangle(0, 0, grayPic.Width, grayPic.Height);
        Rectangle dstRectGray = new Rectangle(0, 0, grayPic.Width, grayPic.Height);
        CopyRegionIntoImage(grayPic, srcRectGray, ref finalPalette, dstRectGray);

        finalPalette.Save(@"" + szFileName + "_final.png");
    }

    /// <summary>
    /// This copies a region from one image into another
    /// </summary>
    /// <param name="srcBitmap"></param>
    /// <param name="srcRegion"></param>
    /// <param name="destBitmap"></param>
    /// <param name="destRegion"></param>
    private void CopyRegionIntoImage(Bitmap srcBitmap, Rectangle srcRegion, ref Bitmap destBitmap, Rectangle destRegion)
    {
        using (Graphics grD = Graphics.FromImage(destBitmap))
        {
            grD.DrawImage(srcBitmap, destRegion, srcRegion, GraphicsUnit.Pixel);
        }
    }
}

