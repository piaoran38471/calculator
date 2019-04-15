using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Windows.Data;
namespace SYD_COPY_FILE
{
    public partial class Form1
    {
        public void copy_file_init()
        {
            this.label_copy_time.Text = "拷贝文件时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.label_nowtime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.label_nowtime.ForeColor = Color.Blue;
            this.timer1.Interval = 1000;
            this.timer1.Start();

            comboBox_timetype_rename.SelectedIndex = 0;
        }
        private void update_state(string name)
        {

            this.label_copy_time.Text = "拷贝文件时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");  //UP
            if (this.label_copy_time.ForeColor == Color.Blue)
                this.label_copy_time.ForeColor = Color.Red;
            else if (this.label_copy_time.ForeColor == Color.Red)
                this.label_copy_time.ForeColor = Color.Black;
            else if (this.label_copy_time.ForeColor == Color.Black)
                this.label_copy_time.ForeColor = Color.Blue;
            else
                this.label_copy_time.ForeColor = Color.Blue;

            this.Text = name;
        }

        private void update_state_rename(string name)
        {

            this.label_copy_time.Text = "重命名/拷贝文件时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");  //UP
            if (this.label_copy_time.ForeColor == Color.Blue)
                this.label_copy_time.ForeColor = Color.Red;
            else if (this.label_copy_time.ForeColor == Color.Red)
                this.label_copy_time.ForeColor = Color.Black;
            else if (this.label_copy_time.ForeColor == Color.Black)
                this.label_copy_time.ForeColor = Color.Blue;
            else
                this.label_copy_time.ForeColor = Color.Blue;

