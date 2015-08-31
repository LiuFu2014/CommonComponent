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
    public partial class UCFilterTop : UserControl
    {
        public UCFilterTop()
        {
            InitializeComponent();
        }
        public delegate void delegateLostFocus(bool IsFilter, string Filterstr);
        public event delegateLostFocus eventLostFocus;
        protected void LostFocusExcute(bool IsFilter,string Filterstr)
        {
            if (eventLostFocus != null)
            {
                eventLostFocus(IsFilter,Filterstr);
            }
        }
        string _ColDataName = string.Empty;
        public string ColDataName
        {
            get
            {
                return _ColDataName;
            }
            set
            {
                _ColDataName = value;
            }
        }
        public virtual void SetFocusEx(string FilterStr)
        { 
        
        
        }
        public virtual void Setlistdatasource(string[] listdatasource)
        { 
        
        
        }


        
    }

  
}
