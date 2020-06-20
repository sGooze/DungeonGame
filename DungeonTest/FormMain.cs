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
    using LibDungeon.Objects;
    //using Logic;
    using System.Runtime.CompilerServices;

    public partial class FormMain : Form
    {
        LibDungeon.Dungeon Dungeon;
        //DungeonFloor floor = new DungeonFloor();
        //Carver carver;
        public FormMain()
        {
            InitializeComponent();

            LibDungeon.Dungeon.ClientMessageSent += Dungeon_ClientMessageSent;

            ClientSize = new Size(80 * 10, 80 * 10);
        }

        public bool StartNewGame()
        {
            var dlg = new FormNewGame();
            if (dlg.ShowDialog() != DialogResult.OK)
                return false;
            txtMessages.Clear();
            Dungeon = new LibDungeon.Dungeon();
            Dungeon.SpawnPlayer(dlg.PlayerClass, dlg.PlayerSubclassId, dlg.PlayerName);
            FormUpdate();
            return true;
        }

        private void Dungeon_ClientMessageSent(object sender, string e)
        {
            txtMessages.AppendText(e);
            txtMessages.AppendText("\r\n");
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

                case Keys.D:
                    cmd = (e.Shift) 
                        ? LibDungeon.Dungeon.PlayerCommand.OpenDoor : LibDungeon.Dungeon.PlayerCommand.CloseDoor;
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
                FormUpdate();
            }
        }

        private void FormUpdate()
        {
            if (Dungeon.PlayerPawn.Health == 0) {
                if (MessageBox.Show($"Вы погибли! Ваш итоговый счёт: {Dungeon.PlayerPawn.Score} очков. Желаете сыграть ещё раз?",
                    "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    StartNewGame();
                }
                else return;
            }

            lblName.Text = Dungeon.PlayerPawn.Name;
            lblHealth.Text = $"{Dungeon.PlayerPawn.Health}/{Dungeon.PlayerPawn.MaxHealth}";
            lblLevel.Text = $"Этаж {Dungeon.CurrentLevel + 1}";
            lblHunger.Text = $"Голод: {Dungeon.PlayerPawn.Hunger}/{Actor.maxHunger}";

            barHealth.Maximum = Dungeon.PlayerPawn.MaxHealth;
            barHealth.Value = Dungeon.PlayerPawn.Health;

            lbInventory.Items.Clear();
            lbEquipment.Items.Clear();
            lbInventory.Items.AddRange(Dungeon.PlayerPawn.Inventory.ToArray());
            lbEquipment.Items.AddRange(Dungeon.PlayerPawn.Equipment.ToArray());
            pictureBox1.Refresh();
        }

        private void btnDispose_Click(object sender, EventArgs e)
        {
            if (lbInventory.SelectedIndex == -1) 
                return;
            var item = (lbInventory.SelectedItem as BaseItem);
            Dungeon.PlayerPawn.Inventory.Remove(item);
            FormUpdate();
        }

        private void lbInventory_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbEquipment.SelectedIndex = -1;
        }

        private void lbEquipment_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbInventory.SelectedIndex = -1;
        }

        private void btnEquip_Click(object sender, EventArgs e)
        {
            if (lbEquipment.SelectedIndex != -1)
            {
                // Деактивировать предмет
                Dungeon.PlayerPawn.UnuseItem(lbEquipment.SelectedItem as BaseItem);
                Dungeon.PlayerMove(LibDungeon.Dungeon.PlayerCommand.RemoveItem);
            }
            else if (lbInventory.SelectedIndex != -1)
            {
                Dungeon.PlayerPawn.UseItem(lbInventory.SelectedItem as BaseItem);
                Dungeon.PlayerMove(LibDungeon.Dungeon.PlayerCommand.EquipItem);
            }
            FormUpdate();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            if (!StartNewGame())
                Close();
        }
    }
}
