/*

  手机没电了，坑爹的在网上找不到一个好的闹钟
软件。算了，自己写一个...
                    By LiuFu
                    2015.11.16

*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevComponents.DotNetBar.Controls;

namespace TimingPlaySongs
{
    public partial class frm_Main : Form
    {
        BackgroundWorker worker1 = new BackgroundWorker();
        bool isClock = false;

        public frm_Main()
        {
            InitializeComponent();
            //Init();
        }
        void Init()
        {
            worker1.DoWork += ClockChange;
            worker1.RunWorkerAsync();
        }

        private void ClockChange(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (DateTime.Now.ToString()==clock.Value.ToString())
                {
                    continue;
                }
                else
                {
                    ChangeClockValue(DateTime.Now);
                }
            }
        }

        private void ChangeClockValue(DateTime dateTime)
        {
            if (clock.InvokeRequired)
            {
                clock.Invoke(new Action<AnalogClockControl>(p=>p.Value=dateTime),clock);
            }
            else
            {
                clock.Value = dateTime;
            }
        }

        private void timer_Main_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now.ToString() != clock.Value.ToString())
            {
                ChangeClockValue(DateTime.Now);
            }

            if (DateTime.Now.ToLongTimeString()==dateTimePicker1.Value.ToLongTimeString())
            {
                if (isClock&&lab_SongsName.Text!="无")
                {
                    WMPLib.IWMPMedia a = player_Main.newMedia(lab_SongsName.Text);
                    player_Main.currentPlaylist.appendItem(a);
                    player_Main.Ctlcontrols.play();//播放
                    player_Main.settings.setMode("loop", true);//循环播放
                    //player_Main.Ctlcontrols.stop();//停止
                    player_Main.settings.volume = 100;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SetClock_Click(object sender, EventArgs e)
        {
            if (((Button)sender).Text=="设定")
            {
                isClock = true;
                ((Button)sender).Text = "取消";
            }
            else
            {
                isClock = false;
                ((Button)sender).Text = "设定";
            }
        }

        private void btn_SelectSongs_Click(object sender, EventArgs e)
        {
            openFileDialog1.DefaultExt = ".mp3";
            openFileDialog1.Title = "打开";
            openFileDialog1.Filter = "音乐文件|*.mp3";
            openFileDialog1.FileName = "";
            if(openFileDialog1.ShowDialog()==DialogResult.Cancel)
            {
                return;
            }
            else
            {
                lab_SongsName.Text = openFileDialog1.FileName;
            }
        }
    }
}
