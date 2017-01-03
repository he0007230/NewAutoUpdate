using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Xml;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using OpenNETCF.Net.NetworkInformation;
using SYNCC;
using ArmAssistBll;

namespace AutoUpdate
{
    public partial class Form1 : Form
    {
        private TextBox _nowControlModule;    //当前获取焦点的控件
        //public object _nextControlModule;   //下一个焦点控件
        private string _codeStr;
        private int _pFlag;
        private string _outStr;
        private TcpClient m_socketClient;
        private string _applicationName;
        private int _applicationCount;
        private int quitFlag;
        private string _stockNo;
        private string _IpAddress;

        // 2M 的接收缓冲区，目的是一次接收完服务器发回的消息
        byte[] m_receiveBuffer = new byte[2048 * 1024];

        private SYSTEM_POWER_STATUS_EX status;


        private string _serverIP;
        private int _serverPort;

        [DllImport("coredll.Dll")]
        public static extern int SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        #region 可以实现 WINCE 系统重新启动

        [DllImport("Coredll.dll")]
        extern static int KernelIoControl(int dwIoControlCode, IntPtr lpInBuf, int nInBufSize, IntPtr
        lpOutBuf, int nOutBufSize, ref int lpBytesReturned);

        [DllImport("Coredll.dll")]
        extern static void SetCleanRebootFlag();



        /************************************************** 
 * 函 数 名：HardReset 
 * 功能描述：可以写成一个系统重启的函数 
 * 输入参数：无 
 * 输出参数：无 
 * 返 回 值: 无 
 * 作    者：lcb 
 * 日    期：2012-8-15 
 * 修 改 人：chen 
 * 日    期：2015-1-6 
 ***************************************************/
        public void HardReset()
        {
            int IOCTL_HAL_REBOOT = 0x101003C;
            int bytesReturned = 0;
            SetCleanRebootFlag();
            KernelIoControl(IOCTL_HAL_REBOOT, IntPtr.Zero, 0, IntPtr.Zero, 0, ref bytesReturned);
        }
        #endregion

        #region  隐藏/显示 Windows 任务栏

        [DllImport("coredll.dll", EntryPoint = "FindWindow")]
        public static extern int FindWindow(string lpWindowName, string lpClassName);

        [DllImport("coredll.Dll")]
        public static extern void SetForegroundWindow(IntPtr hwnd);

        [DllImport("coredll.dll", EntryPoint = "ShowWindow")]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        //显示窗口常量  
        public const int SW_SHOW = 5;
        //隐藏窗口常量  
        public const int SW_HIDE = 0;


        /************************************************** 
 * 函 数 名：ShowTaskBar
 * 功能描述：显示任务栏 
 * 输入参数：无
 * 输出参数：无 
 * 返 回 值: 无 
 * 作    者：lcb 
 * 日    期：2012-8-15 
 * 修 改 人：chen 
 * 日    期：2015-1-6 
 ***************************************************/
        private void ShowTaskBar()
        {
            int Hwnd = FindWindow("HHTaskBar", null);
            if (Hwnd != 0)
            {
                //ShowWindow(Hwnd, SW_SHOW); //显示任务栏  
            }
        }

        /************************************************** 
 * 函 数 名：HideTaskBar 
 * 功能描述：隐藏任务栏 
 * 输入参数：无
 * 输出参数：无 
 * 返 回 值: 无 
 * 作    者：lcb 
 * 日    期：2012-8-15 
 * 修 改 人：chen 
 * 日    期：2015-1-6 
 ***************************************************/
        private void HideTaskBar()
        {
            int Hwnd = FindWindow("HHTaskBar", null);
            if (Hwnd != 0)
            {
                //隐藏任务栏  
                //ShowWindow(Hwnd, SW_HIDE);
            }
        }
        #endregion  

        #region h获取设备电量代码
        private class SYSTEM_POWER_STATUS_EX
        {
            public byte ACLineStatus = 0;
            public byte BatteryFlag = 0;
            public byte BatteryLifePercent = 0;
            public byte Reserved1 = 0;
            public uint BatteryLifeTime = 0;
            public uint BatteryFullLifeTime = 0;
            public byte Reserved2 = 0;
            public byte BackupBatteryFlag = 0;
            public byte BackupBatteryLifePercent = 0;
            public byte Reserved3 = 0;
            public uint BackupBatteryLifeTime = 0;
            public uint BackupBatteryFullLifeTime = 0;
        }
        [DllImport("coredll")]
        private static extern int GetSystemPowerStatusEx(SYSTEM_POWER_STATUS_EX lpSystemPowerStatus, bool fUpdate);
        #endregion
        public Form1()
        {
            InitializeComponent();
            init();
        }



