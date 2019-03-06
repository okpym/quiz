using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Vision.v1;
using Google.Apis.Services;
using Google.Apis.Vision.v1.Data;
using System.Diagnostics;
using System.Net;
using mshtml;
using Moda.Korean.TwitterKoreanProcessorCS;
using Newtonsoft.Json;

using System.Runtime.InteropServices;
using System.Drawing.Imaging;

using Image = System.Drawing.Image;
using Color = System.Drawing.Color;
using System.Text.RegularExpressions;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        int[] num = new int[4]; // 19.01.12a
        int[] num_sample = new int[4];
        string[] answer = new string[4];
        string question;
        string summary;
        string summary2;
        int max = 0;
        int max_backup = 0;
        int[] number = new int[4];
        int answer_cnt;
        int counter_timer1;
        int counter_timer2;
        Stopwatch stopwatch = new Stopwatch();
        IntPtr ptr_quiz;

        public Form1()
        {
            InitializeComponent();
            InitializeTimer();

            KeyPreview = true;

            textBox9.Text = "6";
            textBox10.Text = "6";
            textBox11.Text = "20";
            textBox12.Text = "30";

            progressBar1.Minimum = 0;
            progressBar2.Minimum = 0;
            progressBar3.Minimum = 0;
            progressBar4.Minimum = 0;
            progressBar5.Minimum = 0;

            progressBar1.Maximum = 100;
            progressBar2.Maximum = 100;
            progressBar3.Maximum = 100;
            progressBar4.Maximum = 100;
            progressBar5.Maximum = 100;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            capture_and_search();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            // Capture
            print_picture();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter("QfeatQnA.txt", true))
            {
                // First Line
                if (radioButton5.Checked == true)
                {
                    file.WriteLine("2");
                    textBox2.Text = "O";
                    textBox3.Text = "X";
                }
                else if (radioButton6.Checked == true)
                {
                    file.WriteLine("3");
                }
                else if (radioButton7.Checked == true)
                {
                    file.WriteLine("4");
                }
                else
                {
                    file.WriteLine("-1");
                }

                // Second line
                if (radioButton1.Checked == true)
                {
                    file.WriteLine("1");
                }
                else if (radioButton2.Checked == true)
                {
                    file.WriteLine("2");
                }
                else if (radioButton3.Checked == true)
                {
                    file.WriteLine("3");
                }
                else if (radioButton4.Checked == true)
                {
                    file.WriteLine("4");
                }
                else
                {
                    file.WriteLine("-1");
                }

                file.WriteLine(question);
                file.WriteLine(textBox2.Text);
                file.WriteLine(textBox3.Text);
                if (radioButton5.Checked == false)
                {
                    file.WriteLine(textBox4.Text);
                }
                if (radioButton5.Checked == false && radioButton6.Checked == false)
                {
                    file.WriteLine(textBox5.Text);
                }
            }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter("chance.txt", true))
            {
                // First Line
                file.WriteLine(num[0].ToString() + " " + num[1].ToString() + " " + num[2].ToString() + " " + num[3].ToString());
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

            richTextBox1.Clear();
            richTextBox1.SelectionFont = new Font("Arial", 16);
            richTextBox1.SelectionColor = Color.Red;
            richTextBox1.SelectedText = "3초" + "\n";

            Thread.Sleep(3000);

            ScreenCapture sc = new ScreenCapture();
            ptr_quiz = sc.CapturePtr();

            pictureBox2.Image = print_picture();
            richTextBox1.SelectedText = ptr_quiz.ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if(timer1.Enabled == false)
            {
                timer1.Enabled = true;
                button5.Text = "Start";
            }
            else
            {
                timer1.Enabled = false;
                button5.Text = "Stop";
            }
        }


        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F11)
            {
                capture_and_search();
            }
            else if (e.KeyData == Keys.F12)
            {
                // Capture
                print_picture();
            }
        }

        private void InitializeTimer()
        {
            counter_timer1 = 0;
            counter_timer2 = 0;
            timer1.Interval = 100;
            //timer1.Enabled = true;
            // Hook up timer's tick event handler.  
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);

        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {

            // Exit loop code.  
            //timer1.Enabled = false;
            //richTextBox1.Text += "1";

            ScreenCapture sc = new ScreenCapture();
            Image img = sc.CaptureWindow(ptr_quiz, Int32.Parse(textBox9.Text), Int32.Parse(textBox10.Text), Int32.Parse(textBox11.Text), Int32.Parse(textBox12.Text));

            Bitmap bmp = new Bitmap(img);
            
            Color clr = bmp.GetPixel(5, 5); // Get the color of pixel at position 5,5
            int red = clr.R;
            int green = clr.G;
            int blue = clr.B;

            Color clr2 = bmp.GetPixel(5, bmp.Height/3); // Get the color of pixel at position 5,5
            int red2= clr2.R;
            int green2 = clr2.G;
            int blue2 = clr2.B;

            Color clr3 = bmp.GetPixel(bmp.Width - 5, bmp.Height/3); // Get the color of pixel at position 5,5
            int red3 = clr3.R;
            int green3 = clr3.G;
            int blue3 = clr3.B;

            bmp.SetPixel(5, 5, Color.Black);
            bmp.SetPixel(5, bmp.Height/3, Color.Black);
            bmp.SetPixel(bmp.Width - 5, bmp.Height / 3, Color.Black);
            

            pictureBox2.Image = bmp;


            //Color pixel = sc.GetPixelColor(ptr_quiz, 100, 100);

            if (clr.Name == "ffffffff" && clr2.Name == "ffffffff" && clr3.Name == "ffffffff" && counter_timer1 == 0)
            {
                if (counter_timer2 < 3)
                {
                    counter_timer2++;
                }
                else
                {
                    richTextBox1.Text = "Question";
                    counter_timer1 = 100;
                    counter_timer2 = 0;

                    capture_and_search();
                }
            }
            else
            {
                if (counter_timer1 > 0)
                {
                    counter_timer1--;
                    progressBar5.Value = counter_timer1;
                    richTextBox1.Text += "-";
                }
                counter_timer2 = 0;
            }
        }

        public static byte[] ImageToByte(System.Drawing.Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        private void init_qna()
        {
            for (int i = 0; i < 4; i++)
            {
                this.answer[i] = "";
                this.number[i] = 0;
                this.num[i] = 0;
                this.num_sample[i] = 0;
            }
            this.question = "";
            this.answer_cnt = 0;
            this.max = 0;
            this.max_backup = 0;

            progressBar1.Value = 0;
            progressBar2.Value = 0;
            progressBar3.Value = 0;
            progressBar4.Value = 0;

            richTextBox1.Clear();
        }

        private Image print_picture()
        {
            ScreenCapture sc = new ScreenCapture();
            Image img = sc.CaptureWindow(ptr_quiz, Int32.Parse(textBox9.Text), Int32.Parse(textBox10.Text), Int32.Parse(textBox11.Text), Int32.Parse(textBox12.Text));
            return img;
        }

        private string text_change(string String)
        {
            textBox1.BackColor = System.Drawing.Color.White;

            // 삭제 문구
            string[] lines1 = System.IO.File.ReadAllLines("erase.txt", Encoding.Default);
            foreach (string line in lines1)
            {
                String = String.Replace(line, "");
            }

            // 반전
            string[] lines2 = System.IO.File.ReadAllLines("reverse.txt", Encoding.Default);
            foreach (string line in lines2)
            {
                if (-1 != String.IndexOf(line))
                {
                    textBox1.BackColor = System.Drawing.Color.Red;
                    //String = String.Replace(line, "");
                }
                else
                {
                    //textBox1.BackColor = System.Drawing.Color.White;
                }
            }
            String = String.Replace("  ", " ");
            String = String.Replace("  ", " ");
            return String;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = checkBox1.Checked;
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.AbsoluteUri == webBrowser1.Url.AbsoluteUri)
            {
                high_light(1, 1);
                print_answer();
            }
        }

        private void webBrowser3_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.AbsoluteUri == webBrowser3.Url.AbsoluteUri)
            {
                high_light(3, 1);
                print_answer();
            }
        }

        private void print_answer()
        {
            int max = 0;
            int max_num = 5;
            int total = num[0] + num[1] + num[2] + num[3];
            textBox7.Text = this.num[0].ToString() + " " + this.num[1].ToString() + " " + this.num[2].ToString() + " " + this.num[3].ToString() + "\r\n";
            for (int i = 0; i < 4; i++)
            {
                if (max < num[i])
                {
                    max = num[i];
                    max_num = i;
                }
            }
            if (max_num != 5 && max > 2)
            {
                textBox7.Text += (max_num + 1).ToString() + ": " + answer[max_num] + " " + (max * 100 / total).ToString() + "%";
            }


            if (total != 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    switch (i)
                    {
                        case 0:
                            progressBar1.Value = num[i] * 100 / total;
                            break;
                        case 1:
                            progressBar2.Value = num[i] * 100 / total;
                            break;
                        case 2:
                            progressBar3.Value = num[i] * 100 / total;
                            break;
                        case 3:
                            progressBar4.Value = num[i] * 100 / total;
                            break;
                        default:
                            break;

                    }
                }
            }
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;

            textBox2.BackColor = Color.White;
            textBox3.BackColor = Color.White;
            textBox4.BackColor = Color.White;
            textBox5.BackColor = Color.White;

            switch (max_num)
            {
                case 0:
                    radioButton1.Checked = true;
                    textBox2.BackColor = color_of_question(num[0], num[0] + num[1] + num[2] + num[3]);
                    break;
                case 1:
                    radioButton2.Checked = true;
                    textBox3.BackColor = color_of_question(num[1], num[0] + num[1] + num[2] + num[3]);
                    break;
                case 2:
                    radioButton3.Checked = true;
                    textBox4.BackColor = color_of_question(num[2], num[0] + num[1] + num[2] + num[3]);
                    break;
                case 3:
                    radioButton4.Checked = true;
                    textBox5.BackColor = color_of_question(num[3], num[0] + num[1] + num[2] + num[3]);
                    break;
                default:
                    break;
            }
        }

        private Color color_of_question(int num, int total)
        {
            int percentage = num * 100 / total;
            if (total >= 5 && percentage >= 99 ||
               total >= 10 && percentage >= 97 ||
               total >= 25 && percentage >= 95 ||
               total >= 100 && percentage >= 90 ||
               total >= 1000 && percentage >= 85 ||
               total >= 5000 && percentage >= 75)
            {
                return Color.LightGreen;
            }
            else if (total >= 5 && percentage >= 70 ||
                    total >= 10 && percentage >= 70 ||
                    total >= 25 && percentage >= 70 ||
                    total >= 100 && percentage >= 50 ||
                    total >= 1000 && percentage >= 50 ||
                    total >= 5000 && percentage >= 50)
            {
                return Color.Yellow;
            }
            else
            {
                return Color.Salmon;
            }

        }

        public int WordCheck(string String, string Word)
        {
            string[] StringArray = String.Split(new string[] { Word }, StringSplitOptions.None);

            return StringArray.Length - 1;
        }


        private void high_light(int num, int mul)
        {
            int[] count = new int[4];

            IHTMLDocument2 doc2;
            if (num == 1)
            {
                doc2 = (mshtml.IHTMLDocument2)webBrowser1.Document.DomDocument;
            }
            else
            {
                doc2 = (mshtml.IHTMLDocument2)webBrowser3.Document.DomDocument;
            }

            string[] ReplacementTag = new string[5];


            ReplacementTag[0] = "<span style='background-Color: rgb(255, 0, 0);'>";
            ReplacementTag[1] = "<span style='background-Color: rgb(29, 219, 22);'>";
            ReplacementTag[2] = "<span style='background-Color: rgb(255, 228, 0);'>";
            ReplacementTag[3] = "<span style='background-Color: rgb(103, 153, 255);'>";
            ReplacementTag[4] = "<span style='background-Color: rgb(36, 252, 255);'>";

            StringBuilder strBuilder = new StringBuilder(doc2.body.outerHTML);
            string HTMLString = strBuilder.ToString();

            List<string> SearchWords = new List<string>();

            for (int i = 0; i < 5; i++)
            {
                // Question
                if (i == 4)
                {
                    string str = Regex.Replace(question, @"\p{Lu}", "");
                    str = Regex.Replace(str, @"\p{Ll}", "");

                    string[] split = str.Split(' ');
                    foreach (string item in split)
                    {
                        if (item.Count() > 1)
                        {
                            SearchWords.Add(item);
                        }
                    }
                }
                // Anwser
                else if (answer[i] != null)
                {
                    string[] split = answer[i].Split(' ');
                    foreach (string item in split)
                    {
                        if (item.Count() > 1)
                        {
                            SearchWords.Add(item);
                        }
                    }
                }

                // 표시하기
                foreach (string item in SearchWords)
                {
                    int index = HTMLString.IndexOf(item, 0, StringComparison.InvariantCultureIgnoreCase);
                    while (index > 0 && index < HTMLString.Length)
                    {
                        HTMLString = HTMLString.Insert(index, ReplacementTag[i]);
                        HTMLString = HTMLString.Insert(index + item.Length + ReplacementTag[i].Length, "</span>");
                        index = HTMLString.IndexOf(item, index + item.Length + ReplacementTag[i].Length + 7, StringComparison.InvariantCultureIgnoreCase);
                        // <19.01.12a
                        if (i < 4)
                        {
                            count[i] += 1;
                            this.num[i] += mul;
                        }
                        // 19.01.12a>
                    }
                }
                SearchWords.Clear();
                doc2.body.innerHTML = HTMLString;
            }

            // Last Seconds
            textBox8.Text += "  " + stopwatch.Elapsed.ToString();
            textBox8.Text += ":  " + count[0].ToString() + " " + count[1].ToString() + " " + count[2].ToString() + " " + count[3].ToString() + "  ";

            if (num == 1)
            {
                textBox8.Text += "구글";
            }
            else if (num == 2)
            {
                textBox8.Text += "다음";
            }
            else if (num == 3)
            {
                textBox8.Text += "구글요약";
            }
            textBox8.Text += "\r\n";
        }

        private void search_word(string url, int mul, string name)
        {
            int[] count = new int[4];

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            // 요청, 응답 받기
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string status = response.StatusCode.ToString();

            if (status == "OK")
            {
                // 응답 Stream 읽기
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);//, Encoding.UTF8);

                // 응답 Stream -> 응답 String 변환
                string HTMLString = reader.ReadToEnd();

                List<string> SearchWords = new List<string>();

                for (int i = 0; i < 5; i++)
                {
                    if (i == 4)
                    {
                        string[] split = question.Split(' ');
                        foreach (string item in split)
                        {
                            if (item.Count() > 1)
                            {
                                SearchWords.Add(item);
                            }
                        }
                    }
                    else if (answer[i] != null)
                    {
                        string[] split = answer[i].Split(' ');
                        foreach (string item in split)
                        {
                            if (item.Count() > 1)
                            {
                                SearchWords.Add(item);
                            }
                        }
                    }
                    foreach (string item in SearchWords)
                    {
                        int index = HTMLString.IndexOf(item, 0, StringComparison.InvariantCultureIgnoreCase);
                        while (index > 0 && index < HTMLString.Length)
                        {
                            index = HTMLString.IndexOf(item, index + item.Length, StringComparison.InvariantCultureIgnoreCase);
                            // Count
                            if (i < 4)
                            {
                                this.num[i] += mul;
                                count[i] += 1;
                            }
                        }
                    }
                    SearchWords.Clear();
                }

                // Last Seconds
                textBox8.Text += "  " + stopwatch.Elapsed.ToString();
                textBox8.Text += ":  " + count[0].ToString() + " " + count[1].ToString() + " " + count[2].ToString() + " " + count[3].ToString() + " " + name + "\r\n";
            }
            else
            {
                Console.WriteLine("Error 발생=" + status);
            }
        }

        private string Change_RichText(string str)
        {

            string change = str;
            //change = change.Replace(@"<b>",@"\b ");
            //change = change.Replace(@"</b>", @"\b0");

            change = change.Replace(@"<b>",@"<<");
            change = change.Replace(@"</b>", @">>");
            return change;
        }

        private void Naver_Api(string query, string type, int mul, string name)
        {
            int[] count = new int[4];
            string max_string = "";

            string display = "100";   // display 10~100까지 읽음
            string sort = "sim";     // sort sim(유사도순) date(날짜순)

            string url = "https://openapi.naver.com/v1/search/" + type + "?query=" + query + "&display=" + display + "&sort=" + sort; // 결과가 JSON 포맷

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            string c_id = "1kwVZ3_v8s26Ix9WWfhN";
            string c_pw = "cNqeOh_NJX";

            request.Headers.Add("X-Naver-Client-Id", c_id); // 클라이언트 아이디
            request.Headers.Add("X-Naver-Client-Secret", c_pw);       // 클라이언트 시크릿

            // 요청, 응답 받기
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string status = response.StatusCode.ToString();
                if (status == "OK")
                {
                    // 응답 Stream 읽기
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);

                    // 응답 Stream -> 응답 String 변환
                    string text = reader.ReadToEnd();
                    dynamic array = JsonConvert.DeserializeObject(text);

                    int total = array.display;

                    for (int j = 0; j < total; j++)
                    {
                        List<string> SearchWords = new List<string>();
                        List<string> QuestionWords = new List<string>();

                        for (int i = 0; i < 4; i++)
                        {
                            if (answer[i] != null)
                            {
                                string[] split = answer[i].Split(' ');
                                foreach (string item in split)
                                {
                                    if (item.Count() > 1) // 2글자 이상
                                    {
                                        SearchWords.Add(item);
                                    }
                                }
                            }
                            string title = array.items[j].ToString();
                            foreach (string item in SearchWords)
                            {
                              
                                if (WordCheck(title, item) > 0)
                                {
                                    count[i] += WordCheck(title, item);
                                }
                            }
                            //// 19.03.03 New                
                            int cnt = 0;
                            string description = array.items[j].description.ToString();
                            string[] split2 = summary2.Split(' ');
                            foreach (string item in split2)
                            {
                                SearchWords.Add(item);
                            }
                            foreach (string item in SearchWords)
                            {
                                if (WordCheck(description, item) > 0)
                                {
                                    cnt++;
                                }
                            }
                            if (cnt > max)
                            {
                                max = cnt;
                                max_string = description;
                            }
                            SearchWords.Clear();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Error 발생=" + status);
                }
            }
            catch (WebException e)
            {
            }
            if (mul != 0)
            {
                textBox8.Text += "  " + stopwatch.Elapsed.ToString();
                textBox8.Text += ":  " + count[0].ToString() + " " + count[1].ToString() + " " + count[2].ToString() + " " + count[3].ToString() + " " + name + "\r\n";

                for(int i = 0; i<4;i++)
                    num[i] += count[i];


                max_string = Change_RichText(max_string);
                if (max_backup < max)
                {
                    richTextBox1.Clear();
                    //richTextBox1.Rtf = max_string.ToString();
                    richTextBox1.SelectedText = max + ":" + max_string + "\r\n" + "\r\n";
                    max_backup = max;

                }
                else if(max_backup == max)
                {
                    richTextBox1.SelectedText += max + ":" + max_string + "\r\n" + "\r\n";
                }
                max = 0;
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    num_sample[i] += count[i];
                }
            }
        }

        private void Daum_Api(string query, string type, int mul, string name)
        {
            int[] count = new int[4];
            string max_string = "";

            string page = "1";   // page 1~50까지 읽음
            string sort = "accuracy";     // sort accuracy(유사도순) recency(날짜순)
            string size = "50";     // 한 페이지에 보여질 문서의 개수 1~50

            string url = "https://dapi.kakao.com/v2/search/" + type + "?query=" + query + "&page=" + page + "&sort=" + sort+ "&size=" + size; // 결과가 JSON 포맷

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            string authorization = "be52e80cc2fdea561e24ef49c56847a7";

            request.Headers.Add("Authorization", "KakaoAK "+ authorization);       // 클라이언트 시크릿

            // 요청, 응답 받기
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string status = response.StatusCode.ToString();
                if (status == "OK")
                {
                    // 응답 Stream 읽기
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);

                    // 응답 Stream -> 응답 String 변환
                    string text = reader.ReadToEnd();
                    dynamic array = JsonConvert.DeserializeObject(text);

                    int total;
                    if(array.meta.pageable_count > 50)
                    {
                        total = 50;     
                    }
                    else
                    {
                        total = array.meta.pageable_count;
                    }                

                    for (int j = 0; j < total; j++)
                    {
                        List<string> SearchWords = new List<string>();

                        for (int i = 0; i < 4; i++)
                        {
                            if (answer[i] != null)
                            {
                                string[] split = answer[i].Split(' ');
                                foreach (string item in split)
                                {
                                    if (item.Count() > 1)
                                    {
                                        SearchWords.Add(item);
                                    }
                                }
                            }
                            foreach (string item in SearchWords)
                            {
                                string title = array.documents[j].ToString();
                                if (WordCheck(title, item) > 0)
                                {
                                    count[i] += WordCheck(title, item);
                                    
                                }
                            }

                            //// 19.03.03 New                
                            int cnt = 0;
                            string description = array.documents[j].contents.ToString();
                            string[] split2 = summary2.Split(' ');
                            foreach (string item in split2)
                            {
                                SearchWords.Add(item);
                            }
                            foreach (string item in SearchWords)
                            {
                                if (WordCheck(description, item) > 0)
                                {
                                    cnt++;
                                }
                            }
                            if (cnt > max)
                            {
                                max = cnt;
                                max_string = description;
                            }
                            SearchWords.Clear();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Error 발생=" + status);
                }
            }
            catch (WebException e)
            {
            }
            textBox8.Text += "  " + stopwatch.Elapsed.ToString();
            textBox8.Text += ":  " + count[0].ToString() + " " + count[1].ToString() + " " + count[2].ToString() + " " + count[3].ToString() + " " + name + "\r\n";

            for (int i = 0; i < 4; i++)
                num[i] += count[i];

            max_string = Change_RichText(max_string);
            if (max_backup < max)
            {
                richTextBox1.Clear();

               richTextBox1.SelectedText = max + ":" + max_string + "\r\n" + "\r\n";
                max_backup = max;
            }
            else if (max_backup == max)
            {
                richTextBox1.SelectedText += max + ":" + max_string + "\r\n" + "\r\n";

            }
            max = 0;
        }

        static string CleanInput(string strIn)
        {         
            string str = Regex.Replace(strIn, @"\p{P}", "");
            str = Regex.Replace(str, @"\p{S}", "");
            str = str.Replace("  ", " ");
            str = str.Trim();

            return str;
        }

        private void summary_qna()
        {
            // 필요없는 문구 없애기
            question = text_change(question);

            // 공간 없애기
            question = CleanInput(question);
            /*
            question = question.Replace("\n", " ");
            question = question.Replace("?", "");
            question = question.Replace("|", "");
            question = question.Replace(",", "");
            question = question.Replace(".", "");
            question = question.Replace("[", "");
            question = question.Replace("]", "");
            question = question.Replace("!", "");
            question = question.Replace("\"", "");

            question = question.Replace("/", "");
            
            question = question.Replace("  ", " ");
            question = question.Trim(); // 선후행 공백 제거
            */
            
            for (int i = 0; i < 4; i++)
            {
                if (answer[i] != null)
                {
                    answer[i] = CleanInput(answer[i]);
                    
                }
            }
            // 중복된 문구 없애기
            string[] split = answer[0].Split(' ');      // 답 1에 있는 얘를 추출

            for (int i = 1; i < 4; i++)
            {
                if (answer[i] != "")
                {
                    string[] split1 = answer[i].Split(' ');

                    foreach (string item in split)
                    {
                        foreach (string item1 in split1)
                        {
                            if (item1.Contains(item) || item.Contains(item1))
                            { 
                                answer[i] = answer[i].Replace(item, "");
                                answer[i] = answer[i].Trim();
                                answer[0] = answer[0].Replace(item, "");
                                answer[0] = answer[0].Trim();
                            }
                        }
                    }
                }
            }

            ///////////////////////////////////
            textBox1.Text = question + "\r\n";
            textBox2.Text = answer[0];
            textBox3.Text = answer[1];
            textBox4.Text = answer[2];
            textBox5.Text = answer[3];

            textBox8.Text = answer[0] + "  " + answer[1] + "  " + answer[2] + "  " + answer[3] +"\r\n" +"\r\n";

            // Question 명사, 동사 추출
            StringBuilder result = new StringBuilder();
            var tokens = TwitterKoreanProcessorCS.Tokenize(question);

            foreach (var token in tokens)
            {
                if (token.Pos.ToString() == "Noun" || token.Pos.ToString() == "ProperNoun" || token.Pos.ToString() == "Verb")
                {
                    result.AppendFormat(format: "{0} ",
                    args: new object[] { token.Text });
                }
            }
            summary = result.ToString();
            textBox1.Text += summary + "\r\n";

            // 명사만
            StringBuilder result2 = new StringBuilder();
            foreach (var token in tokens)
            {
                if (token.Pos.ToString() == "Noun" || token.Pos.ToString() == "ProperNoun")
                {
                    result2.AppendFormat(format: "{0} ",
                    args: new object[] { token.Text });
                }
            }
            summary2 = result2.ToString();
            textBox1.Text += summary2 + "\r\n";

        }

        private void Wiki_QA(string question)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://aiopen.etri.re.kr:8000/WikiQA");
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"access_key\": \"cad0208b-f4a6-4afa-b5fa-8b60ccf76a40\", " +
                                  "  \"argument\": {" +
                                  "  \"type\": \"hybridqa\"," +
                                  "  \"question\": \"" + question +"\"" +
                                  "   }" +
                                  "}";
                streamWriter.Write(json);
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var responseText = streamReader.ReadToEnd();
                //Now you have your response.
                //or false depending on information in the response

                dynamic array = JsonConvert.DeserializeObject(responseText);

                //textBox9.Text = array.return_object.WiKiInfo.AnswerInfo.ToString();
            }
        }

        

        private void capture_and_search()
        {
            // Timer
            //Stopwatch stopwatch = new Stopwatch();
            stopwatch.Reset();
            stopwatch.Start();

            // initial
            init_qna();

            // Capture
            Image capture = print_picture();

            pictureBox2.Image = capture;

            //구글 api 자격증명
            GoogleCredential credential = null;

            //다운받은 '사용자 서비스 키'를 지정하여 자격증명을 만듭니다.
            using (var stream = new FileStream(Application.StartupPath + "\\My Project.json", FileMode.Open, FileAccess.Read))
            {
                string[] scopes = { VisionService.Scope.CloudPlatform };
                credential = GoogleCredential.FromStream(stream);
                credential = credential.CreateScoped(scopes);
            }

            //자격증명을 가지고 구글 비전 서비스를 생성합니다.
            var service = new VisionService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "google vision",
                GZipEnabled = true,
            });

            service.HttpClient.Timeout = new TimeSpan(1, 1, 1);

            //이미지를 읽어 들입니다.
            byte[] file = ImageToByte(capture);

            // 구글 Image 분석 요청 생성
            BatchAnnotateImagesRequest batchRequest = new BatchAnnotateImagesRequest();
            batchRequest.Requests = new List<AnnotateImageRequest>();
            batchRequest.Requests.Add(new AnnotateImageRequest()
            {
                //"TEXT_DETECTION"로 설정하면 이미지에 텍스트만 추출 합니다.
                Features = new List<Feature>() { new Feature() { Type = "DOCUMENT_TEXT_DETECTION", MaxResults = 1 } },       // 18.10.18a    TEXT_DETECTION
                ImageContext = new ImageContext() { },//LanguageHints = new List<string>() { "ko", "en" } },
                //ImageContext = new ImageContext() { LanguageHints = new List<string>() { "ko" } },
                Image = new Google.Apis.Vision.v1.Data.Image() { Content = Convert.ToBase64String(file) }
            });

            var annotate = service.Images.Annotate(batchRequest);
            string text = "";

            //요청 결과 받기
            BatchAnnotateImagesResponse batchAnnotateImagesResponse = annotate.Execute();
            if (batchAnnotateImagesResponse.Responses.Any())
            {
                AnnotateImageResponse annotateImageResponse = batchAnnotateImagesResponse.Responses[0];
                if (annotateImageResponse.Error != null)
                {//에러
                    if (annotateImageResponse.Error.Message != null)
                        text = annotateImageResponse.Error.Message;
                }
                else
                {//정상 처리
                    text = annotateImageResponse.TextAnnotations[0].Description;//.Replace("\n", "\r\n");

                    string[] response = new string[5];

                    for (int k = 0; k < annotateImageResponse.FullTextAnnotation.Pages[0].Blocks.Count && k < 5; k++)
                    {
                        for (int n = 0; n < annotateImageResponse.FullTextAnnotation.Pages[0].Blocks[k].Paragraphs.Count; n++)
                        {
                            for (int i = 0; i < annotateImageResponse.FullTextAnnotation.Pages[0].Blocks[k].Paragraphs[n].Words.Count; i++)
                            {
                                for (int j = 0; j < annotateImageResponse.FullTextAnnotation.Pages[0].Blocks[k].Paragraphs[n].Words[i].Symbols.Count; j++)
                                {
                                    response[k] += annotateImageResponse.FullTextAnnotation.Pages[0].Blocks[k].Paragraphs[n].Words[i].Symbols[j].Text;
                                }
                                if (annotateImageResponse.FullTextAnnotation.Pages[0].Blocks[k].Paragraphs[n].Words.Count != i + 1)
                                {
                                    response[k] += " ";
                                }
                            }
                        }
                    }

                    question = response[0];

                    for(int i = 0;i<4; i++)
                    {
                        if(response[i+1] != null)
                        {
                            answer[i] = response[i+1];
                            answer_cnt++;
                        }
                    }
                }
            }
            summary_qna();  // 문답 정리
            ////////////////////////////////////// 검색 단계 //////////////////////////////////////

            //UTF8로 바꾸기
            //인코딩 방식을 지정
            System.Text.Encoding utf8 = System.Text.Encoding.UTF8;

            //변환하고자 하는 문자열을 UTF8 방식으로 변환하여 byte 배열로 반환
            // UTF8 → Byte
            //string convertUtoB(quest_answer);

            
            byte[] utf8Bytes = utf8.GetBytes(question);
            byte[] utf8BytesSPACE = utf8.GetBytes(" ");
            byte[] utf8Summary = utf8.GetBytes(summary);
            byte[] utf8BytesA1 = utf8.GetBytes(answer[0]);
            byte[] utf8BytesA2 = utf8.GetBytes(answer[1]);
            byte[] utf8BytesA3 = utf8.GetBytes(answer[2]);
            byte[] utf8BytesA4 = utf8.GetBytes(answer[3]);

            //UTF-8을 string으로 변한
            string utf8String = "";
            foreach (byte b in utf8Bytes)
            {
                utf8String += "%" + String.Format("{0:X}", b);
            }
            string utf8StringSPACE = "";
            foreach (byte b in utf8BytesSPACE)
            {
                utf8StringSPACE += "%" + String.Format("{0:X}", b);
            }
            string utf8StringSum = "";
            foreach (byte b in utf8Summary)
            {
                utf8StringSum += "%" + String.Format("{0:X}", b);
            }
            string utf8StringQA1 = "";
            foreach (byte b in utf8BytesA1)
            {
                utf8StringQA1 += "%" + String.Format("{0:X}", b);
            }
            string utf8StringQA2 = "";
            foreach (byte b in utf8BytesA2)
            {
                utf8StringQA2 += "%" + String.Format("{0:X}", b);
            }
            string utf8StringQA3 = "";
            foreach (byte b in utf8BytesA3)
            {
                utf8StringQA3 += "%" + String.Format("{0:X}", b);
            }
            string utf8StringQA4 = "";
            foreach (byte b in utf8BytesA4)
            {
                utf8StringQA4 += "%" + String.Format("{0:X}", b);
            }

            string url1 = "https://search.naver.com/search.naver?where=nexearch&sm=top_hty&fbm=1&ie=utf8&query=" + utf8String;  // Naver
            string url1_1 = "https://search.naver.com/search.naver?where=nexearch&sm=top_hty&fbm=1&ie=utf8&query=" + utf8StringSum; //Naver 요약

            string url1_2 = "https://search.naver.com/search.naver?where=nexearch&sm=top_hty&fbm=1&ie=utf8&query=" + utf8String + utf8StringSPACE + utf8StringQA1 + utf8StringSPACE + utf8StringQA2 + utf8StringSPACE + utf8StringQA3 + utf8StringSPACE + utf8StringQA4;  // Naver 문답

            string url1_3_1 = "https://search.naver.com/search.naver?where=nexearch&sm=top_hty&fbm=1&ie=utf8&query=" + utf8String + utf8StringSPACE + utf8StringQA1;  // Naver 문답1
            string url1_3_2 = "https://search.naver.com/search.naver?where=nexearch&sm=top_hty&fbm=1&ie=utf8&query=" + utf8String + utf8StringSPACE + utf8StringQA2;  // Naver 문답2
            string url1_3_3 = "https://search.naver.com/search.naver?where=nexearch&sm=top_hty&fbm=1&ie=utf8&query=" + utf8String + utf8StringSPACE + utf8StringQA3;  // Naver 문답3
            string url1_3_4 = "https://search.naver.com/search.naver?where=nexearch&sm=top_hty&fbm=1&ie=utf8&query=" + utf8String + utf8StringSPACE + utf8StringQA4;  // Naver 문답4


            string url2 = "https://www.google.co.kr/search?q=" + question + " " + answer[0] + " " + answer[1] + " " + answer[2] + " " + answer[3];
            string url2_1 = "https://www.google.co.kr/search?q=" + question;
            string url2_2 = "https://www.google.co.kr/search?q=" + summary;

            string url8_1 = "https://www.google.co.kr/search?q=" + question + " " + answer[0];
            string url8_2 = "https://www.google.co.kr/search?q=" + question + " " + answer[1];
            string url8_3 = "https://www.google.co.kr/search?q=" + question + " " + answer[2];
            string url8_4 = "https://www.google.co.kr/search?q=" + question + " " + answer[3];

            string url3 = "https://terms.naver.com/search.nhn?query=" + utf8String; // Naver 지식 백과
            string url4 = "https://dict.naver.com/search.nhn?dicQuery=" + utf8String; // Naver 사전
            string url5 = "https://hanja.dict.naver.com/word?q=" + utf8String; // Naver 한자사전
            //string url6 = "http://alldic.daum.net/grammar_checker.do"; // 다음 맞춤법 
            string url7 = "https://search.daum.net/search?w=tot&DA=YZR&t__nil_searchbox=btn&sug=&sugo=&q=" + utf8String; // 다음 검색
            
            Naver_Api(question, "blog", 1, "Naver 블로그");
            Naver_Api(question, "news", 1, "Naver 뉴스");
            Naver_Api(question, "encyc", 1, "Naver 백과사전");
            
            // Naver_Api(question, "webkr", 1, "Naver 웹문서");
            // Naver_Api(question, "doc", 1, "Naver 전문자료");

            Naver_Api(summary, "blog", 1, "Naver summary");
            //Naver_Api(summary2, "blog", 1, "Naver summary2");

            Daum_Api(question, "blog", 1, "Daum 블로그");
            Daum_Api(question, "web", 1, "Daum web 문서");
            Daum_Api(summary, "blog", 1, "Daum summary");

            // Internet Browser
            webBrowser1.Navigate(url2_1); //Google + 문 // +5 
            //webBrowser2.Navigate(url7); // 다음검색
            //webBrowser3.Navigate(url2); //Google + 문답
            webBrowser3.Navigate(url2_2); //Google + 문 요약
                                          //webBrowser4.Navigate(url5);

            if ((num[0] + num[1] + num[2] + num[3]) < 5)
            {
                foreach (string str in answer)
                {
                    Naver_Api(str + " " + summary, "blog", 1, "Naver 답 포함");
                }
            }
            //Wiki_QA(question);

            print_answer();
        }
    }

    /// <summary>
    /// Provides functions to capture the entire screen, or a particular window, and save it to a file.
    /// </summary>
    public class ScreenCapture
    {
        /// <summary>
        /// Creates an Image object containing a screen shot of the entire desktop
        /// </summary>
        /// <returns></returns>
        
        public Image CaptureScreen()
        {
            return CaptureWindow(User32.GetDesktopWindow(),0,0,0,0);
        }
        
        public IntPtr CapturePtr()
        {
            return (User32.GetForegroundWindow());
        }
        /// <summary>
        /// Creates an Image object containing a screen shot of a specific window
        /// </summary>
        /// <param name="handle">The handle to the window. (In windows forms, this is obtained by the Handle property)</param>
        /// <returns></returns>
        public Image CaptureWindow(IntPtr handle, int left_p, int right_p, int top_p, int bottom_p)
        {
            int left, right, top, bottom;
            

            // get te hDC of the target window
            IntPtr hdcSrc = User32.GetWindowDC(handle);
            // get the size
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(handle, ref windowRect);

            left = (windowRect.right - windowRect.left) * left_p / 100;
            right =(windowRect.right - windowRect.left) * right_p / 100;
            top = (windowRect.bottom - windowRect.top) * top_p / 100;
            bottom = (windowRect.bottom - windowRect.top) * bottom_p / 100;

            int width = (windowRect.right) - (windowRect.left) - left - right;
            int height = (windowRect.bottom) - (windowRect.top) - bottom - top;
            // create a device context we can copy to
            IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
            // select the bitmap object
            IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
            // bitblt over
            GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, left, top, GDI32.SRCCOPY);
            // restore selection
            GDI32.SelectObject(hdcDest, hOld);
            // clean up 
            GDI32.DeleteDC(hdcDest);
            User32.ReleaseDC(handle, hdcSrc);
            // get a .NET image object for it
            Image img = Image.FromHbitmap(hBitmap);
            // free up the Bitmap object
            GDI32.DeleteObject(hBitmap);
            return img;
        }
        /// <summary>
        /// Captures a screen shot of a specific window, and saves it to a file
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="filename"></param>
        /// <param name="format"></param>
        public void CaptureWindowToFile(IntPtr handle, string filename, ImageFormat format)
        {
            Image img = CaptureWindow(handle,0,0,0,0);
            img.Save(filename, format);
        }
        /// <summary>
        /// Captures a screen shot of the entire desktop, and saves it to a file
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="format"></param>
        public void CaptureScreenToFile(string filename, ImageFormat format)
        {
            Image img = CaptureScreen();
            img.Save(filename, format);
        }

        public Color GetPixelColor(IntPtr hwnd, int x, int y)
        {
            IntPtr hdc = User32.GetDC(hwnd);
            uint pixel = GDI32.GetPixel(hdc, x, y);
            User32.ReleaseDC(hwnd, hdc);
            Color color = Color.FromArgb((int)(pixel & 0x000000FF),
                            (int)(pixel & 0x0000FF00) >> 8,
                            (int)(pixel & 0x00FF0000) >> 16);
            return color;
        }

        /// <summary>
        /// Helper class containing Gdi32 API functions
        /// </summary>
        private class GDI32
        {

            public const int SRCCOPY = 0x00CC0020; // BitBlt dwRop parameter
            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
                int nWidth, int nHeight, IntPtr hObjectSource,
                int nXSrc, int nYSrc, int dwRop);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
                int nHeight);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);
            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
            [DllImport("gdi32.dll")]
            public static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        }

        /// <summary>
        /// Helper class containing User32 API functions
        /// </summary>
        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }
            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);
            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
            [DllImport("user32")]
            public static extern IntPtr GetForegroundWindow();
            [DllImport("user32.dll")]
            public static extern IntPtr GetDC(IntPtr hwnd);

        }
    }
}
