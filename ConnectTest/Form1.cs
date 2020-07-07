using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConnectTest
{
    public partial class Form1 : Form
    {

        public static bool isRunning = false;
        public static string version = "0.2";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if(File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\ConnectTest_Conf.txt")) {
                string txt = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\ConnectTest_Conf.txt");

                hour.Text = txt.Split(' ')[0];
                minute.Text = txt.Split(' ')[1];
                second.Text = txt.Split(' ')[2];
                site.Text = txt.Split(' ')[3];
            }

            ver.Text = string.Format("Connect Tester v{0} by NamuTree0345", version);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            

            if (isRunning) { checkTimer.Stop();  isRunning = false; button1.Text = "시작/설정 저장"; } else {
                try
                {
                    
                    checkTimer.Interval = getMs(int.Parse(hour.Text), int.Parse(minute.Text), int.Parse(second.Text));
                    checkTimer.Start();
                    isRunning = true;
                    button1.Text = "종료";
                    File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\ConnectTest_Conf.txt", hour.Text + " " + minute.Text + " " + second.Text + " " + site.Text);
                }
                catch (FormatException)
                {
                    MessageBox.Show("시/분/초는 정수로 적어야 됩니다.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (IOException ex) {
                    MessageBox.Show("설정을 저장하는데 오류가 났습니다. 메시지: " + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static int getMs(int hour, int minute, int seconds) {
            return (seconds * 1000) + (minute * 60000) + (hour * 600000);
        }

        private void checkTimer_Tick(object sender, EventArgs e)
        {
            notifyIcon.BalloonTipTitle = "HTTP 연결 확인";
            notifyIcon.BalloonTipText = "연결 상태 확인중...";
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(site.Text);
                req.Method = "GET";
                using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
                {
                    notifyIcon.BalloonTipText = "연결 상태: " + resp.StatusCode;
                    notifyIcon.ShowBalloonTip(3000);
                }
            } catch {
                notifyIcon.BalloonTipIcon = ToolTipIcon.Error;
                notifyIcon.BalloonTipText = "연결 상태: 오류!";
                notifyIcon.ShowBalloonTip(3000);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\ConnectTest_Conf.txt"))
            {
                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\ConnectTest_Conf.txt");
                MessageBox.Show("설정을 초기화했습니다. 프로그램을 다시 시작해 주시면 적용됩니다.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else {
                MessageBox.Show("설정이 존재하지 않습니다.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
