using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeedSomeSleep
{
    public partial class Form1 : Form
    {
        static int minute = 1000 * 60;
        int intervalAfterHide = minute * 15;
        int intervalAfterShow = minute;
        string[] questions;

        public Form1()
        {
            InitializeComponent();
            addMessageToLog("Запуск программы");
            timer1.Interval = intervalAfterShow; 
            timer1.Enabled = true;
            timer1.Tick += timer1_Tick;
            TopMost = true;
            ShowIcon = false;
            string path = Directory.GetCurrentDirectory() + "\\questions.txt";
            string pathSettings = Directory.GetCurrentDirectory() + "\\settings.ini";
            try
            {
                questions = File.ReadAllLines(path, Encoding.UTF8);
                updateQuestion();
            }
            catch
            {
                addMessageToLog($"Не найден файл с вопросами {path}, создайте его (по одному вопросу на каждой строке)");
            }

            try
            {
                var settings = File.ReadAllLines(pathSettings, Encoding.UTF8);
                intervalAfterHide = Int32.Parse(settings[1]) * minute;
                intervalAfterShow = Int32.Parse(settings[3]) + minute;
            }
            catch
            {
                addMessageToLog($"Не найден файл с настройками {pathSettings}, применены настройки по умолчанию");
            }

        }

        void BacklogProcedure()
        {

        }

        void timer1_Tick(object sender, EventArgs e)
        {
            var now = DateTime.Now;

            bool doWork = (0 <= now.Hour && now.Hour <= 7);
            if (!doWork)
            {
                addMessageToLog("Программа спит, т.к. сейчас не время контроля");
                return;
            }
                


            //если форма показывается
            if (this.Visible)
            {
                //15 секунд пырился в нее и ничего не ответил
                if (textBox1.Text.Equals(""))
                    addMessageToLog("за отведенное время ответ не был дан!");
                else
                {
                    addMessageToLog("ОК нажат не был, но ответ был дан");
                    getAnswer();
                }
                    
                //то скрываем ее на определенное время
                this.Hide();
                timer1.Interval = intervalAfterHide;
            }
            
            //если форма скрыта
            else
            {
                updateQuestion();
                //то показываем ее ровно на 15 секунд для ответа
                this.Show();
                timer1.Interval= intervalAfterShow;
            }
                
        }

        private void updateQuestion()
        {
            if (questions is null || questions.Length == 0)
                return;
            Random rnd = new Random();

            //Получить очередное (в данном случае - первое) случайное число
            int value = rnd.Next(questions.Length);

            label1.Text = questions[value] + " ?";


        }

        private void button1_Click(object sender, EventArgs e)
        {
            getAnswer();
        }

        private void getAnswer()
        {
            string answer = $"вопрос: { label1.Text}; ответ: { textBox1.Text}";
            addMessageToLog(answer);
            textBox1.Text = "";
            this.Hide();
            timer1.Interval = intervalAfterHide;
        }

        private void addMessageToLog(string message)
        {
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\answers");
            string currentLog = Directory.GetCurrentDirectory() + "\\answers\\" + $"{DateTime.Now.ToString("yyyy-MM-dd")}.txt";
            File.AppendAllText(currentLog, $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] {message}{Environment.NewLine}");
            

        }

    }
}
