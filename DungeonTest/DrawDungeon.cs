using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

using LibDungeon.Levels;

namespace DungeonTest
{
    public static class DungeonExtension
    {
        public static void Draw(this DungeonFloor floor, Graphics g)
        {
            // Рисование пола
            for (int i = 0; i < DungeonFloor.width; i++)
            {
                for (int j = 0; j < DungeonFloor.height; j++)
                {
                    var tile = floor.Tiles[i, j];
                    Brush brush;
                    /*switch (tile.Solidity)
                    {
                        case Tile.SolidityType.Floor:
                            brush = Brushes.LightGray; break;
                        case Tile.SolidityType.Wall:
                            brush = Brushes.Black; break;
                        case Tile.SolidityType.Door:
                            brush = Brushes.DarkOrange; break;
                        default:
                            brush = Brushes.Pink; break;
                    }*/

                    switch (tile)
                    {
                        case Floor tfloor:
                            brush = Brushes.LightGray; break;
                        case Wall twall:
                            brush = Brushes.Black; break;
                        case Door tdoor:
                            brush = Brushes.DarkOrange; break;
                        case Ladder tladder:
                            brush = (tladder.Direction == Ladder.LadderDirection.Down) ? Brushes.Green : Brushes.Blue;
                            break;
                        default:
                            brush = Brushes.Pink; break;
                    }
                    g.FillRectangle(brush, i * 10, j * 10, 10, 10);
                    if (tile.Visited)
                        g.DrawRectangle(Pens.Red, i * 10, j * 10, 10, 10);
                }
            }

            foreach(var item in floor.FloorItems)
            {
                g.FillEllipse(Brushes.DarkRed, item.X * 10, item.Y * 10, 10, 10);
            }

            foreach (var actor in floor.Actors)
            {
                g.FillEllipse(Brushes.Blue, actor.X * 10, actor.Y * 10, 10, 10);
            }

            
        }
    }

    public partial class FormMain : Form
    {
        LibDungeon.Dungeon Dungeon = new LibDungeon.Dungeon();

        /// <summary>
        /// Отрисовывает игровой мир
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            var graph = e.Graphics;
            graph.Clear(Color.Black);
            //floor.Draw(graph);
            (Dungeon.CurrentLevel as DungeonFloor).Draw(graph);
            graph.FillRectangle(Brushes.Aqua, Dungeon.PlayerPawn.X * 10, Dungeon.PlayerPawn.Y * 10, 10, 10);
            graph.DrawString("@", SystemFonts.DefaultFont,
                Brushes.Black, Dungeon.PlayerPawn.X * 10, Dungeon.PlayerPawn.Y * 10);
        }
    }
}
