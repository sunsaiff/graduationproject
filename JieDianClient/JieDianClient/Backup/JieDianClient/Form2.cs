using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CoAP.EndPoint;
using CoAP.EndPoint.Resources;
using System.IO;
using MySql.Data.MySqlClient;
using System.Threading;
using CoAP;

namespace JieDianClient
{
    public partial class Form2 : Form
    {
        public Form2()
        {        
            InitializeComponent();           
        }

        private void button1_Click(object sender, EventArgs e)
        {
             if (pictureBox1.Image != null)
                { //将图片对象image转换成缓冲流imageStream                 
                   MemoryStream imageStream = new MemoryStream();                  
                   pictureBox1.Image.Save(imageStream, System.Drawing.Imaging.ImageFormat.Jpeg); //获得图片的字节数组imageByte                  
                   byte[] imageByte = imageStream.GetBuffer();                   //建立数据库连接                  
                   MySqlConnection conn = new MySqlConnection("Server=localhost;Uid=root;Password=271828;Database=jiedian"); 
                   conn.Open();                   //设置命令参数                  
                   string insertStr="insert into jd_image(ip,time,image) values(?IP,?TIME,?IMAGEBYTE)";                 
                   MySqlCommand comm = new MySqlCommand();                 
                   comm.Connection = conn;                 
                   comm.CommandText = insertStr;                 
                   comm.CommandType = CommandType.Text;                  //设置数据库字段类型MediumBlob的值为图片字节数组imageByte                  
                   String ip1 = textBox1.Text.Trim(); 
                   comm.Parameters.Add(new MySqlParameter("?IP", MySqlDbType.VarChar)).Value = ip1;
                   String timenow = DateTime.Now.ToString();
                   comm.Parameters.Add(new MySqlParameter("?TIME", MySqlDbType.Datetime)).Value = timenow;
                   comm.Parameters.Add(new MySqlParameter("?imageByte", MySqlDbType.MediumBlob)).Value = imageByte;                   //执行命令                 
                   try {
                       comm.ExecuteNonQuery();
                       String str1 = "存入图片的时间为：";
                       MessageBox.Show(ip1);
                       rTB_log.Text = str1+timenow +"\n存入图片的IP为："+ip1; 
                       
                   }                 
                   catch (Exception ex)                 
                   {                      
                       MessageBox.Show(ex.ToString());                 
                   }                   
                   comm.Dispose();                 
                   conn.Close();                 
                   conn.Dispose();   
//////////////////////////////////////////////////////////////////////保存图片的代码
                   //////////////////////////////////////////////////////////////////////////////
                   String Opath = @"E:\实验室项目\IPV6项目\Coap_Hearth_Protection\Coap_Hearth_Protection（moni数据）\NjuptTestServer";
                   String photoname = DateTime.Now.Ticks.ToString();
                   if (Opath.Substring(Opath.Length - 1, 1) != @"/")
                       Opath = Opath + @"/";
                   String path1 = Opath + DateTime.Now.ToShortDateString();
                   // if (!Directory.Exists(path1))
                   //   Directory.CreateDirectory(path1);
                   System.Drawing.Bitmap objPic;
                   try
                   {
                       objPic = new System.Drawing.Bitmap(pictureBox1.Image);
                       //objNewPic = new System.Drawing.Bitmap(objPic, pictureBoxShow.Width, pictureBoxShow.Height);
                       //objNewPic=new System.Drawing.Bitmap(objPic,320,240);//图片保存的大小尺寸  
                       objPic.Save(Opath + "//" + "camera" + ".jpg", System.Drawing.Imaging.ImageFormat.Png);
                   }
                   catch (Exception exp) { throw exp; }
                   finally
                   {
                       objPic = null;
                       //objNewPic = null;
                   }
//////////////////////////////////////////////////////////////////////////////////////////////////////////
                 


               } 
        }

