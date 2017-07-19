using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JHStock
{
    public partial class FormPictureBox : Form
    {
        private Bitmap bitmap;
        public FormPictureBox(Bitmap bitmap)
        {
            InitializeComponent();
            this.bitmap = bitmap;
            pictureBox1.Image = this.bitmap;
        }
    }
}
