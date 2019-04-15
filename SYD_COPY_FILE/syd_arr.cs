using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SYD_COPY_FILE
{
    public partial class Form1
    {

        #region define 

        private static UInt32 UTF8_CODE_BASS= 0X4E00;
        private static UInt32 UTF8_CODE_END= 0X9FA5;
        private static UInt32 UTF8_MASK_ADDR= 0X0000;      //4E00-9FA5  0X9FA5-0X4E00=0X51A5=20901  20901*2=41802  41802/1024=40.822265625 
       
        private static UInt32 UTF8_SYMBLE1_BASS= 0X2010;
        private static UInt32 UTF8_SYMBLE1_END= 0X201F;
        private static UInt32 UTF8_SYMBLE1_ADDR= ((UTF8_CODE_END-UTF8_CODE_BASS+1)*32);  //((0X9FA5-0X4E00+1)*20)
       
        private static UInt32 UTF8_SYMBLE2_BASS= 0X3001;
        private static UInt32 UTF8_SYMBLE2_END= 0X301F;
        private static UInt32 UTF8_SYMBLE2_ADDR= (UTF8_SYMBLE1_ADDR+(UTF8_SYMBLE1_END-UTF8_SYMBLE1_BASS+1)*32);  //((0X9FA5-0X4E00+1)*20+(0X301F-0X3001+1)*20)
       
        private static UInt32 UTF8_SYMBLE3_BASS= 0XFF01;
        private static UInt32 UTF8_SYMBLE3_END= 0XFF0F;
        private static UInt32 UTF8_SYMBLE3_ADDR = (UTF8_SYMBLE2_ADDR + (UTF8_SYMBLE2_END - UTF8_SYMBLE2_BASS + 1) * 32);  //((0X9FA5-0X4E00+1)*20+(0X301F-0X3001+1)*20+(0XFF0F-0XFF01+1)*20)

        private static UInt32 UTF8_ASCII_BASS = 0X20;
        private static UInt32 UTF8_ASCII_END = 0X7E;
        private static UInt32 UTF8_ASCII_ADDR = (UTF8_SYMBLE3_ADDR + (UTF8_SYMBLE3_END - UTF8_SYMBLE3_BASS + 1) * 32);


        private static UInt32 UTF8_MASK_32X32_ADDR = 0;

        private static UInt32 UTF8_SYMBLE1_32X32_ADDR = (UTF8_MASK_32X32_ADDR + (UTF8_CODE_END - UTF8_CODE_BASS + 1) * 128);

        private static UInt32 UTF8_SYMBLE2_32X32_ADDR = (UTF8_SYMBLE1_32X32_ADDR + (UTF8_SYMBLE1_END - UTF8_SYMBLE1_BASS + 1) * 128);

        private static UInt32 UTF8_SYMBLE3_32X32_ADDR = (UTF8_SYMBLE2_32X32_ADDR + (UTF8_SYMBLE2_END - UTF8_SYMBLE2_BASS + 1) * 128);

        private static UInt32 UTF8_ASCII_32X32_ADDR = (UTF8_SYMBLE3_32X32_ADDR + (UTF8_SYMBLE3_END - UTF8_SYMBLE3_BASS + 1) * 128);

        #endregion

        public void syd_arr_init()
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "syd_arr_ok.txt";
            label_outfilename.Text = path;

            comboBox_datatype.SelectedIndex=Settings1.Default.arr_data_type;
            comboBox_fonttype.SelectedIndex=Settings1.Default.arr_font_type;
            comboBox_mode.SelectedIndex=Settings1.Default.arr_fun_sel;
            source_file_textBox.Text=Settings1.Default.arr_source_file_text;
            textBox_filesize.Text=Settings1.Default.arr_extract_len;
        }

        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }

        private void source_file_button_Click(object sender, EventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.FileName = "source_file";

            dlg.DefaultExt = ".txt";

            dlg.Filter = "txt file (.txt)|*.txt|wav file (.wav)|*.wav|bin file (.bin)|*.bin";

            if (dlg.ShowDialog() == false)
                return;
            source_file_textBox.Text = dlg.FileName;

            reintput_file(source_file_textBox.Text);

            label_intputsize.Text = (textInput.Text.Length/2).ToString();
        }
        private void source_file_textBox_DragEnter(object sender, DragEventArgs e)
        {
            IDataObject dataObject = e.Data;
            if (dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }
        private void source_file_textBox_DragDrop(object sender, DragEventArgs e)
        {
            IDataObject dataObject = e.Data;

            if (dataObject == null) return;

            if (dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])dataObject.GetData(DataFormats.FileDrop);

                foreach (string file in files)
                {
                    System.IO.FileInfo fi = new System.IO.FileInfo(file);
                    if (sender == source_file_textBox)
                        this.source_file_textBox.Text = fi.FullName;
                }
            }
        }

       private string HoverTreeClearMark(string input)
        {
            input = Regex.Replace(input, @"/\*[\s\S]*?\*/", "", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"^\s*$\n", "", RegexOptions.Multiline);  //有点多
            input = Regex.Replace(input, @"//[^\n]*", "", RegexOptions.Multiline);
            return input;
        }

       /// <summary>读取文件，返回一个含有文件数据的行列表</summary>
       /// <param name="TxtFilePath">文件路径</param>
       /// <returns>文件数据的行列表</returns>
       private List<string> ReadTxtFromFile(string TxtFilePath)
       {
           // 1 首先创建一个泛型为string 的List列表
           List<string> AllRowStrList = new List<string>();
           {
               // 2 加载文件
               System.IO.StreamReader sr = new
               System.IO.StreamReader(TxtFilePath,Encoding.Default);
               String line; // 3 调用StreamReader的ReadLine()函数
               while ((line = sr.ReadLine()) != null)
               {   // 4 将每行添加到文件List中
                   AllRowStrList.Add(line);
               }
               // 5 关闭流
               sr.Close();
           }
           // 6 完成操作
           return AllRowStrList;
       }
       private void bintoarr()
       {
           int i = 0, data_residue = 0;
           //string orgTxt1 = HoverTreeClearMark(textInput.Text.Trim());
           string orgTxt1 = textInput.Text.Trim();

           orgTxt1 = orgTxt1.Replace("\n", "").Replace("\r", "").Replace("\t", "").Replace("0X", "").Replace("0x", "").Replace(",", "").Replace("\r\n", "");
           //List<string> lstArray = orgTxt1.Split(new char[1] { ';' }).ToList();
           List<string> lstArray = new List<string>();

           data_residue = orgTxt1.Length % 32;

           //将字符串分割为长度为4的字符数组
           for (i = 0; i < (orgTxt1.Length / 32); i = i + 1)
           {
               try
               {
                   lstArray.Add(orgTxt1.Substring(32 * i, 32)); //i-起始位置，4-子串长度
               }
               catch (Exception e1)
               {
                   MessageBox.Show(e1.ToString(), "异常提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                   continue;
               }
           }
           if (data_residue != 0)
           {
               lstArray.Add(orgTxt1.Substring(32 * i, orgTxt1.Length % 32)); //i-起始位置，4-子串长度
           }

           UInt16 ii = 0;

           string str = "", str1 = "";
           for (i = 0; i < lstArray.Count; i++)
           {
               str = lstArray[i];
               try
               {
                   if ((i == lstArray.Count - 1) & (data_residue != 0))  //最后一个
                   {
                       for (ii = 0; ii < data_residue; ii++)
                       {
                           if (ii == 0) str = str.Insert(0, "0x");
                           else if (ii == 1) str = str.Insert(4, ",0x");
                           else if ((ii > 1) & (ii < (data_residue / 2 - 1))) str = str.Insert((ii - 2) * 5 + 9, ",0x");
                           else if (ii == (data_residue / 2 - 1))
                           {
                               str = str.Insert((ii - 2) * 5 + 9, ",0x");
                               str = str.Insert((ii - 1) * 5 + 9, ",\r\n");
                           }
                       }
                   }
                   else
                   {
                       for (ii = 0; ii < 16; ii++)
                       {
                           if (ii == 0) str = str.Insert(0, "0x");
                           else if (ii == 1) str = str.Insert(4, ",0x");
                           else if ((ii > 1) & (ii < 15)) str = str.Insert((ii - 2) * 5 + 9, ",0x");
                           else if (ii == 15)
                           {
                               str = str.Insert((ii - 2) * 5 + 9, ",0x");
                               str = str.Insert((ii - 1) * 5 + 9, ",\r\n");
                           }
                       }
                   }
               }
               catch
               {
                   MessageBox.Show("出错位置第" + (i + 1).ToString() + "个数组");
                   Application.Exit();
               }
               lstArray[i] = str;
           }

           if (comboBox_datatype.SelectedIndex == 1)
           {
               for (i = 0; i < lstArray.Count; i++)
               {
                   str = lstArray[i];
                   str1 = "";
                   if ((i == lstArray.Count - 1) & (data_residue != 0))  //最后一个
                   {
                       for (ii = 0; ii < (data_residue / 4); ii++)
                       {
                           str1 += str.Substring(ii * 10 + 5, 4) + str.Substring(ii * 10 + 2, 3);
                       }
                       if (((data_residue / 2) % 2) != 0)
                       {
                           str1 += "0x00" + str.Substring(ii * 10 + 2, 3);
                       }
                   }
                   else
                   {
                       for (ii = 0; ii < 8; ii++)
                       {
                           str1 += str.Substring(ii * 10 + 5, 4) + str.Substring(ii * 10 + 2, 3);
                       }
                   }
                   if ((i % 2) != 0) str1 += "\r\n";
                   lstArray[i] = str1;
               }
           }
           else if (comboBox_datatype.SelectedIndex == 2)
           {
               for (i = 0; i < lstArray.Count; i++)
               {
                   str = lstArray[i];
                   str1 = "";
                   if ((i == lstArray.Count - 1) & (data_residue != 0))  //最后一个
                   {
                       for (ii = 0; ii < (data_residue / 8); ii++)
                       {
                           str1 += str.Substring(ii * 20 + 15, 4) + str.Substring(ii * 20 + 12, 2) + str.Substring(ii * 20 + 7, 2) + str.Substring(ii * 20 + 2, 3);
                       }
                       if (((data_residue / 2) % 4) == 1)
                       {
                           str1 += "0x000000" + str.Substring(ii * 20 + 2, 3);
                       }
                       else if (((data_residue / 2) % 4) == 2)
                       {
                           str1 += "0x0000" + str.Substring(ii * 20 + 7, 2) + str.Substring(ii * 20 + 2, 3);
                       }
                       else if (((data_residue / 2) % 4) == 3)
                       {
                           str1 += "0x00" + str.Substring(ii * 20 + 12, 2) + str.Substring(ii * 20 + 7, 2) + str.Substring(ii * 20 + 2, 3);
                       }
                   }
                   else
                   {
                       for (ii = 0; ii < 4; ii++)
                       {
                           str1 += str.Substring(ii * 20 + 15, 4) + str.Substring(ii * 20 + 12, 2) + str.Substring(ii * 20 + 7, 2) + str.Substring(ii * 20 + 2, 3);
                       }
                   }
                   if ((i % 2) != 0) str1 += "\r\n";
                   lstArray[i] = str1;
               }
           }

           int extract_len = lstArray.Count;
           if (textBox_filesize.Text.Length != 0)
           {
               extract_len = Convert.ToInt32(textBox_filesize.Text) / 16;
               if ((extract_len == 0) | (extract_len > lstArray.Count)) extract_len = lstArray.Count;
           }

           string path = label_outfilename.Text;
           using (FileStream fsWrite = new FileStream(path, FileMode.Create, FileAccess.Write))
           {
               byte[] buffer = null;
               for (i = 0; i < extract_len; i++)
               {
                   buffer = Encoding.Default.GetBytes(lstArray[i]);
                   fsWrite.Write(buffer, 0, buffer.Length);
               }
           }

           if (extract_len != lstArray.Count)
           {
               richTextBox_out.Text = System.IO.File.ReadAllText(path);
           }
           else
           {
               richTextBox_out.Text = string.Join("", lstArray.ToArray());
           }

           MessageBox.Show("保存成功!");
       }

       private void RGB_565()
       {
           int i = 0, data_residue = 0;
           string orgTxt1 = textInput.Text.Trim();

           orgTxt1 = orgTxt1.Replace("\n", "").Replace("\r", "").Replace("\t", "").Replace("0X", "").Replace("0x", "").Replace(",", "").Replace("\r\n", "");
           List<string> lstArray = new List<string>();

           data_residue = orgTxt1.Length % 6;

           //将字符串分割为长度为4的字符数组
           for (i = 0; i < (orgTxt1.Length / 6); i = i + 1)
           {
               try
               {
                   lstArray.Add(orgTxt1.Substring(6 * i, 6)); //i-起始位置，4-子串长度
               }
               catch (Exception e1)
               {
                   MessageBox.Show(e1.ToString(), "异常提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                   continue;
               }
           }
           if (data_residue != 0)
           {
               lstArray.Add(orgTxt1.Substring(6 * i, orgTxt1.Length % 6)); //i-起始位置，4-子串长度
           }

           UInt16 ii = 0;

           string str = "";

           Int32[] RGB_List = new Int32[lstArray.Count];

           Int32 temp = 0;

           for (i = 0; i < lstArray.Count; i++)
           {
               str = lstArray[i];
               try
               {
                   temp = Convert.ToInt32(lstArray[i],16);
                   Byte a, b, c;
                   a = (Byte)(temp >> 19);
                   b = (Byte)(temp >> 10);
                   c = (Byte)(temp >> 3);
                   temp = (a << 11) | (b << 5) | c;
                   RGB_List[i] = temp;
               }
               catch
               {
                   MessageBox.Show("出错位置第" + (i + 1).ToString() + "个数组");
                   Application.Exit();
               }
               lstArray[i] = str;
           }
           int extract_len = lstArray.Count;

           string path = label_outfilename.Text;
           path = path.Replace(".bin", string.Empty).Replace(".BIN", string.Empty);
           path = path + ".txt";
           using (FileStream fsWrite = new FileStream(path, FileMode.Create, FileAccess.Write))
           {
               byte[] buffer = null;
               for (i = 0; i < lstArray.Count/8; i++)
               {
                   str = "";
                   for (int j = 0; j < 8; j++)
                   {
                       str = str + "0x" + ((byte)RGB_List[i * 8 + j]).ToString("X2") + ",0x" + (RGB_List[i * 8 + j] >> 8).ToString("X2") + ",";
                   }

                   buffer = Encoding.Default.GetBytes(str + "\r");

                   fsWrite.Write(buffer, 0, buffer.Length);
               }
           }

           richTextBox_out.Text = System.IO.File.ReadAllText(path);

           MessageBox.Show("保存成功!");
       }


       private void fonttxt_to_bin()
       {
           int i = 0, j = 0, k = 0;
           string orgTxt1 = textInput.Text.Trim();
           orgTxt1 = orgTxt1.Replace("\n", "").Replace("\r", "").Replace("\t", "").Replace("0X", "").Replace("0x", "").Replace(",", "").Replace("\r\n", "");

           List<string> lstArray = orgTxt1.Split(new string[] { "/*", "*/" }, StringSplitOptions.RemoveEmptyEntries).ToList();

           string str = "";
           long bin_len = 0;

           for (i = 0,j=0,k=0; i < lstArray.Count / 2; i++)
           {
               bin_len = bin_len +lstArray[i * 2].Length/2;

               str = lstArray[i*2+1];

               j = str.IndexOf("\"",0);
               k = str.IndexOf("\"", j+1);

               try
               {
                   str = str.Substring(j+1,k-j-1);
                   lstArray[i * 2 + 1] = str;
               }
               catch
               {
                   MessageBox.Show("出错位置第" + (i + 1).ToString() + "个数组");
                   Application.Exit();
                   return;
               }
           }

           byte[] bin = new byte[bin_len];
           UInt32 index = 0,addr=0;

           for (i = 0; i < lstArray.Count / 2; i++)
           {
               str = lstArray[i * 2+1];
               byte[] bytes = System.Text.Encoding.Unicode.GetBytes(str);

               if (bytes.Length == 1)
               {
                   index = bytes[0];
               }
               else if (bytes.Length == 2)
               {
                   index = (UInt32)(bytes[1] << 8) | (UInt32)bytes[0];
               }

               if (comboBox_fonttype.SelectedIndex == 0)
               {
                   if ((index <= UTF8_CODE_END) && (index >= UTF8_CODE_BASS))
                   {
                       index = index - UTF8_CODE_BASS;
                       addr = index * 32;
                   }
                   else if ((index <= UTF8_SYMBLE1_END) && (index >= UTF8_SYMBLE1_BASS))
                   {
                       index = index - UTF8_SYMBLE1_BASS;
                       addr = index * 32 + UTF8_SYMBLE1_ADDR;
                   }
                   else if ((index <= UTF8_SYMBLE2_END) && (index >= UTF8_SYMBLE2_BASS))
                   {
                       index = index - UTF8_SYMBLE2_BASS;
                       addr = index * 32 + UTF8_SYMBLE2_ADDR;
                   }
                   else if ((index <= UTF8_SYMBLE3_END) && (index >= UTF8_SYMBLE3_BASS))
                   {
                       index = index - UTF8_SYMBLE3_BASS;
                       addr = index * 32 + UTF8_SYMBLE3_ADDR;
                   }
                   else if ((index <= UTF8_ASCII_END) && (index >= UTF8_ASCII_BASS))
                   {
                       index = index - UTF8_ASCII_BASS;
                       addr = index * (32 / 2) + UTF8_ASCII_ADDR;
                   }
               }
               else if (comboBox_fonttype.SelectedIndex == 1)
               {
                   if ((index <= UTF8_CODE_END) && (index >= UTF8_CODE_BASS))
                   {
                       index = index - UTF8_CODE_BASS;
                       addr = index * 128 + UTF8_MASK_32X32_ADDR;
                   }
                   else if ((index <= UTF8_SYMBLE1_END) && (index >= UTF8_SYMBLE1_BASS))
                   {
                       index = index - UTF8_SYMBLE1_BASS;
                       addr = index * 128 + UTF8_SYMBLE1_32X32_ADDR;
                   }
                   else if ((index <= UTF8_SYMBLE2_END) && (index >= UTF8_SYMBLE2_BASS))
                   {
                       index = index - UTF8_SYMBLE2_BASS;
                       addr = index * 128 + UTF8_SYMBLE2_32X32_ADDR;
                   }
                   else if ((index <= UTF8_SYMBLE3_END) && (index >= UTF8_SYMBLE3_BASS))
                   {
                       index = index - UTF8_SYMBLE3_BASS;
                       addr = index * 128 + UTF8_SYMBLE3_32X32_ADDR;
                   }
                   else if ((index <= UTF8_ASCII_END) && (index >= UTF8_ASCII_BASS))
                   {
                       index = index - UTF8_ASCII_BASS;
                       addr = index * (128 / 2) + UTF8_ASCII_32X32_ADDR;
                   }
               }

                

               for (k = 0; k < lstArray[i*2].Length/2; k++)
               {
                   bin[addr + k] = Convert.ToByte(lstArray[i*2].Substring(k*2,2), 16);
               }
           }

           string path = label_outfilename.Text;
           path = path.Replace(".txt", string.Empty).Replace(".TXT", string.Empty);
           path = path + ".bin";
           FileStream fs = new FileStream(path, FileMode.Create);
           BinaryWriter bw = new BinaryWriter(fs);
           bw.Write(bin, 0, bin.Length);
           bw.Flush();
           bw.Close();

           MessageBox.Show("保存成功!");
       }

       private void jlinktxt_to_arr()
       {
           int i = 0;
           string orgTxt1 = textInput.Text.Trim();

           List<string> lstArray = orgTxt1.ToLower().Split(new char[2] { '\r', '\n' }).ToList();

           string str = "";
           for (i = 0; i < lstArray.Count; i++)
           {
               str = lstArray[i];
               str = str.Remove(0, 11).Insert(0, "0x").Replace(" ", ",0x");

               lstArray[i] = str + ",";
           }

           for (i = 0; i < lstArray.Count / 2; i++)
           {
               lstArray[i] = lstArray[i * 2] + lstArray[i * 2 + 1];
               richTextBox_out.AppendText(lstArray[i] + "\r\n");
           }
           if ((lstArray.Count % 2) != 0)
           {
               lstArray[i] = lstArray[i * 2];
               richTextBox_out.AppendText(lstArray[i] + "\r\n");
           }

           if (comboBox_datatype.SelectedIndex == 1)
           {

           }
           else if (comboBox_datatype.SelectedIndex == 2)
           {

           }

           int extract_len = lstArray.Count / 2;

           if ((lstArray.Count % 2) != 0)
           {
               extract_len += 1;
           }

           if (textBox_filesize.Text.Length != 0)
           {
               extract_len = Convert.ToInt32(textBox_filesize.Text) / 16;
               if ((extract_len == 0) | (extract_len > lstArray.Count)) extract_len = lstArray.Count;
           }

           string path = label_outfilename.Text;
           path = path.Replace(".txt", "_ok.txt").Replace(".TXT", "_ok.TXT");
           using (FileStream fsWrite = new FileStream(path, FileMode.Create, FileAccess.Write))
           {
               byte[] buffer = null;
               for (i = 0; i < extract_len; i++)
               {
                   buffer = Encoding.Default.GetBytes(lstArray[i] + "\r\n");
                   fsWrite.Write(buffer, 0, buffer.Length);
               }
           }

           MessageBox.Show("保存成功!");
       }

       private void Chinese_to_utf8_arr()
       {
           int i = 0,j=0;
           string orgTxt1 = textInput.Text.Trim();

           List<string> lstArray = orgTxt1.ToLower().Split(new char[2] { '\r', '\n' }).ToList();

           string str = "";
           byte[] buffer_utf8;

           for (i = 0; i < lstArray.Count; i++)
           {
               buffer_utf8 = Encoding.UTF8.GetBytes(lstArray[i]);
               str = "uint8_t buf[]={" + "0x" + buffer_utf8.Length.ToString("X02") + ",";
               for (j = 0; j <buffer_utf8.Length; j++)
               {
                   str =str+ "0x" + buffer_utf8[j].ToString("X") + ",";
               }
               lstArray[i] = str + "};" + "//" + lstArray[i];
           }

           richTextBox_out.Text = "";

           for (i = 0; i < lstArray.Count; i++)
           {
               richTextBox_out.AppendText(lstArray[i] + "\r\n");
           }

           string path = label_outfilename.Text;
           path = path.Replace(".txt", "_ok.txt").Replace(".TXT", "_ok.TXT");
           using (FileStream fsWrite = new FileStream(path, FileMode.Create, FileAccess.Write))
           {
               byte[] buffer = null;
               for (i = 0; i < lstArray.Count; i++)
               {
                   buffer = Encoding.Default.GetBytes(lstArray[i] + "\r\n");
                   fsWrite.Write(buffer, 0, buffer.Length);
               }
           }

           MessageBox.Show("保存成功!");
       }

       private void keil_memery()
       {
           int i = 0, j = 0;
           string orgTxt1 = textInput.Text.Trim();

           List<string> lstArray = orgTxt1.ToLower().Split(new char[2] { '\r', '\n' }).ToList();

           string str = "";

           richTextBox_out.Text = "";

           for (i = 1; i < (lstArray.Count-2); i++)
           {
               richTextBox_out.AppendText(lstArray[i].Substring(9,32) + "\r\n");
           }

           string path = label_outfilename.Text;
           path = path.Replace(".txt", "_ok.txt").Replace(".TXT", "_ok.TXT");
           using (FileStream fsWrite = new FileStream(path, FileMode.Create, FileAccess.Write))
           {
               byte[] buffer = null;
               for (i = 1; i < (lstArray.Count-2); i++)
               {
                   buffer = Encoding.Default.GetBytes(lstArray[i].Substring(9, 32) + "\r\n");
                   fsWrite.Write(buffer, 0, buffer.Length);
               }
           }

           MessageBox.Show("保存成功!");
       }

       private void Data_filled_complement_zero()
       {
           int i = 0, j = 0;
           string orgTxt1 = textInput.Text.Trim();

           List<string> lstArray = orgTxt1.ToLower().Split(new char[1] { ' '}).ToList();

           richTextBox_out.Text = "";

           for (i = 0; i < lstArray.Count; i++)
           {
               if (lstArray[i].Length==1) 
                  richTextBox_out.AppendText("0"+lstArray[i]+" ");
               else
                   richTextBox_out.AppendText(lstArray[i] + " ");
           }

           string path = label_outfilename.Text;
           path = path.Replace(".txt", "_ok.txt").Replace(".TXT", "_ok.TXT");
           using (FileStream fsWrite = new FileStream(path, FileMode.Create, FileAccess.Write))
           {
               byte[] buffer = null;
               buffer = Encoding.Default.GetBytes(richTextBox_out.Text);
               fsWrite.Write(buffer, 0, buffer.Length);
           }

           MessageBox.Show("保存成功!");
       }

       private string Byte_reversal(string input)
       {
           string str = "";
           for (int i = 0; i < (input.Length) / 2; i++)
           {
               str = str + input.Substring(input.Length-(i+1)*2,2);
           }
               return str;
       }

       private void Data_reversal()
       {
           int i = 0, j = 0;
           string orgTxt1 = textInput.Text.Trim();

           orgTxt1 = orgTxt1.Replace(" ", "").Replace("-", "");

           List<string> lstArray = orgTxt1.ToLower().Split(new char[2] { '\r', '\n' }).ToList();

           richTextBox_out.Text = "";

           for (i = 0; i < lstArray.Count; i++)
           {
               richTextBox_out.AppendText(Byte_reversal(lstArray[i])+"\r\n");
           }

           string path = label_outfilename.Text;
           path = path.Replace(".txt", "_ok.txt").Replace(".TXT", "_ok.TXT");
           using (FileStream fsWrite = new FileStream(path, FileMode.Create, FileAccess.Write))
           {
               byte[] buffer = null;
               buffer = Encoding.Default.GetBytes(richTextBox_out.Text);
               fsWrite.Write(buffer, 0, buffer.Length);
           }

           MessageBox.Show("保存成功!");
       }

        private void draw_Click(object sender, EventArgs e)
        {
            if (comboBox_mode.SelectedIndex == 0)
            {
                bintoarr();
            }
            else if (comboBox_mode.SelectedIndex == 1)
            {
                RGB_565();
            }
            else if (comboBox_mode.SelectedIndex == 2)
            {
                fonttxt_to_bin();
            }
            else if (comboBox_mode.SelectedIndex == 3)
            {
                jlinktxt_to_arr();
            }
            else if (comboBox_mode.SelectedIndex == 4)
            {
                Chinese_to_utf8_arr();
            }
            else if (comboBox_mode.SelectedIndex == 5)
            {
                keil_memery();
            }
            else if (comboBox_mode.SelectedIndex == 6)
            {
                Data_filled_complement_zero();
            }
            else if (comboBox_mode.SelectedIndex == 7)
            {
                Data_reversal();
            }
        }

        private void button_clear_Click(object sender, EventArgs e)
        {
            textInput.Clear();
            richTextBox_out.Clear();
        }

        private void button_reintput_Click(object sender, EventArgs e)
        {
            reintput_file(source_file_textBox.Text);
        }

        private void reintput_file(string FileName)
        {
            FileInfo fi = new FileInfo(FileName);

            if ((fi.Extension).ToLower() == ".txt")
            {
                label_outfilename.Text = source_file_textBox.Text;
                textInput.Text = System.IO.File.ReadAllText(source_file_textBox.Text, Encoding.Default);
            }
            else if (fi.Extension == ".wav")
            {
                string path = FileName;
                label_outfilename.Text = path.Replace(".WAV", string.Empty).Replace(".wav", string.Empty) + "_ok.txt";
                byte[] text = System.IO.File.ReadAllBytes(source_file_textBox.Text);
                var str = DateTime.Now.ToString();
                var encode = Encoding.UTF8;
                var hex = BitConverter.ToString(text, 0).Replace("-", string.Empty).ToLower();
                textInput.Text = hex;
            }
            else if (fi.Extension == ".bin")
            {
                string path = FileName;
                label_outfilename.Text = path.Replace(".BIN", string.Empty).Replace(".bin", string.Empty) + "_ok.txt";
                byte[] text = System.IO.File.ReadAllBytes(source_file_textBox.Text);
                var str = DateTime.Now.ToString();
                var encode = Encoding.UTF8;
                var hex = BitConverter.ToString(text, 0).Replace("-", string.Empty).ToLower();
                textInput.Text = hex;
            }
        }
        }
    }

