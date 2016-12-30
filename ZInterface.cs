using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows;

namespace NativeWifi
{
    public class ZInterface
    {
        #region 字段
        private List<WIFI> wifi;
        private List<String> wifisids;
        private Dictionary<string,WIFI> wifidic = new Dictionary<string, WIFI>();
        private string connectedSsid;
        #endregion


        //可连接的wifi实例列表
        public List<WIFI> Wifi
        {
            get
            {
                return wifi;
            }

            set
            {
                wifi = value;
            }
        }
        //wifi的ssid列表
        public List<string> WifiSsids
        {
            get
            {
                return wifisids;
            }

            set
            {
                wifisids = value;
            }
        }

        //已经连接的ssid
        public string ConnectedSsid
        {
            get
            {
                return connectedSsid;
            }

            set
            {
                connectedSsid = value;
            }
        }

        public Dictionary<string, WIFI> Wifidic
        {
            get
            {
                return wifidic;
            }

            set
            {
                wifidic = value;
            }
        }

        public ZInterface()
        {
            Wifi = new List<WIFI>();
            WifiSsids = new List<string>();

        }


        static string GetStringForSSID(Wlan.Dot11Ssid ssid)
        {
            return Encoding.UTF8.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }

        /// <summary>  
        /// 枚举所有无线设备接收到的SSID  
        /// </summary>  
        public void ScanSSID()
        {
            wifi.Clear();
            wifisids.Clear();
            Wifidic.Clear();
            WlanClient client = new WlanClient();
            foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
            {
                // Lists all networks with WEP security  
                Wlan.WlanAvailableNetwork[] networks = wlanIface.GetAvailableNetworkList(0);
                foreach (Wlan.WlanAvailableNetwork network in networks)
                {
                    WIFI targetSSID = new WIFI();

                    targetSSID.wlanInterface = wlanIface;
                    targetSSID.wlanSignalQuality = (int)network.wlanSignalQuality;
                    targetSSID.SSID = GetStringForSSID(network.dot11Ssid);
                    //targetSSID.SSID = Encoding.Default.GetString(network.dot11Ssid.SSID, 0, (int)network.dot11Ssid.SSIDLength);  
                    targetSSID.dot11DefaultAuthAlgorithm = network.dot11DefaultAuthAlgorithm.ToString();
                    targetSSID.dot11DefaultCipherAlgorithm = network.dot11DefaultCipherAlgorithm.ToString();
                    wifi.Add(targetSSID);
                    WifiSsids.Add(GetStringForSSID(network.dot11Ssid));
                    if (!Wifidic.ContainsKey(GetStringForSSID(network.dot11Ssid)))
                    {
                        string id = GetStringForSSID(network.dot11Ssid);
                        Wifidic.Add(GetStringForSSID(network.dot11Ssid), targetSSID);
                    }

                    if(network.flags == (Wlan.WlanAvailableNetworkFlags.Connected | Wlan.WlanAvailableNetworkFlags.HasProfile))
                    //if (network.flags == (Wlan.WlanAvailableNetworkFlags.Connected))
                    {
                        Console.WriteLine("bingo!");
                        ConnectedSsid = GetStringForSSID(network.dot11Ssid);

                    }

                    //if ( network.dot11DefaultCipherAlgorithm == Wlan.Dot11CipherAlgorithm.WEP )  
                    //{  
                    //    Console.WriteLine( "Found WEP network with SSID {0}.", GetStringForSSID(network.dot11Ssid));  
                    //}  
                    //Console.WriteLine("Found network with SSID {0}.", GetStringForSSID(network.dot11Ssid));  
                    //Console.WriteLine("dot11BssType:{0}.", network.dot11BssType.ToString());  
                    //Console.WriteLine("dot11DefaultAuthAlgorithm:{0}.", network.dot11DefaultAuthAlgorithm.ToString());  
                    //Console.WriteLine("dot11DefaultCipherAlgorithm:{0}.", network.dot11DefaultCipherAlgorithm.ToString());  
                    //Console.WriteLine("dot11Ssid:{0}.", network.dot11Ssid.ToString());  

                    //Console.WriteLine("flags:{0}.", network.flags.ToString());  
                    //Console.WriteLine("morePhyTypes:{0}.", network.morePhyTypes.ToString());  
                    //Console.WriteLine("networkConnectable:{0}.", network.networkConnectable.ToString());  
                    //Console.WriteLine("numberOfBssids:{0}.", network.numberOfBssids.ToString());  
                    //Console.WriteLine("profileName:{0}.", network.profileName.ToString());  
                    //Console.WriteLine("wlanNotConnectableReason:{0}.", network.wlanNotConnectableReason.ToString());  
                    //Console.WriteLine("wlanSignalQuality:{0}.", network.wlanSignalQuality.ToString());  
                    //Console.WriteLine("-----------------------------------");  
                    // Console.WriteLine(network.ToString());  
                }
            }
        } // EnumSSID  

