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
    public partial class FormImportCustomStocks : Form
    {
        public FormImportCustomStocks(List<string> items, Stocks _stocks)
        {
            InitializeComponent();      
            this.items = items;
            this.itemsShow = new List<string>();      
            this._stocks = _stocks;
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            string importtext = textBoxInput.Text;
            List<string> itemstxt = importtext.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            bool change = false;
            foreach(string txt in itemstxt)
            {
                
                List<string> find = items.FindAll(s => s.Contains(txt.ToUpper()));
                if (find.Count == 1)
                {
                    if (!itemsShow.Contains(find[0]))
                    {
                        itemsShow.AddRange(find);
                        change = true;
                    }
                }
            }
            if (change)
            {
                listBox2.Items.Clear();
                listBox2.Items.AddRange(itemsShow.ToArray());
            }
            
        }

        private void buttonClearNextList_Click(object sender, EventArgs e)
        {
            if (itemsShow.Count > 0)
            {
                itemsShow.Clear();
                listBox2.Items.Clear();
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            ImportOK = true;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.ImportOK = false;
            this.Close();
        }
        private List<string> itemsShow;
        private List<string> items;
        private Stocks _stocks;
        public bool ImportOK { get; set; }
        public List<string> ImportItems { get { return itemsShow; } }

        private void buttonAddTXDBlock_Click(object sender, EventArgs e)
        {
            string str = "";
            foreach (string s  in itemsShow)
            {

                string code = s.Substring(0,8);
                if (code != null && code.StartsWith("SZ"))
                    str += "\r\n0" + code.Substring(2);
                else if (code != null && code.StartsWith("SH"))
                    str += "\r\n1" + code.Substring(2);
            }

            Tools.MFile.WriteAllText(_stocks.Gcfg.Baseconfig.BlockPath(), str);
            MessageBox.Show("已输出");
        }

        private void textBoxInput_KeyDown(object sender, KeyEventArgs e)       
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                ((TextBox)sender).SelectAll();
            }
        }
    }
}
