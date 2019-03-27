using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScanTemplate
{
    public partial class FormDeleteConfirm : Form
    {
        public FormDeleteConfirm()
        {
            InitializeComponent();
            Initbuttons();
            Msg = new StringBuilder();
        }
        private void Initbuttons()
        {
            List<Char> charmsg = new List<char>();
            for (int i = 0; i < 24; i++)
                charmsg.Add((Char)('A' + i));
            Random random = new Random(24);
            Font f1 = buttonCancel.Font;
            Font f2 = new Font(f1.FontFamily,18);
            for (int i = 23; i >= 0; i--)
            {
                int index = 0;
                if(i>0)
                    index = random.Next(i);
                Button btn = new Button();
                char c = charmsg[index];
                charmsg.RemoveAt(index);
                btn.Tag = c;
                btn.Text = c.ToString();
                btn.Click += new EventHandler(btn_Click);
                btn.Margin = new System.Windows.Forms.Padding(0);
                btn.Dock = DockStyle.Fill;
                btn.Font = f2;
                TPL.Controls.Add(btn);
                TPL.SetRow(btn, i / 8);
                TPL.SetColumn(btn, i % 8);
            }
        }
        void btn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            Msg.Append(btn.Tag);
            textBoxMsg.Text = Msg.ToString();
        }
        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            if (Msg.ToString() == "DEL")
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;                
            }
            this.Close();
        }
        private void buttonClear_Click(object sender, EventArgs e)
        {
            Msg.Clear();
            textBoxMsg.Text = Msg.ToString();
        }
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;            
            this.Close();
        }
        public StringBuilder Msg;
    }
}
