using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace JHStock
{
    public class ZoomBox
    {
        public ZoomBox()
        {
            Reset();
        }
        public void Reset()
        {
            img_location = new Point(0, 0);
            img_scale = new SizeF(1, 1);
        }
        public Rectangle ImgToBoxSelection(Rectangle rectangle, Point imgstart)
        {
            RectangleF r = rectangle;
            r.Offset(-imgstart.X, -imgstart.Y);
            if (r.X < 1 || r.Y < 1)
                return new Rectangle(0, 0, 0, 0);
            r.X /= img_scale.Width;
            r.Y /= img_scale.Height;
            r.Width /= img_scale.Width;
            r.Height /= img_scale.Height;
            r.Offset(img_location.X, img_location.Y);
            return new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height);
        }
        public Rectangle BoxToImgSelection(Rectangle rectangle, Point imgstart)
        {
            RectangleF r = rectangle;
            r.Offset(-img_location.X, -img_location.Y);
            r.X *= img_scale.Width;
            r.Y *= img_scale.Height;
            r.Width *= img_scale.Width;
            r.Height *= img_scale.Height;
            r.Offset(imgstart.X, imgstart.Y);
            return new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height);
        }
        public Point BoxToImgPoint(Point r, Point imgstart)
        {
            r.Offset(-img_location.X, -img_location.Y);
            r.X = (int)(r.X * img_scale.Width);
            r.Y = (int)(r.Y * img_scale.Height);
            r.Offset(imgstart.X, imgstart.Y);
            return r;
        }
        public void UpdateBoxScale(PictureBox pictureBox1)
        {
            if (pictureBox1.Image != null)
            {
                Rectangle imgrect = GetPictureBoxZoomSize(pictureBox1);
                System.Drawing.SizeF size = pictureBox1.Image.Size;
                img_location = imgrect.Location;
                img_scale = new SizeF((float)(size.Width * 1.0 / imgrect.Width), (float)(size.Height * 1.0 / imgrect.Height));
            }
            else
            {
                Reset();
            }
        }

        public Rectangle GetPictureBoxZoomSize(PictureBox p_PictureBox)
        {
            if (p_PictureBox != null)
            {
                System.Reflection.PropertyInfo _ImageRectanglePropert = p_PictureBox.GetType().GetProperty("ImageRectangle", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                return (System.Drawing.Rectangle)_ImageRectanglePropert.GetValue(p_PictureBox, null);
            }
            return new System.Drawing.Rectangle(0, 0, 0, 0);
        }
        private SizeF img_scale;
        private Point img_location;
    }
}