        public static string GetCurrentConnection()
        {
            WlanClient client = new WlanClient();
            foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
            {
                Wlan.WlanAvailableNetwork[] networks = wlanIface.GetAvailableNetworkList(0);
                foreach (Wlan.WlanAvailableNetwork network in networks)
                {
                    if (wlanIface.InterfaceState == Wlan.WlanInterfaceState.Connected && wlanIface.CurrentConnection.isState == Wlan.WlanInterfaceState.Connected)
                    {
                        return wlanIface.CurrentConnection.profileName;
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>  
        /// 连接到未加密的SSID  
        /// </summary>  
        /// <param name="ssid"></param>  
        public void ConnectToSSID(WIFI ssid)
        {
            // Connects to a known network with WEP security  
            string profileName = ssid.SSID; // this is also the SSID  

            string mac = StringToHex(profileName); //   

            //string key = "";  
            //string profileXml = string.Format("<?xml version=\"1.0\"?><WLANProfile xmlns=\"http://www.microsoft.com/networking/WLAN/profile/v1\"><name>{0}</name><SSIDConfig><SSID><hex>{1}</hex><name>New{0}</name></SSID></SSIDConfig><connectionType>ESS</connectionType><MSM><security><authEncryption><authentication>open</authentication><encryption>none</encryption><useOneX>false</useOneX></authEncryption><sharedKey><keyType>networkKey</keyType><protected>false</protected><keyMaterial>{2}</keyMaterial></sharedKey><keyIndex>0</keyIndex></security></MSM></WLANProfile>", profileName, mac, key);  
            //string profileXml2 = "<?xml version=\"1.0\"?><WLANProfile xmlns=\"http://www.microsoft.com/networking/WLAN/profile/v1\"><name>Hacker SSID</name><SSIDConfig><SSID><hex>54502D4C494E4B5F506F636B657441505F433844323632</hex><name>TP-LINK_PocketAP_C8D262</name></SSID>        </SSIDConfig>        <connectionType>ESS</connectionType><connectionMode>manual</connectionMode><MSM> <security><authEncryption><authentication>open</authentication><encryption>none</encryption><useOneX>false</useOneX></authEncryption></security></MSM></WLANProfile>";  
            //wlanIface.SetProfile( Wlan.WlanProfileFlags.AllUser, profileXml2, true );  
            //wlanIface.Connect( Wlan.WlanConnectionMode.Profile, Wlan.Dot11BssType.Any, profileName );  
            string myProfileXML = string.Format("<?xml version=\"1.0\"?><WLANProfile xmlns=\"http://www.microsoft.com/networking/WLAN/profile/v1\"><name>{0}</name><SSIDConfig><SSID><hex>{1}</hex><name>{0}</name></SSID></SSIDConfig><connectionType>ESS</connectionType><connectionMode>manual</connectionMode><MSM><security><authEncryption><authentication>open</authentication><encryption>none</encryption><useOneX>false</useOneX></authEncryption></security></MSM></WLANProfile>", profileName, mac);
            ssid.wlanInterface.SetProfile(Wlan.WlanProfileFlags.AllUser, myProfileXML, true);
            ssid.wlanInterface.Connect(Wlan.WlanConnectionMode.Profile, Wlan.Dot11BssType.Any, profileName);
            //Console.ReadKey();  
        }


        /*这是一个连接有密码的ssid的
        public void ConnectToSSID(WIFI ssid, string key)
        {
            try
            {
                String auth = string.Empty;
                String cipher = string.Empty;
                bool isNoKey = false;
                String keytype = string.Empty;
                switch (ssid.dot11DefaultAuthAlgorithm)
                {
                    case Wlan.Dot11AuthAlgorithm.IEEE80211_Open:
                        auth = "open"; break;
                    //case Wlan.Dot11AuthAlgorithm.IEEE80211_SharedKey:   
                    // 'not implemented yet;   
                    //break;   
                    case Wlan.Dot11AuthAlgorithm.RSNA:
                        auth = "WPA2PSK"; break;
                    case Wlan.Dot11AuthAlgorithm.RSNA_PSK:
                        auth = "WPA2PSK"; break;
                    case Wlan.Dot11AuthAlgorithm.WPA:
                        auth = "WPAPSK"; break;
                    case Wlan.Dot11AuthAlgorithm.WPA_None:
                        auth = "WPAPSK"; break;
                    case Wlan.Dot11AuthAlgorithm.WPA_PSK:
                        auth = "WPAPSK"; break;
                }

                switch (ssid.dot11DefaultCipherAlgorithm)
                {
                    case Wlan.Dot11CipherAlgorithm.CCMP:
                        cipher = "AES";
                        keytype = "passPhrase";
                        break;
                    case Wlan.Dot11CipherAlgorithm.TKIP:
                        cipher = "TKIP";
                        keytype = "passPhrase";
                        break;
                    case Wlan.Dot11CipherAlgorithm.None:
                        cipher = "none"; keytype = "";
                        isNoKey = true;
                        break;
                    case Wlan.Dot11CipherAlgorithm.WEP:
                        cipher = "WEP";
                        keytype = "networkKey";
                        break;
                    case Wlan.Dot11CipherAlgorithm.WEP40:
                        cipher = "WEP";
                        keytype = "networkKey";
                        break;
                    case Wlan.Dot11CipherAlgorithm.WEP104:
                        cipher = "WEP";
                        keytype = "networkKey";
                        break;
                }

                if (isNoKey && !string.IsNullOrEmpty(key))
                {
                    infoTB.Text = "无法连接网络！";
                    Loger.WriteLog("无法连接网络",
                        "SSID:" + this.ssid.SSID + "\r\n"
                        + "Dot11AuthAlgorithm:" + ssid.dot11DefaultAuthAlgorithm + "\r\n"
                        + "Dot11CipherAlgorithm:" + ssid.dot11DefaultAuthAlgorithm.ToString());
                    return;
                }
                else if (!isNoKey && string.IsNullOrEmpty(key))
                {
                    infoTB.Text = "无法连接网络！";
                    Loger.WriteLog("无法连接网络",
                        "SSID:" + this.ssid.SSID + "\r\n"
                        + "Dot11AuthAlgorithm:" + ssid.dot11DefaultAuthAlgorithm + "\r\n"
                        + "Dot11CipherAlgorithm:" + ssid.dot11DefaultAuthAlgorithm.ToString());
                    return;
                }
                else
                {
                    string profileName = ssid.profileNames; // this is also the SSID   
                    string mac = StringToHex(profileName);
                    string profileXml = string.Empty;
                    if (!string.IsNullOrEmpty(key))
                    {
                        profileXml = string.Format("<?xml version=\"1.0\"?><WLANProfile xmlns=\"http://www.microsoft.com/networking/WLAN/profile/v1\"><name>{0}</name><SSIDConfig><SSID><hex>{1}</hex><name>{0}</name></SSID></SSIDConfig><connectionType>ESS</connectionType><connectionMode>auto</connectionMode><autoSwitch>false</autoSwitch><MSM><security><authEncryption><authentication>{2}</authentication><encryption>{3}</encryption><useOneX>false</useOneX></authEncryption><sharedKey><keyType>{4}</keyType><protected>false</protected><keyMaterial>{5}</keyMaterial></sharedKey><keyIndex>0</keyIndex></security></MSM></WLANProfile>",
                            profileName, mac, auth, cipher, keytype, key);
                    }
                    else
                    {
                        profileXml = string.Format("<?xml version=\"1.0\"?><WLANProfile xmlns=\"http://www.microsoft.com/networking/WLAN/profile/v1\"><name>{0}</name><SSIDConfig><SSID><hex>{1}</hex><name>{0}</name></SSID></SSIDConfig><connectionType>ESS</connectionType><connectionMode>auto</connectionMode><autoSwitch>false</autoSwitch><MSM><security><authEncryption><authentication>{2}</authentication><encryption>{3}</encryption><useOneX>false</useOneX></authEncryption></security></MSM></WLANProfile>",
                            profileName, mac, auth, cipher, keytype);
                    }

                    ssid.wlanInterface.SetProfile(Wlan.WlanProfileFlags.AllUser, profileXml, true);
                    //ssid.wlanInterface.Connect(Wlan.WlanConnectionMode.Profile, Wlan.Dot11BssType.Any, ssid.profileNames);  
                    bool success = ssid.wlanInterface.ConnectSynchronously(Wlan.WlanConnectionMode.Profile, Wlan.Dot11BssType.Any, profileName, 15000);
                    if (!success)
                    {
                        infoTB.Text = "连接网络失败！";
                        Loger.WriteLog("连接网络失败",
                            "SSID:" + this.ssid.SSID + "\r\n"
                            + "Dot11AuthAlgorithm:" + ssid.dot11DefaultAuthAlgorithm + "\r\n"
                            + "Dot11CipherAlgorithm:" + ssid.dot11DefaultAuthAlgorithm.ToString() + "\r\n");
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                infoTB.Text = "无法连接网络！" + e.Message;
                Loger.WriteLog("无法连接网络",
                    "SSID:" + this.ssid.SSID + "\r\n"
                    + "Dot11AuthAlgorithm:" + ssid.dot11DefaultAuthAlgorithm + "\r\n"
                    + "Dot11CipherAlgorithm:" + ssid.dot11DefaultAuthAlgorithm.ToString() + "\r\n"
                    + e.Message);
                return;
            }
        }
        */
        public bool ConnectToSSID(WIFI ssid, string key , ref string errmsg)
        {
            try
            {
                String auth = string.Empty;
                String cipher = string.Empty;
                bool isNoKey = false;
                String keytype = string.Empty;
                Wlan.Dot11AuthAlgorithm switchcase = (Wlan.Dot11AuthAlgorithm)Enum.Parse(typeof(Wlan.Dot11AuthAlgorithm), ssid.dot11DefaultAuthAlgorithm);
                switch (switchcase)
                {
                    case Wlan.Dot11AuthAlgorithm.IEEE80211_Open:
                        auth = "open"; break;
                    //case Wlan.Dot11AuthAlgorithm.IEEE80211_SharedKey:   
                    // 'not implemented yet;   
                    //break;   
                    case Wlan.Dot11AuthAlgorithm.RSNA:
                        auth = "WPA2PSK"; break;
                    case Wlan.Dot11AuthAlgorithm.RSNA_PSK:
                        auth = "WPA2PSK"; break;
                    case Wlan.Dot11AuthAlgorithm.WPA:
                        auth = "WPAPSK"; break;
                    case Wlan.Dot11AuthAlgorithm.WPA_None:
                        auth = "WPAPSK"; break;
                    case Wlan.Dot11AuthAlgorithm.WPA_PSK:
                        auth = "WPAPSK"; break;
                }

                Wlan.Dot11CipherAlgorithm switchcase2 = (Wlan.Dot11CipherAlgorithm)Enum.Parse(typeof(Wlan.Dot11CipherAlgorithm), ssid.dot11DefaultCipherAlgorithm);
                switch (switchcase2)
                {
                    case Wlan.Dot11CipherAlgorithm.CCMP:
                        cipher = "AES";
                        keytype = "passPhrase";
                        break;
                    case Wlan.Dot11CipherAlgorithm.TKIP:
                        cipher = "TKIP";
                        keytype = "passPhrase";
                        break;
                    case Wlan.Dot11CipherAlgorithm.None:
                        cipher = "none"; keytype = "";
                        isNoKey = true;
                        break;
                    case Wlan.Dot11CipherAlgorithm.WEP:
                        cipher = "WEP";
                        keytype = "networkKey";
                        break;
                    case Wlan.Dot11CipherAlgorithm.WEP40:
                        cipher = "WEP";
                        keytype = "networkKey";
                        break;
                    case Wlan.Dot11CipherAlgorithm.WEP104:
                        cipher = "WEP";
                        keytype = "networkKey";
                        break;
                }

                
                
                    string profileName = ssid.SSID; // this is also the SSID   
                    string mac = StringToHex(profileName);
                    string profileXml = string.Empty;
                    if (!string.IsNullOrEmpty(key))
                    {
                        profileXml = string.Format("<?xml version=\"1.0\"?><WLANProfile xmlns=\"http://www.microsoft.com/networking/WLAN/profile/v1\"><name>{0}</name><SSIDConfig><SSID><hex>{1}</hex><name>{0}</name></SSID></SSIDConfig><connectionType>ESS</connectionType><connectionMode>auto</connectionMode><autoSwitch>false</autoSwitch><MSM><security><authEncryption><authentication>{2}</authentication><encryption>{3}</encryption><useOneX>false</useOneX></authEncryption><sharedKey><keyType>{4}</keyType><protected>false</protected><keyMaterial>{5}</keyMaterial></sharedKey><keyIndex>0</keyIndex></security></MSM></WLANProfile>",
                            profileName, mac, auth, cipher, keytype, key);
                    }
                    else
                    {
                        profileXml = string.Format("<?xml version=\"1.0\"?><WLANProfile xmlns=\"http://www.microsoft.com/networking/WLAN/profile/v1\"><name>{0}</name><SSIDConfig><SSID><hex>{1}</hex><name>{0}</name></SSID></SSIDConfig><connectionType>ESS</connectionType><connectionMode>auto</connectionMode><autoSwitch>false</autoSwitch><MSM><security><authEncryption><authentication>{2}</authentication><encryption>{3}</encryption><useOneX>false</useOneX></authEncryption></security></MSM></WLANProfile>",
                            profileName, mac, auth, cipher, keytype);
                    }

                    ssid.wlanInterface.SetProfile(Wlan.WlanProfileFlags.AllUser, profileXml, true);
                    //ssid.wlanInterface.Connect(Wlan.WlanConnectionMode.Profile, Wlan.Dot11BssType.Any, ssid.profileNames);  
                    bool success = ssid.wlanInterface.ConnectSynchronously(Wlan.WlanConnectionMode.Profile, Wlan.Dot11BssType.Any, profileName, 15000);
                    if (!success)
                    {
                        errmsg = "连接网络失败"+
                            "SSID:" + ssid.SSID + "\r\n"
                            + "Dot11AuthAlgorithm:" + ssid.dot11DefaultAuthAlgorithm + "\r\n"
                            + "Dot11CipherAlgorithm:" + ssid.dot11DefaultAuthAlgorithm.ToString() + "\r\n";

                        return false;                    
                }
                else
                {
                    return true;
                }
                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }
        }





        /*写错了
        public void ConnectToSSID(WIFI ssid, string key)
        {
            try
            {
                String auth = string.Empty;
                String cipher = string.Empty;
                bool isNoKey = false;
                String keytype = string.Empty;
                Wlan.Dot11AuthAlgorithm switchcase = (Wlan.Dot11AuthAlgorithm)Enum.Parse(typeof(Wlan.Dot11AuthAlgorithm), ssid.dot11DefaultAuthAlgorithm);
                //int switchcase = Int32.Parse(ssid.dot11DefaultAuthAlgorithm);
                switch (switchcase)
                {
                    case 1:
                        auth = "open"; break;
                    //case Wlan.Dot11AuthAlgorithm.IEEE80211_SharedKey:   
                    // 'not implemented yet;   
                    //break;   
                    case 3:
                        auth = "WPA2PSK"; break;
                    case 4:
                        auth = "WPA2PSK"; break;
                    case 5:
                        auth = "WPAPSK"; break;
                    case 6:
                        auth = "WPAPSK"; break;
                    case 7:
                        auth = "WPAPSK"; break;
                }
                switchcase = Int32.Parse(ssid.dot11DefaultCipherAlgorithm);
                switch (switchcase)
                {
                    case 0x04:
                        cipher = "AES";
                        keytype = "passPhrase";
                        break;
                    case 0x02:
                        cipher = "TKIP";
                        keytype = "passPhrase";
                        break;
                    case 0x00:
                        cipher = "none"; keytype = "";
                        isNoKey = true;
                        break;
                    case 0x101:
                        cipher = "WEP";
                        keytype = "networkKey";
                        break;
                    case 0x01:
                        cipher = "WEP";
                        keytype = "networkKey";
                        break;
                    case 0x05:
                        cipher = "WEP";
                        keytype = "networkKey";
                        break;
                }

                
                
                    string profileName = ssid.SSID; // this is also the SSID   
                    string mac = StringToHex(profileName);
                    string profileXml = string.Empty;
                    if (!string.IsNullOrEmpty(key))
                    {
                        profileXml = string.Format("<?xml version=\"1.0\"?><WLANProfile xmlns=\"http://www.microsoft.com/networking/WLAN/profile/v1\"><name>{0}</name><SSIDConfig><SSID><hex>{1}</hex><name>{0}</name></SSID></SSIDConfig><connectionType>ESS</connectionType><connectionMode>auto</connectionMode><autoSwitch>false</autoSwitch><MSM><security><authEncryption><authentication>{2}</authentication><encryption>{3}</encryption><useOneX>false</useOneX></authEncryption><sharedKey><keyType>{4}</keyType><protected>false</protected><keyMaterial>{5}</keyMaterial></sharedKey><keyIndex>0</keyIndex></security></MSM></WLANProfile>",
                            profileName, mac, auth, cipher, keytype, key);
                    }
                    else
                    {
                        profileXml = string.Format("<?xml version=\"1.0\"?><WLANProfile xmlns=\"http://www.microsoft.com/networking/WLAN/profile/v1\"><name>{0}</name><SSIDConfig><SSID><hex>{1}</hex><name>{0}</name></SSID></SSIDConfig><connectionType>ESS</connectionType><connectionMode>auto</connectionMode><autoSwitch>false</autoSwitch><MSM><security><authEncryption><authentication>{2}</authentication><encryption>{3}</encryption><useOneX>false</useOneX></authEncryption></security></MSM></WLANProfile>",
                            profileName, mac, auth, cipher, keytype);
                    }

                    ssid.wlanInterface.SetProfile(Wlan.WlanProfileFlags.AllUser, profileXml, true);
                    //ssid.wlanInterface.Connect(Wlan.WlanConnectionMode.Profile, Wlan.Dot11BssType.Any, ssid.profileNames);  
                    bool success = ssid.wlanInterface.ConnectSynchronously(Wlan.WlanConnectionMode.Profile, Wlan.Dot11BssType.Any, profileName, 15000);
                    if (!success)
                    {
                        
                        MessageBox.Show("连接网络失败",
                            "SSID:" + ssid.SSID + "\r\n"
                            + "Dot11AuthAlgorithm:" + ssid.dot11DefaultAuthAlgorithm + "\r\n"
                            + "Dot11CipherAlgorithm:" + ssid.dot11DefaultAuthAlgorithm.ToString() + "\r\n");
                        return;
                    }
                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());

            }
        }
        */


        
        public void ConnectWifi(string id)
        {
            try
            {
                WIFI ssid = Wifidic[id];
                ConnectToSSID(ssid);
            }catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            

        }

        public bool ConnectWifi(string id,string key,ref string errmsg)
        {
            try
            {
                WIFI ssid = Wifidic[id];
                bool success = ConnectToSSID(ssid ,key,ref errmsg);
                if (!success)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
                //MessageBox.Show(e.ToString());
            }


        }

        /// <summary>  
        /// 字符串转Hex  
        /// </summary>  
        /// <param name="str"></param>  
        /// <returns></returns>  
        public static string StringToHex(string str)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = System.Text.Encoding.Default.GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)  
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(Convert.ToString(byStr[i], 16));
            }

            return (sb.ToString().ToUpper());
        }


       

        //获取已经联网的设备
        public static List<string> GetActiveDevice()
        {
            List<string> connectAdaptor = new List<string>();

            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                Process process = new Process();
                process.StartInfo.FileName = "C:\\Windows\\System32\\cmd";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.StandardInput.WriteLine("netsh interface show interface name=" + "\"" + adapter.Name + "\"");
                process.StandardInput.WriteLine("exit");
                process.WaitForExit();
                string temp = process.StandardOutput.ReadToEnd();
                Console.WriteLine(temp);
                if (temp.Contains("已连接"))
                {
                    Console.WriteLine(adapter.Name);                                        
                    connectAdaptor.Add(adapter.Name);
                }
                

            }
            return connectAdaptor;
        }