        /// <summary>
        /// 获取电量
        /// </summary>
        /// <returns></returns>
        private int GetPower()
        {
            if (GetSystemPowerStatusEx(status, false) == 1)
            {
                if (status.BatteryLifePercent > 100)
                    status.BatteryLifePercent = 100;
                return status.BatteryLifePercent;
            }
            else
            {
                return -1;
            }
        }

        private void ShowPower()
        {
            //statusBar1.Text = "电量:" + NLSSysInfo.GetPowerPercent().ToString() + "%";
            //statusBar1.Text = NLSSysCtrl.GetTickCount().ToString();
            //statusBar2.Text = _serverIP.Substring(_serverIP.Length - 3, 3) + "用户:" + _workerName + "   | 电量:" + GetPower();
        }

        /// <summary>
        /// 新的通信方式
        /// </summary>
        private void NewTransmit()
        {
            string msg;
            if (!WifiCtrl.GetInstance().isConnectWifi(_IpAddress,out msg))
            {
                //MessageBox.Show(msg+",请换个地方重新开机!");
                _outStr = msg;
                return;
            }
            CompactFormatter.TransDTO transDTO = new CompactFormatter.TransDTO();
            transDTO.AppName = _applicationName;
            transDTO.CodeStr = _codeStr;
            transDTO.IP = _IpAddress;
            transDTO.pFlag = _pFlag;
            transDTO.StockNo = _stockNo;
            transDTO.Remark = msg;
            NetWorkScript.Instance.write(1, 1, 1, transDTO);
            NetWorkScript.Instance.AsyncReceive();
            if (NetWorkScript.Instance.messageList.Count > 0)
            {
                SocketModel socketModel = NetWorkScript.Instance.messageList[0];
                NetWorkScript.Instance.messageList.RemoveAt(0);
                _outStr = socketModel.message.ToString();
            }
            else
            {
                NetWorkScript.Instance.release();
                _outStr = "没有返回信息!";
            }
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        private byte[] DownLoadFile()
        {
            CompactFormatter.TransDTO transDTO = new CompactFormatter.TransDTO();
            transDTO.AppName = _applicationName;
            transDTO.CodeStr = _codeStr;
            transDTO.IP = _IpAddress;
            transDTO.pFlag = _pFlag;
            transDTO.StockNo = _stockNo;
            NetWorkScript.Instance.write(1, 1, 1, transDTO);
            NetWorkScript.Instance.AsyncReceive();
            if (NetWorkScript.Instance.messageList.Count > 0)
            {
                SocketModel socketModel = NetWorkScript.Instance.messageList[0];
                NetWorkScript.Instance.messageList.RemoveAt(0);
                return (byte[])socketModel.message;

            }
            else
            {
                NetWorkScript.Instance.release();
                return null;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void init()
        {
            //ShowWindow(this.Handle, SW_SHOW);
            //SetForegroundWindow(this.Handle);
            //SetWindowPos(this.Handle, -1, 0, 0, 0, 0, 1 | 2);

            //m_socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            status = new SYSTEM_POWER_STATUS_EX();
            quitFlag = 0;
            _outStr = "";
            _codeStr = "";
            _applicationName = "AutoUpdate";
            _pFlag = 98;
            _applicationCount = 0;
            tabControl1.SelectedIndex = tabControl1.TabPages.IndexOf(main);
            _nowControlModule = main_focus;
            try
            {
                _IpAddress = WifiCtrl.GetInstance().GetWifiStatus().CurrentIpAddress.ToString();
                if (_IpAddress == "0.0.0.0")
                {
                    _IpAddress = IPHelper.GetIpAddress();
                }
            }
            catch 
            {
                _IpAddress = IPHelper.GetIpAddress();
            }
            main_focus.Focus();
            //Disconnect();
            //Connect();
            listView1.Items.Clear();
            XmlDocument xml = new XmlDocument();
            try
            {
                //openFileDialog1.ShowDialog();
                //string path = openFileDialog1.FileName;
                xml.Load("\\Program Files\\CONFIG.XML");
                //XmlNode Setting = xml.SelectSingleNode("Root/System");
                _serverIP = xml.SelectSingleNode("Root/System/server_ip").InnerText;
                _serverPort = int.Parse(xml.SelectSingleNode("Root/System/server_port").InnerText);
                _stockNo = xml.SelectSingleNode("/Root/System/stock_no").InnerText;
                XmlNodeList Application = xml.SelectSingleNode("Root/Applications").ChildNodes;
                foreach (XmlNode app in Application)
                {
                    //MessageBox.Show(app.SelectSingleNode("name").InnerText,"app");
                    AddListviewItem(app.SelectSingleNode("name").InnerText, app.Name, app.SelectSingleNode("version").InnerText
                        , app.SelectSingleNode("wince_path").InnerText, app.SelectSingleNode("pc_path").InnerText);

                }

            }
            catch
            {
                //MessageBox.Show(ee.Message, "错误");
                UpdateConfig();

            }
            //HideTaskBar();
            //Connect();
            ShowPower();
            label4.Text ="IP地址：" + _IpAddress;
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        private void Connect()
        {
            //lock (this)
            //{
                //try
                ///{
                   /// m_socketClient = new TcpClient(_serverIP, _serverPort);
                    //m_socketClient.ReceiveTimeout = 20 * 1000;

                    /*

                    if (m_socketClient.Client.Connected)
                    {
                        //this.AddInfo("连接成功.");
                    }
                    else
                    {
                        //this.AddInfo("连接失败.");
                         ShowMessage("连接失败!","错误");
                    }
                     * */
                   // return;

                //}
                //catch
               // {
               // }
            //}
            //_oldTime = GetTick();
        }

        /// <summary>
        /// 与服务器断开连接
        /// </summary>
        private void Disconnect()
        {
          //  lock (this)
          //  {
               // if (m_socketClient == null)
               // {
              // //     return;
            //   }
        //
              //  try
               // {
              //      m_socketClient.Close();
              //      //this.AddInfo("断开连接成功！");
             //   }
              //  catch
             //   {
                    //this.AddInfo("断开连接时出错: " + err.Message);
                    // ShowMessage("断开连接时出错: " + err.Message,"错误");
              //  }
           //     finally
             //   {
           //         m_socketClient = null;
             //   }
          //  }
        }


        /// <summary>
        /// 发送信息
        /// </summary>
        private void SendOneDatagram()
        {
            if (m_socketClient != null)
            {
                this.Disconnect();
            }
            this.Connect();

            string datagramText2 = "1#" + _pFlag + "#" + _codeStr + "#" + _applicationName + "#";

            byte[] b = Encoding.UTF8.GetBytes(datagramText2);//按照指定编码将string编程字节数组
            string datagramText = string.Empty;
            for (int i = 0; i < b.Length; i++)//逐字节变为16进制字符，以%隔开
            {
                datagramText += "%" + Convert.ToString(b[i], 16);
            }

            //byte[] encbuff = System.Text.Encoding.UTF8.GetBytes(datagramText);
            //datagramText = Convert.ToBase64String(encbuff);
            //if (ShowMessage(datagramText, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            //{
            //Application.Exit();
            //}
            //datagramText = textBox1.Text + "#" + textBox2.Text + "#" + textBox3.Text + "|" + textBox4.Text + "|" + textBox5.Text + "|";
            //datagramText += textBox6.Text + "|" + textBox8.Text + "|" + textBox7.Text + "|#";

            byte[] Cmd = Encoding.ASCII.GetBytes(datagramText);
            byte check = (byte)(Cmd[0] ^ Cmd[1]);
            for (int i = 2; i < Cmd.Length; i++)
            {
                check = (byte)(check ^ Cmd[i]);
            }
            datagramText = "<" + datagramText + (char)check + ">";
            byte[] datagram = Encoding.ASCII.GetBytes(datagramText);

            try
            {
                m_socketClient.Client.Send(datagram);
                //this.AddInfo("send text = " + datagramText);

                //if (ck_AsyncReceive.Checked)  // 异步接收回答
                // {
                //m_socketClient.Client.BeginReceive(m_receiveBuffer, 0, m_receiveBuffer.Length, SocketFlags.None, this.EndReceiveDatagram, this);
                //}
                // else
                // {
                this.Receive();
                //}
            }
            catch (Exception err)
            {

                    //this.AddInfo("发送错误: " + err.Message);
                    MessageBox.Show("连接服务器失败: " + err.Message, "错误");
                    //this.AddInfo("连接服务器失败:!\r\n" + err.Message);
                    _outStr = "";
                    this.CloseClientSocket();

            }
        }

        private void Receive()
        {
            try
            {
                int len = m_socketClient.Client.Receive(m_receiveBuffer, 0, m_receiveBuffer.Length, SocketFlags.None);
                if (len > 0)
                {
                    CheckReplyDatagram(len);
                }
            }
            catch (Exception err)
            {
                //this.AddInfo("接收错误: " + err.Message);
                MessageBox.Show("接收错误: " + err.Message, "错误");
                this.CloseClientSocket();
            }
        }

        private void CheckReplyDatagram(int len)
        {
            string datagramText = Encoding.ASCII.GetString(m_receiveBuffer, 0, len);
            //byte[] decbuff = Convert.FromBase64String(replyMesage);
            if (datagramText[0] != '%')
            {
                _outStr = "返回的信息错误！";
                return;
            }
            string[] chars = datagramText.Substring(1, datagramText.Length - 1).Split('%');
            byte[] b = new byte[chars.Length];
            //逐个字符变为16进制字节数据
            for (int i = 0; i < chars.Length; i++)
            {
                b[i] = Convert.ToByte(chars[i], 16);
            }
            //按照指定编码将字节数组变为字符串
            //string content = Encoding.UTF8.GetString(b);
            _outStr = Encoding.UTF8.GetString(b, 0, b.Length);
            //this.AddInfo(replyMesage);
        }

        /// <summary>
        /// 关闭客户端连接
        /// </summary>
        private void CloseClientSocket()
        {
            try
            {
                //m_socketClient.Client.Shutdown(SocketShutdown.Both);
                m_socketClient.Client.Close();
                m_socketClient.Close();
            }
            catch
            {
            }
            finally
            {
                m_socketClient = null;
            }
        }

        /// <summary>
        /// 启动蜂鸣器
        /// </summary>
        private void buz_on()
        {
            /*
            int m_iFreq = 2730;
            int m_iVolume = 60;
            int m_iMdelay = 100;
            int m_iBuzCtrlRe = -1;
            m_iBuzCtrlRe = NLSSysCtrl.buz_ctrl(m_iFreq, m_iVolume, m_iMdelay);
             * */

        }

        /// <summary>
        /// 退出程序
        /// </summary>
        public void Quit()
        {
            if (MessageBox.Show("是否退出?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                //Disconnect();
                //ShowTaskBar();
                _nowControlModule = null;
                Application.Exit();
            }
        }

        /// <summary>
        /// 焦点控制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IsLostFocus(object sender, EventArgs e)
        {
            if (_nowControlModule != null)
            {
                _nowControlModule.Focus();
            }
        }


        /// <summary>
        /// 生成程序清单
        /// </summary>
        /// <param name="name"></param>
        /// <param name="application_name"></param>
        /// <param name="version"></param>
        /// <param name="wince_path"></param>
        /// <param name="pc_path"></param>
        private void AddListviewItem(string name,string application_name,string version,string wince_path,string pc_path)
        {
            ListViewItem item = new ListViewItem();
            item.SubItems[0].Text="["+(listView1.Items.Count+1).ToString()+"]"+name;
            item.SubItems.Add(application_name);
            item.SubItems.Add(version);
            item.SubItems.Add(wince_path);
            item.SubItems.Add(pc_path);
            listView1.Items.Add(item);
            _applicationCount++;
        }

        /// <summary>
        /// 配置文件更新
        /// </summary>
        private void UpdateConfig()
        {
            
            textBox1.Text = "正在更新配置文件\r\n请等待。。。";
            tabControl1.SelectedIndex = tabControl1.TabPages.IndexOf(update);
            _nowControlModule = PROGRESS;
            PROGRESS.Focus();
            //if (m_socketClient != null)
            //{
                //this.Disconnect();
            //}
            /*
            bool isConnect=this.Connect();
            while (!isConnect)
            {
                if (_serverPort < _lastPort)
                {
                    _serverPort += 1;
                    
                    this.Disconnect();

                    isConnect = this.Connect();
                }
                else
                {
                    _serverPort = _firstPort;
                    MessageBox.Show("更新配置文件失败！");
                    tabControl1.SelectedIndex = tabControl1.TabPages.IndexOf(main);
                    _nowControlModule = main_focus;
                    main_focus.Focus();
                    return; 
                }
            }
             * */
            //this.Connect();
            
            //_codeStr = listView1.Items[0].SubItems[1].Text + "|" + listView1.Items[0].SubItems[2].Text;
            _codeStr = "UpdateConfig";
            _pFlag = 98;
            //SendOneDatagram();
            //NewTransmit();
            byte[] tmp = DownLoadFile();

            if (tmp != null)
            {
                try
                {

                    FileStream MyFileStream = new FileStream("\\Program Files\\CONFIG.XML", FileMode.Create, FileAccess.Write);
                    MyFileStream.Write(tmp, 0, tmp.Length);
                    MyFileStream.Close();
                    MessageBox.Show("更新完成！", "提示");

                }
                catch
                {
                    MessageBox.Show("更新失败！", "提示");
                }

            }
            else
            {
                MessageBox.Show("接收失败！", "提示");
            }
            tabControl1.SelectedIndex = tabControl1.TabPages.IndexOf(main);
            _nowControlModule = main_focus;
            main_focus.Focus();

            //todo update xml config
            /*
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.LoadXml(_outStr);
                //MessageBox.Show(_outStr,"xml");
                xml.Save("\\Program Files\\CONFIG.XML");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message,"错误");
                tabControl1.SelectedIndex = tabControl1.TabPages.IndexOf(main);
                _nowControlModule = main_focus;
                main_focus.Focus();
                return;
            }
           // MessageBox.Show("success","ok");

            listView1.Items.Clear();
            try
            {
                //openFileDialog1.ShowDialog();
                //string path = openFileDialog1.FileName;
                xml.Load("\\Program Files\\CONFIG.XML");
                XmlNodeList Application = xml.SelectSingleNode("Root/Applications").ChildNodes;
                foreach (XmlNode app in Application)
                {
                    //MessageBox.Show(app.SelectSingleNode("name").InnerText,"app");
                    AddListviewItem(app.SelectSingleNode("name").InnerText, app.Name, app.SelectSingleNode("version").InnerText
                        , app.SelectSingleNode("wince_path").InnerText, app.SelectSingleNode("pc_path").InnerText);

                }
                MessageBox.Show("更新完成！", "提示");

            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message, "错误");
                //UpdateConfig();
            }
            finally
            {
                if (m_socketClient != null)
                {
                    this.Disconnect();
                }
                tabControl1.SelectedIndex = tabControl1.TabPages.IndexOf(main);
                _nowControlModule = main_focus;
                main_focus.Focus();
            }
             * */
        }

        /// <summary>
        /// 接收文件数据
        /// </summary>
        /// <param name="s"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static byte[] ReceiveData(Socket s, int size)
        {
            int total = 0;
            int dataleft = size;
            byte[] data = new byte[size];
            int recv;
            while (total < size)
            {
                recv = s.Receive(data, total, dataleft, SocketFlags.None);
                if (recv == 0)
                {
                    data = null;
                    break;
                }

                total += recv;
                dataleft -= recv;
            }
            return data;
        }

        private void CheckAppVersion(int id)
        {
            _codeStr = id + "#" + listView1.Items[id].SubItems[2].Text;
            _pFlag = 96;
            NewTransmit();
            string[] data = _outStr.Split('#');
            if (data[0] == "UPDATE" && data.Length == 2)
            {
                _codeStr = id.ToString();
                _pFlag = 99;
                byte[] tmp = DownLoadFile();
                if (tmp != null)
                {
                    try
                    {

                        FileStream MyFileStream = new FileStream(listView1.Items[id].SubItems[3].Text, FileMode.Create, FileAccess.Write);
                        MyFileStream.Write(tmp, 0, tmp.Length);
                        MyFileStream.Close();

                        XmlDocument xml = new XmlDocument();
                        xml.Load("\\Program Files\\CONFIG.XML");
                        xml.SelectSingleNode("Root/Applications/" + listView1.Items[id].SubItems[1].Text + "/version").InnerText = data[1];
                        xml.Save("\\Program Files\\CONFIG.XML");
                        
                    }
                    catch
                    {
                    }


                }

            }
        }


        /// <summary>
        /// 更新程序
        /// </summary>
        private void UpdateApplication_veto(int id)
        {
            if (m_socketClient != null)
            {
                this.Disconnect();
            }
            this.Connect();
            _codeStr = listView1.Items[id].SubItems[1].Text + "|" + listView1.Items[id].SubItems[2].Text;
            //_codeStr = "System|2014122001";
            _pFlag = 99;
            //SendOneDatagram();
            NewTransmit();
            if (_outStr != "NEW")
            {
                //textBox1.Text = _outStr;
                if (_outStr == "")
                {
                    MessageBox.Show("获取更新信息失败！", "错误");
                    tabControl1.SelectedIndex = tabControl1.TabPages.IndexOf(main);
                    _nowControlModule = main_focus;
                    main_focus.Focus();
                    return;
                }
                string[] data = _outStr.Split('|');
                string appname = data[0];
                string fileName = data[1];
                int packetSize = int.Parse(data[2]);
                int packetCount = int.Parse(data[3]);
                int lastPacketSize = int.Parse(data[4]);
                //MessageBox.Show(_outStr,"outstr");
                textBox1.Text = "正在更新程序：\r\n    " + appname;
                FileStream MyFileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                int receiveCount = 0;
                int last = 0;
                if (lastPacketSize > 0)
                    last = 1;
                progressBar1.Maximum = last + packetCount;
                PROGRESS.Text = "0/" + progressBar1.Maximum;
                while (receiveCount < progressBar1.Maximum)
                {
                    try
                    {
                        if (receiveCount == packetCount)
                            packetSize = lastPacketSize;
                        //int len = m_socketClient.Client.Receive(m_receiveBuffer, 0, m_receiveBuffer.Length, SocketFlags.None);
                        try
                        {
                            byte[] receiveBuffer = ReceiveData(m_socketClient.Client, packetSize);
                            receiveCount++;
                            MyFileStream.Write(receiveBuffer, 0, packetSize);
                            progressBar1.Value = receiveCount;
                            PROGRESS.Text = "   " + receiveCount + "/" + progressBar1.Maximum;
                        }
                        catch (Exception eee)
                        {
                            MessageBox.Show("更新出错:" + eee.Message, "错误");
                            PROGRESS.Text = "0/0";
                            progressBar1.Value = 0;
                            break;
                        }
                        //if (len == 0)
                        //{
                        // MessageBox.Show("更新出错！", "提示");
                        //PROGRESS.Text = "0/0";
                        //progressBar1.Value = 0;
                        // break;
                        //}
                        //else
                        // {
                        //revdata=
                        //receiveCount++;
                        //Encoding.Convert(Encoding.ASCII,Encoding.UTF8,m_receiveBuffer);
                        //char[] cc = null;
                        //Convert.ToBase64CharArray(m_receiveBuffer,0,m_receiveBuffer.Length,cc,0);
                        //将接收到的数据包写入到文件流对象  
                        //byte[] byte_data = Encoding.UTF8.GetBytes(cc);
                        // MyFileStream.Write(m_receiveBuffer, 0, packetSize);
                        //progressBar1.Value = receiveCount;
                        //PROGRESS.Text = "   " + receiveCount + "/" + progressBar1.Maximum;
                        //显示已发送包的个数  
                        //MessageBox.Show("已发送包个数"+SendedCount.ToString());  
                        //}
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("更新出错: " + err.Message, "错误包" + receiveCount.ToString());
                        PROGRESS.Text = "0/0";
                        progressBar1.Value = 0;
                        break;
                    }
                }
                //关闭文件流  
                MyFileStream.Close();
                MessageBox.Show("更新完成！", "提示");
                PROGRESS.Text = "0/0";
                progressBar1.Value = 0;
            }
            else
            {
                MessageBox.Show("程序为最新版,不需要更新！", "提示");
            }
            if (m_socketClient != null)
            {
                this.Disconnect();
            }
            tabControl1.SelectedIndex = tabControl1.TabPages.IndexOf(main);
            _nowControlModule = main_focus;
            main_focus.Focus();

        }

        /// <summary>
        /// 主界面按键捕获
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void main_focus_KeyDown(object sender, KeyEventArgs e)
        {
            //MessageBox.Show(e.KeyValue.ToString(),"KEY_VALUE");
            //statusBar1.Text = e.KeyValue.ToString();

            switch (e.KeyCode)
            {
                case Keys.D0:
                    quitFlag = 0;
                    textBox1.Text = "\r\n按(1-9)更新程序\r\n按ESC返回主界面";
                    tabControl1.SelectedIndex = tabControl1.TabPages.IndexOf(update);
                    _nowControlModule = PROGRESS;
                    PROGRESS.Focus();
                    break;
                case Keys.D9:
                    quitFlag = 0;
                    tb_testMsg.BackColor = Color.Gray;
                    tb_testMsg.Text = "\r\n\r\n                测试界面\r\n\r\n          按1开始自动检测\r\n\r\n          按ESC返回主界面";
                    tabControl1.SelectedIndex = tabControl1.TabPages.IndexOf(test);
                    _nowControlModule = tb_test_focus;
                    _nowControlModule.Focus();
                    break;
                case Keys.Escape:
                    if(quitFlag>5 && quitFlag<90)
                    {
                        quitFlag = 0;
                        Quit();
                    }
                    else if (quitFlag > 90)
                    {
                        quitFlag = 0;
                    }
                    break;
                case Keys.Back:
                    if (quitFlag < 6)
                    {
                        quitFlag++;
                    }
                    break;
                case Keys.Space:
                    tabControl1.SelectedIndex = tabControl1.TabPages.IndexOf(test);
                    _nowControlModule = null;
                    break;

            }

            if ((e.KeyValue > 48) && (e.KeyValue < (49 + _applicationCount)))
            {
                string msg;
                if (!WifiCtrl.GetInstance().isConnectWifi(_IpAddress, out msg))
                {
                    _outStr = msg;
                    return;
                }
                if (quitFlag == 99)
                {
                    return;
                }
                quitFlag = 99;
                CheckAppVersion(e.KeyValue - 49);
                SwitchApp(e.KeyValue - 49);
            }
        }

        /// <summary>
        /// 切换程序
        /// </summary>
        /// <param name="id"></param>
        private void SwitchApp(int id)
        {
            //Disconnect();
            if (!ProcessCE.IsRunning(listView1.Items[id].SubItems[3].Text))
            {
                ProcessContext pi = new ProcessContext();
                ProcessCE.CreateProcess(listView1.Items[id].SubItems[3].Text,
                                  "", IntPtr.Zero,
                                  IntPtr.Zero, 0, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, pi);
                //Application.Exit();
            }
            
        }

        /// <summary>
        /// 更新界面操作选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PROGRESS_KeyDown(object sender, KeyEventArgs e)
        {
            ShowPower();
            if ((e.KeyValue > 48) && (e.KeyValue < (49 + _applicationCount)))
            {
                string msg;
                if (!WifiCtrl.GetInstance().isConnectWifi(_IpAddress, out msg))
                {
                    _outStr = msg;
                    return;
                }
                textBox1.Text = "正在更新程序...";
                listView1.Items[e.KeyValue - 49].SubItems[2].Text = "00000000";
                CheckAppVersion(e.KeyValue-49);
                MessageBox.Show("更新成功!");
                textBox1.Text = "\r\n按(1-9)更新程序\r\n按ESC返回主界面";
                tabControl1.SelectedIndex = tabControl1.TabPages.IndexOf(main);
                _nowControlModule = main_focus;
                main_focus.Focus();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                tabControl1.SelectedIndex = tabControl1.TabPages.IndexOf(main);
                _nowControlModule = main_focus;
                main_focus.Focus();
            }
            else if (e.KeyCode == Keys.D8)
            {
                UpdateConfig();
            }

        }

        /// <summary>
        /// 测试界面控制模块
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tb_test_focus_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    tabControl1.SelectedIndex = tabControl1.TabPages.IndexOf(main);
                    _nowControlModule = main_focus;
                    main_focus.Focus();
                    break;
                case Keys.D1:
                    Test();
                    break;
            }
        }

        private void Test()
        {
            WirelessNetworkInterface wni = WifiCtrl.GetInstance().GetWifiStatus();
            int count = 0;
            int rtt = 0;
            int rttTotal = 0;
            string tmp = "";
            string errMsg = "";
            tb_testMsg.BackColor = Color.Gray;
            tb_testMsg.Text = "\r\n  [1]开始检测Wifi...";
            if (wni.InterfaceOperationalStatus == InterfaceOperationalStatus.NonOperational)
            {
                tb_testMsg.Text = "\r\n\r\n                检测结果\r\n  连接不到Wifi!\r\n\r\n";
                tb_testMsg.BackColor = Color.Red;
                return;
            }
            else
            {
                tb_testMsg.Text += "连接正常\r\n     SSID:"+wni.AssociatedAccessPoint+"  信号:";
                switch (wni.SignalStrength.Strength)
                {
                    case StrengthType.NoSignal:
                        tb_testMsg.Text += "没信号";
                        errMsg = "Wifi信号弱;";
                        break;
                    case StrengthType.VeryLow:
                        tb_testMsg.Text += "极低";
                        errMsg = "Wifi信号弱;";
                        break;
                    case StrengthType.Low:
                        tb_testMsg.Text += "低";
                        break;
                    case StrengthType.Good:
                        tb_testMsg.Text += "好";
                        break;
                    case StrengthType.VeryGood:
                        tb_testMsg.Text += "很好";
                        break;
                    case StrengthType.Excellent:
                        tb_testMsg.Text += "极好";
                        break;
                }
            }
            tmp = "\r\n\r\n  [2]测试连接服务器...";
            tb_testMsg.Text += tmp;
            for (int i = 0; i < 10; i++)
            {
                if (WifiCtrl.GetInstance().Ping(_serverIP, out rtt))
                {
                    count++;
                    rttTotal += rtt;
                }
                else
                {
                    rttTotal += 500;
                }
            }
            tb_testMsg.Text += "连接正常\r\n     状态:";
            if (count > 7)
            {
                tb_testMsg.Text += "很好";
            }
            else if (count > 5 && count < 8)
            {
                tb_testMsg.Text += "一般";
            }
            else if (count > 2 && count < 6)
            {
                tb_testMsg.Text += "较慢";
                errMsg += "连接不稳定;";
            }
            else if (count < 3)
            {
                tb_testMsg.Text = "\r\n\r\n                检测结果\r\n\r\n\r\n  网络异常！\r\n\r\n";
                tb_testMsg.ForeColor = Color.Red;
                return;

            }
            tb_testMsg.Text += "\r\n     丢包[" + count + "/10] 延迟:" + rttTotal / 10 + "ms\r\n";
            if (count > 2)
            {
                tb_testMsg.Text += "\r\n  [3]测试通讯功能...";

                //if (m_socketClient != null)
                //{
                    //this.Disconnect();
               // }
                //this.Connect();

                _pFlag = 97;
                _codeStr = tb_testMsg.Text;
                //SendOneDatagram();
                NewTransmit();
                if (_outStr == "test")
                {
                    tb_testMsg.Text += "通讯正常\r\n";
                }
                else
                {
                    tb_testMsg.Text = "  \r\n\r\n                检测结果\r\n\r\n\r\n  与服务器通讯异常！\r\n\r\n";
                    tb_testMsg.BackColor = Color.Red;
                    return;
                }
                //if (m_socketClient != null)
                //{
                    //this.Disconnect();
               // }
            }
            int now_power = GetPower();
            if (now_power > 0 && now_power < 25)
            {
                errMsg += "电量低;";
            }

            if (errMsg == "")
            {
                tb_testMsg.Text += "\r\n  [4]测试结果:正常\r\n\r\n";
                tb_testMsg.BackColor = Color.Green;
            }
            else
            {
                tb_testMsg.BackColor = Color.Yellow;
                tb_testMsg.Text += "\r\n  [4]不理想,有以下问题:\r\n " + errMsg + "\r\n\r\n";
            }
            tb_testMsg.Text += "     按1重新检测\r\n     按ESC返回主界面";
        }
    }
}