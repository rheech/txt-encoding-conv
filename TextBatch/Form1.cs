using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using href.Utils;

namespace TextBatch
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            txtOutputFolder.Text = Microsoft.VisualBasic.Interaction.Environ("HOMEDRIVE") + @"\TextOutput";
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cboEncoding.SelectedIndex = 0;
            ConvList.Columns.Add("Source Files", 300);
            ConvList.Columns.Add("Type", 50);
            ConvList.Columns.Add("FileSize(KB)", 100);
            ConvList.Columns.Add("Status", 70);
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd;
            fbd = new FolderBrowserDialog();

            fbd.ShowNewFolderButton = false;
            fbd.Description = "Please choose a output folder:";

            DialogResult dr;
            dr = fbd.ShowDialog();
            
            if (dr == DialogResult.OK)
                txtOutputFolder.Text = fbd.SelectedPath;
        }

        private void tsOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd;
            ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Text file (*.txt)|*.txt|All files (*.*)|*.*";
            ofd.ShowDialog();

            for (int i = 0; i < ofd.FileNames.Length; i++)
            {
                ListViewItem lvt;
                System.IO.FileInfo fi;
                fi = new System.IO.FileInfo(ofd.FileNames[i]);

                string EncodingInfo = GetFileEncoding(ofd.FileNames[i]).ToString();

                if (EncodingInfo != "Error")
                {
                    lvt = ConvList.Items.Add(ofd.FileNames[i]);
                    lvt.SubItems.Add(EncodingInfo);
                    lvt.SubItems.Add(fi.Length.ToString());
                    lvt.SubItems.Add("");
                }
            }
        }

        public static string GetFileEncoding(string srcFile)
        {
            try
            {
                return EncodingTools.DetectInputCodepage(System.IO.File.ReadAllBytes(srcFile)).EncodingName;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error occured while reading file.\r\n" +
                    srcFile + "\r\n\r\n" + e.Message.ToString(),
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "Error";
            }
        }

        private void tsConvert_Click(object sender, EventArgs e)
        {
            if (ConvList.Items.Count == 0)
            {
                MessageBox.Show("Nothing to convert.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string FileName;
            byte[] bt;

            try
            {
                if (Microsoft.VisualBasic.FileSystem.Dir(txtOutputFolder.Text, Microsoft.VisualBasic.FileAttribute.Directory) == "")
                    Microsoft.VisualBasic.FileSystem.MkDir(txtOutputFolder.Text);

                for (int i = 0; i < ConvList.Items.Count; i++)
                {
                    FileName = ConvList.Items[i].SubItems[0].Text;
                    bt = System.IO.File.ReadAllBytes(FileName);

                    string btbt;
                    btbt = System.IO.File.ReadAllText(FileName, EncodingTools.DetectInputCodepage(bt));


                    System.IO.FileInfo fi;
                    fi = new System.IO.FileInfo(FileName);

                    System.IO.File.WriteAllText(txtOutputFolder.Text + "\\" + fi.Name, btbt, EncodingCombo);
                    ConvList.Items[i].SubItems[3].Text = "Succeed";
                }
            }
            catch
            {
                MessageBox.Show("Error occured while building a path", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Encoding EncodingCombo
        {
            get
            {
                switch (cboEncoding.SelectedItem.ToString())
                {
                    case "ANSI":
                        return System.Text.Encoding.Default;
                        break;
                    case "Unicode":
                        return System.Text.Encoding.Unicode;
                        break;
                    case "Unicode big endian":
                        return System.Text.Encoding.BigEndianUnicode;
                        break;
                    case "UTF-8":
                        return System.Text.Encoding.UTF8;
                        break;
                    default:
                        return System.Text.Encoding.Default;
                }
            }
        }

        private void tsClear_Click(object sender, EventArgs e)
        {
            ConvList.Items.Clear();
        }
    }
}