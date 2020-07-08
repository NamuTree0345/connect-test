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
        public static string version = "0.3";

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

                succ.Checked = bool.Parse(txt.Split(' ')[4]);
                fail.Checked = bool.Parse(txt.Split(' ')[5]);
                attamp.Checked = bool.Parse(txt.Split(' ')[6]);
                showSec.Text = txt.Split(' ')[7];
            }

            ver.Text = string.Format("Connect Tester v{0} by NamuTree0345", version);
        }

        private void changeEnabled(bool enabled) {
            hour.Enabled = enabled;
            minute.Enabled = enabled;
            second.Enabled = enabled;

            site.Enabled = enabled;

            succ.Enabled = enabled;
            fail.Enabled = enabled;
            attamp.Enabled = enabled;
            showSec.Enabled = enabled;

            button2.Enabled = enabled;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            

            if (isRunning) { checkTimer.Stop(); isRunning = false; changeEnabled(true); button1.Text = "시작/설정 저장"; } else {
                try
                {
                    changeEnabled(false);
                    checkTimer.Interval = getMs(int.Parse(hour.Text), int.Parse(minute.Text), int.Parse(second.Text));
                    int.Parse(showSec.Text);
                    checkTimer.Start();
                    isRunning = true;
                    button1.Text = "종료";
                    File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\ConnectTest_Conf.txt", string.Format("{0} {1} {2} {3} {4} {5} {6} {7}", hour.Text, minute.Text, second.Text, site.Text, succ.Checked, fail.Checked, attamp.Checked, int.Parse(showSec.Text)));
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

        public static int getMs(int seconds) {
            return seconds * 1000;
        }

        private void checkTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                notifyIcon.BalloonTipTitle = "HTTP 연결 확인";
                notifyIcon.BalloonTipText = "연결 상태 확인중...";
                notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                if (attamp.Checked) notifyIcon.ShowBalloonTip(getMs(int.Parse(showSec.Text)));
                try
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(site.Text);
                    req.Method = "GET";
                    using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
                    {
                        notifyIcon.BalloonTipText = "연결 상태: " + resp.StatusCode;
                        if (succ.Checked) notifyIcon.ShowBalloonTip(getMs(int.Parse(showSec.Text)));
                    }
                }
                catch
                {
                    notifyIcon.BalloonTipIcon = ToolTipIcon.Error;
                    notifyIcon.BalloonTipText = "연결 상태: 오류!";
                    if (fail.Checked) notifyIcon.ShowBalloonTip(getMs(int.Parse(showSec.Text)));
                }
            }
            catch {
                notifyIcon.BalloonTipIcon = ToolTipIcon.Error;
                notifyIcon.BalloonTipText = "연결 상태: 오류!";
                if (fail.Checked) notifyIcon.ShowBalloonTip(getMs(int.Parse(showSec.Text)));
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
