using CefSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TiktokDemo
{
    public partial class Form1 : Form
{

       
        public Form1()
        {
            InitializeComponent();
        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            btnParse.Enabled = true;
            string  url = txtURL.Text;
            chromiumWebBrowser1.LoadUrl(url);

           




        }

        public static ServiceStack.Redis.RedisClient client = new ServiceStack.Redis.RedisClient("127.0.0.1", 6379);


        HttpListener listener = new HttpListener();
        private void btnParse_Click_1(object sender, EventArgs e)
        {
            //timer1.Enabled = false;

            timer1.Enabled= true;
            btnParse.Enabled= false;
            if (listener.IsListening)
            {
                listener.Stop();
            }
           
            listener.Prefixes.Add("http://localhost:7878/"); //要监听的url范围
            listener.Start(); //开始监听端口，接收客户端请求
            Console.WriteLine("Listening");

            ThreadStart threadStart = new ThreadStart(theadFun);
            Thread thread = new Thread(threadStart);
            thread.Start();

        }

        private void theadFun()
        {
            try
            {
                while (true)
                {
                    //获取一个客户端请求为止
                    HttpListenerContext context = listener.GetContext();
                    //将其处理过程放入线程池
                    System.Threading.ThreadPool.QueueUserWorkItem(ProcessHttpClient, context);
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            finally
            {
                listener.Stop(); //关闭HttpListener
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
          
        }


        //客户请求处理
        static void ProcessHttpClient(object obj)
        {
            HttpListenerContext context = obj as HttpListenerContext;
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            //do something as you want
            string responseString = client.Get<string>("caipiao") ;// string.Format(" {0}", DateTime.Now);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            //关闭输出流，释放相应资源
            output.Close();
        }


        private void timer1_Tick_1(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("function tempFunction() {");

            //sb.AppendLine("     var obj=document.getElementsByClassName('tiktok-1mo2fkg-DivUserLinkContainer e797se20');");
            //sb.AppendLine("     var obj=document.getElementsByClassName('tiktok-rfcpgv-PUserDesc e797se29');");
            sb.AppendLine("     var obj=document.body;");



            // sb.AppendLine(" alert(obj.innerHTML) ; ");


            sb.AppendLine("     return obj.innerHTML;");
            sb.AppendLine("}");
            sb.AppendLine("tempFunction();");




            var task = chromiumWebBrowser1.EvaluateScriptAsync(sb.ToString());
            task.ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    var response = t.Result;

                    if (response.Success == true)
                    {


                        var result = response.Result.ToString();
                        client.Set<string>("caipiao", result.ToString());




                    }
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
