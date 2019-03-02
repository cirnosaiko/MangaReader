using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()//初始化
        {
            InitializeComponent();
        }

        private void 推出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            Application.Exit();
        }

        uint MenuCount = 0;
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)//点击菜单
        {
            MenuCount++;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)//最小化到托盘
        {
            /*
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                notifyIcon1.BalloonTipTitle = "MangaViewer";
                notifyIcon1.BalloonTipText = "已经最小化到托盘";
                notifyIcon1.ShowBalloonTip(500);
            }
            */
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)//双击托盘打开窗口
        {
            this.Show();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)//窗口中鼠标移动
        {
            //toolStripStatusLabel1.Text = e.X.ToString()+" "+e.Y.ToString();
        }

        //获取所有图片文件的路径，和数组长度
        private string[] LoadPicturesPath(string Directory,out int ArrayLength)
        {
            string imgtype = "*.BMP|*.JPG|*.GIF|*.PNG";
            string[] ImageType = imgtype.Split('|');

            string[] bmpFile = System.IO.Directory.GetFiles(Directory, ImageType[0]);
            string[] jpgFile = System.IO.Directory.GetFiles(Directory, ImageType[1]);
            string[] gifFile = System.IO.Directory.GetFiles(Directory, ImageType[2]);
            string[] pngFile = System.IO.Directory.GetFiles(Directory, ImageType[3]);

            ArrayLength = bmpFile.Length + jpgFile.Length + gifFile.Length + pngFile.Length;

            string[] PicFilePath = new string[ArrayLength];
            bmpFile.CopyTo(PicFilePath, 0);
            jpgFile.CopyTo(PicFilePath, bmpFile.Length);
            gifFile.CopyTo(PicFilePath, jpgFile.Length);
            pngFile.CopyTo(PicFilePath, gifFile.Length);

            return PicFilePath;
        }

        private void SetFormTitle(string title)//设置窗口标题
        {
            this.Text = "MangaReader " + FinialFolder(title);
        }

        private void ClearAllPics()//清空释放所有panel1图片
        {
            foreach (PictureBox a in panel1.Controls)
            {
                DisposePicutrebox(a);
            }
            this.panel1.Controls.Clear();
            GC.Collect();
        }

        private void PicWidthHeight(string s, out int W, out int H)//返回图片长宽
        {
            Bitmap p = new Bitmap(s);
            W = p.Size.Width; 
            H = p.Size.Height;
            p.Dispose();
        }

        private string CoverPath(string[] PathArray, int ArrayLength)//返回封面地址
        {
            string coverPath = PathArray[0];
            for (int i = 0; i < ArrayLength; i++)
            {
                if (PathArray[i] != null && (PathArray[i].Contains("folder") || PathArray[i].Contains("Folder")))
                {
                    coverPath = PathArray[i];
                    break;
                }                    
            }
            return coverPath;
        }

        private void StartEndPicPosition()//第一张图和最后一张图位置
        {
            foreach (PictureBox x in panel1.Controls)//第一张图位置
            {
                if (x.Name.StartsWith("pic" + Add0(NumOfStartPic)))
                {
                    PositionYofStartPic = x.Location.Y;
                    break;
                }
            }
            foreach (PictureBox x in panel1.Controls)//第一张图位置
            {
                if (x.Name.StartsWith("pic" + Add0(NumOfEndPic)))
                {
                    PositionYofEndPic = x.Location.Y;
                    break;
                }
            }
        }

        int NumOfStartPic = 1;
        int NumOfEndPic = 5;
        int PositionYofStartPic = 0;
        int PositionYofEndPic = 0;
        int NumOfPics = 0;
        string[] PicturesPath;
        private void LoadPictures(string s)
        {
            int texboxname = 0;
            textBox2.Text = FinialFolder(s);
            SetFormTitle(textBox2.Text);


            
            PicturesPath = LoadPicturesPath(s, out NumOfPics);//所有图片路径

            if(PicturesPath.Length != 0)
                pictureBox1.Image = Image.FromFile(CoverPath(PicturesPath, NumOfPics));//封面图片                

            ClearAllPics();

            int HeightLocation = 6;
            int picHeight, picWidth;          
            for (int t = 0; t < NumOfPics; t++)                 //展示N个图片
            {
                PictureBox pic = new PictureBox();
                this.panel1.Controls.Add(pic);
                pic.BorderStyle = BorderStyle.FixedSingle;      //边框
                pic.SizeMode = PictureBoxSizeMode.Zoom;         //缩放

                PicWidthHeight(PicturesPath[t], out picWidth, out picHeight); //图片长宽

                pic.Width = panel1.Width - 25;                  //控件宽
                pic.Height = (int)((float)picHeight / (float)picWidth * (float)pic.Width);//控件长

                pic.Location = new Point(3, HeightLocation);    //位置
                HeightLocation += pic.Height + 6;

                //pic.Image = Image.FromFile(PicturesPath[t]);    //载入图片
                pic.Name = "pic" + (Add0(++ texboxname));              //控件名
            }
            firstLoadImages(PicturesPath);
            //LoadImages(PicturesPath);
            //StartEndPicPosition();//获取显示的所有图的位置
        }

        private void firstLoadImages(string[] pathArray)//初次加载图片
        {
            //需要加载的图片数量
            int NumOfLoadPics = pathArray.Length;
            if (NumOfLoadPics > 5)
                NumOfLoadPics = 5;

            for (int i = 1; i <= NumOfLoadPics; i++)//加载n张图片
            {
                foreach (PictureBox x in panel1.Controls)
                {
                    if (x.Name.StartsWith("pic" + Add0(i)))
                    {
                        x.Image = Image.FromFile(pathArray[i-1]);    //载入图片
                        break;
                    }
                }
            }
        }

        private void LoadImages(string[] pathArray, int StartPicNum, int EndPicNum, string c)//释放多余的图片，给加载新图片
        {
            if (c == "next")
            {
                //释放前一个
                foreach (PictureBox x in panel1.Controls)
                {
                    if (x.Name.StartsWith("pic" + Add0(StartPicNum - 1)))
                    {
                        DisposePicutrebox(x);
                        break;
                    }
                }
                //加载
                foreach (PictureBox x in panel1.Controls)
                {
                    if (x.Name.StartsWith("pic" + Add0(EndPicNum)))
                        x.Image = Image.FromFile(pathArray[EndPicNum - 1]);    //载入图片
                }
            }
            if (c == "previous")
            {
                //释放后一个
                foreach (PictureBox x in panel1.Controls)
                {
                    if (x.Name.StartsWith("pic" + Add0(EndPicNum + 1)))
                    {
                        DisposePicutrebox(x);
                        break;
                    }
                }
                //加载
                foreach (PictureBox x in panel1.Controls)
                {
                    if (x.Name.StartsWith("pic" + Add0(StartPicNum)))
                        x.Image = Image.FromFile(pathArray[StartPicNum - 1 ]);    //载入图片
                }
            }            
        }

        private string Add0(int i)//补足4位数
        {
            return i.ToString().PadLeft(4, '0');
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //ChangePanelSize();
            LoadPictures("d:");
            this.AllowDrop = true;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel2.Text = DateTime.Now.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            //dialog.Multiselect = true;//该值确定是否可以选择多个文件
            dialog.Description = "请选择文件夹";
            //dialog.Filter = "所有文件(*.*)|*.*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = dialog.SelectedPath;
                textBox1.Text = file;
                
                LoadPictures(file);
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            ChangePictureBoxSize();
            splitContainer1.SplitterDistance = 111;
            label2.Location = new Point(this.Width-157, label2.Location.Y);
        }

        private void ChangePictureBoxSize()
        {
            int PanelWidth = panel1.Width;
            //panel1.Width = FormWidth - pictureBox1.Width - 30;
            //textBox1.Width= FormWidth - pictureBox1.Width - 30;

            //int FormHeight = this.Height;
            //panel1.Height = FormHeight - textBox1.Height - 100;

            foreach (PictureBox p in panel1.Controls)
            {
                p.Width = PanelWidth - 30;
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)//拖拽导入
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
            /*
            Panel z = new Panel();
            this.Controls.Add(z);
            z.Name = "z";
            z.Height = this.Height/3;z.Width = this.Width;
            z.Location = new Point(0, this.Height / 3);
            z.Show();
            z.BringToFront();
            */

            Label t = new Label();
            this.Controls.Add(t);
            t.Text = "拖拽文件夹导入";
            t.Name = "t";
            t.BringToFront();
            t.BorderStyle = BorderStyle.FixedSingle;
            t.Size = new Size(this.Width, this.Height / 3);
            //t.Size = new Size(250, 30);
            t.Font = new Font("黑体",25);
            //t.ForeColor = Color.Transparent;
            t.Location = new Point(this.Width /2 - t.Size.Width / 2, this.Height /2 - t.Height / 2);
            t.BackColor = Color.Transparent;
            t.TextAlign = ContentAlignment.MiddleCenter;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)//拖拽导入
        {
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            textBox1.Text = path;
            if (PathIsFolder(path) == true)
                LoadPictures(path);
            else
                textBox1.Text = "File path illegal";

            foreach (Control x in this.Controls)
            {
                if ((x.Name == "t") && (x is Label))
                {
                    foreach (Control p in x.Controls)
                    {
                        p.Dispose();
                    }
                    x.Visible = false;
                    x.Dispose();
                    break;
                }
            }

        }

        private bool PathIsFolder(string path)//判断路径是否为文件夹
        {
            
            if (System.IO.File.Exists(path))
            {
                return false;// 是文件
            }
            else if (System.IO.Directory.Exists(path))
            {
                return true;// 是文件夹
            }
            else
            {
                return false; // 都不是
            }
        }

        private string FinialFolder(string path)
        {
            string[] p = path.Split('\\');
            return p[p.Length-1]; 
        }

        int NowCenterPic = 2;
        private void panel1_MouseWheel(object sender, MouseEventArgs e)//鼠标滚轮
        {
            panel1.VerticalScroll.Value += 10;
            panel1.Refresh();
            panel1.Invalidate();
            panel1.Update();

            //textBox2.Text=panel1.VerticalScroll.Maximum.ToString();

            //textBox1.Text = panel1.AutoScrollPosition.Y.ToString();
            //textBox2.Text = panel1.AutoScrollPosition.Y.ToString();

            //textBox1.Text += "  "+panel1.Height.ToString();

            int t = (-panel1.AutoScrollPosition.Y) / ((panel1.VerticalScroll.Maximum) / NumOfPics) + 2;
            if ((t-1==NowCenterPic) && t<= NumOfPics-2 && t>=3)//向下翻
            {
                NowCenterPic = t;
                LoadImages(PicturesPath, NowCenterPic - 2, NowCenterPic + 2 , "next");
            }
            if ((t + 1 == NowCenterPic) && t <= NumOfPics - 2 && t >= 3)//向上翻
            {
                NowCenterPic = t;
                LoadImages(PicturesPath, NowCenterPic - 2, NowCenterPic + 2 , "previous");
            }
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)//鼠标滚轮
        {
            this.panel1.Focus();
        }

        private void label1_Click(object sender, EventArgs e)//清空所有图片
        {
            ClearAllPics();
        }

        private void DisposePicutrebox(PictureBox t)//释放单个picturebox资源
        {
            if(t.Image != null)
                t.Image.Dispose();
            t.Image = null;
            //t.Dispose();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", textBox1.Text);
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)//分割条移动
        {
            ChangePictureBoxSize();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            Application.Exit();
        }

        private void panel1_Scroll(object sender, ScrollEventArgs e)//鼠标拖动滚动条
        {
            //textBox1.Text = System.DateTime.Now.ToLongTimeString();

            int t = (-panel1.AutoScrollPosition.Y) / ((panel1.VerticalScroll.Maximum) / NumOfPics) + 2;
            //textBox1.Text = "t="+ t.ToString() + ",nowcenterpic=" + NowCenterPic.ToString();

            if (t != NowCenterPic)
            {
                for(int i=-2;i<=2;i++)//释放
                {
                    if(!withinRange(t-2,t+2, NowCenterPic + i))//要释放的不在范围内                        
                    {
                        foreach (PictureBox x in panel1.Controls)
                        {
                            if (x.Name.StartsWith("pic" + Add0(NowCenterPic + i)))
                            {
                                DisposePicutrebox(x);
                                break;
                            }
                        }
                    }   
                }
                for (int i = -2; i <= 2; i++)//加载
                {
                    if (!withinRange(NowCenterPic - 2, NowCenterPic + 2, t + i))//要加载的不在范围内
                    {
                        //加载
                        foreach (PictureBox x in panel1.Controls)
                        {
                            if (x.Name.StartsWith("pic" + Add0(t + i)))
                                x.Image = Image.FromFile(PicturesPath[t + i - 1]);    //载入图片
                        }
                    }
                }
                NowCenterPic = t;
            }
        }

        private bool withinRange(int rangeMin, int rangeMax, int testNUM)
        {
            if (testNUM <= rangeMax && testNUM >= rangeMin)
                return true;
            else
                return false;
        }
    }
}