            this.Text = name;
        }

        private void source_copyfile_button_Click(object sender, EventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.FileName = "source_file";

           // dlg.DefaultExt = ".txt";

            dlg.Filter = "All files（*.*）|*.*|All files(*.*)|*.* ";

            if (dlg.ShowDialog() == false)
                return;

            if (sender == source_copyfile_button)
                this.source_copyfile_textBox.Text = dlg.FileName;
            else if (sender == source_copyfile_button_sync)
                this.source_copyfile_textBox_sync.Text = dlg.FileName;
            else if (sender == destination_file_button_sync)
                this.destination_file_textBox_sync.Text = dlg.FileName;
            else if (sender == destination_file_button_copy_sourcefile_sync)
                this.destination_file_textBox_two_sync.Text = dlg.FileName;
            else if (sender == button_copy_destinationfileend_sync)
                this.textBox_copy_destinationfileend_sync.Text = dlg.FileName;
            else if (sender == destination_file_button_copy_sourcefile)
                this.destination_file_textBox_two.Text = dlg.FileName;
            else if (sender == button_copy_destinationfileend)
                this.textBox_copy_destinationfileend.Text = dlg.FileName;
        }
        private void source_copyfile_textBox_DragEnter(object sender, DragEventArgs e)
        {
            IDataObject dataObject = e.Data;
            if (dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }
        private void source_copyfile_textBox_DragDrop(object sender, DragEventArgs e)
        {
            IDataObject dataObject = e.Data;

            if (dataObject == null) return;

            if (dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])dataObject.GetData(DataFormats.FileDrop);

                foreach (string file in files)
                {
                    System.IO.FileInfo fi = new System.IO.FileInfo(file);
                    if (sender == source_copyfile_textBox)
                        this.source_copyfile_textBox.Text = fi.FullName;
                    else if (sender == destination_file_textBox)
                        this.destination_file_textBox.Text = fi.FullName;
                    else if (sender == destination_file_textBox_two)
                        this.destination_file_textBox_two.Text = fi.FullName;
                    else if (sender == textBox_copy_destinationfileend)
                        this.textBox_copy_destinationfileend.Text = fi.FullName;
                    else if (sender == source_copyfile_textBox_rename)
                        this.source_copyfile_textBox_rename.Text = fi.FullName;
                    else if (sender == source_copyfile_textBox_sync)
                        this.source_copyfile_textBox_sync.Text = fi.FullName;
                    else if (sender == destination_file_textBox_sync)
                        this.destination_file_textBox_sync.Text = fi.FullName;
                    else if (sender == destination_file_textBox_two_sync)
                        this.destination_file_textBox_two_sync.Text = fi.FullName;
                    else if (sender == textBox_copy_destinationfileend_sync)
                        this.textBox_copy_destinationfileend_sync.Text = fi.FullName;
                    else if (sender == textBox_splitbinfile)
                        this.textBox_splitbinfile.Text = fi.FullName;
                }
            }
        }
        private void destination_file_button_Click(object sender, EventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.FileName = "destination_file";

           // dlg.DefaultExt = ".txt";

            dlg.Filter = "All files（*.*）|*.*|All files(*.*)|*.* ";

            if (dlg.ShowDialog() == false)
                return;
            destination_file_textBox.Text = dlg.FileName;
        }
        private void destination_file_button_copy_sourcefile_Click(object sender, EventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.FileName = "destination_file";

            //dlg.DefaultExt = ".txt";

            dlg.Filter = "All files（*.*）|*.*|All files(*.*)|*.* ";

            if (dlg.ShowDialog() == false)
                return;
            destination_file_textBox_two.Text = dlg.FileName;
        }
        private void copy_button_Click(object sender, EventArgs e)
        {
            if (source_copyfile_textBox.TextLength != 0)
            {
                string name = Path.GetFileNameWithoutExtension(source_copyfile_textBox.Text);

                if (destination_file_textBox.TextLength != 0)
                    File.Copy(@source_copyfile_textBox.Text, @destination_file_textBox.Text, true);
                else
                    MessageBox.Show("destination file inexistence");

                if (destination_file_textBox_two.TextLength != 0)
                    File.Copy(@source_copyfile_textBox.Text, @destination_file_textBox_two.Text, true);
                //else
                //    MessageBox.Show("destination file1 inexistence");

                update_state(name);
            }
            else 
               MessageBox.Show("source file inexistence");

            if (sender != null)
                copy_sync(sender, e);
        }

        private void copy_sync(object sender, EventArgs e)
        {
            if (checkBox_synccopy.Checked)
            {
                if (sender == button_copy_sourcefile)
                {
                    copy_button_Click_sync(null, e);
                }
                else if (sender == button_copy_destinationfile)
                {
                    button_copy_destinationfile_Click_sync(null, e);
                }
                else if (sender == button_copy_sourcefile_all)
                {
                    button_copy_sourcefile_all_Click_sync(null, e);
                }

                else if (sender == button_copy_sourcefile_sync)
                {
                    copy_button_Click(null, e);
                }
                else if (sender == button_copy_destinationfile_sync)
                {
                    button_copy_destinationfile_Click(null, e);
                }
                else if (sender == button_copy_sourcefile_all_sync)
                {
                    button_copy_sourcefile_all_Click(null, e);
                }
            }
        }

        private void copy_button_Click_sync(object sender, EventArgs e)
        {
            if (source_copyfile_textBox_sync.TextLength != 0)
            {
                string name = Path.GetFileNameWithoutExtension(source_copyfile_textBox_sync.Text);

                if (destination_file_textBox_sync.TextLength != 0)
                    File.Copy(@source_copyfile_textBox_sync.Text, @destination_file_textBox_sync.Text, true);
                else
                    MessageBox.Show("destination file inexistence");

                if (destination_file_textBox_two_sync.TextLength != 0)
                    File.Copy(@source_copyfile_textBox_sync.Text, @destination_file_textBox_two_sync.Text, true);
                //else
                //    MessageBox.Show("destination file1 inexistence");

                update_state(name);
            }
            else
                MessageBox.Show("source file inexistence");

            if (sender!=null)
                copy_sync(sender, e);
        }

        private void button_copy_destinationfile_Click(object sender, EventArgs e)
        {
            if (destination_file_textBox.TextLength != 0)
            {
                string name = Path.GetFileNameWithoutExtension(source_copyfile_textBox.Text);

                if (textBox_copy_destinationfileend.TextLength != 0)
                    File.Copy(@destination_file_textBox.Text, textBox_copy_destinationfileend.Text, true);
                else
                    MessageBox.Show("destination file inexistence");

                update_state(name);
            }
            else
                MessageBox.Show("source file inexistence");

            if (sender != null)
                copy_sync(sender, e);
        }

        private void button_copy_destinationfile_Click_sync(object sender, EventArgs e)
        {
            if (destination_file_textBox_sync.TextLength != 0)
            {
                string name = Path.GetFileNameWithoutExtension(destination_file_textBox_sync.Text);

                if (textBox_copy_destinationfileend_sync.TextLength != 0)
                    File.Copy(@destination_file_textBox_sync.Text, textBox_copy_destinationfileend_sync.Text, true);
                else
                    MessageBox.Show("destination file inexistence");

                update_state(name);
            }
            else
                MessageBox.Show("source file inexistence");

            if (sender != null)
                copy_sync(sender, e);
        }

        private void button_copy_sourcefile_all_Click(object sender, EventArgs e)
        {
            copy_button_Click(sender, e);
            button_copy_destinationfile_Click(sender, e);

            if (sender != null)
                copy_sync(sender, e);
        }

        private void button_copy_sourcefile_all_Click_sync(object sender, EventArgs e)
        {
            copy_button_Click_sync(sender, e);
            button_copy_destinationfile_Click_sync(sender, e);

            if (sender != null)
                copy_sync(sender, e);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.label_nowtime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (SC_MAX == true)
            {
                SC_MAX = false;
                pictureBoxinterface_Click(null, null);
            }
        }

        //private void source_copyfile_textBox_TextChanged(object sender, EventArgs e)
        //{
        //    this.Text = Path.GetFileName(source_copyfile_textBox.Text); ;
        //}

        private void source_copyfile_button_rename_Click(object sender, EventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.FileName = "source_file";

            //dlg.DefaultExt = ".txt";

            dlg.Filter = "All files（*.*）|*.*|All files(*.*)|*.* ";

            if (dlg.ShowDialog() == false)
                return;
            source_copyfile_textBox_rename.Text = dlg.FileName;
        }

        private void destination_file_button_generate_rename_Click(object sender, EventArgs e)
        {
            if (source_copyfile_textBox_rename.TextLength != 0)
            {
                string name1 = Path.GetFileNameWithoutExtension(source_copyfile_textBox_rename.Text);
                try
                {
                    string pathcopy = Path.GetDirectoryName(source_copyfile_textBox_rename.Text);  //"获取文件所在的目录："  无文件名
                    string name = Path.GetFileNameWithoutExtension(source_copyfile_textBox_rename.Text);  //"获取文件的名称没有后缀："
                    string suffix = Path.GetExtension(source_copyfile_textBox_rename.Text);//获取路径的后缀扩展名称："  后缀
                    if (checkBox2.Checked == true)
                    {
                        name = name.Remove(name.Length - 8);
                    }
                    else if (checkBox4.Checked == true)
                    {
                        name = name.Remove(name.Length - 8);
                        name = name + DateTime.Now.ToString(comboBox_timetype_rename.SelectedItem.ToString());
                    }
                    else
                    {
                        name = source_copyfile_prefix_textBox_rename.Text + name + source_copyfile_suffix_textBox_rename.Text;
                        if (checkBox3.Checked == true)
                        {
                            //name = name + DateTime.Now.ToString("yyyy-MM-dd HHmmss");
                            if (checkBox_systemtime_rename.Checked == true)
                            {
                                FileInfo fi = new FileInfo(source_copyfile_textBox_rename.Text);
                                name = name + fi.LastWriteTime.ToString(comboBox_timetype_rename.SelectedItem.ToString());
                                //Response.Write("创建时间：" + fi.CreationTime.ToString() + "写入文件的时间" + fi.LastWriteTime + "访问的时间" + fi.LastAccessTime);
                            }
                            else
                            {
                                name = name + DateTime.Now.ToString(comboBox_timetype_rename.SelectedItem.ToString());
                            }
                        }
                    }

                    pathcopy = pathcopy + "\\" + name + suffix;
                    File.Copy(@source_copyfile_textBox_rename.Text, @pathcopy, true);
                    destination_file_textBox_rename.Text = pathcopy;

                    if (checkBox2.Checked == true)
                    {
                        File.Delete(source_copyfile_textBox_rename.Text);
                    }

                    //System.Collections.Specialized.StringCollection strcoll = new System.Collections.Specialized.StringCollection();
                    //strcoll.Add(source_copyfile_textBox_rename.Text);
                    //strcoll.Add(@"c:\ab");
                    //Clipboard.SetFileDropList(strcoll);

                    string[] file = new string[1];
                    file[0] = pathcopy;
                    DataObject dataObject = new DataObject();
                    dataObject.SetData(DataFormats.FileDrop, file);
                    Clipboard.SetDataObject(dataObject, true);
                }
                catch
                {
                    MessageBox.Show("source file error");
                }
                update_state_rename(name1);
            }
            else
                MessageBox.Show("source file inexistence");
        }

        private void checkBox_synccopy_sync_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == checkBox_synccopy)
            {
                if (checkBox_synccopy.Checked)
                    checkBox_synccopy_sync.Checked = true;
                else
                    checkBox_synccopy_sync.Checked = false;
            }
            else if (sender == checkBox_synccopy_sync)
            {
                if (checkBox_synccopy_sync.Checked)
                    checkBox_synccopy.Checked = true;
                else
                    checkBox_synccopy.Checked = false;
            }
        }

        private void button_copy_sourcefile4_Click(object sender, EventArgs e)
        {
            source_copyfile_prefix_textBox_rename.Text = "";
            source_copyfile_suffix_textBox_rename.Text = "";
        }

        private void button_splitbinfile_Click(object sender, EventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.FileName = "source_file";

            dlg.DefaultExt = ".bin";

            dlg.Filter = "bin file (.bin)|*.bin";

            if (dlg.ShowDialog() == false)
                return;
            textBox_splitbinfile.Text = dlg.FileName;
        }

        private void button_split_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox_splitbinfile.Text))//可以上传压缩包.zip 或者jar包
            {
                try
                {
                    string savepath, str;
                    byte[] byteArray = File2Bytes(textBox_splitbinfile.Text);//文件转成byte二进制数组
                    savepath = textBox_splitbinfile.Text.Replace(".bin", string.Empty).Replace(".BIN", string.Empty);
                    string str_addr = textBox_section1.Text;
                    TextBox textbox = textBox_sectionout1;
                    int byte_cnt = 0, len = 0;
                    if (str_addr.Length == 10)
                    {
                        str = str_addr.Substring(2, 8);
                        if (comboBox_splittype.SelectedIndex == 0)
                            len = Convert.ToInt32(str, 16);
                        else if (comboBox_splittype.SelectedIndex == 1)
                        {
                            len = Convert.ToInt32(str, 16);
                            if (len <= byte_cnt)
                            {
                                MessageBox.Show("input addr error!");
                                return;
                            }
                            len = len - byte_cnt;
                        }

                        if (len != 0)
                        {
                            if ((len + byte_cnt) > byteArray.Length)
                            {
                                len = byteArray.Length - byte_cnt;
                            }
                            str = savepath + "_section1" + ".bin";
                            Bytes2File(byteArray, byte_cnt, len, str);
                            byte_cnt += len;
                            textbox.Text = str;
                            if (byte_cnt >= byteArray.Length) return;
                        }
                        else textbox.Text = "";
                    }
                    else textbox.Text = "";

                    str_addr = textBox_section2.Text;
                    textbox = textBox_sectionout2;
                    if (str_addr.Length == 10)
                    {
                        str = str_addr.Substring(2, 8);
                        if (comboBox_splittype.SelectedIndex == 0)
                            len = Convert.ToInt32(str, 16);
                        else if (comboBox_splittype.SelectedIndex == 1)
                        {
                            len = Convert.ToInt32(str, 16);
                            if (len <= byte_cnt)
                            {
                                MessageBox.Show("input addr error!");
                                return;
                            }
                            len = len - byte_cnt;
                        }
                        if (len != 0)
                        {
                            if ((len + byte_cnt) > byteArray.Length)
                            {
                                len = byteArray.Length - byte_cnt;
                            }
                            str = savepath + "_section2" + ".bin";
                            Bytes2File(byteArray, byte_cnt, len, str);
                            byte_cnt += len;
                            textbox.Text = str;
                            if (byte_cnt >= byteArray.Length) return;
                        }
                        else textbox.Text = "";
                    }
                    else textbox.Text = "";

                    str_addr = textBox_section3.Text;
                    textbox = textBox_sectionout3;
                    if (str_addr.Length == 10)
                    {
                        str = str_addr.Substring(2, 8);
                        if (comboBox_splittype.SelectedIndex == 0)
                            len = Convert.ToInt32(str, 16);
                        else if (comboBox_splittype.SelectedIndex == 1)
                        {
                            len = Convert.ToInt32(str, 16);
                            if (len <= byte_cnt)
                            {
                                MessageBox.Show("input addr error!");
                                return;
                            }
                            len = len - byte_cnt;
                        }
                        if (len != 0)
                        {
                            if ((len + byte_cnt) > byteArray.Length)
                            {
                                len = byteArray.Length - byte_cnt;
                            }
                            str = savepath + "_section3" + ".bin";
                            Bytes2File(byteArray, byte_cnt, len, str);
                            byte_cnt += len;
                            textbox.Text = str;
                            if (byte_cnt >= byteArray.Length) return;
                        }
                        else textbox.Text = "";
                    }
                    else textbox.Text = "";

                    str_addr = textBox_section4.Text;
                    textbox = textBox_sectionout4;
                    if (str_addr.Length == 10)
                    {
                        str = str_addr.Substring(2, 8);
                        if (comboBox_splittype.SelectedIndex == 0)
                            len = Convert.ToInt32(str, 16);
                        else if (comboBox_splittype.SelectedIndex == 1)
                        {
                            len = Convert.ToInt32(str, 16);
                            if (len <= byte_cnt)
                            {
                                MessageBox.Show("input addr error!");
                                return;
                            }
                            len = len - byte_cnt;
                        }
                        if (len != 0)
                        {
                            if ((len + byte_cnt) > byteArray.Length)
                            {
                                len = byteArray.Length - byte_cnt;
                            }
                            str = savepath + "_section4" + ".bin";
                            Bytes2File(byteArray, byte_cnt, len, str);
                            byte_cnt += len;
                            textbox.Text = str;
                            if (byte_cnt >= byteArray.Length) return;
                        }
                        else textbox.Text = "";
                    }
                    else textbox.Text = "";

                    str_addr = textBox_section5.Text;
                    textbox = textBox_sectionout5;
                    if (str_addr.Length == 10)
                    {
                        str = str_addr.Substring(2, 8);
                        if (comboBox_splittype.SelectedIndex == 0)
                            len = Convert.ToInt32(str, 16);
                        else if (comboBox_splittype.SelectedIndex == 1)
                        {
                            len = Convert.ToInt32(str, 16);
                            if (len <= byte_cnt)
                            {
                                MessageBox.Show("input addr error!");
                                return;
                            }
                            len = len - byte_cnt;
                        }
                        if (len != 0)
                        {
                            if ((len + byte_cnt) > byteArray.Length)
                            {
                                len = byteArray.Length - byte_cnt;
                            }
                            str = savepath + "_section5" + ".bin";
                            Bytes2File(byteArray, byte_cnt, len, str);
                            byte_cnt += len;
                            textbox.Text = str;
                            if (byte_cnt >= byteArray.Length) return;
                        }
                        else textbox.Text = "";
                    }
                    else textbox.Text = "";

                    textbox = textBox_sectionout6;
                    len = byteArray.Length - byte_cnt;
                    if (len != 0)
                    {
                        str = savepath + "_section6" + ".bin";
                        Bytes2File(byteArray, byte_cnt, len, str);
                        byte_cnt += len;
                        textbox.Text = str;
                        if (byte_cnt >= byteArray.Length) return;
                    }
                    else textbox.Text = "";



                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("source file error!");
            }
        }

        // 将byte数组转换为文件并保存到指定地址
        // param name="buff"byte数组
        // param name="savepath"保存地址
        // int lenght 分解个数
        public static void Bytes2File(byte[] buff, byte[] buff1, string savepath)
        {
            string path;
            FileStream fs;
            BinaryWriter bw;

            byte[] buff2 = new byte[3 * 4096];
            for (int i = 0; i < buff2.Length; i++)
            {
                buff2[i] = 0xff;
            }

            savepath = savepath.Replace(".bin", string.Empty).Replace(".BIN", string.Empty);
            path = savepath + "_Combine" + ".bin";
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            fs = new FileStream(path, FileMode.CreateNew);
            bw = new BinaryWriter(fs);
            bw.Write(buff, 0, buff.Length);
            bw.Write(buff2, 0, buff2.Length);
            bw.Write(buff1, 0, buff1.Length);
            bw.Close();
            fs.Close();
        }

        public static void Bytes2File(byte[] buff, byte[] buff1, byte[] buff2, byte[] buff3, byte[] buff4, byte[] buff5, string savepath)
        {
            FileStream fs;
            BinaryWriter bw;

            if (System.IO.File.Exists(savepath))
            {
                System.IO.File.Delete(savepath);
            }
            fs = new FileStream(savepath, FileMode.CreateNew);
            bw = new BinaryWriter(fs);
            bw.Write(buff, 0, buff.Length);
            bw.Write(buff1, 0, buff1.Length);
            bw.Write(buff2, 0, buff2.Length);
            bw.Write(buff3, 0, buff3.Length);
            bw.Write(buff4, 0, buff4.Length);
            bw.Write(buff5, 0, buff5.Length);
            bw.Close();
            fs.Close();
        }

        // 将byte数组转换为文件并保存到指定地址
        // param name="buff"byte数组
        // param name="savepath"保存地址
        // int lenght 分解个数
        public static void Bytes2File(byte[] buff, int index, int count, string savepath)
        {
            FileStream fs;
            BinaryWriter bw;

            if (System.IO.File.Exists(savepath))
            {
                System.IO.File.Delete(savepath);
            }
            fs = new FileStream(savepath, FileMode.CreateNew);
            bw = new BinaryWriter(fs);
            bw.Write(buff, index, count);
            bw.Close();
            fs.Close();
        }

        private void button_combinopen_Click(object sender, EventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.FileName = "source_file";

            dlg.DefaultExt = ".bin";

            dlg.Filter = "bin file (.bin)|*.bin";

            if (dlg.ShowDialog() == false)
                return;

            if ((Button)(sender) == button_sectionintput1)
            {
                textBox_sectionintput1.Text = dlg.FileName;
            }
            else if ((Button)(sender) == button_sectionintput2)
            {
                textBox_sectionintput2.Text = dlg.FileName;
            }
            else if ((Button)(sender) == button_sectionintput3)
            {
                textBox_sectionintput3.Text = dlg.FileName;
            }
            else if ((Button)(sender) == button_sectionintput4)
            {
                textBox_sectionintput4.Text = dlg.FileName;
            }
            else if ((Button)(sender) == button_sectionintput5)
            {
                textBox_sectionintput5.Text = dlg.FileName;
            }
            else if ((Button)(sender) == button_sectionintput6)
            {
                textBox_sectionintput6.Text = dlg.FileName;
            }
        }

        private void button_combine_Click(object sender, EventArgs e)
        {
            byte[] byteArray1 = null, byteArray2 = null, byteArray3 = null, byteArray4 = null, byteArray5 = null, byteArray6 = null;
            if (textBox_sectionintput1.Text.Length != 0) byteArray1 = File2Bytes(textBox_sectionintput1.Text);//文件转成byte二进制数组
            if (textBox_sectionintput2.Text.Length != 0) byteArray2 = File2Bytes(textBox_sectionintput2.Text);
            if (textBox_sectionintput3.Text.Length != 0) byteArray3 = File2Bytes(textBox_sectionintput3.Text);
            if (textBox_sectionintput4.Text.Length != 0) byteArray4 = File2Bytes(textBox_sectionintput4.Text);
            if (textBox_sectionintput5.Text.Length != 0) byteArray5 = File2Bytes(textBox_sectionintput5.Text);
            if (textBox_sectionintput6.Text.Length != 0) byteArray6 = File2Bytes(textBox_sectionintput6.Text);
            string savepath = Path.GetDirectoryName(textBox_sectionintput1.Text); ;
            savepath = savepath + "\\BIN_Combin" + ".bin";
            Bytes2File(byteArray1, byteArray2, byteArray3, byteArray4, byteArray5, byteArray6, savepath);
        }

        private void button_copy_sourcefile5_Click(object sender, EventArgs e)
        {
            textBox_section1.Text = "0x00000000";
            textBox_section2.Text = "0x00000000";
            textBox_section3.Text = "0x00000000";
            textBox_section4.Text = "0x00000000";
            textBox_section5.Text = "0x00000000";
        }

        private void source_copyfile_textBox_sync_TextChanged(object sender, EventArgs e)
        {
            byte[] byteArray = File2Bytes(source_copyfile_textBox_sync.Text);//文件转成byte二进制数组
            source_copyfile_textBox_sync_size.Text = byteArray.Length.ToString() + "(0x" + byteArray.Length.ToString("X") + ")";
            UInt32 Checksum = 0;
            for (int i = 0; i < byteArray.Length; i++)
            {
                Checksum += byteArray[i];
            }
            source_copyfile_textBox_sync_checksum.Text = "0x" + Checksum.ToString("X");
        }
        private void comboBox_Common_path_SelectedIndexChanged(object sender, EventArgs e)
        {
            destination_file_textBox_sync.Text = comboBox_Common_path.Text +"\\"+ System.IO.Path.GetFileName(source_copyfile_textBox_sync.Text); 
        }
    }
}
