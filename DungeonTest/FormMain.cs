using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using LibDungeon.Levels;

namespace DungeonTest
{
    //using Logic;
    using System.Runtime.CompilerServices;

    public partial class FormMain : Form
    {
        //DungeonFloor floor = new DungeonFloor();
        //Carver carver;
        public FormMain()
        {
            InitializeComponent();

            ClientSize = new Size(80 * 10, 80 * 10);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox1.Refresh();
        }

        int sparsing = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }

        private void PlayerKeyDown(object sender, KeyEventArgs e)
        {
            LibDungeon.Dungeon.PlayerCommand cmd;
            switch (e.KeyCode)
            {
                case Keys.Right:
                case Keys.NumPad6:
                    cmd = LibDungeon.Dungeon.PlayerCommand.Move0;
                    break;
                case Keys.NumPad9:
                    cmd = LibDungeon.Dungeon.PlayerCommand.Move45;
                    break;
                case Keys.Up:
                case Keys.NumPad8:
                    cmd = LibDungeon.Dungeon.PlayerCommand.Move90;
                    break;
                case Keys.NumPad7:
                    cmd = LibDungeon.Dungeon.PlayerCommand.Move135;
                    break;
                case Keys.Left:
                case Keys.NumPad4:
                    cmd = LibDungeon.Dungeon.PlayerCommand.Move180;
                    break;
                case Keys.NumPad1:
                    cmd = LibDungeon.Dungeon.PlayerCommand.Move225;
                    break;
                case Keys.Down:
                case Keys.NumPad2:
                    cmd = LibDungeon.Dungeon.PlayerCommand.Move270;
                    break;
                case Keys.NumPad3:
                    cmd = LibDungeon.Dungeon.PlayerCommand.Move315;
                    break;
                case Keys.NumPad5:
                    cmd = LibDungeon.Dungeon.PlayerCommand.Wait;
                    break;

                case Keys.OemPeriod:
                    cmd = LibDungeon.Dungeon.PlayerCommand.LadderDown;
                    break;
                case Keys.Oemcomma:
                    cmd = LibDungeon.Dungeon.PlayerCommand.LadderUp;
                    break;

                case Keys.F10:
                    Close();
                    return;
                default:
                    return;
            }
            if (!Dungeon.PlayerMove(cmd))
                System.Media.SystemSounds.Hand.Play();
            else
            {
                pictureBox1.Refresh();
            }
        }
    }
}
