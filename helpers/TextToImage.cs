using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Helpers;

namespace DETP.helpers
{
    public class TextToImage
    {
        public static Bitmap convert(string text)
        {
            Bitmap bmp = new Bitmap(120, 30);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.Navy);
            g.DrawString(text, new Font("Courier", 16),
                                 new SolidBrush(Color.WhiteSmoke), 2, 2);
            g.FillRectangle(new HatchBrush(HatchStyle.BackwardDiagonal, Color.FromArgb(255, 0, 0, 0), Color.Transparent), g.ClipBounds);
            g.FillRectangle(new HatchBrush(HatchStyle.ForwardDiagonal, Color.FromArgb(255, 0, 0, 0), Color.Transparent), g.ClipBounds);

            return bmp;
        }

        public static string ToBase64(Bitmap bitmap)
        {
            Bitmap bImage = bitmap;  // Your Bitmap Image
            System.IO.MemoryStream ms = new MemoryStream();
            bImage.Save(ms, ImageFormat.Jpeg);
            byte[] byteImage = ms.ToArray();
            return Convert.ToBase64String(byteImage);
        }
    }
}
