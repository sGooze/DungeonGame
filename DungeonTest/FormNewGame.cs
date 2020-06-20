using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Globalization;
using System.Reflection;

using LibDungeon.Objects;

namespace DungeonTest
{
    public partial class FormNewGame : Form
    {
        List<(PlayerClassAttribute, Type)> classList = new List<(PlayerClassAttribute, Type)>();

        public string PlayerName { get; set; }
        public int PlayerSubclassId { get; set; }
        public Type PlayerClass { get; private set; }


        public FormNewGame()
        {
            InitializeComponent();
            var asm = Assembly.Load("LibDungeon");
            var player_classes = asm.GetTypes()
                .Where(x => x.IsDefined(typeof(PlayerClassAttribute)))
                .Select(x => (x, x.GetCustomAttributes(typeof(PlayerClassAttribute)))).ToArray();

            foreach (var c in player_classes)
                foreach (var a in c.Item2)
                    classList.Add((a as PlayerClassAttribute, c.x));

            foreach (var l in classList)
                listClasses.Items.Add(l.Item1.Name);
            //var types = Assembly.ReflectionOnlyLoad("LibDungeon").GetTypes();
        }

        Random r = new Random();
        private void btnRandomName_Click(object sender, EventArgs e)
        {
            var seed = new DateTime(Math.Min((long)r.Next() * r.Next(), DateTime.MaxValue.Ticks)).ToString("MMMM");

            txtName.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                new string(seed.ToCharArray(0, seed.Length - 1).OrderBy(x => r.Next()).ToArray())
            );
        }

        private void listClasses_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listClasses.SelectedIndex == -1)
            {
                txtDescription.Text = "";
                return;
            }
            PlayerClass = classList[listClasses.SelectedIndex].Item2;
            txtDescription.Text = classList[listClasses.SelectedIndex].Item1.Description;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtName.Text) || listClasses.SelectedIndex == -1)
            {
                MessageBox.Show("Заполните форму!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            PlayerName = txtName.Text;
            PlayerSubclassId =
                Array.IndexOf(
                    PlayerClass.GetCustomAttributes(typeof(PlayerClassAttribute)).ToArray(),
                    classList[listClasses.SelectedIndex].Item1
            );
            if (PlayerSubclassId == -1)
                return;
            Close();
        }

        private void FormNewGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                return;
            if (String.IsNullOrWhiteSpace(txtName.Text) || listClasses.SelectedIndex == -1)
                e.Cancel = true;
        }
    }
}