        private void button4_Click(object sender1, EventArgs ee)
        {
#region

            this.Text = "获取节点信息！";
            pictureBox1.Image = null;
            richTextBox1.ResetText();
            String payload = null;
            Request request = new Request(Method.GET);

                
            String re_uri = textBox1.Text + "/" + comboBox1.Text;
            if (comboBox1.Text == "camera") re_uri = re_uri + "?size=1";
            request.URI = new Uri(re_uri);
            MessageBox.Show(re_uri);

            request.SetPayload(payload);
            //request.Token = TokenManager.Instance.AcquireToken();
            //request.ResponseQueueEnabled = true;
            //request.SeparateResponseEnabled = true;

            request.Timeout += new EventHandler(request_Timeout);
            
            try
            {
                request.Respond += OnResponse;
                request.Send();                         
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed executing request: " + ex.Message);
            }
#endregion
            //Thread th = new Thread(ThreadChild);
            //th.Start(); 
           
        }

        void OnResponse(Object sender, ResponseEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler<ResponseEventArgs>(OnResponse), sender, e);
                return;
            }

            Response response = e.Response;
            //response.ContentType == MediaType.TextPlain, MediaType.ImageJpeg
            if (response == null)
            {
                MessageBox.Show("Request timeout");
            }
            else
            {         
                if (response.PayloadString != null)
                {
                    if (response.Payload == null) MessageBox.Show("Time (ms): " + response.RTT);
                    richTextBox1.Text = response.PayloadString;
                    tabControl2.SelectedIndex = 0;
                    //tabControl2.SelectTab("文本");
                }
          
                if (response.Payload == null) MessageBox.Show("response.Payload == null");
                else   if (MediaType.IsImage(response.ContentType))
                {
                    pictureBox1.Image = CoapClient.ImageByte.ByteToImage(response.Payload);
                    tabControl2.SelectedIndex = 1;
                    //tabControl2.SelectTab("Image");
                }
            }
            //if (!response.IsEmptyACK && !loop)
            //    Environment.Exit(0);
        }

        void request_Timeout(object sender, EventArgs e)
        {
            MessageBox.Show("Request timeout");
        }
        static void ThreadChild()
        { 
            MessageBox.Show("这是子线程执行的代码");
    
        }



        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = null;
            MySqlConnection sqlconnection = new MySqlConnection();
            sqlconnection.ConnectionString = "server=localhost;user id=root;password=271828;database=jiedian";
            sqlconnection.Open();


            //String ps = "'and password='";
            //String ed = "';";
            //String sql = "select * from jd_user where username='" + Tb_UserName.Text + ps + Mtb_MiMa.Text + ed;

            //string insertStr = "insert into jd_image(ip,time,image) values(?IP,?Time,?Image)";
            //MySqlCommand comm = new MySqlCommand();
            //comm.Connection = conn;
            //comm.CommandText = insertStr;
            //comm.CommandType = CommandType.Text;
            //comm.Parameters.Add(new MySqlParameter("?Image", MySqlDbType.MediumBlob)).Value = imageByte;
            //comm.Parameters.Add(new MySqlParameter("?Time", MySqlDbType.Datetime)).Value = DateTime.Now.ToString();
            //comm.Parameters.Add(new MySqlParameter("?IP", MySqlDbType.VarChar)).Value = textBox1.Text.Trim();  


            String chaxun = "select image from jd_image where ip=?IP and time=?Time";
            MySqlCommand comm = new MySqlCommand();
            comm.Connection = sqlconnection;
            comm.CommandText = chaxun;
            comm.CommandType = CommandType.Text;
            comm.Parameters.Add(new MySqlParameter("?Time", MySqlDbType.Datetime)).Value = tb_time.Text.Trim();
            comm.Parameters.Add(new MySqlParameter("?IP", MySqlDbType.VarChar)).Value = tb_ip.Text.Trim();
            MySqlDataReader dr = comm.ExecuteReader();
            if (dr.Read())
            {                  //读出图片字节数组至byte[]                  
                byte[] imageByte = new byte[dr.GetBytes(0, 0, null, 0, int.MaxValue)];
                dr.GetBytes(0, 0, imageByte, 0, imageByte.Length);                   //将图片字节数组加载入缓冲流   
                MemoryStream imageStream = new MemoryStream(imageByte);                   //从缓冲流生成图片      
                Image image = Image.FromStream(imageStream, true);
                pictureBox2.Image = image;
            }
            dr.Dispose();
            comm.Dispose();
            sqlconnection.Close();
            sqlconnection.Dispose();

   
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                MySqlConnection sqlconnection = new MySqlConnection();
                sqlconnection.ConnectionString = "server=localhost;user id=root;password=271828;database=jiedian";
                sqlconnection.Open();
                String sql = "select * from jd_user";
                MySqlCommand mySqlCommand = new MySqlCommand(sql, sqlconnection);
                MySqlDataAdapter sda = new MySqlDataAdapter(mySqlCommand);
                DataSet ds = new DataSet();
                sda.Fill(ds, "用户表");
                dataGridView2.DataSource = ds;
                dataGridView2.DataMember = "用户表";
                dataGridView2.AutoGenerateColumns = true;
                for (int i = 1; i < this.dataGridView1.ColumnCount; i++)
                {
                    this.dataGridView1.Columns[i].DefaultCellStyle.SelectionBackColor = Color.White;
                    this.dataGridView1.Columns[i].DefaultCellStyle.SelectionForeColor = Color.Black;
                    this.dataGridView1.Columns[i].ReadOnly = true;
                }
                sqlconnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show("错误：" + ee.Message, "错误");
            }
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedCells == null)             
            {                  
                MessageBox.Show("请选择要删¦除的项！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);             
            }             
            else             
            {
                if (dataGridView2.CurrentCell.ColumnIndex == 0)                
                {
                    string st = dataGridView2[0, dataGridView2.CurrentCell.RowIndex].Value.ToString();
                    MySqlConnection sqlconnection = new MySqlConnection();
                    sqlconnection.ConnectionString = "server=localhost;user id=root;password=271828;database=jiedian";
                    sqlconnection.Open();
                    String sql = "delete from jd_user where username='" + st + "'";
                    MySqlCommand mySqlCommand = new MySqlCommand(sql, sqlconnection);
                    mySqlCommand.ExecuteNonQuery();
                    MessageBox.Show("已删除！请刷新！");
                    sqlconnection.Close();
                 
                }             
            }         
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                MySqlConnection sqlconnection = new MySqlConnection();
                sqlconnection.ConnectionString = "server=localhost;user id=root;password=271828;database=jiedian";
                sqlconnection.Open();
                String sql = "select * from jd_user where username='"+textBox2.Text+"';";
                MySqlCommand mySqlCommand = new MySqlCommand(sql, sqlconnection);
                MySqlDataAdapter sda = new MySqlDataAdapter(mySqlCommand);
                DataSet ds = new DataSet();
                sda.Fill(ds, "用户表");
                dataGridView2.DataSource = ds;
                dataGridView2.DataMember = "用户表";
                dataGridView2.AutoGenerateColumns = true;
                for (int i = 1; i < this.dataGridView1.ColumnCount; i++)
                {
                    this.dataGridView1.Columns[i].DefaultCellStyle.SelectionBackColor = Color.White;
                    this.dataGridView1.Columns[i].DefaultCellStyle.SelectionForeColor = Color.Black;
                    this.dataGridView1.Columns[i].ReadOnly = true;
                }
                sqlconnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show("错误：" + ee.Message, "错误");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
          /*  if (tb_username.Text != null&&tb_mima!=null)
            {
                            //建立数据库连接                  
                MySqlConnection conn = new MySqlConnection("Server=localhost;Uid=root;Password=271828;Database=jiedian");
                conn.Open();                   //设置命令参数                  
                string insertStr = "insert into jd_user(username,password) values(?UN,?MM)";
                MySqlCommand comm = new MySqlCommand();
                comm.Connection = conn;
                comm.CommandText = insertStr;
                comm.CommandType = CommandType.Text;
                comm.Parameters.Add(new MySqlParameter("?UN", MySqlDbType.String)).Value = tb_username.Text.Trim();
                comm.Parameters.Add(new MySqlParameter("?MM", MySqlDbType.String)).Value = tb_mima.Text.Trim();
                try
                {
                    comm.ExecuteNonQuery();
                    MessageBox.Show("添加成功！！");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                } comm.Dispose();
                conn.Close();
                conn.Dispose();
            }*/
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
      
         
    
