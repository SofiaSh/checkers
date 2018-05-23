using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace шашки
{
    public partial class Form1 : Form
    {
        public bool turn_sr = true;
        public bool srub = true;
        public int Black = 0;
        public int White = 0;
        public bool pat = false;
        public bool stroke = true;//переменная обозначающая, чей ход (белые 0, черные 1)
        public bool l = false; //количество вхождений
        public int IDID;// ID прошлой клетки
        public int a = 8; public int b = 4; //размер массивов
        MyPictureBox[,] Pct_array;//массив PictureBox
        public int[,] field;
        /*Массив, содержащий расположение шашек
        0-пустое поле,
        1-черная шашка,
        2-черная дамка,
        3-белая шашка,
        4-белая дамка*/
        public Form1()
        {
            InitializeComponent();
            Pct_array = new MyPictureBox[a, b];
            field = new int[a, b];
        }
        //Расположение шашек
        public int startID = 0;
        int Pct_top = 75;
        int Pct_left;
        int Pct_heigh = 50;
        int Pct_width = 50;
        int ID1;

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.BackColor = Color.Transparent;
            label1.Text = "Шашек на поле:";
            label2.BackColor = Color.Transparent;
            label3.BackColor = Color.Transparent;
            //Размещение шашек
            for (int k = 0; k < a; k++)
            {
                if (k % 2 == 0)
                    Pct_left = 100;
                else
                    Pct_left = 50;
                for (int i = 0; i < b; i++)
                {
                    Pct_array[k, i] = new MyPictureBox(startID, Pct_top, Pct_left, Pct_width, Pct_heigh)
                    {
                        Parent = this
                    };
                    if (startID < 12) field[k, i] = 1;//черная шашка
                    if ((startID >= 12) && (startID < 20)) field[k, i] = 0;//пустое
                    if (startID > 19) field[k, i] = 3;//белая шашка
                    Pct_left = Pct_left + 100;
                    startID++;
                    Pct_array[k, i].MouseClick += this.Click_pb;
                    Pct_array[k, i].MouseUp += this.Up_pb;
                }
                Pct_top = Pct_top + 50;
            }
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (field[i, j] == 1 || field[i, j] == 2)
                        Black++;
                    if (field[i, j] == 3 || field[i, j] == 4)
                        White++;
                }
            }
            label2.Text = "Чёрных: " + Black.ToString();
            label2.Text += "\nБелых: " + White.ToString();
            label3.Text = "Ход Белых";
        }

        void Click_pb(object sender, System.EventArgs e)
        {
            MyPictureBox picture = sender as MyPictureBox;
            for (int i = 0; i < 8; i++) //Обнуление возможных полей для хода
            {
                for (int j = 0; j < 4; j++)
                {
                    if (field[i, j] == 0)
                    {
                        Pct_array[i, j].Image = Properties.Resources.Клетка;
                    }
                    Pct_array[i, j].Pressed = false;
                }
            }
            if (picture.Pressed == false)
            {
                ID1 = picture.IDPictureBox;//рабочая переменная (укорачивает запись)
                if (stroke)
                {
                    if (field[ID1 / b, ID1 % b] == 3)
                    {
                        coursel_sr();
                        if (srub&&turn_sr)
                            coursel_w();
                    }
                    if ((field[IDID / b, IDID % b] == 3) && l && (field[ID1 / b, ID1 % b] == 0))
                    {
                        if (srub && turn_sr)
                            corse_simple();
                        else
                            course_srub();
                    }
                }
                else
                {
                    if (field[ID1 / b, ID1 % b] == 1)
                    {
                        coursel_sr();
                        if (srub && turn_sr)
                            coursel_b();
                    }
                    if ((field[IDID / b, IDID % b] == 1) && l && (field[ID1 / b, ID1 % b] == 0))
                    {
                        if (srub && turn_sr)
                            corse_simple();
                        else
                            course_srub();
                    }
                }
                picture.Pressed = true;
            }
        }
        void Up_pb(object sender, MouseEventArgs e)//счетчик шашек на поле
        {
            Black = 0;
            White = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (field[i, j] == 1 || field[i, j] == 2)
                        Black++;
                    if (field[i, j] == 3 || field[i, j] == 4)
                        White++;
                }
            }
            if (stroke)
                label3.Text = "Ход Белых";
            else
                label3.Text = "Ход Чёрных";
            label2.Text = "Чёрных: " + Black.ToString();
            label2.Text += "\nБелых: " + White.ToString();
             if (Black == 0) 
                 end_game_w();
            if (White==0)
                end_game_b();
            if (pat && l == false)
            { 
                if(stroke)
                end_game_w();
                else
                    end_game_b();
            }
        }
        void coursel_sr()//подсветка поля, когда можно срубить
        {
            turn_sr = true;
            //когда можно срубить шашку сверху
            if ((ID1 / b) > 1)
            {
                if (ID1 / b % 2 == 1)//выбрана шашка на нечетной строке
                {

                    if (((ID1 % b != 3) && (field[ID1 / b - 2, ID1 % b + 1] == 0)) &&//cправа на нечетной строке вверх
                      (((field[ID1 / b - 1, ID1 % b] == 1) && stroke) || ((field[ID1 / b - 1, ID1 % b] == 3) && stroke == false)))
                    {
                        Pct_array[(ID1 / b) - 2, (ID1 % b) + 1].Image = Properties.Resources.Клетка_выбор2;
                        IDID = ID1;
                        l = true;
                        turn_sr = false;
                    }
                    //слева на нечетной строке вверх
                    if (((ID1 % b != 0) && (field[ID1 / b - 2, ID1 % b - 1] == 0)) &&
                       (((field[ID1 / b - 1, ID1 % b - 1] == 1) && stroke) || ((field[ID1 / b - 1, ID1 % b - 1] == 3) && stroke == false)))
                    {
                        Pct_array[(ID1 / b) - 2, (ID1 % b) - 1].Image = Properties.Resources.Клетка_выбор2;
                        IDID = ID1;
                        l = true;
                        turn_sr = false;
                    }
                }
                else//выбрана шашка на четной строке вверх
                {
                    if (((ID1 % b != 3) && (field[ID1 / b - 2, ID1 % b + 1] == 0)) && //cправа на четной вверх
                       (((field[ID1 / b - 1, ID1 % b + 1] == 1) && stroke) || ((field[ID1 / b - 1, ID1 % b + 1] == 3) && stroke == false)))
                    {
                        Pct_array[ID1 / b - 2, ID1 % b + 1].Image = Properties.Resources.Клетка_выбор2;
                        IDID = ID1;
                        l = true;
                        turn_sr = false;
                    }
                    if (((ID1 % b != 0) && (field[ID1 / b - 2, ID1 % b - 1] == 0)) &&//слева на четной вверх
                       (((field[ID1 / b - 1, ID1 % b] == 1) && stroke) || ((field[ID1 / b - 1, ID1 % b] == 3) && stroke == false)))
                    {
                        Pct_array[ID1 / b - 2, ID1 % b - 1].Image = Properties.Resources.Клетка_выбор2;
                        IDID = ID1;
                        l = true;
                        turn_sr = false;
                    }
                }
            }
            //когда можно срубить снизу
            if ((ID1 / b) < 6)
            {
                if ((ID1 / b) % 2 == 1)//выбрана шашка на нечетной строке
                {
                    if (((ID1 % b != 3) && (field[ID1 / b + 2, ID1 % b + 1] == 0)) &&//cправа на нечетной строке вниз
                       (((field[ID1 / b + 1, ID1 % b] == 1) && stroke) || ((field[ID1 / b + 1, ID1 % b] == 3) && stroke == false)))
                    {
                        Pct_array[(ID1 / b) + 2, (ID1 % b) + 1].Image = Properties.Resources.Клетка_выбор2;
                        IDID = ID1;
                        l = true;
                        turn_sr = false;
                    }
                    //слева на нечетной строке вниз
                    if (((ID1 % b != 0) && (field[ID1 / b + 2, ID1 % b - 1] == 0)) &&
                       (((field[ID1 / b + 1, ID1 % b - 1] == 1) && stroke) || ((field[ID1 / b + 1, ID1 % b - 1] == 3) && stroke == false)))
                    {
                        Pct_array[(ID1 / b) + 2, (ID1 % b) - 1].Image = Properties.Resources.Клетка_выбор2;
                        IDID = ID1;
                        l = true;
                        turn_sr = false;
                    }
                }
                else//выбрана шашка  на четной строке
                {
                    if (((ID1 % b != 3) && (field[ID1 / b + 2, ID1 % b + 1] == 0)) &&//cправа на четной вниз
                       (((field[ID1 / b + 1, ID1 % b + 1] == 1) && stroke) || ((field[ID1 / b + 1, ID1 % b + 1] == 3) && stroke == false)))
                    {
                        Pct_array[ID1 / b + 2, ID1 % b + 1].Image = Properties.Resources.Клетка_выбор2;
                        IDID = ID1;
                        l = true;
                        turn_sr = false;
                    }
                    if (((ID1 % b != 0) && (field[ID1 / b + 2, ID1 % b - 1] == 0)) &&//слева на четной вниз
                        (((field[ID1 / b + 1, ID1 % b] == 1) && stroke) || ((field[ID1 / b + 1, ID1 % b] == 3) && stroke == false)))
                    {
                        Pct_array[ID1 / b + 2, ID1 % b - 1].Image = Properties.Resources.Клетка_выбор2;
                        IDID = ID1;
                        l = true;
                        turn_sr = false;
                    }
                }
            }
        }
        void coursel_w()//подсветка полей простого хода белой
        {
            if ((ID1 / b) > 0)
            {
                //клетки для простого хода белой
                if (field[(ID1 / b) - 1, ID1 % b] == 0)
                {
                    Pct_array[(ID1 / b) - 1, ID1 % b].Image = Properties.Resources.Клетка_выбор2;
                    IDID = ID1;
                    l = true;
                }
                if (((((ID1 / b) % 2 == 1) && (ID1 % b != 0)) || (((ID1 / b) % 2 == 0) && (ID1 % b != 3))))
                {
                    if ((ID1 / b) % 2 == 1)//еcли нечетная строка
                    {
                        if (field[(ID1 / b) - 1, (ID1 % b) - 1] == 0)
                        {
                            Pct_array[(ID1 / b) - 1, (ID1 % b) - 1].Image = Properties.Resources.Клетка_выбор2;
                            IDID = ID1;
                            l = true;
                        }
                    }
                    else
                    {
                        if (field[(ID1 / b) - 1, (ID1 % b) + 1] == 0)
                        {
                            Pct_array[(ID1 / b) - 1, (ID1 % b) + 1].Image = Properties.Resources.Клетка_выбор2;
                            IDID = ID1;
                            l = true;
                        }
                    }
                }
            }
        }
        void coursel_b()//подсветка полей  простого хода черной
        {
            if ((ID1 / b) < 7)
            {
                if (field[(ID1 / b) + 1, ID1 % b] == 0)
                {
                    Pct_array[(ID1 / b) + 1, ID1 % b].Image = Properties.Resources.Клетка_выбор2;
                    IDID = ID1;
                    l = true;
                }
                if ((((ID1 / b) % 2 == 1) && (ID1 % b != 0)) || (((ID1 / b) % 2 == 0) && (ID1 % b != 3)))
                {
                    if ((ID1 / b) % 2 == 1)//еcли нечетная строка
                    {
                        if (field[(ID1 / b) + 1, (ID1 % b) - 1] == 0)
                        {
                            Pct_array[(ID1 / b) + 1, (ID1 % b) - 1].Image = Properties.Resources.Клетка_выбор2;
                            IDID = ID1;
                            l = true;
                        }
                    }
                    else
                    {
                        if (field[(ID1 / b) + 1, (ID1 % b) + 1] == 0)
                        {
                            Pct_array[(ID1 / b) + 1, (ID1 % b) + 1].Image = Properties.Resources.Клетка_выбор2;
                            IDID = ID1;
                            l = true;
                        }
                    }
                }
            }
        }
        void corse_simple()//простой ход шашки
        {
            if (((ID1 / b) == ((IDID / b) - 1) && stroke) || (((ID1 / b) == ((IDID / b) + 1)) && stroke == false))//подходящая строка
            {
                if ((ID1 % b == IDID % b) ||//если один столбец и 
                 ((IDID / b % 2 == 1) && (ID1 % b + 1 == IDID % b)) ||//нечетная строка подходящий столбец
                 ((IDID / b % 2 == 0) && (ID1 % b - 1 == IDID % b)))//четная строка и подходящий столбец
                    course();
            }
        }
        void course_srub()//когда можно срубить шашку
        {
            //когда можно срубить шашку сверху
            if ((IDID / b) > 1)
            {
                if (IDID / b % 2 == 1)//выбрана шашка на нечетной строке
                {

                    if (((IDID % b != 3) && (field[IDID / b - 2, IDID % b + 1] == 0) && (ID1 == IDID - 7)) &&//cправа на нечетной строке вверх
                      (((field[IDID / b - 1, IDID % b] == 1) && stroke) || ((field[IDID / b - 1, IDID % b] == 3) && stroke == false)))
                    {
                        field[IDID / b - 1, IDID % b] = 0;
                        Pct_array[IDID / b - 1, IDID % b].Image = Properties.Resources.Клетка;
                        course();
                    }
                    //слева на нечетной строке вверх
                    if (((IDID % b != 0) && (field[IDID / b - 2, IDID % b - 1] == 0) && (ID1 == IDID - 9)) &&
                       (((field[IDID / b - 1, IDID % b - 1] == 1) && stroke) || ((field[IDID / b - 1, IDID % b - 1] == 3) && stroke == false)))
                    {
                        field[IDID / b - 1, IDID % b - 1] = 0;
                        Pct_array[IDID / b - 1, IDID % b - 1].Image = Properties.Resources.Клетка;
                        course();
                    }
                }
                else//выбрана шашка на четной строке вверх
                {
                    if (((IDID % b != 3) && (field[IDID / b - 2, IDID % b + 1] == 0) && (ID1 == IDID - 7)) && //cправа на четной вверх
                       (((field[IDID / b - 1, IDID % b + 1] == 1) && stroke) || ((field[IDID / b - 1, IDID % b + 1] == 3) && stroke == false)))
                    {
                        field[IDID / b - 1, IDID % b + 1] = 0;
                        Pct_array[IDID / b - 1, IDID % b + 1].Image = Properties.Resources.Клетка;
                        course();
                    }
                    if (((IDID % b != 0) && (field[IDID / b - 2, IDID % b - 1] == 0) && (ID1 == IDID - 9)) &&//слева на четной вверх
                       (((field[IDID / b - 1, IDID % b] == 1) && stroke) || ((field[IDID / b - 1, IDID % b] == 3) && stroke == false)))
                    {
                        field[IDID / b - 1, IDID % b] = 0;
                        Pct_array[IDID / b - 1, IDID % b].Image = Properties.Resources.Клетка;
                        course();
                    }
                }
            }
            //когда можно срубить снизу
            if ((IDID / b) < 6)
            {
                if ((IDID / b) % 2 == 1)//выбрана шашка на нечетной строке
                {
                    if (((IDID % b != 3) && (field[IDID / b + 2, IDID % b + 1] == 0) && (ID1 == IDID + 9)) &&//cправа на нечетной строке вниз
                       (((field[IDID / b + 1, IDID % b] == 1) && stroke) || ((field[IDID / b + 1, IDID % b] == 3) && stroke == false)))
                    {
                        field[IDID / b + 1, IDID % b] = 0;
                        Pct_array[IDID / b + 1, IDID % b].Image = Properties.Resources.Клетка;
                        course();
                    }
                    //слева на нечетной строке вниз
                    if (((IDID % b != 0) && (field[IDID / b + 2, IDID % b - 1] == 0) && (ID1 == IDID + 7)) &&
                       (((field[IDID / b + 1, IDID % b - 1] == 1) && stroke) || ((field[IDID / b + 1, IDID % b - 1] == 3) && stroke == false)))
                    {
                        field[IDID / b + 1, IDID % b - 1] = 0;
                        Pct_array[IDID / b + 1, IDID % b - 1].Image = Properties.Resources.Клетка;
                        course();
                    }
                }
                else//выбрана шашка  на четной строке
                {
                    if (((IDID % b != 3) && (field[IDID / b + 2, IDID % b + 1] == 0) && (ID1 == IDID + 9)) &&//cправа на четной вниз
                       (((field[IDID / b + 1, IDID % b + 1] == 1) && stroke) || ((field[IDID / b + 1, IDID % b + 1] == 3) && stroke == false)))
                    {
                        field[IDID / b + 1, IDID % b + 1] = 0;
                        Pct_array[IDID / b + 1, IDID % b + 1].Image = Properties.Resources.Клетка;
                        course();
                    }
                    if (((IDID % b != 0) && (field[IDID / b + 2, IDID % b - 1] == 0) && (ID1 == IDID + 7)) &&//слева на четной вниз
                        (((field[IDID / b + 1, IDID % b] == 1) && stroke) || ((field[IDID / b + 1, IDID % b] == 3) && stroke == false)))
                    {
                        field[IDID / b + 1, IDID % b] = 0;
                        Pct_array[IDID / b + 1, IDID % b].Image = Properties.Resources.Клетка;
                        course();
                    }
                }
            }
        }
        void course()//вспомогательный метод для хода
        {
            if (stroke)
            {
                field[(ID1 / b), (ID1 % b)] = 3;
                Pct_array[(ID1 / b), ID1 % b].Image = Properties.Resources.Шашка_белая;
                stroke = false;
            }
            else
            {
                field[(ID1 / b), (ID1 % b)] = 1;
                Pct_array[(ID1 / b), ID1 % b].Image = Properties.Resources.Шашка_черная;
                stroke = true;
            }
            field[IDID / b, IDID % b] = 0;
            Pct_array[(IDID / b), IDID % b].Image = Properties.Resources.Клетка;
            l = false;
            pat = true;
            srub = true;
            if (stroke)
                pat_w();
            else
                pat_b();
        }
        void pat_w()//есть ли ходы
        {
            for (int I = 0; I < 32; I++)
            {
                if (field[I / b, I % b] == 3)
                {
                    //когда можно срубить шашку сверху
                    if ((I / b) > 1)
                    {
                        if ((I / b) % 2 == 1)//выбрана шашка на нечетной строке
                        {
                            if ((I % b != 3) && (field[I / b - 2, I % b + 1] == 0) &&//cправа на нечетной строке вверх
                              (field[I / b - 1, I % b] == 1))
                            { 
                                pat = false;
                                srub = false;
                            }
                            //слева на нечетной строке вверх
                            if ((I % b != 0) && (field[I / b - 2, I % b - 1] == 0) &&
                               (field[I / b - 1, I % b - 1] == 1))
                            { 
                              pat = false;
                              srub = false; 
                            }
                        }
                        else//выбрана шашка на четной строке вверх
                        {
                            if ((I % b != 3) && (field[I / b - 2, I % b + 1] == 0) && //cправа на четной вверх
                               (field[I / b - 1, I % b + 1] == 1))
                            {
                                pat = false;
                                srub = false;
                            }
                            if ((I % b != 0) && (field[I / b - 2, I % b - 1] == 0) &&//слева на четной вверх
                               (field[I / b - 1, I % b] == 1))
                            {
                                pat = false;
                                srub = false;
                            }
                        }
                    }
                    //когда можно срубить снизу
                    if ((I / b) < 6)
                    {
                        if ((I / b) % 2 == 1)//выбрана шашка на нечетной строке
                        {
                            if ((I % b != 3) && (field[I / b + 2, I % b + 1] == 0) &&//cправа на нечетной строке вниз
                               (field[I / b + 1, I % b] == 1))
                            {
                                pat = false;
                                srub = false;
                            }
                            //слева на нечетной строке вниз
                            if ((I % b != 0) && (field[I / b + 2, I % b - 1] == 0) &&
                               (field[I / b + 1, I % b - 1] == 1))
                            {
                                pat = false;
                                srub = false;
                            }
                        }
                        else//выбрана шашка  на четной строке
                        {
                            if ((I % b != 3) && (field[I / b + 2, I % b + 1] == 0) &&//cправа на четной вниз
                               (field[I / b + 1, I % b + 1] == 1))
                            {
                                pat = false;
                                srub = false;
                            }
                            if ((I % b != 0) && (field[I / b + 2, I % b - 1] == 0) &&//слева на четной вниз
                                (field[I / b + 1, I % b] == 1))
                            {
                                pat = false;
                                srub = false;
                            }
                        }
                    }
                    if ((I / b) > 0)//клетки для простого хода белой
                    {
                        if (field[(I / b) - 1, I % b] == 0)
                            pat = false;
                        if (((I / b) % 2 == 1) && (I % b != 0) && (field[(I / b) - 1, I % b - 1] == 0))
                            pat = false;
                        if (((I / b) % 2 == 0) && (I % b != 3) && (field[(I / b) - 1, I % b + 1] == 0))
                            pat = false;
                    }
                }
            }
        }
        void pat_b()//есть ли ходы
        {
            for (int I = 0; I < 32; I++)
            {
                if ((field[I / b, I % b] == 1) && pat)
                {
                    //когда можно срубить шашку сверху
                    if ((I / b) > 1)
                    {
                        if (I / b % 2 == 1)//выбрана шашка на нечетной строке
                        {
                            if ((I % b != 3) && (field[I / b - 2, I % b + 1] == 0) &&//cправа на нечетной строке вверх
                              (field[I / b - 1, I % b] == 3))
                            {
                                pat = false;
                                srub = false;
                            }
                            //слева на нечетной строке вверх
                            if ((I % b != 0) && (field[I / b - 2, I % b - 1] == 0) &&
                               (field[I / b - 1, ID1 % b - 1] == 3))
                            {
                                pat = false;
                                srub = false;
                            }
                        }
                        else//выбрана шашка на четной строке вверх
                        {
                            if ((I % b != 3) && (field[I / b - 2, I % b + 1] == 0) && //cправа на четной вверх
                               (field[I / b - 1, I % b + 1] == 3))
                            {
                                pat = false;
                                srub = false;
                            }
                            if ((I % b != 0) && (field[I / b - 2, I % b - 1] == 0) &&//слева на четной вверх
                               (field[I / b - 1, I % b] == 3))
                            {
                                pat = false;
                                srub = false;
                            }
                        }
                    }
                    //когда можно срубить снизу
                    if ((I / b) < 6)
                    {
                        if ((I / b) % 2 == 1)//выбрана шашка на нечетной строке
                        {
                            if ((I % b != 3) && (field[I / b + 2, I % b + 1] == 0) &&//cправа на нечетной строке вниз
                               (field[I / b + 1, I % b] == 3))
                            {
                                pat = false;
                                srub = false;
                            }
                            //слева на нечетной строке вниз
                            if ((I % b != 0) && (field[I / b + 2, I % b - 1] == 0) &&
                               (field[I / b + 1, I % b - 1] == 3))
                            {
                                pat = false;
                                srub = false;
                            }
                        }
                        else//выбрана шашка  на четной строке
                        {
                            if ((I % b != 3) && (field[I / b + 2, I % b + 1] == 0) &&//cправа на четной вниз
                               (field[I / b + 1, I % b + 1] == 3))
                            {
                                pat = false;
                                srub = false;
                            }
                            if ((I % b != 0) && (field[I / b + 2, I % b - 1] == 0) &&//слева на четной вниз
                                (field[I / b + 1, I % b] == 3))
                            {
                                pat = false;
                                srub = false;
                            }
                        }
                    }
                    //клетки для простого хода черной
                    if ((I / b) < 7)
                    {
                        if (field[(I / b)+1, I % b] == 0)
                            pat = false;
                        if (((I / b) % 2 == 1) && (I % b != 0) && (field[(I / b) + 1, I % b - 1] == 0))
                            pat = false;
                        if (((I / b) % 2 == 0) && (I % b != 3) && (field[(I / b) + 1, I % b + 1] == 0))
                            pat = false;
                    }
                }
            }
        }
        void end_game_w()
        {

            if (MessageBox.Show("Игра окончена.\nВыиграли белые.\nНачать новую игру?", "Конец игры", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Application.Restart();
            else Application.Exit();

        }
        void end_game_b()
        {

            if (MessageBox.Show("Игра окончена.\nВыиграли чёрные.\nНачать новую игру?", "Конец игры", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Application.Restart();
            else Application.Exit();

        }
        //Пользовательское меню 
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }
        private void новаяИграToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void выходToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Игра окончена\nНачать новую игру?", "Конец игры", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Application.Restart();
            else Application.Exit();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void выходToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void правилаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("1. Шашки ходят только вперед\n2. Рубить можно в любую сторону", "Правила игры", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

    }
}
