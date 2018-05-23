using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace шашки
{
    class MyPictureBox : PictureBox
    {
        public int IDPictureBox { private set; get; }

        public bool Pressed = false;

        public MyPictureBox(int ID, int Pct_top, int Pct_left, int Pct_width, int Pct_heigh)
        {
            IDPictureBox = ID;
            Top = Pct_top;
            Left = Pct_left;
            Width = Pct_width;
            Height = Pct_heigh;

            if (ID < 12)
            {
                Image = Properties.Resources.Шашка_черная;
            }

            else
            {
                if (ID > 19)
                    Image = Properties.Resources.Шашка_белая;
                else
                    Image = Properties.Resources.Клетка;
            }
        }

    }
}
