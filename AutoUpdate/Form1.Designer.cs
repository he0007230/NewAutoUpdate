namespace AutoUpdate
{
    partial class Form1
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem();
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem();
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.main = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.main_focus = new System.Windows.Forms.TextBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.update = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.PROGRESS = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.test = new System.Windows.Forms.TabPage();
            this.tb_test_focus = new System.Windows.Forms.TextBox();
            this.tb_testMsg = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.main.SuspendLayout();
            this.update.SuspendLayout();
            this.test.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.main);
            this.tabControl1.Controls.Add(this.update);
            this.tabControl1.Controls.Add(this.test);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(236, 316);
            this.tabControl1.TabIndex = 0;
            // 
            // main
            // 
            this.main.Controls.Add(this.label6);
            this.main.Controls.Add(this.label4);
            this.main.Controls.Add(this.label5);
            this.main.Controls.Add(this.main_focus);
            this.main.Controls.Add(this.listView1);
            this.main.Location = new System.Drawing.Point(4, 25);
            this.main.Name = "main";
            this.main.Size = new System.Drawing.Size(228, 287);
            this.main.Text = "main";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(-7, 247);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(222, 20);
            this.label6.Text = "【0】程序更新 | 【9】测试界面";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular);
            this.label4.Location = new System.Drawing.Point(3, 267);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(212, 17);
            this.label4.Text = "label4";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(33, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(160, 18);
            this.label5.Text = "请选择对应的程序";
            // 
            // main_focus
            // 
            this.main_focus.BackColor = System.Drawing.SystemColors.Control;
            this.main_focus.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.main_focus.Location = new System.Drawing.Point(-15, 0);
            this.main_focus.Name = "main_focus";
            this.main_focus.ReadOnly = true;
            this.main_focus.Size = new System.Drawing.Size(18, 23);
            this.main_focus.TabIndex = 1;
            this.main_focus.KeyDown += new System.Windows.Forms.KeyEventHandler(this.main_focus_KeyDown);
            this.main_focus.LostFocus += new System.EventHandler(this.IsLostFocus);
            // 
            // listView1
            // 
            this.listView1.BackColor = System.Drawing.SystemColors.Control;
            this.listView1.Font = new System.Drawing.Font("Tahoma", 20F, System.Drawing.FontStyle.Bold);
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            listViewItem1.BackColor = System.Drawing.SystemColors.Control;
            listViewItem1.Text = "1、更新程序";
            listViewItem2.BackColor = System.Drawing.SystemColors.Control;
            listViewItem2.Text = "2、物资管理";
            listViewItem3.BackColor = System.Drawing.SystemColors.Control;
            listViewItem3.Text = "3、参数设置";
            this.listView1.Items.Add(listViewItem1);
            this.listView1.Items.Add(listViewItem2);
            this.listView1.Items.Add(listViewItem3);
            this.listView1.Location = new System.Drawing.Point(3, 29);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(222, 215);
            this.listView1.TabIndex = 0;
            this.listView1.View = System.Windows.Forms.View.List;
            // 
            // update
            // 
            this.update.Controls.Add(this.label3);
            this.update.Controls.Add(this.textBox1);
            this.update.Controls.Add(this.PROGRESS);
            this.update.Controls.Add(this.progressBar1);
            this.update.Location = new System.Drawing.Point(4, 25);
            this.update.Name = "update";
            this.update.Size = new System.Drawing.Size(228, 287);
            this.update.Text = "update";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(3, 152);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(117, 32);
            this.label3.Text = "更新进度";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.Control;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold);
            this.textBox1.Location = new System.Drawing.Point(3, 38);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(222, 111);
            this.textBox1.TabIndex = 4;
            this.textBox1.WordWrap = false;
            // 
            // PROGRESS
            // 
            this.PROGRESS.BackColor = System.Drawing.SystemColors.Control;
            this.PROGRESS.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.PROGRESS.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold);
            this.PROGRESS.Location = new System.Drawing.Point(113, 219);
            this.PROGRESS.Name = "PROGRESS";
            this.PROGRESS.ReadOnly = true;
            this.PROGRESS.Size = new System.Drawing.Size(112, 35);
            this.PROGRESS.TabIndex = 2;
            this.PROGRESS.Text = "   0/0";
            this.PROGRESS.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PROGRESS_KeyDown);
            this.PROGRESS.LostFocus += new System.EventHandler(this.IsLostFocus);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(3, 187);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(222, 26);
            // 
            // test
            // 
            this.test.Controls.Add(this.tb_test_focus);
            this.test.Controls.Add(this.tb_testMsg);
            this.test.Location = new System.Drawing.Point(4, 25);
            this.test.Name = "test";
            this.test.Size = new System.Drawing.Size(228, 287);
            this.test.Text = "test";
            // 
            // tb_test_focus
            // 
            this.tb_test_focus.BackColor = System.Drawing.SystemColors.GrayText;
            this.tb_test_focus.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tb_test_focus.Location = new System.Drawing.Point(-25, 0);
            this.tb_test_focus.Name = "tb_test_focus";
            this.tb_test_focus.Size = new System.Drawing.Size(22, 23);
            this.tb_test_focus.TabIndex = 3;
            this.tb_test_focus.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tb_test_focus_KeyUp);
            this.tb_test_focus.LostFocus += new System.EventHandler(this.IsLostFocus);
            // 
            // tb_testMsg
            // 
            this.tb_testMsg.BackColor = System.Drawing.SystemColors.GrayText;
            this.tb_testMsg.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tb_testMsg.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular);
            this.tb_testMsg.Location = new System.Drawing.Point(0, 0);
            this.tb_testMsg.Multiline = true;
            this.tb_testMsg.Name = "tb_testMsg";
            this.tb_testMsg.Size = new System.Drawing.Size(232, 287);
            this.tb_testMsg.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(238, 298);
            this.ControlBox = false;
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Location = new System.Drawing.Point(0, -30);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "form1";
            this.tabControl1.ResumeLayout(false);
            this.main.ResumeLayout(false);
            this.update.ResumeLayout(false);
            this.test.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage main;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.TabPage update;
        private System.Windows.Forms.TextBox PROGRESS;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox main_focus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabPage test;
        private System.Windows.Forms.TextBox tb_testMsg;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tb_test_focus;

    }
}

