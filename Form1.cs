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

using Google.Apis.Auth.OAuth2;
using Google.Apis.Vision.v1;
using Google.Apis.Services;
using Google.Apis.Vision.v1.Data;
using System.Diagnostics;
using System.Net;
//using mshtml;
using mshtml;
using Moda.Korean.TwitterKoreanProcessorCS;
using Newtonsoft.Json;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        int[] num = new int[4]; // 19.01.12a
        int pic_x, pic_y;
        string[] answer = new string[4];
        string question;
        string summary;
        string summary2;
        int[] number = new int[4];
        int answer_cnt;
        Stopwatch stopwatch = new Stopwatch();

        public Form1()
        {
            InitializeComponent();
            hScrollBar1.Minimum = 200;
            hScrollBar1.Maximum = pictureBox1.Width;

            vScrollBar1.Minimum = 300;
            vScrollBar1.Maximum = pictureBox1.Height;

            pic_x = pictureBox1.Location.X;
            pic_y = pictureBox1.Location.Y;

            //webBrowser4.Navigate("http://alldic.daum.net/grammar_checker.do"); //다음 맞춤

            //wiki_QA("휘게는 어느나라어 인가?");



        }



        private void button1_Click(object sender, EventArgs e)
        {
            // <19.01.12a
            this.num[0] = 0;
            this.num[1] = 0;
            this.num[2] = 0;
            this.num[3] = 0;
            // 19.01.12a>
            capture_and_search();
        }

        public static byte[] ImageToByte(System.Drawing.Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            pictureBox1.Location = new Point(pic_x + (hScrollBar1.Maximum - hScrollBar1.Value) / 2, pictureBox1.Location.Y);
            pictureBox1.Size = new Size(hScrollBar1.Value, pictureBox1.Size.Height);
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            pictureBox1.Location = new Point(pictureBox1.Location.X, pic_y + (vScrollBar1.Maximum - vScrollBar1.Value) / 2);
            pictureBox1.Size = new Size(pictureBox1.Size.Width, vScrollBar1.Value);
        }

        private void init_qna()
        {
            for (int i = 0; i < 4; i++)
            {
                this.answer[i] = "";
                this.number[i] = 0;
            }
            this.question = "";
            this.answer_cnt = 0;
        }

        private string text_change(string String)
        {
            textBox1.BackColor = System.Drawing.Color.White;

            // 삭제 문구

            string[] lines1 = System.IO.File.ReadAllLines("erase.txt", Encoding.Default);
            foreach (string line in lines1)
            {
                //String = String.Replace(line, "");
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
            //String = String.Replace("  ", " ");
            //String = String.Replace("  ", " ");
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
                high_light(1,1);
                //textBox8.Text += "\n" + "1:" + stopwatch.Elapsed.ToString();
                print_answer();
            }
        }
        private void webBrowser2_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.AbsoluteUri == webBrowser2.Url.AbsoluteUri)
            {
                high_light(2,1);
                //textBox8.Text += "\n" + "2:" + stopwatch.Elapsed.ToString();
                print_answer();
            }

        }
        private void webBrowser3_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.AbsoluteUri == webBrowser3.Url.AbsoluteUri)
            {
                high_light(3,1);
                //textBox8.Text += "\n" + "3:" + stopwatch.Elapsed.ToString();
                print_answer();
            }
        }

        private void print_answer()
        {
            int max = 1;
            int max_num = 5;
            int total = num[0] + num[1] + num[2] + num[3];
            textBox7.Text = this.num[0].ToString() + " " + this.num[1].ToString() + " " + this.num[2].ToString() + " " + this.num[3].ToString()+ "\r\n";
            for(int i=0;i<4;i++)
            {
                if(max < num[i])
                {
                    max = num[i];
                    max_num = i;
                }
            }
            if (max_num != 5 && max > 2)
            {
                textBox7.Text += (max_num + 1).ToString() + ": " + answer[max_num] + " " + (max * 100/total).ToString() + "%";
            }

        }

        public int WordCheck(string String, string Word)
        {
            string[] StringArray = String.Split(new string[] { Word }, StringSplitOptions.None);

            return StringArray.Length - 1;
        }


        private void high_light(int num,int mul)
        {
            int[] count = new int[4];

            IHTMLDocument2 doc2;
            if (num == 1)
            {
                doc2 = (mshtml.IHTMLDocument2)webBrowser1.Document.DomDocument;
            }
            else if (num == 2)
            {
                doc2 = (mshtml.IHTMLDocument2)webBrowser2.Document.DomDocument;
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
                    string[] split = question.Split(' ');
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

            if (num ==1)
            {
                textBox8.Text += "구글";
            }
            else if(num == 2)
            {
                textBox8.Text += "다음";
            }
            else if (num == 3)
            {
                textBox8.Text += "구글요약";
            }
            else if (num == 4)
            {
                //textBox8.Text += "구글문요약";
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

        private void naver_api(string query,int type, int mul, string name)
        {
            int[] count = new int[4];

            string display = "100";   // display 10~100까지 읽음
            string sort = "sim";     // sort sim(유사도순) date(날짜순)

            string url = ""; //= "https://openapi.naver.com/v1/search/blog?query=" + query + "&display=" + display + "&sort=" + sort; // 결과가 JSON 포맷
            switch (type)
            {
                case 0:
                    url = "https://openapi.naver.com/v1/search/blog?query=" + query + "&display=" + display + "&sort=" + sort; // 결과가 JSON 포맷
                    break;
                case 1:
                    url = "https://openapi.naver.com/v1/search/news?query=" + query + "&display=" + display + "&sort=" + sort; // 결과가 JSON 포맷
                    break;
                case 2:
                    url = "https://openapi.naver.com/v1/search/encyc?query=" + query + "&display=" + display + "&sort=" + sort; // 결과가 JSON 포맷
                    break;

                case 3:
                    url = "https://openapi.naver.com/v1/search/webkr?query=" + query + "&display=" + display + "&sort=" + sort; // 결과가 JSON 포맷
                    break;
                case 4:
                    url = "https://openapi.naver.com/v1/search/doc?query=" + query + "&display=" + display + "&sort=" + sort; // 결과가 JSON 포맷
                    break;
            }
            
            // https://openapi.naver.com/v1/search/news.json 뉴스
            // https://openapi.naver.com/v1/search/encyc.json 백과사전
            // https://openapi.naver.com/v1/search/webkr.json 웹문서
            // https://openapi.naver.com/v1/search/doc.json 전문자료

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            string c_id = "1kwVZ3_v8s26Ix9WWfhN";
            string c_pw = "cNqeOh_NJX";
            /*
            switch (count_request % 4)
            {
                case 0:
                    c_id = "1kwVZ3_v8s26Ix9WWfhN";
                    c_pw = "cNqeOh_NJX";
                    break;
                case 1:
                    c_id = "4Py3I37chC6dKkjYDW3t";
                    c_pw = "WDoxEs6YEx";
                    break;
                case 2:
                    c_id = "7uwRBqcbUvEjR8a3oLwO";
                    c_pw = "teW9tgMoQZ";
                    break;
                case 3:
                default:
                    c_id = "xgZhUGM11pTpdTIioQrD";
                    c_pw = "gwdhc7Clq0";
                    break;
            }
            */

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

                    for(int j = 0; j <total;j++)
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
                                string title = array.items[j].ToString();
                                if (WordCheck(title, item) > 0)
                                {
                                    count[i] += WordCheck(title, item);
                                    this.num[i] += mul * count[i];
                                }
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
                /*
                key1.total = 999999;
                resp--;
                Console.WriteLine(e.Message.ToString());

                if (resp == 0)   // 50번 예외처리 되면 끄자
                {
                    thisDay2 = DateTime.Now;   // 18.02.19a

                    //Console.WriteLine((thisDay2 - thisDay).ToString());    // 18.02.19a

                    //sw3.WriteLine(count_request + "\t" + count_all + "\t" + (thisDay2 - thisDay).ToString());    // 18.02.19a

                    Delay(7200000); //  2hour 멈춤
                    resp = 20;
                }
                Delay(1000); //  1s 멈춤
                */
            }

            textBox8.Text += "  " + stopwatch.Elapsed.ToString();
            textBox8.Text += ":  " + count[0].ToString() + " " + count[1].ToString() + " " + count[2].ToString() + " " + count[3].ToString() + " " + name + "\r\n";

        }

        private void textBox7_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Enter)
            {
                capture_and_search();
            }
        }

        private void summary_qna()
        {
            // 필요없는 문구 없애기
            question = text_change(question);

            // 공간 없애기
            question = question.Replace("\n", " ");
            question = question.Replace("?", "");
            question = question.Replace("|", "");
            question = question.Replace(",", "");
            question = question.Replace(".", "");
            question = question.Replace("[", "");
            question = question.Replace("]", "");
            question = question.Replace("!", "");
            question = question.Replace("\"", "");
            question = question.Replace("<", "");
            question = question.Replace(">", "");
            question = question.Replace("/", "");

            question = question.Replace("  ", " ");
            question = question.Trim(); // 선후행 공백 제거

            for (int i = 0; i < 4; i++)
            {
                if (answer[i] != null)
                {
                    answer[i] = answer[i].Replace("|", "");
                    answer[i] = answer[i].Replace(" . ", "");
                    answer[i] = answer[i].Replace(" .", "");
                    answer[i] = answer[i].Replace("  ", " ");
                    answer[i] = answer[i].Trim();
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

        private void wiki_QA(string question)
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

                //// \"request_id\": \"reserved field\", " +

                streamWriter.Write(json);
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var responseText = streamReader.ReadToEnd();
                //Now you have your response.
                //or false depending on information in the response

                dynamic array = JsonConvert.DeserializeObject(responseText);

                textBox9.Text = array.return_object.WiKiInfo.AnswerInfo.ToString();
               // textBox9.Text = array.return_object
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
            Bitmap bitmap = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(PointToScreen(new Point(this.pictureBox1.Location.X, this.pictureBox1.Location.Y)), new Point(0, 0), pictureBox1.Size);

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
            byte[] file = ImageToByte(bitmap);

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
            
            naver_api(question, 0, 1, "API 블로그");
            naver_api(question, 1, 1, "API 뉴스");
            naver_api(question, 2, 1, "API 백과사전");
           // naver_api(question, 3, 1, "API 웹문서");
           // naver_api(question, 4, 1, "API 전문자료");

           

            naver_api(summary, 0, 1, "API summary");
            naver_api(summary2, 0, 1, "API summary2");


            // Internet Browser
            webBrowser1.Navigate(url2_1); //Google + 문 // +5 
            //webBrowser2.Navigate(url7); // 다음검색
            //webBrowser3.Navigate(url2); //Google + 문답
            webBrowser3.Navigate(url2_2); //Google + 문 요약
                                          //webBrowser4.Navigate(url5);

            //wiki_QA(question);

            print_answer();

        }
    }
}
