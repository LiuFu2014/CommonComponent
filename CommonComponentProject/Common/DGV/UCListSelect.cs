using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Common
{
    public partial class UCListSelect:UCFilterTop
    {
        public UCListSelect()
        {
            InitializeComponent();
            foreach (System.Windows.Forms.Control control in this.Controls)
            {
                control.LostFocus += new EventHandler(UCListSelect_LostFocus);
            }
        }
        List<int> checkeditems = new List<int>();
        string[] listdata;
        string txtstr = "";
        public override void Setlistdatasource(string[] listdatasource)
        {
            listdata = listdatasource;
            if (listdatasource == null || listdatasource.Length == 0)
            {
                return;
            }
            foreach (string li in listdatasource)
            {
                checkedListBox1.Items.Add(li);
            }
            textBox1.Text = txtstr;

        }
        private void UCListSelect_LostFocus(object sender, EventArgs e)
        {
            if (this.DisplayRectangle.Contains(this.PointToClient(System.Windows.Forms.Control.MousePosition)))
            {
                return;
            }
            LostFocusExcute(false,"");
        }
        public override void SetFocusEx(string OFilters)
        {
            textBox1.Focus();
        }





        private void UCListSelect_Click(object sender, EventArgs e)
        {
            textBox1.Focus();
        }


 

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                checkeditems.Add(e.Index);
                if (("," + textBox1.Text + ",").Contains("," + checkedListBox1.Items[e.Index].ToString() + ","))
                    return;
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    textBox1.Text += checkedListBox1.Items[e.Index].ToString();
                }
                else
                {
                    textBox1.Text += "," + checkedListBox1.Items[e.Index].ToString();
                }
            }
            else
            {
                checkeditems.Remove(e.Index);
                string s = textBox1.Text;
                s = "," + s + ",";
                s = s.Replace("," + checkedListBox1.Items[e.Index].ToString() + ",", ",");
                if (s.StartsWith(","))
                    s = s.Substring(1, s.Length - 1);
                if (s.EndsWith(","))
                    s = s.Substring(0, s.Length - 1);
                textBox1.Text = s;
            }
        }


        string GetFilter()
        {
            txtstr = textBox1.Text;
            string filterstr = "";
            if (!string.IsNullOrEmpty(txtstr))
            {
                string[] cs = txtstr.Split(',');
                if (cs.Length != 0)
                {
                    bool ta = true;
                    foreach (string s in cs)
                    {

                        string newColumnFilter = string.Empty;
                        if (string.IsNullOrEmpty(s))
                        {
                            newColumnFilter = String.Format(
                     "LEN(ISNULL(CONVERT([{0}],'System.String'),''))=0",
                     ColDataName);
                        }
                        else
                        {
                            newColumnFilter = "Convert(" + ColDataName + ",'System.String') like '%" + s.Trim() + "%'";
                        }
                        if (ta)
                        {
                            ta = false;
                            filterstr = newColumnFilter;
                            continue;
                        }
                        filterstr += " or " + newColumnFilter;
                    }
                    
                }
            }
            if (!string.IsNullOrEmpty(filterstr))
            {

                filterstr = " ( " + filterstr + " ) ";
            }


            return filterstr;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LostFocusExcute(true, GetFilter());
        }

    }
}
