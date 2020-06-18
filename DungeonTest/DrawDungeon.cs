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
        static Image sprites = Image.FromFile("Resources\\sheet.png");
        public static void Draw(this DungeonFloor floor, LibDungeon.Objects.Actor player, Size out_zone, Graphics g)
        {
            // Ограничиваем выводимые спрайты размером контрола
            // По центру располагается игрок
            int borderX = out_zone.Width / 2 - 32,
                borderY = out_zone.Height / 2 - 32;

            // Рисование пола
            for (int i = 0; i < floor.Width; i++)
            {
                for (int j = 0; j < floor.Height; j++)
                {
                    int screenX = (i - player.X) * 32, 
                        screenY = (j - player.Y) * 32;

                    if (Math.Abs(screenX) >= borderX || Math.Abs(screenY) >= borderY)
                        continue;

                    screenX += borderX; screenY += borderY;
                    var tile = floor.Tiles[i, j];
                    if (tile.Visible)
                    {
                        Rectangle zone = new Rectangle(0, 0, 32, 32);
                        switch (tile)
                        {
                            case Floor tfloor:
                                zone.X = 32 * 2;
                                break;
                            case Wall twall:
                                zone.X = 32 * 5;
                                break;
                            case Door tdoor:
                                zone.X = 32 * (tdoor.IsOpen ? 1 : 0);
                                break;
                            case Ladder tladder:
                                zone.X = 32 * (tladder.Direction == Ladder.LadderDirection.Down ? 3 : 4);
                                break;
                            /*default:
                                brush = Brushes.Pink; 
                                break;*/
                        }
                        g.DrawImage(sprites, screenX, screenY, zone, GraphicsUnit.Pixel);
                        //g.DrawImage(sprites, i*32, j*32, zone, GraphicsUnit.Pixel);
                    }
                    else if(tile.Visited && !(tile is Wall))
                    {
                        g.FillRectangle(Brushes.DarkBlue, screenX, screenY, 32, 32);
                    }
                }
            }
            g.FillRectangle(Brushes.Aqua, borderX, borderY, 32, 32);

            foreach (var item in floor.FloorItems)
            {
                int screenX = (item.X - player.X) * 32,
                    screenY = (item.Y - player.Y) * 32;
                if (floor.Tiles[item.X, item.Y].Visible &&
                    (Math.Abs(screenX) < borderX && Math.Abs(screenY) < borderY))
                    g.FillEllipse(Brushes.DarkRed, screenX + borderX, screenY + borderY, 32, 32);
            }

            foreach (var actor in floor.FloorActors)
            {
                int screenX = (actor.X - player.X) * 32,
                    screenY = (actor.Y - player.Y) * 32;
                if (floor.Tiles[actor.X, actor.Y].Visible &&
                    (Math.Abs(screenX) < borderX && Math.Abs(screenY) < borderY))
                    g.FillRectangle(Brushes.OrangeRed, screenX + borderX, screenY + borderY, 32, 32);
            }


        }
    }

    public partial class FormMain : Form
    {
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
            (Dungeon.CurrentLevel as DungeonFloor).Draw(Dungeon.PlayerPawn, pictureBox1.Size, graph);
            //graph.FillRectangle(Brushes.Aqua, Dungeon.PlayerPawn.X * 10, Dungeon.PlayerPawn.Y * 10, 10, 10);
            //graph.DrawString("@", SystemFonts.DefaultFont,
            //    Brushes.Black, Dungeon.PlayerPawn.X * 10, Dungeon.PlayerPawn.Y * 10);
        }
    }
}
