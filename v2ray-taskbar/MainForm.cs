/*
 * 由SharpDevelop创建。
 * 用户： Le
 * 日期: 2015/10/20
 * 时间: 09:17
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;


namespace v2ray_taskbar
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
        private string m_taskbarName;
        private string m_v2rayName;

        private string m_v2ray_exe;
        private string m_v2ray_conf;

		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
            ReadV2rayPara();

            Process[] processcollection = Process.GetProcessesByName(m_taskbarName/*"v2ray-taskbar"*/);
			if (processcollection.Length >= 2) {
				MessageBox.Show("应用程序已经在运行中。。");
				this.notifyIconV2ray.Visible = false;
				Environment.Exit(1);
			} else {
				this.V2ray_Process();
				this.WatcherStrat();
			}
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		// 运行代理程序
		void V2ray_Process()
		{
			try {
				Process p = new Process();
                p.StartInfo.FileName = m_v2ray_exe/*"v2ray.exe"*/;
                p.StartInfo.Arguments = "-config "+m_v2ray_conf;

				p.StartInfo.UseShellExecute = false;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.CreateNoWindow = true;
				p.OutputDataReceived += new DataReceivedEventHandler((sender, e) => {
					if (!String.IsNullOrEmpty(e.Data)) {
						this.AppendText(e.Data + Environment.NewLine);
					}
				});
				p.Start();
				p.BeginOutputReadLine();
			} catch (Exception) {
				MessageBox.Show("请检查当前目录下是否有 v2ray.exe 程序。。");
				this.notifyIconV2ray.Visible = false;
				Environment.Exit(0);
			}
		}
		
		delegate void AppendTextDelegate(string text);
		void AppendText(string text)
		{
			if (this.textBoxTaskbar.InvokeRequired) {
				Invoke(new AppendTextDelegate(AppendText), new object[] { text });
			} else {
				this.textBoxTaskbar.AppendText(text);
			}
		}
		// 最小化隐藏
		void V2ray_SizeChanged(object sender, EventArgs e)
		{
			if (this.WindowState == FormWindowState.Minimized) {
				this.Hide();
				this.Visible = false;
			}
		}
		// 窗体显示
		void notifyIconV2ray_MouseClick(object sender, MouseEventArgs e)
		{
			if (this.Visible == false && e.Button == MouseButtons.Left) {
				this.Visible = true;
				this.WindowState = FormWindowState.Normal;
				this.Activate();
				this.textBoxTaskbar.SelectionStart = this.textBoxTaskbar.Text.Length;
				this.textBoxTaskbar.ScrollToCaret();
			} else if (e.Button == MouseButtons.Left) {
				this.Hide();
				this.Visible = false;
			}
		}
		// 退出
		void Exit_Click(object sender, EventArgs e)
		{
			this.notifyIconV2ray.Visible = false;
			try {
				Process[] killp = Process.GetProcessesByName(m_v2rayName/*"v2ray"*/);
				foreach (System.Diagnostics.Process p in killp) {
					p.Kill();
				}
				Environment.Exit(0);
			} catch (Exception) {
				Environment.Exit(0);
			}
		}
		// 重载后台程序
		void V2ray_Click(object sender, EventArgs e)
		{
			this.Reloaded();
		}
		// 重载
		void Reloaded()
		{
			this.textBoxTaskbar.Clear();
			if (this.Visible == false) {
				this.Visible = true;
				this.WindowState = FormWindowState.Normal;
				this.Activate();
			}
			try {
                Process[] killp = Process.GetProcessesByName(m_v2rayName/*"v2ray"*/);
				foreach (System.Diagnostics.Process p in killp) {
					p.Kill();
				}
			} finally {
				this.V2ray_Process();
			}
		}
		// 清空 textBoxV2ray 内容
		void TextBoxClear(object sender, EventArgs e)
		{
			this.textBoxTaskbar.Clear();
		}
		// 复制 textBoxV2ray 内容
		void TextBoxCopy(object sender, EventArgs e)
		{
			if (this.textBoxTaskbar.SelectedText != "") {
				Clipboard.SetDataObject(this.textBoxTaskbar.SelectedText);
			}
		}
		// 默认隐藏
		void V2ray_Shown(object sender, EventArgs e)
		{
			this.Hide();
			this.Visible = false;
		}
		// 监控文件修改
		void WatcherStrat()
		{
			FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = Path.GetDirectoryName(m_v2ray_conf);
            watcher.Filter = Path.GetFileName(m_v2ray_conf);
			watcher.NotifyFilter = NotifyFilters.LastWrite;
			watcher.SynchronizingObject = this;
			watcher.Changed += new FileSystemEventHandler(OnChanged);
			watcher.EnableRaisingEvents = true;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
		}
        //配置文件更改后的响应函数
		void OnChanged(object source, FileSystemEventArgs e)
		{
			try {
               this.Reloaded();
			} catch (Exception) {
			}
		}
        //读取运行参数,文件的路径都转换为绝对路径
        public void ReadV2rayPara()
        {
            //v2ray默认参数
            string curPath = Application.StartupPath;
            Directory.SetCurrentDirectory(curPath);
            m_v2ray_exe = curPath + "\\v2ray.exe";

            //从ini配置文件读取v2ray的运行参数
            string iniFullName = curPath + "\\v2ray-taskbar.ini";
            if (!File.Exists(iniFullName))
            {
                string msg = string.Format("当前目录缺少v2ray-taskbar配置文件：\"v2ray-taskbar.ini\"！");
                MessageBox.Show(msg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            IniOper iniOper = new IniOper(iniFullName);

            string cli= iniOper.ReadValue("config", "v2ray_exe");
            if (cli == "")//参数值为空表示使用默认参数
            {
                if (File.Exists(curPath + "\\v2ray.exe"))
                {
                    m_v2ray_conf = curPath + "\\v2ray.exe";
                }
                else
                {
                    string msg = string.Format("当前目录找不到v2ray执行文件：\"v2ray.exe\"，请在ini文件中指定正确的可执行文件！");
                    MessageBox.Show(msg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }
            }
            else//使用ini文件中指定的参数
            {
                string fullPath = Path.GetFullPath(cli);
                if (File.Exists(fullPath))
                {
                    m_v2ray_exe = fullPath;
                }
                else
                {
                    string msg = string.Format("找不到v2ray可执行文件。\n请检查ini文件中指定的v2ray可执行文件是否有效：\"{0}\"", fullPath);
                    MessageBox.Show(msg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }
            }

            string conf = iniOper.ReadValue("config", "v2ray_conf");
            if (conf == "")//参数值为空表示使用默认参数
            {
                if (File.Exists(curPath + "\\config.json"))
                {
                    m_v2ray_conf = curPath + "\\config.json";
                }
                else//使用ini文件中指定的参数
                {
                    string msg = string.Format("当前目录找不到v2ray的默认配置文件:\"config.json\"，请在ini文件中指定正确的配置文件！");
                    MessageBox.Show(msg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }

            }
            else
            {
                string fullPath = Path.GetFullPath(conf);
                if (File.Exists(fullPath))
                {
                    m_v2ray_conf = fullPath;
                }
                else
                {
                    string msg = string.Format("找不到v2ray的配置文件。\n请检查ini文件中指定的v2ray配置文件是否有效:\"{0}\"", fullPath);
                    MessageBox.Show(msg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }
            }

            string tbFullName = Application.ExecutablePath;
            m_taskbarName = Path.GetFileNameWithoutExtension(tbFullName);
            m_v2rayName = Path.GetFileNameWithoutExtension(m_v2ray_exe);
        }
	}
}