        #region 承载网络部分
        public static class ApControl
        {
            public static bool Create(string id, string pwd, ref string error)
            {


                if ((id == "") || (pwd == ""))
                {
                    error = "用户名和密码不能为空";
                    return false;
                }
                else if (pwd.Length < 8)
                {
                    error = "密码不能少于8位";
                    return false;
                }
                else
                {
                    string command = "netsh wlan set hostednetwork mode=allow ssid=" + id + " key=" + pwd;
                    string str2 = executeCmd(command);
                    if (((str2.IndexOf("承载网络模式已设置为允许") > -1) && (str2.IndexOf("已成功更改承载网络的 SSID。") > -1)) && (str2.IndexOf("已成功更改托管网络的用户密钥密码。") > -1))
                    {
                        return true;
                    }
                    else
                    {
                        error = "创建失败:" + str2;
                        return false;
                    }
                }
                //return 0;
            }

            //启动承载网络
            public static bool Start(ref string error)
            {
                string str2 = executeCmd("netsh wlan start hostednetwork");
                if (str2.IndexOf("已启动承载网络") > -1)
                {
                    return true;
                }
                else
                {
                    error = "启动失败" + str2;
                    return false;
                }
            }

            //创建并开始
            public static bool CreatAndStart(string id, string pwd, ref string error)
            {
                if (IsNetworkCardSupport() == false)
                {
                    error = "抱歉，您的网卡不支持承载网络";
                    return false;
                }


                if (!Create(id, pwd, ref error))
                {
                    return false;
                }
                else
                {
                    if (!Start(ref error))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    };
                }

            }

