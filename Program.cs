using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Program
{
    public static void Main(string[] args)
    {
        MDPaletteGen gen = new MDPaletteGen();
        gen.SaveFullPalette("gen");
    }
}