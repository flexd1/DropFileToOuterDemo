using System.Runtime.InteropServices;
using static System.Net.WebRequestMethods;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MoveFileDemo
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x20;
        private const int MOUSEEVENTF_MIDDLEUP = 0x40;
        private const int MOUSEEVENTF_MOVE = 0x01;

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        public Form1()
        {
            InitializeComponent();
            //listView.View = View.Details;
            listView.AllowDrop = true;



        }

        private void listView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            button2_Click(null,null);
            SimulateMouseDrag(-50, 0, 0,0);
        }

        public static async void SimulateMouseDrag(int x, int y, int width, int height)
        {
            await Task.Delay(2000);
            // 模拟鼠标左键按下  
            mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
            await Task.Delay(200);
            // 模拟鼠标移动  
            for (int i = 0; i <= width; i++)
            {
                await Task.Delay(20);
                mouse_event(MOUSEEVENTF_MOVE, x + i, y, 0, 0);
            }
            for (int i = 0; i <= height; i++)
            {
                await Task.Delay(20);
                mouse_event(MOUSEEVENTF_MOVE, x, y + i, 0, 0);
            }

            // 模拟鼠标左键释放  
            mouse_event(MOUSEEVENTF_LEFTUP, x + width, y + height, 0, 0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 获取ListView在屏幕坐标中的位置
            System.Drawing.Point listViewLocation = listView.PointToScreen(Point.Empty);

            // 将鼠标移动到ListView的位置
            Cursor.Position = new System.Drawing.Point(listViewLocation.X + listView.Width / 2-310, listViewLocation.Y + listView.Height / 2 -40);
        }

        private void listView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (listView.SelectedItems.Count <= 0)
                { 
                    return;
                }

                //put selected files into a string array

                string[] files = new String[listView.SelectedItems.Count];

                int i = 0;
                foreach (ListViewItem item in listView.SelectedItems)
                {
                    files[i++] = item.Tag.ToString();
                }

                //create a dataobject holding this array as a filedrop

                DataObject data = new DataObject(DataFormats.FileDrop, files);

                //also add the selection as textdata

                data.SetData(DataFormats.StringFormat, files[0]);

                //Do DragDrop
                DoDragDrop(data, DragDropEffects.Copy);
            }


        }
        private void ListFolder(string directory)
        {

            String[] fileList = System.IO.Directory.GetFiles(directory);
            listView.Items.Clear();
            listView.Columns.Clear();
            listView.Columns.Add("Name", 300);
            listView.Columns.Add("Size", 100);
            listView.Columns.Add("Time", 200);

            foreach (string fileName in fileList)
            {
                //Show file name
                ListViewItem itemName = new ListViewItem(System.IO.Path.GetFileName(fileName));
                itemName.Tag = fileName;

                //Show file icon


                //Show file size
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);
                long size = fileInfo.Length;

                String strSize;
                if (size < 1024)
                {
                    strSize = size.ToString();
                }
                else if (size < 1024 * 1024)
                {
                    strSize = String.Format("{0:###.##}KB", (float)size / 1024);
                }
                else if (size < 1024 * 1024 * 1024)
                {
                    strSize = String.Format("{0:###.##}MB", (float)size / (1024 * 1024));
                }
                else
                {
                    strSize = String.Format("{0:###.##}GB", (float)size / (1024 * 1024 * 1024));
                }

                ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem();
                subItem.Text = strSize;
                subItem.Tag = size;
                itemName.SubItems.Add(subItem);

                //Show file time
                subItem = new ListViewItem.ListViewSubItem();
                DateTime fileTime = System.IO.File.GetLastWriteTime(fileName);

                subItem.Text = (string)fileTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"); ;
                subItem.Tag = fileTime;

                itemName.SubItems.Add(subItem);
                listView.Items.Add(itemName);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (Directory.Exists(textBox1.Text))
                {
                    // 用户输入的是一个文件夹路径
                    ListFolder(textBox1.Text);
                }
                else
                {
                    // 路径既不是文件也不是文件夹
                    // throw new Exception("路径既不是文件也不是文件夹");
                    MessageBox.Show("路径有误请重新输入", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
            catch (Exception)
            {

                throw;
            }
            
        }

    }
}
