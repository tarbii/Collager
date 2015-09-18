using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using LinqToTwitter;

namespace Collager.Models
{
    public class Collage
    {
        public static Bitmap CreateCollage(List<User> users, int height, int width)
        {
            const double minSliceRatio = 0.35;

            var threshold = users.Max(x => x.StatusesCount) / 20;

            var elements = users
                .Select(x => new Element<User>
                {
                    Object = x, 
                    Value = x.StatusesCount < threshold ? threshold : x.StatusesCount
                })
                .OrderByDescending(x => x.Value)
                .ToList();

            var slice = Treemap.GetSlice(elements, 1, minSliceRatio);

            var rectangles = Treemap.GetRectangles(slice, width, height).ToList();

            return DrawTreemap(rectangles, width, height);
        }

        public static Bitmap DrawTreemap(
            IEnumerable<SliceRectangle<User>> rectangles, int width, int height)
        {
            var listRectangles = rectangles.ToList();

            var font = new Font("Arial Bold", 8);

            var bmp = new Bitmap(width, height);
            var gfx = Graphics.FromImage(bmp);

            gfx.FillRectangle(Brushes.SlateGray, new RectangleF(0, 0, width, height));
            var client = new WebClient();


            foreach (var r in listRectangles)
            {
                var user = r.Slice.Elements.First().Object;
                var path = user.ProfileImageUrlHttps.Replace("_normal", "");
                try
                {
                    var byteArray = client.DownloadData(path);
                    var stream = new MemoryStream(byteArray);
                    var image = new Bitmap(stream);
                    
                    var localBmp = new Bitmap(r.Width, r.Height);
                    using (var g = Graphics.FromImage(localBmp))
                    {
                        var rect = FillInBackground(r, path);
                        g.DrawImage(WhitenImage(new Bitmap(image, image.Width, image.Height)), rect);
                        rect = FillInImage(r, path);
                        g.DrawImage(image, rect);
                    }
                    gfx.DrawImage(localBmp, new Rectangle(r.X + 5, r.Y + 5, r.Width - 1 - 5, r.Height - 1 - 5));
                }
                catch (WebException ex)
                {
                    gfx.DrawString("Failed", font, Brushes.Red, r.X, r.Y);
                }
            }


            return bmp;
        }

        public static Rectangle FillInBackground(SliceRectangle<User> r, string path)
        {
            var client = new WebClient();
            var byteArray = client.DownloadData(path);
            var stream = new MemoryStream(byteArray);
            var image = new Bitmap(stream);

            var ratioX = (double)r.Width / image.Width;
            var ratioY = (double)r.Height / image.Height;

            var ratio = ratioX > ratioY ? ratioX : ratioY;

            var newHeight = Convert.ToInt32(image.Height * ratio);
            var newWidth = Convert.ToInt32(image.Width * ratio);

            var posX = Convert.ToInt32((r.Width - (image.Width * ratio)) / 2);
            var posY = Convert.ToInt32((r.Height - (image.Height * ratio)) / 2);
            
            return new Rectangle(posX, posY, newWidth - 1, newHeight - 1);
        }

        public static Rectangle FillInImage(SliceRectangle<User> r, string path)
        {
            var client = new WebClient();
            var byteArray = client.DownloadData(path);
            var stream = new MemoryStream(byteArray);
            var image = new Bitmap(stream);

            var ratioX = (double)r.Width / image.Width;
            var ratioY = (double)r.Height / image.Height;

            var ratio = ratioX < ratioY ? ratioX : ratioY;

            var newHeight = Convert.ToInt32(image.Height * ratio);
            var newWidth = Convert.ToInt32(image.Width * ratio);

            var posX = Convert.ToInt32((r.Width - (image.Width * ratio)) / 2);
            var posY = Convert.ToInt32((r.Height - (image.Height * ratio)) / 2);

            return new Rectangle(posX, posY, newWidth - 1, newHeight - 1);
        }

        public static Bitmap WhitenImage(Bitmap bmp)
        {

            var r = new Rectangle(0, 0, bmp.Width, bmp.Height);
            const int alpha = 200;
            using (Graphics g = Graphics.FromImage(bmp))
            {
                using (Brush cloudBrush = new SolidBrush(Color.FromArgb(alpha, Color.Azure)))
                {
                    g.FillRectangle(cloudBrush, r);
                }
            }

            return bmp;
        }
    }
}