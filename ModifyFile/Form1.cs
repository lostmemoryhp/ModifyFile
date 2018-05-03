using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModifyFile
{
    public partial class frmMain : Form
    {
        string dir;
        IList<IFileModifier> fileModifiers = new List<IFileModifier>();
        SynchronizationContext m_SyncContext = null;
        public frmMain()
        {
            InitializeComponent();
            //fileModifiers.Add(new ImageFileModifier());
            fileModifiers.Add(new PngFileModifier());
            m_SyncContext = SynchronizationContext.Current;
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtDir.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        void AppendText(String text)
        {
            m_SyncContext.Post(txt =>
            {
                this.txtInfo.AppendText(txt.ToString() + "\n");
            }, text);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            txtInfo.Clear();
            dir = txtDir.Text.Trim();
            if (string.IsNullOrEmpty(dir))
            {
                MessageBox.Show("请选择文件夹。");
                return;
            }
            EnabledControl(false);

            Task.Run(() =>
            {
                changeDir(dir);
            })
            .ContinueWith(t =>
            {
                m_SyncContext.Post(state =>
                {
                    EnabledControl(true);
                    MessageBox.Show("修改完成");
                }, null);
            });
        }

        void EnabledControl(bool enabled)
        {
            txtDir.Enabled = enabled;
            btnSelectFolder.Enabled = enabled;
            btnStart.Enabled = enabled;
        }

        void changeDir(string parentdir)
        {
            DirectoryInfo dir = new DirectoryInfo(parentdir);
            if (dir.Name.StartsWith(".") || dir.Name.ToLower() == "build")
            {
                AppendText("跳过====" + dir.FullName);
                return;
            }
            //SetText("目录====" + dir.FullName);
            var files = dir.EnumerateFiles().ToList();
            files.AsParallel().ForAll(f =>
            {
                foreach (var fm in fileModifiers)
                {
                    try
                    {
                        if (fm.canModify(f))
                        {
                            try
                            {
                                fm.modify(f);
                                AppendText("修改文件====" + f.FullName);
                                break;
                            }
                            catch (Exception ex)
                            {
                                AppendText("出错，" + f.FullName + " == " + ex.Message);
                            }

                        }
                    }
                    catch (Exception e)
                    {
                        AppendText("canModify出错，" + f.FullName + " == " + e.Message);
                    }

                }
            });
            var children = dir.EnumerateDirectories().ToList();
            foreach (var childDir in children)
            {
                changeDir(childDir.FullName);
            }
        }


    }
}
