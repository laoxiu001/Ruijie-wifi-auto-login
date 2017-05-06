using System;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Net.Sockets;
using Microsoft.Win32;
using System.Net.NetworkInformation;
using System.Threading;
using System.Collections;


namespace 校园网
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            /*
             软件加载时运行代码
             */
            textBox1.Text = Properties.Settings.Default.userName;
            textBox2.Text = Properties.Settings.Default.password;
            //checkBox必须放后面，防止登陆失败return之后无法读取正常数据
            checkBox1.Checked = Boolean.Parse(Properties.Settings.Default.start);
            checkBox2.Checked = Boolean.Parse(Properties.Settings.Default.autoLogin);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            saveChange();
            MessageBox.Show("恭喜，配置保存成功");
        }
        private void button1_Click(object sender, EventArgs e)
        {
            /*
             登陆按钮功能
             */
            string username, pwd, url, data, nasip = isInternet();
            if (notEmpty())
            {
                username = textBox1.Text;
                pwd = textBox2.Text;
                url = "http://222.179.99.144:8080/eportal/webGateModeV2.do?method=login&mac=0026c7609610&t=wireless-v2-plain";
                data = "&username=" + username + "&pwd=" + pwd + "&" + nasip;
                GET(url,data);
                if (isInternet()=="")
                {
                    //登陆成功
                    label4.Text = "恭喜，登陆成功";
                }else{
                    label4.Text = "很抱歉，登陆失败";
                }
            }

        }
        private void button2_Click(object sender, EventArgs e)
        {
            /*
             注销功能
             */
            if (notEmpty())
            {
                /*string url, userIndex = "", username, data;
                username = textBox1.Text;
                url = "http://222.179.99.144:8080/eportal/webGateModeV2.do?method=logout";
                if (comboBox1.Text == "赋棠苑")
                {
                    userIndex = "6263663064333734386366383133636466653533316163386636636636373466";
                }
                else if (comboBox1.Text == "现技中心")
                {
                    userIndex = "3365623733623038393364633762653130323864613465383332656165376335";
                }
                userIndex = userIndex + getIp() + numFormat(textBox1.Text);
                data = "&userIndex=" + userIndex;//GET提交参数
                GET(url, data);*/

                String url = "http://222.179.99.144:8080/eportal/userV2.do?method=offline";
                String data = "";
                GET(url, data);

                if (isInternet() != "")
                {
                    label4.Text = "恭喜，注销成功";
                }
                else {
                    label4.Text = "抱歉，注销失败";
                }
                
            }
        }
        public String GET(String url, String data)
        {
            /*
             GET封装方法
             */
            //创建Get请求
            String retString = "";
            try
            {
                url = url + data;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "text/html;charset=GBK";
                //接受返回来的数据
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                request.AllowAutoRedirect = false;
                StreamReader streamReader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));
                retString = streamReader.ReadToEnd();//返回值
                String statusCode = response.StatusCode.ToString();//返回状态
                streamReader.Close();
                stream.Close();
                response.Close();
                return retString;
            }
            catch (WebException we)
            {
                return we.Message;
            }
        }
        public Boolean notEmpty()
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("请输入正确的用户名");
                return false;
            }
            else if (textBox2.Text == "")
            {
                MessageBox.Show("请输入密码");
                return false;
            }
            return true;
        }
        public String getIp() {
            /*
             获取本地局域网IP封装方法
             */
            string a = "";
            IPAddress[] arrIPAddresses = Dns.GetHostAddresses(Dns.GetHostName());
                foreach (IPAddress ip in arrIPAddresses)
                {
                    if (ip.AddressFamily.Equals(AddressFamily.InterNetwork))//IPv4
                    {
                        if (ip.ToString().IndexOf("10.") > -1)
                        {
                            //字符串A中包含字符串B
                            a = ip.ToString();
                        }
                    }
                }
            /*
            将获取的本地局域网IP进行加密封装方法
            */
                return ipFormat(a);
         }
        public String numFormat(String number)
        {
            int count = number.Length;
            for (int x = 0; x < count; x++)
            {
                number = number.Insert(2 * x, "3");
            }

            number = "5f" + number;
            return number;
        }
        public String ipFormat(String ip)
        {
            ip = ip.Replace('.', 'e');
            int count = ip.Length;
            for (int x = 0; x < count; x++)
            {
                ip = ip.Insert(2 * x, "3");
            }
            ip = ip.Replace("3e", "2e");
            ip = "5f" + ip;
            return ip;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            /*
             设置软件开机自启
             */
            if (checkBox1.Checked == true)
            {
                Properties.Settings.Default.start = checkBox1.Checked.ToString();
                Properties.Settings.Default.Save();
                //获取程序执行路径..
                string starupPath = Application.ExecutablePath;
                //class Micosoft.Win32.RegistryKey. 表示Window注册表中项级节点,此类是注册表装.
                RegistryKey loca = Registry.LocalMachine;
                RegistryKey run = loca.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");

                try
                {
                    //SetValue:存储值的名称
                    run.SetValue("qidong", starupPath);
                    /// MessageBox.Show("已启用开机运行!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    loca.Close();
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else
            {
                Properties.Settings.Default.start = checkBox1.Checked.ToString();
                Properties.Settings.Default.Save();
                // MessageBox.Show("没有选中");
                //获取程序执行路径..
                string starupPath = Application.ExecutablePath;
                //class Micosoft.Win32.RegistryKey. 表示Window注册表中项级节点,此类是注册表装.
                RegistryKey loca = Registry.LocalMachine;
                RegistryKey run = loca.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");

                try
                {
                    //SetValue:存储值的名称
                    run.DeleteValue("qidong");
                    MessageBox.Show("已停止开机运行!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    loca.Close();
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }
        public void saveChange() {
            /*
                此为保存用户配置项封装方法
             */
            Properties.Settings.Default.userName = textBox1.Text;
            Properties.Settings.Default.password = textBox2.Text;
            Properties.Settings.Default.start = checkBox1.Checked.ToString();
            Properties.Settings.Default.autoLogin = checkBox2.Checked.ToString();
            Properties.Settings.Default.Save();
        }

        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            //鼠标经过显示密码
            textBox2.PasswordChar = '\0';
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            //鼠标离开隐藏密码
            textBox2.PasswordChar = '●';
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            /*
             启动软件自动登录功能
             */
            if (checkBox2.Checked == true)
            {
                button1.PerformClick();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            /*
             关于软件
             */
            MessageBox.Show("此为学习C#的第一个练手程序。\n可用于校园网wifi的登陆、注销。\n作者：HJL，时间：2017.2.28\nWin10下使用开机自启功能需要以管理员身份运行\n版本号：V1.0");
        }
        public String isInternet(){
            /*
            访问百度，如果成功返回空字符串；如果重定向跳转登陆界面，返回nasip
             */
            String url = "http://www.baidu.com";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=GBK";
            //接受返回来的数据
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            request.AllowAutoRedirect = false;
            StreamReader streamReader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));
            String retString = streamReader.ReadToEnd();//返回值
            streamReader.Close();
            stream.Close();
            response.Close();
            String[] spilt = retString.Split('&');
            foreach (String nasip in spilt)
            {
                if (nasip.IndexOf("nasip") != -1)
                {
                    return nasip;
                }
            }
            return "";
            
        }
    }
}
