using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using Tools;
using ARTemplate;

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
            _stocklist = new StockLists(_stocks.Gcfg);
        }
        private void FormImportCustomStocks_Load(object sender, EventArgs e)
        {
            if (_stocklist != null)
                listBoxStockList.Items.AddRange(_stocklist.StockListList.ToArray());
        }
        private void listBoxStockList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxStockList.SelectedIndex == -1) return;
            StockList sl = (StockList)listBoxStockList.SelectedItem;
            if (sl.ListCodes.Count > 0)
            {
                itemsShow.Clear();
               ImportStocklist(sl.ListCodes);
            }
        }
        private void buttonImport_Click(object sender, EventArgs e)
        {
            string importtext = textBoxInput.Text;
            List<string> itemstxt = importtext.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            ImportStocklist(itemstxt);            
        }
        private void buttonSaveToList_Click(object sender, EventArgs e)
        {
            if (itemsShow.Count == 0) return;
            if (InputBox.Input("文件名称"))
            {
                string name = InputBox.strValue;
                if (name.Trim() == "") return;

                StockList sl = new StockList(itemsShow, name);
                _stocklist.StockListList.Add(sl);
                _stocklist.SaveStockList();
                listBoxStockList.Items.Add(sl);
            }

        }
        private void ImportStocklist(List<string> itemstxt)
        {
            bool change = false;
            foreach (string txt in itemstxt)
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
        private StockLists _stocklist;
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

        private void listBoxStockList_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                StockList item = (StockList)(listBoxStockList.SelectedItem);
                if (item != null)
                {
                    _stocklist.StockListList.Remove(item);
                    _stocklist.SaveStockList();
                }

            }
        }
    }
    
    public class StockLists
    {
        private GlobalConfig globalConfig;

        public StockLists(GlobalConfig globalConfig)
        {
            this.globalConfig = globalConfig;
            StockListList = new List<StockList>();
            LoadStockList();
        }

        private void LoadStockList()
        {
            string fn = globalConfig.Baseconfig.WorkPath + "savestockslist.json";
            if (File.Exists(fn))
            {
                try
                {
                    StockListList = JsonConvert.DeserializeObject<List<StockList>>(File.ReadAllText(fn));
                }
                catch (Exception e)
                {
                    StockListList = new List<StockList>();
                   
                }
            }
        }
        public void SaveStockList()
        {
            string fn = globalConfig.Baseconfig.WorkPath + "savestockslist.json";
            string str = JsonConvert.SerializeObject(StockListList);
            MFile.WriteAllText(fn, str);
        }
        public List<StockList> StockListList { get; set; }
    }
    public class StockList
    {
        public StockList(List<string> itemsShow, string name)
        {
            if (itemsShow == null)
                itemsShow = new List<string>();
            this.ListCodes = new List<string>(itemsShow );
            this.ListName = name;
            StoreDate = System.DateTime.Now;
        }
        public override string ToString()
        {
            return ListName + StoreDate.ToShortDateString();
        }
        public string ListName { get; set; }
        public DateTime StoreDate { get; set; }
        public List<String> ListCodes { get; set; }
    }

    public class InputBox
    {
        public InputBox()
        {
        }

        public static bool Input(string keyname, Form parent = null)
        {
            String Type = "";
            if (keyname.StartsWith("S_") || keyname.StartsWith("I_") || keyname.StartsWith("SI_"))
            {
                Type = keyname.Substring(0, keyname.IndexOf("_"));
                keyname = keyname.Substring(Type.Length + 1);
            }
            FormInput f = new FormInput(keyname);

            f.Type = Type;
            if (f.ShowDialog(parent) == DialogResult.OK)
            {
                if (Type == "S" || keyname == "考试名称" || keyname == "校对" || keyname == "文件名称"
                    || Type == "记录名称")
                    strValue = f.StrValue;
                else if (Type == "I" || keyname == "选择题" || keyname == "非选择题")
                    IntValue = f.IntValue;
                else if (Type == "SI" || keyname == "自定义")
                {
                    IntValue = f.IntValue;
                    strValue = f.StrValue;
                }
                if (f.StrValue == "" || f.IntValue < 0)
                    return false;
                return true;
            }
            f.Dispose();
            f = null;
            return false;
        }
        public static bool Input(string keyname, ComboBox.ObjectCollection objectCollection, Form parent = null)
        {
            //FormInputComboBox f = new FormInputComboBox(keyname, objectCollection);
            //if (f.ShowDialog(parent) == DialogResult.OK)
            //{
            //    strValue = f.StrValue;
            //}
            return true;
        }

        public static bool Input(string keyname, List<object> objectCollection, Form parent = null)
        {
            //FormInputComboBox f = new FormInputComboBox(keyname, objectCollection);
            //if (f.ShowDialog(parent) == DialogResult.OK)
            //{
            //    strValue = f.StrValue;
            //    try
            //    {
            //        IntValue = Convert.ToInt32(strValue);
            //    }
            //    catch { ;}
            //}
            return true;
        }
        public static string strValue;
        public static int IntValue;
        public static float FloatValue;
    }
}
