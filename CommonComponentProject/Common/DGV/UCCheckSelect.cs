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
    public partial class UCCheckSelect : UCFilterTop
    {
        public UCCheckSelect()
        {
            InitializeComponent();
            foreach (System.Windows.Forms.Control control in this.Controls)
            {
                control.LostFocus += new EventHandler(UCCheckSelect_LostFocus);
            }
            radioButton1.Checked = true;
        }
        int CheckedValue = 0;
        string Filters = string.Empty;
        private void UCCheckSelect_Click(object sender, EventArgs e)
        {
            radioButton1.Focus();
        }

  
        private void UCCheckSelect_LostFocus(object sender, EventArgs e)
        {
            if (this.DisplayRectangle.Contains(this.PointToClient(System.Windows.Forms.Control.MousePosition)))
            {
                return;
            }

            string NewFilterstr=string.Empty;
            if (radioButton3.Checked)
            {
                CheckedValue = 0;
                NewFilterstr = ColDataName + " =false or " + ColDataName + " is null";
            }
            else if (radioButton2.Checked)
            {
                CheckedValue = 1;
                NewFilterstr = ColDataName + " =true ";
            }
            else
            {
                CheckedValue = 2;
                NewFilterstr = string.Empty;
            }
            Filters = NewFilterstr;
            LostFocusExcute(true, Filters);
        }

        public override void SetFocusEx(string OFilters)
        {
            if (Filters != OFilters)
            {
                CheckedValue = 0;
                radioButton1.Checked = true;
            }
            else
            {
                switch (CheckedValue)
                {
                    case 0:
                        radioButton3.Checked = true;
                        break;
                    case 1:
                        radioButton2.Checked = true;
                        break;
                    default:
                        radioButton1.Checked = true;
                        break;
                }
            }
            radioButton1.Focus();
        }
       
    }
}
