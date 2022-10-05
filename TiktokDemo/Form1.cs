using CefSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
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
            string  url = txtURL.Text;
            chromiumWebBrowser1.LoadUrl(url);

            Thread.Sleep(1000 * 10);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("function tempFunction() {");
            sb.AppendLine("     var arry=[];");
            //sb.AppendLine("     var obj=document.getElementsByClassName('tiktok-1mo2fkg-DivUserLinkContainer e797se20');");
            sb.AppendLine("     var obj=document.getElementsByClassName('tiktok-rfcpgv-PUserDesc e797se29');");
            
            sb.AppendLine("  for (var i in obj){ arry.push(obj[i].innerHTML); }");


            sb.AppendLine("     return arry;");
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
                        FileStream  fs = File.OpenWrite(@"d:\tk.txt");
                        StreamWriter streamWriter = new StreamWriter(fs);

                        List<Object> list = (List<Object>)response.Result;
                        //MessageBox.Show(list.Count.ToString());
                        Regex regex = new Regex("/@([^\"]+)");

                        List<string> listTiktok = new List<string>();

                        foreach (Object item in list)
                        {
                            if (item != null && item is string)
                            {
                                string tempString = item.ToString();
                                Match m = regex.Match(tempString);
                                if (m.Success)

                                {
                                    string value = m.Groups[0].Value.Remove(0, 2);
                                    if (value.LastIndexOf("/live") > -1)
                                    {
                                        value = value.Substring(0, value.Length - 5);
                                      

                                    }
                                    streamWriter.WriteLine(value);
                                    listTiktok.Add(value);
                                }
                                else
                                {
                                    streamWriter.WriteLine(tempString);
                                    listTiktok.Add(tempString);

                                }
                            }

                          


                        }
                        streamWriter.Close();
                        fs.Close();


                        listBox1.DataSource = listTiktok;
                    }
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());

        }

        private void btnParse_Click_1(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("function tempFunction() {");
            sb.AppendLine("     var arry=[];");
            sb.AppendLine("     var obj=document.getElementsByClassName('tiktok-1mo2fkg-DivUserLinkContainer e797se20');");
            sb.AppendLine("  for (var i in obj){ arry.push(obj[i].innerHTML); }");
           
            
            sb.AppendLine("     return arry;");
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
                        List<Object> list = (List < Object > )response.Result;
                        Regex regex = new Regex("/@([^\"]+)");
                      
                        List<string> listTiktok = new List<string>();
                        
                        foreach (Object item in list)
                        {
                            if (item != null && item is string)
                            { 
                                string tempString = item.ToString();
                                Match m = regex.Match(tempString);
                                if (m.Success)

                                {
                                    string value = m.Groups[0].Value.Remove(0,2);
                                    if (value.LastIndexOf("/live")>-1)
                                    {
                                        value = value.Substring(0, value.Length - 5);
                                    }
                                    listTiktok.Add(value);
                                }
                            }



                        }

                       
                        listBox1.DataSource = listTiktok;
                    }
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ContextMenuStrip listboxMenu = new ContextMenuStrip();
            ToolStripMenuItem rightMenu = new ToolStripMenuItem("Copy");
            rightMenu.Click += new EventHandler(Copy_Click);
            listboxMenu.Items.AddRange(new ToolStripItem[] { rightMenu });
            listBox1.ContextMenuStrip = listboxMenu;
        }

        private void Copy_Click(object sender, System.EventArgs e)
        {
            try
            {
                Clipboard.SetText(listBox1.Items[listBox1.SelectedIndex].ToString());
            }
            catch { }

        }
    }
}
