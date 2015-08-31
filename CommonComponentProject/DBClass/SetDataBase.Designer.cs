namespace DBClass
{
    partial class SetDataBase
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.dbProviderCbx = new System.Windows.Forms.ComboBox();
            this.ServerCbx = new System.Windows.Forms.ComboBox();
            this.UID = new System.Windows.Forms.TextBox();
            this.PWD = new System.Windows.Forms.TextBox();
            this.DatabaseCbx = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "数据库类型";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "服务器名称";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(42, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "登录账号";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(42, 119);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "登录密码";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(30, 142);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "数据库名称";
            // 
            // dbProviderCbx
            // 
            this.dbProviderCbx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.dbProviderCbx.FormattingEnabled = true;
            this.dbProviderCbx.Items.AddRange(new object[] {
            "SQL SERVER"});
            this.dbProviderCbx.Location = new System.Drawing.Point(101, 26);
            this.dbProviderCbx.Name = "dbProviderCbx";
            this.dbProviderCbx.Size = new System.Drawing.Size(121, 20);
            this.dbProviderCbx.TabIndex = 5;
            this.dbProviderCbx.SelectedIndexChanged += new System.EventHandler(this.dbProviderName_SelectedIndexChanged);
            // 
            // ServerCbx
            // 
            this.ServerCbx.FormattingEnabled = true;
            this.ServerCbx.Location = new System.Drawing.Point(101, 53);
            this.ServerCbx.Name = "ServerCbx";
            this.ServerCbx.Size = new System.Drawing.Size(121, 20);
            this.ServerCbx.TabIndex = 6;
            // 
            // UID
            // 
            this.UID.Location = new System.Drawing.Point(101, 80);
            this.UID.Name = "UID";
            this.UID.Size = new System.Drawing.Size(121, 21);
            this.UID.TabIndex = 7;
            // 
            // PWD
            // 
            this.PWD.Location = new System.Drawing.Point(101, 108);
            this.PWD.Name = "PWD";
            this.PWD.Size = new System.Drawing.Size(121, 21);
            this.PWD.TabIndex = 8;
            // 
            // DatabaseCbx
            // 
            this.DatabaseCbx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DatabaseCbx.FormattingEnabled = true;
            this.DatabaseCbx.Location = new System.Drawing.Point(101, 136);
            this.DatabaseCbx.Name = "DatabaseCbx";
            this.DatabaseCbx.Size = new System.Drawing.Size(121, 20);
            this.DatabaseCbx.TabIndex = 9;
            this.DatabaseCbx.DropDown += new System.EventHandler(this.DatabaseCbx_DropDown);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(32, 180);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "确定";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(147, 180);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 11;
            this.button2.Text = "退出";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // SetDataBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(257, 225);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.DatabaseCbx);
            this.Controls.Add(this.PWD);
            this.Controls.Add(this.UID);
            this.Controls.Add(this.ServerCbx);
            this.Controls.Add(this.dbProviderCbx);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "SetDataBase";
            this.Text = "数据库连接配置";
            this.Load += new System.EventHandler(this.SetDataBase_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox dbProviderCbx;
        private System.Windows.Forms.ComboBox ServerCbx;
        private System.Windows.Forms.TextBox UID;
        private System.Windows.Forms.TextBox PWD;
        private System.Windows.Forms.ComboBox DatabaseCbx;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}