            //停止承载网络
            public static bool Stop(ref string error)
            {
                string str2 = executeCmd("netsh wlan stop hostednetwork");
                if (str2.IndexOf("已停止承载网络") > -1)
                {
                    return true;
                }
                else
                {
                    error = "停止失败" + str2;
                    return false;
                }
            }

            //查看承载网路状态
            public static bool Show(ref string msg)
            {
                string str2 = executeCmd("netsh wlan show hostednetwork");
                msg = str2;
                if (str2.IndexOf("承载网络") > -1)
                {

                    return true;
                }
                else
                {
                    return false;
                }
            }


            public static bool IsNetworkCardSupport()
            {
                string str = executeCmd("netsh wlan show drivers");
                if (str.IndexOf("支持的承载网络  : 是") > -1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            //工具函数，开启新进程执行cmd命令，并且返回控制台打印出来的信息
            private static string executeCmd(string Command)
            {
                Process process = new Process
                {
                    StartInfo = { FileName = " cmd.exe ", UseShellExecute = false, RedirectStandardInput = true, RedirectStandardOutput = true, CreateNoWindow = true }
                };
                process.Start();
                process.StandardInput.WriteLine(Command);
                process.StandardInput.WriteLine("exit");
                process.WaitForExit();
                string str = process.StandardOutput.ReadToEnd();
                process.Close();
                return str;
            }
        }

    }
    #endregion

    public class WIFI
    {
        public string SSID = "NONE";
        public string dot11DefaultAuthAlgorithm = "";
        public string dot11DefaultCipherAlgorithm = "";
        public bool networkConnectable = true;
        public string wlanNotConnectableReason = "";
        public int wlanSignalQuality = 0;
        public WlanClient.WlanInterface wlanInterface = null;
    }

    
}

