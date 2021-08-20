using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;
using System.Timers;

namespace _0805
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitTimer();
        }
        int[] myArry = { 1, 2, 3 };
        char[] myAStr = { 'a', 'b', 'c' };
        byte[] myByte = { 001, 002 };
        private void button1_Click(object sender, EventArgs e)
        {
            string myPath = "a.txt";
            StreamWriter sw = new StreamWriter(myPath);
            foreach(char myChar in myAStr)
            {
                sw.Write(myChar);
            }
            sw.WriteLine();
            sw.WriteLine(sw.ToString());
            sw.Close();
            StreamReader sr = new StreamReader(myPath);
            MessageBox.Show("内容为:\n " + sr.ReadToEnd());
          
        }

        private int myMaxValue()
        {
            int a, b;
            a = Convert.ToInt32(textBox1.Text);
            b = Convert.ToInt32(textBox2.Text);
            return a > b ? a : b;
        }
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            MessageBox.Show("message");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            label2.Text = Convert.ToString(myMaxValue());
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

            label2.Text = Convert.ToString(myMaxValue());
        }

        public DataSet getpath()
        {
            //打开文件
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "Excel(*.xlsx)|*.xlsx|Excel(*.xls)|*.xls";
            file.InitialDirectory = "..\\";
            file.Multiselect = false;

            if (file.ShowDialog() == DialogResult.Cancel)return null;

            //判断文件后缀
            string  path = file.FileName;

            string fileSuffix = System.IO.Path.GetExtension(path);

            if (string.IsNullOrEmpty(fileSuffix))return null;
            //MessageBox.Show(path + "+++" + fileSuffix);
            return myExcel(path,fileSuffix);
        }
        public DataSet myExcel(string path, string fileSuffix)
        {
            //判断Excel文件是2003版本还是2007版本
            if (fileSuffix == ".xls")

                connString = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + path + ";" + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";

            else

                connString = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + path + ";" + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";

            //读取文件

            OleDbConnection conn = new OleDbConnection(connString);
            conn.Open();
            DataTable sTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            string sTable_Rows;
            for (int i = 0; i < sTable.Rows.Count; i++)
            {
                sTable_Rows = sTable.Rows[i][2].ToString().TrimStart('\'').Trim('\'', '$');
                listBox1.Items.Add(sTable_Rows);
            }
            String tableName;
            foreach (string i in listBox1.Items)
            {
                tableName = i + '$';
                string strExcels = "select * from [" + tableName + "]";
                using (OleDbDataAdapter cmd = new OleDbDataAdapter(strExcels, conn))
                {
                    cmd.Fill(ds,i);     //分别存在DataSet里相同worksheet名称的表中
                }
            }
            if (ds == null || ds.Tables.Count <= 0) return null;
            return ds;
        }
        string path;
        string fileSuffix;
        DataTable dt;
        string connString = "";
        DataSet ds = new DataSet();
        private void button2_Click(object sender, EventArgs e)
        {           
            dataGridView1.DataSource = null; //每次打开清空内容 
            dt = getpath().Tables[0];
            dataGridView1.DataSource =dt ;
            //修改样式
            //dataGridView1.Rows[0].DefaultCellStyle.BackColor = Color.FromArgb(0, 255, 255);
            dataGridView1.CurrentCell = null;
        }

        
        private void listBox1_SelectedValueChanged_1(object sender, EventArgs e)
        {
            string mySelectName = listBox1.SelectedItem.ToString();
            dt = ds.Tables[mySelectName];
            dataGridView1.DataSource = dt;
            dataGridView1.CurrentCell = null;
        }
        //读写 DatagridView
        int T_delay;
        //Form myform = new Form3();
        private void myDatagridView()
        {
            DataGridView myView = dataGridView1;
            int cmd, record=0, count=0, totalcount=1;
            for ( int i=1;i< myView.Rows.Count;i++)
            {
                string conStr = Convert.ToString(myView.Rows[i].Cells[3].Value);
                if (conStr =="TRUE" || conStr == "真")
                {
                    myView.Rows[i].Cells[3].Style.BackColor = Color.LightGreen;
                    cmd = Convert.ToInt32(myView.Rows[i].Cells[4].Value);
                    T_delay = Convert.ToInt32(myView.Rows[i].Cells[5].Value);
                    if (T_delay > 0)
                    {
                        lab_totaltime.Text = T_delay.ToString();
                        timer1.Start();
                    }
                    if (cmd == 10) // CMD:10 表示循环开始位置标识; 11 表示循环结束位置标识
                    {
                        i = i + 1;  //下一行为循环的第一步
                        record = i;
                        totalcount = Convert.ToInt32(myView.Rows[i - 1].Cells[11].Value); // 循环标志行第10列里写了循环数
                    }
                    else if (cmd == 11)
                    {
                        if (count < totalcount)
                        {
                            i = record;
                            count += 1;
                            //labelCurrentCount.Text = count.ToString();
                            //labelResidueCount.Text = (totalcount - count).ToString();
                        }
                        else
                        {
                            count = 0;
                        }
                    }
                    else if (cmd == 999)
                    {
                        lab_end.Text = "测试结束!";
                    }
                    else
                    {
                        //添加处理函数处
           
                    }

                }        
            }
        }

        private void myDtime(double T_delay)
        {
            myTimer.Start();
        }

        private void btn_Start_Click(object sender, EventArgs e)
        {
            myDatagridView();
        }

        /// <summary>
        /// 定义Timer类变量, 不使用Timer控件(占用form 主进程)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        System.Timers.Timer myTimer = new System.Timers.Timer();
        //定义委托
        private void InitTimer()
        {
            myTimer.Interval = 1000;
            myTimer.AutoReset = true; //执行一次还是一直执行
            myTimer.Enabled = false; 
            // 是否执行System.Timers.Timer.Elapsed 事件
            //绑定Elapse事件
            myTimer.Elapsed += new System.Timers.ElapsedEventHandler(myTimer_Elapsed);          
        }
        private void myTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            T_delay -= 1;
            SetTB("倒计时: " + T_delay + "s");
            if (T_delay <= 0)
            {
                myTimer.Stop();
            }
        }
        private delegate void SetTBMethodInvok(String value);
        private void SetTB(string value)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new SetTBMethodInvok(SetTB), value);
            }
            else
            {
                //myTimer 做的事
                this.lab_time.Text = value;
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            int t = 2;
            do
            {

                T_delay = Convert.ToInt32(dataGridView1.Rows[t].Cells[5].Value);
                timer1.Start();
                dataGridView1.Rows[t].Cells[3].Style.BackColor = Color.LightGreen;
                t++;
            } while (t <= dataGridView1.Rows.Count);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            T_delay = (T_delay - 1);
            lab_time.Text="倒计时: " + T_delay + "s";
            if (T_delay <= 0)
            {
                timer1.Stop();
                T_delay = 0;
            }
        }
    }
}
