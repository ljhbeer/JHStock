﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ScanTemplate;

namespace ARTemplate
{
    public partial class FormInput : Form
    {
        private string keyname;
        private string LableName;
        public string StrValue { get; set; }
        public int IntValue { get; set; }

        public FormInput(string keyname,string LableName = "")
        {
            this.keyname = keyname;
            InitializeComponent();
            this.AcceptButton = buttonOK;
            this.LableName = LableName;
        }
        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (Type == "S" || keyname == "考试名称"  || keyname == "删除模板确认"
                || keyname == "多个班级确认"  || keyname.Contains("删除确认")
                || keyname.Contains("文件名称") ||  keyname.Contains("记录名称"))//|| keyname == "删除确认"
            {
                StrValue =  textBox1.Text;
            }
            else if (keyname == "校对")
            {
                if (comboBox1.SelectedIndex == -1)
                    StrValue = "";
                else 
                    StrValue = comboBox1.SelectedItem.ToString();
            }
            else if (Type == "I" ||  keyname == "选择题" || keyname == "非选择题")
            {
                try
                {
                    IntValue = Convert.ToInt32(textBox1.Text);
                }
                catch
                {
                    IntValue = -1;
                }
            }
            else if (Type == "SI" || keyname == "自定义")
            {
                try
                {
                    StrValue = textBox1.Text.Trim();
                    IntValue = Convert.ToInt32(textBox2.Text);
                }
                catch
                {
                    IntValue = -2;
                }
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
        private void FormInput_Load(object sender, EventArgs e)
        {
            if (keyname.Contains("删除确认") )               
            {
                FormDeleteConfirm ff = new FormDeleteConfirm();
                this.DialogResult = ff.ShowDialog();
                this.StrValue = ff.Msg.ToString();
                if (this.DialogResult == System.Windows.Forms.DialogResult.OK)
                    this.StrValue = "Del";
                this.Close();
                return;
            }
            label2.Visible = textBox2.Visible = false;
            if (LableName == "")
            {
                if (keyname == "考试名称")
                {
                    label1.Text = "请输入本次考试名称";
                    textBox1.Text = "";
                }
                else if (keyname == "记录名称")
                {
                    label1.Text = "请输入你打算保存的记录名称";
                    textBox1.Text = "";
                }
                else if (keyname == "文件名称")
                {
                    label1.Text = "请输入你打算保存的文件名称";
                    textBox1.Text = "";
                }
                else if (keyname == "多个班级确认")
                {
                    this.Text = keyname;
                    label1.Text = "存在多个班级，我已知晓\r\n，请输入 OK 继续";
                    textBox1.Text = "";
                }
                else if (keyname == "删除确认")
                {
                    this.Text = keyname;
                    label1.Text = "请输入 Del （大小写需要一致）确认删除\r\n，输入其它字符不执行删除";
                    textBox1.Text = "";
                }
                else if (keyname == "删除模板确认")
                {
                    this.Text = keyname;
                    label1.Text = "请输入需要删除的模板前3个字母";
                    textBox1.Text = "";
                }
                else if (keyname == "校对")
                {
                    label1.Visible = textBox1.Visible = false;
                    comboBox1.Visible = true;
                    comboBox1.Location = textBox1.Location;
                    if (comboBox1.Items.Count > 0)
                        comboBox1.SelectedIndex = 0;
                }
                else if (keyname == "选择题")
                {
                    label1.Text = "请输入选择题的个数";
                    textBox1.Text = "5";
                }
                else if (keyname == "非选择题")
                {
                    label1.Text = "请输入每个空的分值，必须大于0,也可以以后统一输入";
                    textBox1.Text = "2";
                }
                else if (keyname == "自定义")
                {
                    label1.Text = "请输入自定义名称";
                    textBox1.Text = "座位号";
                    label2.Text = "输入选项数";
                    textBox2.Text = "10";
                    label2.Visible = textBox2.Visible = true; ;
                }
                else
                {
                    this.Text = keyname;
                    if (Type == "S")
                    {
                        this.label1.Text = "请输入" + keyname;
                        textBox1.Text = "";
                    }
                    else if (Type == "I")
                    {
                        label1.Visible = textBox1.Visible = false;
                        label2.Visible = textBox2.Visible = true;
                        this.label2.Text = "请输入" + keyname;
                        textBox2.Text = "";

                    }
                    else if (Type == "SI")
                    {
                        textBox2.Visible = true;
                        //this.label2.Text = "请输入" + keyname;
                        this.label1.Text = "请输入" + keyname;
                        textBox1.Text = "";
                        textBox2.Text = "";
                    }
                }
            }
            else
            {
                label1.Text = LableName;
                textBox1.Text = "";
                this.Text = LableName;              
                if (Type == "I")
                {
                    label1.Visible = textBox1.Visible = false;
                    label2.Visible = textBox2.Visible = true;
                    this.label2.Text = "请输入" + LableName;
                    textBox2.Text = "";

                }
                else if (Type == "SI")
                {
                    textBox2.Visible = true;
                    textBox1.Text = "";
                    textBox2.Text = "";
                }
            }
            textBox1.Focus();
        }

        public string Type { get; set; }
    }
}