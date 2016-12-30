using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NativeWifi;

namespace Test
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        ZInterface wifiInterface;
        public MainWindow()
        {
            InitializeComponent();
            wifiInterface = new ZInterface();
            wifiInterface.ScanSSID();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            //ApManager.Init();

        }
        //启动按钮
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string errmsg = "";
            //ApManager.CreatAndStart("zhangfan888", "12345678", ref errmsg);
            
            if (!ZInterface.ApControl.CreatAndStart(textBoxSsid.Text, textBoxPwd.Text, ref errmsg))
            {
                MessageBox.Show(errmsg);
            }
            else
            {
                MessageBox.Show("done!");
            }
        }

        //检测按钮
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string msg = "";
            ZInterface.ApControl.Show(ref msg);
            MessageBox.Show(msg);
        }

        //停止按钮
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            string errmsg = "";
            if (!ZInterface.ApControl.Stop(ref errmsg))
            {
                MessageBox.Show(errmsg);
            }
            else
            {
                MessageBox.Show("done!");
            }
        }

        //这个没用，响应窗口拖动事件的
        private void Border_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            //获取正在使用的设备，可能有多个设备同时联网
            List<String> strActiveDevice = ZInterface.GetActiveDevice();

            string temp = "";
            foreach (string device in strActiveDevice)
            {
                temp += device;
                temp += "\n";
            }
            MessageBox.Show(temp);
        }


        //获取可以连接的wifi列表
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            //ZInterface wifiInterface = new ZInterface();
            wifiInterface.ScanSSID();
            List<string> ssidList = wifiInterface.WifiSsids;


            string temp = "";
            foreach (string ssid in ssidList)
            {
                temp += ssid;
                temp += "\n";
            }
            MessageBox.Show(temp);
        }


        //获取当前连接的wifi
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {            
            string connectSsid = ZInterface.GetCurrentConnection();
            MessageBox.Show(connectSsid);
        }

        //连接无线网络
        private void Button_Click_7(object sender, RoutedEventArgs e)
        {

            //如果没有密码,第二个参数请传空字符串
            String errmsg = "";
            bool success = wifiInterface.ConnectWifi(connectId.Text,connectpwd.Text,ref errmsg);
            if (success)
            {
                MessageBox.Show("done!");
            }
            else
            {
                MessageBox.Show(errmsg);
            }

        }

        //查看网卡是否支持承载网络
        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(ZInterface.ApControl.IsNetworkCardSupport().ToString()); 
        }
    }
}
