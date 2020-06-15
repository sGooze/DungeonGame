﻿using System;
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
            //floor = new DungeonFloor();
            //carver = new Carver(floor);
            //timer1.Enabled = true;
            //sparsing = (DungeonFloor.height * DungeonFloor.width) / 1;
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            int coord_x = e.X / 10, coord_y = e.Y / 10;

            Dungeon.PlayerPawn.X = coord_x; Dungeon.PlayerPawn.Y = coord_y;
            Dungeon.Think();
            pictureBox1.Refresh();
        }

        int sparsing = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            /*if (carver != null)
            {
                //if (carver.FullyCarved) { if (sparsing-- > 0) carver.Sparse(); } else carver.Carve();
                //if (sparsing-- > 0) 
                    carver.Sparse();
            }
            pictureBox1.Refresh();*/
        }

        private void FormMain_KeyDown(object sender, KeyEventArgs e)
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
    /*
    public class Carver
    {
        DungeonFloor floor;
        Random rand = new Random();
        List<(int, int)> cardinal = new List<(int, int)>() { (1, 0), (0, 1), (-1, 0), (0, -1) };
        Queue<(int, int)> tileList = new Queue<(int, int)>();

        public bool FullyCarved => tileList.Count == 0;

        public Carver(DungeonFloor dungeonFloor)
        {
            floor = dungeonFloor;
            int x, y;
            x = rand.Next(1, DungeonFloor.width - 2); y = rand.Next(1, DungeonFloor.height - 2);
            floor.Tiles[x, y] = new Floor();
            for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++)
                    if (!((i != 0) && (j != 0)) && (i != j))
                            tileList.Enqueue((i+x, j+y));
        }

        public void Carve()
        {
            meme:
            if (tileList.Count == 0)
                return;

            var tile = tileList.Dequeue();
            if (tile.Item1 < 1 || tile.Item2 < 1 || tile.Item1 > DungeonFloor.width - 2 || tile.Item2 > DungeonFloor.height - 2)
                goto meme;

            if (floor.Tiles[tile.Item1, tile.Item2].Solidity == Tile.SolidityType.Floor)
                goto meme;

            int touching = 0;
            if (rand.Next(0, 100) < 50) cardinal = cardinal.OrderBy(c => rand.Next()).ToList();
            foreach(var card in cardinal)
            {
                int x = tile.Item1 + card.Item1;
                int y = tile.Item2 + card.Item2;
                if (x > 0 && y > 0 && x < DungeonFloor.width - 1 && y < DungeonFloor.height - 1 && floor.Tiles[x, y].Solidity == Tile.SolidityType.Floor)
                    touching++;
            }
            if (touching < 2)
            {
                floor.Tiles[tile.Item1, tile.Item2] = new Floor();
                for (int i = -1; i <= 1; i++)
                    for (int j = -1; j <= 1; j++)
                        if (!((i != 0) && (j != 0)))
                            if (i != j)
                                if (!tileList.Contains((i + tile.Item1, j + tile.Item2)))
                                    tileList.Enqueue((i + tile.Item1, j + tile.Item2));
            }
            else
                goto meme;


            // Mush
            if (rand.Next(0, 2) == 1)
            {
                LinkedList<(int, int)> temp = new LinkedList<(int, int)>();
                for (int i = 0; i < rand.Next(0, tileList.Count); i++)
                {
                    var t = tileList.Dequeue();
                    var _ = (rand.Next(0, 2) == 0) ? temp.AddFirst(t) : temp.AddLast(t);
                }
                foreach (var t in temp)
                    tileList.Enqueue(t);
            }
        }

            int x = 1, y = 1;
        public void Sparse()
        {
            do
            {
                //x = rand.Next(1, DungeonFloor.width - 1); y = rand.Next(1, DungeonFloor.height - 1);
                x = x % (DungeonFloor.width  - 2) + 1;
                y = y % (DungeonFloor.height - 2) + 1;

                if (floor.Tiles[x, y].Solidity == Tile.SolidityType.Wall)
                    continue;

                int touching = 0;
                foreach (var card in cardinal)
                {
                    int dx = x + card.Item1;
                    int dy = y + card.Item2;
                    if (dx >= 0 && dy >= 0 && dx <= DungeonFloor.width - 1 && dy <= DungeonFloor.height - 1 && floor.Tiles[dx, dy].Solidity == Tile.SolidityType.Wall)
                        touching++;
                }
                if (touching != 3)
                    continue;
                break;
            } while (true);
            floor.Tiles[x, y] = new Wall();
        }
    }
    */
    
}
