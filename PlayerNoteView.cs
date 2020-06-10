using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Coursework
{
    class PlayerNoteView: Panel
    {
        APlayer player;
        private Label PlayerName;
        private Label PlayerScore;
        private Label PlayerMoveScore;
        private PictureBox DicePicture;

        public PlayerNoteView() : base()
        {

        }

        public PlayerNoteView(APlayer player, bool active) : base()
        {
            player = this.player;
            Size = new Size(760, 50);
            if (active)
                BackColor = Color.LightGreen;
            else BackColor = Color.Transparent;

            PlayerName = new Label() { Parent = this, Location = new Point(0, 10), Size = new Size(100, 30), Font = new Font(Font.FontFamily, 18), Text = player.Name.ToString() };
            PlayerScore = new Label() { Parent = this, Location = new Point(110, 0), Size = new Size(140, 50)};
            PlayerMoveScore = new Label() { Parent = this, Location = new Point(260, 10), Size = new Size(140, 30), Font = new Font(Font.FontFamily, 18)};
            DicePicture = new PictureBox() { Parent = this, Location = new Point(410, 10), Size = new Size(50, 50), SizeMode = PictureBoxSizeMode.StretchImage };
            //Connect = new Button() { Parent = this, Location = new Point(660, 0), Size = new Size(100, 50), Font = new Font(Font.FontFamily, 9), Text = "Подключиться" };

            //SetStatusColor(Source.GameStatus);

            // вызываем собтие, если пользователь нажал на кнопку "Подключиться"
            //Connect.Click += (object sender, EventArgs e) => {
            //    ConnectEvent?.Invoke(this);
            //};

            // если статус сервера изменился - обновляем данные
            
        }

        
    }
}
