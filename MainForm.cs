using NSoup;
using NSoup.Nodes;
using NSoup.Select;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace G1방송기사월간검색
{
    public partial class MainForm : Form
    {
        public static string year = DateTime.Now.ToString("yyyy"), _mon, _search;
        public static int yeari = Convert.ToInt32(year);

        public MainForm()
        {
            InitializeComponent();

            textBox1.KeyDown += Enter;
            comboBox1.KeyDown += Enter;
            comboBox2.KeyDown += Enter;
            search.KeyDown += Enter;

            for (int i = yeari; i >= 2012; i--)
            {
                comboBox1.Items.Add(i.ToString());
            }
            for (int i = 12; i >= 1; i--)
            {
                comboBox2.Items.Add(i.ToString("00"));
            }
            comboBox1.Text = year;
            comboBox2.Text = DateTime.Now.ToString("MM");
        }

        private void Enter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter) button1_Click(sender, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.ReadOnly = true;
            if (!comboBox1.Text.All(char.IsDigit) || !comboBox2.Text.All(char.IsDigit)) MessageBox.Show("날짜가 숫자가 아닙니다. 다시 시도하세요.", "오류!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (2012 > Convert.ToInt32(comboBox1.Text) || Convert.ToInt32(comboBox1.Text) > yeari) MessageBox.Show("년도는 2012년부터 현재까지 지정 가능합니다.", "년도 오류!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (1 > Convert.ToInt32(comboBox2.Text) || Convert.ToInt32(comboBox2.Text) > 12) MessageBox.Show("월은 1월부터 12월까지 지정 가능합니다.", "월 오류!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (_mon != "" && _mon == comboBox1.Text + comboBox2.Text && _search == search.Text) MessageBox.Show("직전에 실행한 조건과 동일합니다.", "중복 오류!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                _mon = comboBox1.Text + comboBox2.Text;
                _search = search.Text;
                WebClient wc = new WebClient();
                wc.Encoding = Encoding.UTF8;

                for (int d = 1; d <= DateTime.DaysInMonth(Convert.ToInt32(comboBox1.Text), Convert.ToInt32(comboBox2.Text)); d++)
                {
                    var date = _mon + d.ToString("00");
                    Document doc = NSoupClient.Parse(wc.DownloadString("https://www.g1tv.co.kr/news/?mid=1_207_6&date=" + date));
                    Elements datas = doc.Select("ul.list_box li");
                    int i = 0;
                    foreach (Element data in datas)
                    {
                        i++;
                        if (i == 1) continue;
                        else if (data.Select("span.reporter").Text == textBox1.Text + " 기자" || textBox1.Text == "전체")
                        {
                            foreach (string s in _search.Split(','))
                            {
                                if (data.Select("a").Attr("title").ToString().Contains(s))
                                {
                                    richTextBox1.AppendText(date + "\t" + data.Select("a").Attr("title").ToString() + "\t");
                                    richTextBox1.AppendText("https://www.g1tv.co.kr" + data.Select("a").Attr("href").ToString() + "\t");
                                    richTextBox1.AppendText(data.Select("span.reporter").Text + "\n");
                                }
                                continue;
                            }
                        }
                    }
                }
                richTextBox1.ReadOnly = false;
                MessageBox.Show(comboBox1.Text + "년 " + comboBox2.Text + "월 기사 검색이 완료되었습니다.", "검색완료!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
