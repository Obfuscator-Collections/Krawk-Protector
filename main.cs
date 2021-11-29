using MaterialSkin;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using dnlib.DotNet;
using dnlib.DotNet.Writer;
using Microsoft.Win32;
using Krawk_Protector.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Threading;
using MaterialSkin.Controls;
using Krawk.Helpers;
using dnlib.DotNet.Emit;
using System.Security.Cryptography;

namespace Krawk_Protector
{
    public partial class KrawkProtector : Form
    {
        #region Form
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        public KrawkProtector()
        {
            InitializeComponent();
            MaterialSkinManager materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Yellow700, Primary.Yellow700,
                Primary.Yellow700, Accent.Yellow700,
                TextShade.WHITE
            );
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 5, 5));
        }
        private List<string> assemblys = new List<string>();
        private Dictionary<string, string> dict = new Dictionary<string, string>();
        private void Progress()
        {
            for (int i = 0; i < this.materialProgressBar1.Maximum; i++)
            {
                MaterialProgressBar materialProgressBar = this.materialProgressBar1;
                int value = materialProgressBar.Value;
                materialProgressBar.Value = value + 1;
            }
        }
        private void KrawkProtector_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;

            AllowDrop = true;
            label2.Text = "Protector v1.5";
        }
        static string getFile = "";
        private void KrawkProtector_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files) getFile = file;
            materialSingleLineTextField1.Text = getFile;
            if (IsValidAssembly(materialSingleLineTextField1.Text) == true) { materialLabel1.Text = "Is .NET Assembly: Yes"; } else { materialLabel1.Text = "Is .NET Assembly: No"; }
        }
        private void KrawkProtector_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }
        private void materialCheckBox8_CheckedChanged(object sender, EventArgs e)
        {
            if (materialCheckBox8.Checked) { enable_debug = true; } else { enable_debug = false; }
        }
        private void KrawkProtector_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #endregion
        #region Regras
        private bool enable_Constants = false;
        private bool enable_calli = false;
        private bool enable_cflow = false;
        private bool enable_debug = false;
        private bool enable_renamer = false;
        private bool enable_ildasm = false;
        private bool rename_assembly = false;
        private bool antide4dot = false;
        private bool refproxy = false;
        #endregion
        #region checkboxes
        private void materialCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (materialCheckBox1.Checked) { enable_Constants = true; } else { enable_Constants = false; }
        }
        private void materialCheckBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (materialCheckBox7.Checked) { enable_renamer = true; } else { enable_renamer = false; }
        }
        private void materialCheckBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (materialCheckBox6.Checked) { enable_ildasm = true; } else { enable_ildasm = false; }
        }
        private void materialCheckBox10_CheckedChanged(object sender, EventArgs e)
        {
            if (materialCheckBox10.Checked) { refproxy = true; } else { refproxy = false; }
        }
        private void materialCheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (materialCheckBox3.Checked) { rename_assembly = true; } else { rename_assembly = false; }
        }
        private void materialCheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (materialCheckBox2.Checked) { antide4dot = true; } else { antide4dot = false; }
        }
        private void materialCheckBox9_CheckedChanged(object sender, EventArgs e)
        {
            if (materialCheckBox9.Checked) { enable_cflow = true; } else { enable_cflow = false; }
        }
        private void materialCheckBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (materialCheckBox4.Checked) { enable_calli = true; } else { enable_calli = false; }
        }
        private void label2_Click(object sender, EventArgs e)
        {

        }
        #endregion
        public static bool IsValidAssembly(string path)
        {
            try
            {
                var assembly = AssemblyName.GetAssemblyName(path);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
      
        private void materialRaisedButton2_Click(object sender, EventArgs e)
        {
            materialProgressBar1.Value = 0;
            Thread thread = new Thread(new ThreadStart(this.Progress));
            thread.IsBackground = true;
            thread.Start();
            if (materialLabel1.Text == "Is .NET Assembly: No") { MessageBox.Show("Not a valid dotnet assembly!", "K Protector"); return; }
            try
            {
                AssemblyDef asm = AssemblyDef.Load(materialSingleLineTextField1.Text);
                Context krawk = new Context(asm);
                var rule = new Rule(enable_Constants, enable_calli, enable_cflow, enable_debug, enable_renamer, rename_assembly, antide4dot, refproxy);
                new Krawk_Protector.Utils.Core(krawk, rule).DoObfuscation();
                var opts = new NativeModuleWriterOptions(krawk.ManifestModule as ModuleDefMD);
                opts = Writer.Run(krawk);
                if (materialSingleLineTextField1.Text.Contains(".exe")) { (asm.ManifestModule as ModuleDefMD).NativeWrite(materialSingleLineTextField1.Text.Replace(".exe", "_Obf.exe"), opts); }
                if (materialSingleLineTextField1.Text.Contains(".dll")) { (asm.ManifestModule as ModuleDefMD).NativeWrite(materialSingleLineTextField1.Text.Replace(".dll", "_Obf.dll"), opts); }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog load = new OpenFileDialog();
            load.Filter = "Exe|*.exe|Dll|*.dll";
            if (load.ShowDialog() == DialogResult.OK)
            {
                materialSingleLineTextField1.Text = load.FileName;
            }
            if (IsValidAssembly(materialSingleLineTextField1.Text) == true) { materialLabel1.Text = "Is .NET Assembly: Yes"; } else { materialLabel1.Text = "Is .NET Assembly: No"; }
        }
 
        
    }
}



