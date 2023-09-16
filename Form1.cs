using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Threading;
using System.IO;
using System.Management;


namespace RemoteStatus
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            nowConString = "Data Source=" + textBox1.Text + ";Initial Catalog = " + textBox4.Text + "; User Id = " + textBox2.Text + "; Password = " + textBox3.Text + ";";
            home = false;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
            }


        }
        public string currentCpu;
        public string currentFreeMem;
        public string currenttotalMem;
        public string dsaupdatetime;
        public bool home;
        public static string GetOSFriendlyName()
        {
            string result = string.Empty;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
            foreach (ManagementObject os in searcher.Get())
            {
                result = os["Caption"].ToString();
                break;
            }
            return result;
        }
        public void getCPU()
        {



            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                if (home == false)
                {
                    //PowerShellInstance.AddScript(@"wmic cpu get loadpercentage");
                    PowerShellInstance.AddScript(@"$proc =get-counter -Counter ""\Processor(_Total)\% Processor Time"" -SampleInterval 2;$cpu=($proc.readings -split "":"")[-1];$cpu | out-string");
                    Collection<PSObject> PSOutput = PowerShellInstance.Invoke();
                    foreach (PSObject outputItem0 in PSOutput)
                    {
                        if (outputItem0.ToString().Contains("0") | outputItem0.ToString().Contains("1") | outputItem0.ToString().Contains("2") | outputItem0.ToString().Contains("3") | outputItem0.ToString().Contains("4") | outputItem0.ToString().Contains("5") | outputItem0.ToString().Contains("6") | outputItem0.ToString().Contains("7") | outputItem0.ToString().Contains("8") | outputItem0.ToString().Contains("9"))
                        {
                            string be = outputItem0.BaseObject.ToString();
                            if (be.Contains("."))
                            {
                                ks = be.Split('.');
                            }
                            else if (be.Contains(","))
                            {
                                ks = be.Split(',');
                            }
                            string h = ks[1];
                            const int MaxLength = 2;
                            if (h.Length > MaxLength)
                                h = h.Substring(0, MaxLength); // name = "Chris"
                            string je = ks[0] + "." + h;

                            currentCpu = je.Replace(" ", "").Replace("{", "").Replace("}", "").Replace(Environment.NewLine, "");
                        }
                    }
                }

                if (currentCpu == null || currentCpu == "" || home == true)
                {
                    home = true;
                    PowerShellInstance.AddScript(@"wmic cpu get loadpercentage");
                    //PowerShellInstance.AddScript(@"$proc =get-counter -Counter ""\Processor(_Total)\% Processor Time"" -SampleInterval 2;$cpu=($proc.readings -split "":"")[-1];$cpu | out-string");
                    Collection<PSObject> PSOutput1 = PowerShellInstance.Invoke();
                    foreach (PSObject outputItem1 in PSOutput1)
                    {
                        if (outputItem1.ToString().Contains("0") | outputItem1.ToString().Contains("1") | outputItem1.ToString().Contains("2") | outputItem1.ToString().Contains("3") | outputItem1.ToString().Contains("4") | outputItem1.ToString().Contains("5") | outputItem1.ToString().Contains("6") | outputItem1.ToString().Contains("7") | outputItem1.ToString().Contains("8") | outputItem1.ToString().Contains("9"))
                        {
                            string be1 = outputItem1.BaseObject.ToString();
                            //if (be.Contains("."))
                            //{
                            //    ks = be.Split('.');
                            //}
                            //else if (be.Contains(","))
                            //{
                            //    ks = be.Split(',');
                            //}
                            //string h = ks[1];
                            //const int MaxLength = 2;
                            //if (h.Length > MaxLength)
                            //    h = h.Substring(0, MaxLength); // name = "Chris"
                            //string je = ks[0] + "." + h;

                            currentCpu = be1.Replace(" ", "").Replace("{", "").Replace("}", "").Replace(Environment.NewLine, "");
                        }
                    }
                }
            }
        }
        public string[] ks;
        public string isvm;
        public void IsVm()
        {


            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                PowerShellInstance.AddScript("(gwmi Win32_BaseBoard).Manufacturer -eq \"Microsoft Corporation\"");
                Collection<PSObject> PSOutput = PowerShellInstance.Invoke();
                foreach (PSObject outputItem in PSOutput)
                {

                    isvm = outputItem.BaseObject.ToString();
                }
            }
        }
        public void getFreeMem()
        {
            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                PowerShellInstance.AddScript(@"Get-WmiObject Win32_OperatingSystem | fl *freeph* | Out-String");
                Collection<PSObject> PSOutput = PowerShellInstance.Invoke();
                foreach (PSObject outputItem in PSOutput)
                {
                    if (outputItem.BaseObject.ToString().Contains("FreePhysicalMemory"))
                    {
                        string bop = outputItem.BaseObject.ToString().Replace("\r\n", "").Replace(" ", "").Replace("FreePhysicalMemory:", "");
                        Int32 pot = Int32.Parse(bop);
                        pot = pot / 1000;
                        currentFreeMem = pot.ToString();
                    }
                }
            }
        }
        public void getuptime()
        {
            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                PowerShellInstance.AddScript(@"systeminfo | find /i ""Boot Time"" | Out-String");
                Collection<PSObject> PSOutput = PowerShellInstance.Invoke();
                foreach (PSObject outputItem in PSOutput)
                {
                    string s = outputItem.ToString().Replace("System Boot Time:          ", "").Replace(",", "").Replace("\r\n", "");
                    TimeSpan ts = DateTime.Now.Subtract(DateTime.Parse(s));
                    string uptimehour = ts.TotalHours.ToString().Replace(":", ".");
                    string value = uptimehour.Replace(",", ".");
                    dsaupdatetime = value;
                }
            }
        }
        public void getTotMem()
        {
            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                PowerShellInstance.AddScript("(Get-WMIObject Win32_PhysicalMemory | Measure-Object Capacity -Sum).sum / 1mb | out-string");
                Collection<PSObject> PSOutput = PowerShellInstance.Invoke();
                foreach (PSObject outputItem in PSOutput)
                {
                    if (outputItem.BaseObject != null)
                    {
                        currenttotalMem = outputItem.BaseObject.ToString().Replace("\r\n", "");
                    }
                }
            }
        }
        public string drives;
        public void Driveinf()
        {
            drives = "";
            foreach (var item in DriveInfo.GetDrives())
            {
                if (item.IsReady == true && item.DriveType == DriveType.Fixed)
                {
                    string dskinf = item.Name + "=" + item.TotalFreeSpace + @"/" + item.TotalSize + "#";
                    drives = drives + dskinf;
                }


            }
        }
        public void Sum()
        {
            Int32 x = Int32.Parse(currentFreeMem);
            Int32 z = Int32.Parse(currenttotalMem);
            int h = z - x;
            currentUsageMem = h.ToString();
        }
        public string nowConString;
        public List<Server> sds;
        public void OldStat()
        {
            try
            {
                List<string> Stats = new List<string>();
                sds = new List<Server>();
                SqlConnection SQLCon = new SqlConnection(nowConString);
                SQLCon.Open();
                SqlCommand SQLCmd = new SqlCommand("SELECT * FROM stats WHERE Computer='" + Environment.MachineName + "';", SQLCon);
                SqlDataReader reader = SQLCmd.ExecuteReader();
                while (reader.Read())
                {


                    string Computer = reader["Computer"].ToString();
                    string CPU = reader["CPU"].ToString();
                    string RAM = reader["RAM"].ToString();
                    string FreeRAM = reader["FreeRAM"].ToString();
                    string RAMInUse = reader["RAMInUse"].ToString();
                    string Uptime = reader["Uptime"].ToString();
                    string Updated = reader["Updated"].ToString();
                    string OS = reader["OS"].ToString();
                    string IsVM = reader["IsVM"].ToString();
                    string drvs = reader["Drives"].ToString();

                    Stats.Add(Computer);
                    Stats.Add(CPU);
                    Stats.Add(RAM);
                    Stats.Add(FreeRAM);
                    Stats.Add(RAMInUse);
                    Stats.Add(Uptime);
                    Stats.Add(Updated);
                    Stats.Add(OS);
                    Stats.Add(IsVM);
                    Stats.Add(drvs);

                    Server dsadas = new Server(Computer, CPU, RAM, FreeRAM, RAMInUse, Uptime, Updated, IsVM, OS, drvs);
                    sds.Add(dsadas);

                }

                reader.Close();
                SQLCon.Close();
            }
            catch (Exception e)
            {
                notifyIcon1.BalloonTipText = "Error: " + e.ToString();
                notifyIcon1.ShowBalloonTip(60000);
            }
        }
        public string currentUsageMem;
        public List<string> Stats;
        public void DelStat()
        {
            try
            {
                SqlConnection SQLCon = new SqlConnection(nowConString);
                SQLCon.Open();
                SqlCommand SQLCmd = new SqlCommand("DELETE FROM stats WHERE Computer = '" + Environment.MachineName + "'; ", SQLCon);
                SqlDataReader reader = SQLCmd.ExecuteReader();
                while (reader.Read())
                {
                    Thread.Sleep(10);
                }
                reader.Close();
                SQLCon.Close();
            }
            catch (Exception e)
            {
                notifyIcon1.BalloonTipText = "Error: " + e.ToString();
                notifyIcon1.ShowBalloonTip(600);
            }
        }
        public string ue;
        public string[] ks1;
        public void NewStat()
        {
            try
            {
                //TimeSpan uptime;
                //string[] uptimehours;
                //uptime = TimeSpan.FromSeconds(Environment.TickCount / 1000);
                //string uptimehour = uptime.TotalHours.ToString().Replace(":", ".");
                //string value = uptimehour.Replace(",", ".");
                string be = dsaupdatetime;

                if (be.Contains("."))
                {
                    ks1 = be.Split('.');
                }
                else if (be.Contains(","))
                {
                    ks1 = be.Split(',');
                }
                string h = ks1[1];
                const int MaxLength = 2;
                if (h.Length > MaxLength)
                    h = h.Substring(0, MaxLength); // name = "Chris"
                string ue = ks1[0] + "." + h;


                //string h = uptimehours[1];
                //const int MaxLength = 2;
                //if (h.Length > MaxLength)
                //    h = h.Substring(0, MaxLength); // name = "Chris"
                //ue = uptimehours[0] + "." + h;
                SqlConnection SQLCon = new SqlConnection(nowConString);
                SQLCon.Open();
                string command = "INSERT INTO stats (Computer,CPU,RAM,FreeRAM,RAMInUse,Uptime,Updated,IsVM,OS,Drives) VALUES ('" + Environment.MachineName + "','" + currentCpu + "','" + currenttotalMem + "','" + currentFreeMem + "','" + currentUsageMem + "','" + ue + "','" + DateTime.Now.ToString() + "','" + isvm + "','" + GetOSFriendlyName() + "','" + drives + "');";

                command = command.Replace(":", ".");
                SqlCommand SQLCmd = new SqlCommand(command, SQLCon);
                SqlDataReader reader = SQLCmd.ExecuteReader();
                while (reader.Read())
                {
                    Thread.Sleep(10);
                }
                reader.Close();
                SQLCon.Close();
            }
            catch (Exception e)
            {
                notifyIcon1.BalloonTipText = "Error: " + e.ToString();
                notifyIcon1.ShowBalloonTip(600);
            }

            richTextBox1.Invoke((MethodInvoker)delegate { richTextBox1.Text = "Memmory Usage=" + currentUsageMem + "/" + currenttotalMem + "mb" + Environment.NewLine + "CPU Usage=" + currentCpu + "%" + Environment.NewLine + "Uptime=" + ue + " Hours" + Environment.NewLine + "Update=" + DateTime.Now.ToString(); });


        }
        public string[] ks2;
        public void SqlUpdate()
        {
            try
            {
                //TimeSpan uptime;
                //string[] uptimehours;
                //uptime = TimeSpan.FromSeconds(Environment.TickCount / 1000);
                //string uptimehour = uptime.TotalHours.ToString().Replace(":", ".");
                //string value = uptimehour.Replace(",", ".");
                string be = dsaupdatetime;

                if (be.Contains("."))
                {
                    ks2 = be.Split('.');
                }
                else if (be.Contains(","))
                {
                    ks2 = be.Split(',');
                }
                string h = ks2[1];
                const int MaxLength = 2;
                if (h.Length > MaxLength)
                    h = h.Substring(0, MaxLength); // name = "Chris"
                string ue = ks2[0] + "." + h;


                //string h = uptimehours[1];
                //const int MaxLength = 2;
                //if (h.Length > MaxLength)
                //    h = h.Substring(0, MaxLength); // name = "Chris"
                //ue = uptimehours[0] + "." + h;
                SqlConnection SQLCon = new SqlConnection(nowConString);
                SQLCon.Open();
                string command = "UPDATE stats set Drives='" + drives + "',CPU='" + currentCpu + "',RAM='" + currenttotalMem + "',FreeRAM='" + currentFreeMem + "',RAMInUse='" + currentUsageMem + "',Uptime='" + ue + "',Updated='" + DateTime.Now.ToString() + "' WHERE Computer='" + Environment.MachineName + "';";

                command = command.Replace(":", ".");
                SqlCommand SQLCmd = new SqlCommand(command, SQLCon);
                SqlDataReader reader = SQLCmd.ExecuteReader();
                while (reader.Read())
                {
                    Thread.Sleep(10);
                }
                reader.Close();
                SQLCon.Close();
            }
            catch (Exception e)
            {
                notifyIcon1.BalloonTipText = "Error: " + e.ToString();
                notifyIcon1.ShowBalloonTip(600);
            }

            richTextBox1.Invoke((MethodInvoker)delegate { richTextBox1.Text = "Memmory Usage=" + currentUsageMem + "/" + currenttotalMem + "mb" + Environment.NewLine + "CPU Usage=" + currentCpu + "%" + Environment.NewLine + "Uptime=" + ue + " Hours" + Environment.NewLine + "Update=" + DateTime.Now.ToString(); });
        }
        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.ToString() == "Stop")
            {
                timer1.Stop();
                notifyIcon1.Text = "Status: Stopped";
            }
            if (e.ClickedItem.ToString() == "Start")
            {
                timer1.Start();
                notifyIcon1.Text = "Status: Running";
            }
            if (e.ClickedItem.ToString() == "Exit")
            {
                this.Close();
            }
            if (e.ClickedItem.ToString() == "Hide")
            {
                this.Hide();
                ShowInTaskbar = false;

            }
            if (e.ClickedItem.ToString() == "Show")
            {
                this.Show();
                ShowInTaskbar = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            nowConString = "Data Source=" + textBox1.Text + ";Initial Catalog = " + textBox4.Text + "; User Id = " + textBox2.Text + "; Password = " + textBox3.Text + ";";
            timer1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            try
            {


                getuptime();

                OldStat();
                Driveinf();
                getCPU();
                IsVm();
                getFreeMem();
                getTotMem();
                Sum();
                if (sds.Count == 1)
                {
                    SqlUpdate();
                }
                else
                {
                    DelStat();
                    NewStat();
                }

            }
            catch (Exception f)
            {
                notifyIcon1.BalloonTipText = "Error: " + f.ToString();
                notifyIcon1.ShowBalloonTip(600);
            }

        }
    }
}
