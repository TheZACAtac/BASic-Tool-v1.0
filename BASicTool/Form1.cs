using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace BASicTool
{
    public partial class Form1 : Form
    {
        //These are the values of the entries when the file is loaded (or a new file is created)
        //Used to detect whether or not the user has made changes to the file
        float Entry1_Loaded = 0;
        float Entry2_Loaded = 0;
        float Entry3_Loaded = 0;
        float Entry4_Loaded = 0;
        float Entry5_Loaded = 0;

        //Name of loaded file
        string LoadFile = "Untitled";

        //These are the current values of the entries
        float Entry1 = 0;
        float Entry2 = 0;
        float Entry3 = 0;
        float Entry4 = 0;
        float Entry5 = 0;

        //Checks whether or not file has changed
        bool FileChanged()
        {
            return
                (Entry1 != Entry1_Loaded) |
                (Entry2 != Entry2_Loaded) |
                (Entry3 != Entry3_Loaded) |
                (Entry4 != Entry4_Loaded) |
                (Entry5 != Entry5_Loaded)
                ;
        }

        //Used for saving files
        void SaveFile(bool ForceSaveAs)
        {
            //Run save dialog (if neccessary)
            string savfile = "";
            if (!File.Exists(LoadFile) | ForceSaveAs)
            {
                SaveFileDialog SFile = new SaveFileDialog();
                SFile.InitialDirectory = "c:\\";
                SFile.Filter = "Basic AI Speed File (*.bas)|*.bas";
                SFile.FilterIndex = 1;
                SFile.RestoreDirectory = true;
                if (SFile.ShowDialog() == DialogResult.OK)
                {
                    savfile = SFile.FileName;
                }
            }
            else
            {
                savfile = LoadFile;
            }
            //Save file
            if (savfile!="")
            {
                try
                {
                    //Convert entries from float to byte array
                    byte[] Entry1_bytes = BitConverter.GetBytes(Entry1);
                    byte[] Entry2_bytes = BitConverter.GetBytes(Entry2);
                    byte[] Entry3_bytes = BitConverter.GetBytes(Entry3);
                    byte[] Entry4_bytes = BitConverter.GetBytes(Entry4);
                    byte[] Entry5_bytes = BitConverter.GetBytes(Entry5);
                    Array.Reverse(Entry1_bytes);
                    Array.Reverse(Entry2_bytes);
                    Array.Reverse(Entry3_bytes);
                    Array.Reverse(Entry4_bytes);
                    Array.Reverse(Entry5_bytes);
                    //Combine byte arrays
                    byte[] Savebytes = Entry1_bytes.Concat(Entry2_bytes).ToArray<byte>();
                    Savebytes = Savebytes.Concat(Entry3_bytes).ToArray<byte>();
                    Savebytes = Savebytes.Concat(Entry4_bytes).ToArray<byte>();
                    Savebytes = Savebytes.Concat(Entry5_bytes).ToArray<byte>();
                    //Save
                    File.WriteAllBytes(savfile, Savebytes);
                    LoadFile = savfile;
                    Entry1_Loaded = Entry1;
                    Entry2_Loaded = Entry2;
                    Entry3_Loaded = Entry3;
                    Entry4_Loaded = Entry4;
                    Entry5_Loaded = Entry5;
                    UpdateTitle();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error Saving File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //Used for opening files
        void OpenFile()
        {
            string opnfile = "";
            OpenFileDialog OFile = new OpenFileDialog();
            OFile.InitialDirectory = "c:\\";
            OFile.Filter = "Basic AI Speed File (*.bas)|*.bas";
            OFile.FilterIndex = 1;
            OFile.RestoreDirectory = true;
            if (OFile.ShowDialog() == DialogResult.OK)
            {
                opnfile = OFile.FileName;
            }
            if (opnfile!="")
            {
                byte[] Openbytes = File.ReadAllBytes(opnfile);
                if (Openbytes.Length==20)
                {
                    byte[] Entry1_bytes = { Openbytes[0], Openbytes[1], Openbytes[2], Openbytes[3] };
                    byte[] Entry2_bytes = { Openbytes[4], Openbytes[5], Openbytes[6], Openbytes[7] };
                    byte[] Entry3_bytes = { Openbytes[8], Openbytes[9], Openbytes[10], Openbytes[11] };
                    byte[] Entry4_bytes = { Openbytes[12], Openbytes[13], Openbytes[14], Openbytes[15] };
                    byte[] Entry5_bytes = { Openbytes[16], Openbytes[17], Openbytes[18], Openbytes[19] };
                    Array.Reverse(Entry1_bytes);
                    Array.Reverse(Entry2_bytes);
                    Array.Reverse(Entry3_bytes);
                    Array.Reverse(Entry4_bytes);
                    Array.Reverse(Entry5_bytes);
                    Entry1_Loaded = BitConverter.ToSingle(Entry1_bytes,0);
                    Entry2_Loaded = BitConverter.ToSingle(Entry2_bytes, 0);
                    Entry3_Loaded = BitConverter.ToSingle(Entry3_bytes, 0);
                    Entry4_Loaded = BitConverter.ToSingle(Entry4_bytes, 0);
                    Entry5_Loaded = BitConverter.ToSingle(Entry5_bytes, 0);
                    Entry1 = Entry1_Loaded;
                    Entry2 = Entry2_Loaded;
                    Entry3 = Entry3_Loaded;
                    Entry4 = Entry4_Loaded;
                    Entry5 = Entry5_Loaded;
                    TB_Entry1.Text = Entry1.ToString();
                    TB_Entry2.Text = Entry2.ToString();
                    TB_Entry3.Text = Entry3.ToString();
                    TB_Entry4.Text = Entry4.ToString();
                    TB_Entry5.Text = Entry5.ToString();
                    LoadFile = opnfile;
                    UpdateTitle();
                }
                else
                {
                    MessageBox.Show("File must be 20 bytes in size", "Error Loading File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //Puts a * at the end of the filename of the form title if the file has been changed
        void UpdateTitle()
        {
            if (FileChanged())
                Text = Path.GetFileName(LoadFile) + "*";
            else
                Text = Path.GetFileName(LoadFile);
        }

        public Form1()
        {
            InitializeComponent();

            TB_Entry1.Text = Entry1.ToString();
            TB_Entry2.Text = Entry2.ToString();
            TB_Entry3.Text = Entry3.ToString();
            TB_Entry4.Text = Entry4.ToString();
            TB_Entry5.Text = Entry5.ToString();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (FileChanged())
            {
                DialogResult = MessageBox.Show("Save changes to " + LoadFile + "?", "Warning",
             MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (DialogResult == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
                else
                {
                    if (DialogResult == DialogResult.Yes)
                    {
                        SaveFile(false);
                    }
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile(false);
        }

        private void TB_Entry1_Leave(object sender, EventArgs e)
        {
            TB_Entry1.Text = Entry1.ToString();
        }

        private void TB_Entry1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Entry1 = float.Parse(TB_Entry1.Text);
            }
            catch
            {

            }
            UpdateTitle();
        }

        private void TB_Entry2_Leave(object sender, EventArgs e)
        {
            TB_Entry2.Text = Entry2.ToString();
        }

        private void TB_Entry2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Entry2 = float.Parse(TB_Entry2.Text);
            }
            catch
            {

            }
            UpdateTitle();
        }

        private void TB_Entry3_Leave(object sender, EventArgs e)
        {
            TB_Entry3.Text = Entry3.ToString();
        }

        private void TB_Entry3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Entry3 = float.Parse(TB_Entry3.Text);
            }
            catch
            {

            }
            UpdateTitle();
        }

        private void TB_Entry4_Leave(object sender, EventArgs e)
        {
            TB_Entry4.Text = Entry4.ToString();
        }

        private void TB_Entry4_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Entry4 = float.Parse(TB_Entry4.Text);
            }
            catch
            {

            }
            UpdateTitle();
        }

        private void TB_Entry5_Leave(object sender, EventArgs e)
        {
            TB_Entry5.Text = Entry5.ToString();
        }

        private void TB_Entry5_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Entry5 = float.Parse(TB_Entry5.Text);
            }
            catch
            {

            }
            UpdateTitle();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile(true);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool reset = true;//Whether or not to reset the program
            if (FileChanged())
            {
                DialogResult = MessageBox.Show("Save changes to " + LoadFile + "?", "Warning",
             MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (DialogResult != DialogResult.Cancel)
                {
                    if (DialogResult == DialogResult.Yes)
                    {
                        SaveFile(false);
                    }
                }
                else
                {
                    reset = false;
                }
            }
            if (reset)
            {
                Entry1_Loaded = 0;
                Entry2_Loaded = 0;
                Entry3_Loaded = 0;
                Entry4_Loaded = 0;
                Entry5_Loaded = 0;
                Entry1 = 0;
                Entry2 = 0;
                Entry3 = 0;
                Entry4 = 0;
                Entry5 = 0;
                LoadFile = "Untitled";
                TB_Entry1.Text = Entry1.ToString();
                TB_Entry2.Text = Entry2.ToString();
                TB_Entry3.Text = Entry3.ToString();
                TB_Entry4.Text = Entry4.ToString();
                TB_Entry5.Text = Entry5.ToString();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool open = true;//Whether or not to open the file
            if (FileChanged())
            {
                DialogResult = MessageBox.Show("Save changes to " + LoadFile + "?", "Warning",
             MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (DialogResult != DialogResult.Cancel)
                {
                    if (DialogResult == DialogResult.Yes)
                    {
                        SaveFile(false);
                    }
                }
                else
                {
                    open = false;
                }
            }
            if (open)
            {
                OpenFile();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Creator: TheZACAtac\nVersion: v1.0\nWiki: wiki.tockdom.com/wiki/BASic_Tool", "About BASic Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
