using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HouseImage;

namespace JHStock.UserForm
{
    [Flags]
    enum Act : short { None = 0, DefinePoint = 1, DefineId = 2, DefineChoose = 4, DefineUnChoose = 8 };
    
    public delegate void CompleteMouseMove(bool bcompleted);
    public partial class FormShowPicture : Form
    {

        public FormShowPicture(Bitmap bmp)
        {
            InitializeComponent();
            MT = new MoveTracker(pictureBox1);
            m_act = "zoom";
            radioButton1.Checked = true; //"zoom"
            m_Imgselection = new Rectangle(0, 0, 0, 0);
            m_activeselection = new Rectangle(0, 0, 0, 0);
            crop_startpoint = new Rectangle(0, 0, 0, 0).Location;
            zoombox = new ZoomBox();
            bitmap_src = bmp;
            bitmap_show =(Bitmap) bmp.Clone();
            Init();
        }
        private void Init()
        {          
            pictureBox1.Image = bitmap_show;         
            ReSetPictureBoxImage();            
        }
        private void ReSetPictureBoxImage()
        {   //m_select.Clear();
            crop_startpoint.X = crop_startpoint.Y = 0;
            bitmap_show = (Bitmap)bitmap_src.Clone();
            m_Imgselection = new Rectangle(0, 0, 0, 0);
            pictureBox1.Image = bitmap_show;
            zoombox.UpdateBoxScale(pictureBox1);
        }
        private void buttonback_Click(object sender, EventArgs e)
        {
            ReSetPictureBoxImage();
        }
        private void FormShowPicture_Resize(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null && zoombox != null)
                zoombox.UpdateBoxScale(pictureBox1);
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            // ShowMessage("MouseMove");
            if (m_act != "list") return;
            Point p = new Point(e.X, e.Y);
            p = zoombox.BoxToImgPoint(p, crop_startpoint);//
            if (! Rectangle().Contains(p))
                return;
            //Color c = activefloor.GetPix(p);
            //string value = activefloor.GetPixValue(p);
            //ShowMessage(p + c.ToString() + value);

        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (m_act != "list") return;
            Point p = new Point(e.X, e.Y);
            if (! Rectangle().Contains(p))
                return;
            p = zoombox.BoxToImgPoint(p, crop_startpoint);//
            
            pictureBox1.Invalidate();

        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {           
            if (pictureBox1.Image != null && m_activeselection.Width > 2)
            {//DrawActive
                Pen pen = Pens.Yellow;
                e.Graphics.DrawRectangle(pen, zoombox.ImgToBoxSelection(m_activeselection, crop_startpoint));
                Brush br = Brushes.Yellow;
                e.Graphics.FillRectangle(br, zoombox.ImgToBoxSelection(m_activeselection, crop_startpoint));

            }
            if (pictureBox1.Image != null && bitmap_show != null)//&& DrawGrid
            {
                Size imgr = bitmap_show.Size; //m_Imgselection;
                if (imgr.Width < 200 && imgr.Height < 200 && imgr.Width > 10 && imgr.Height > 10)
                {
                    Pen pen = Pens.Red;
                    Rectangle r = zoombox.GetPictureBoxZoomSize(pictureBox1);
                    float hrate = r.Height / imgr.Height;
                    for (int i = 0; i < imgr.Height; i++)
                        e.Graphics.DrawLine(pen, new Point(0 + r.X, (int)(i * r.Height / imgr.Height) + r.Y), new Point(r.Width + r.X, (int)(i * r.Height / imgr.Height) + r.Y));
                    float wrate = r.Width / imgr.Width;
                    for (int i = 0; i < imgr.Width; i++)
                        e.Graphics.DrawLine(pen, new Point((int)(i * r.Width / imgr.Width) + r.X, 0 + r.Y), new Point((int)(i * r.Width / imgr.Width) + r.X, r.Height + r.Y));
                }
            }
        }
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            m_act = (string)((RadioButton)sender).Tag;
            if (m_act == "")
                m_act = "none";
            ShowMessage("act:" + m_act);

            MT.ClearEvent();
            if (m_act == "select" || m_act == "zoom" || m_act == "selectlist" || m_act == "selectdelete" || m_act == "listbyselection")
            {
                MT.StartDraw(true);
                MT.completevent += CompleteSelection;
            }
        }
        private void CompleteSelection(bool bcomplete)
        {
            if (bcomplete)
            {
                ShowMessage("Complete: " + m_act);
                m_Imgselection = zoombox.BoxToImgSelection(MT.Selection, crop_startpoint);//
                switch (m_act)
                {
                    case "zoom": CompleteZoom(); break;
                    case "select": CompleteSelect(m_Imgselection); break;
                }
            }
            pictureBox1.Invalidate();
        }
        private void CompleteZoom()
        {
            m_Imgselection.Intersect( Rectangle());
            if (m_Imgselection.Width == 0)
                return;
            bitmap_show.Dispose();
            bitmap_show = bitmap_src.Clone(m_Imgselection, bitmap_src.PixelFormat);
            crop_startpoint = m_Imgselection.Location;
            pictureBox1.Image = bitmap_show;
            //if (checkBoxCropSave.Checked)
            //    pictureBox1.Image.Save("show2.jpg");
            zoombox.UpdateBoxScale(pictureBox1);
        }
        private void CompleteSelect(Rectangle s)
        {
            // = m_Imgselection;
            ShowMessage(s.ToString());
            //activefloor.AddSelect(s);

        }
        private void ShowMessage(string p)
        {
            textBoxShow.Text = p;
        }
        private Rectangle Rectangle() { return new Rectangle(0, 0, bitmap_src.Width, bitmap_src.Height); }
        private Bitmap bitmap_src;
        private Bitmap bitmap_show;
        private Rectangle m_activeselection;     
        private Point crop_startpoint;
        private Rectangle m_Imgselection;
        private MoveTracker MT;
        private ZoomBox zoombox;
     
        private string m_act;   
    }
}
