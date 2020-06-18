namespace DungeonTest
{
    partial class FormMain
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnDispose = new System.Windows.Forms.Button();
            this.btnEquip = new System.Windows.Forms.Button();
            this.lbEquipment = new System.Windows.Forms.ListBox();
            this.lbInventory = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtMessages = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.DimGray;
            this.pictureBox1.Location = new System.Drawing.Point(206, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(415, 356);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // timer1
            // 
            this.timer1.Interval = 1;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnDispose);
            this.panel1.Controls.Add(this.btnEquip);
            this.panel1.Controls.Add(this.lbEquipment);
            this.panel1.Controls.Add(this.lbInventory);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 484);
            this.panel1.TabIndex = 1;
            // 
            // btnDispose
            // 
            this.btnDispose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDispose.Location = new System.Drawing.Point(122, 449);
            this.btnDispose.Name = "btnDispose";
            this.btnDispose.Size = new System.Drawing.Size(75, 23);
            this.btnDispose.TabIndex = 4;
            this.btnDispose.Text = "Выбросить";
            this.btnDispose.UseVisualStyleBackColor = true;
            this.btnDispose.Click += new System.EventHandler(this.btnDispose_Click);
            // 
            // btnEquip
            // 
            this.btnEquip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEquip.Location = new System.Drawing.Point(12, 449);
            this.btnEquip.Name = "btnEquip";
            this.btnEquip.Size = new System.Drawing.Size(75, 23);
            this.btnEquip.TabIndex = 3;
            this.btnEquip.Text = "Использовать";
            this.btnEquip.UseVisualStyleBackColor = true;
            // 
            // lbEquipment
            // 
            this.lbEquipment.FormattingEnabled = true;
            this.lbEquipment.Location = new System.Drawing.Point(12, 208);
            this.lbEquipment.Name = "lbEquipment";
            this.lbEquipment.Size = new System.Drawing.Size(185, 95);
            this.lbEquipment.TabIndex = 2;
            // 
            // lbInventory
            // 
            this.lbInventory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lbInventory.FormattingEnabled = true;
            this.lbInventory.Location = new System.Drawing.Point(12, 309);
            this.lbInventory.Name = "lbInventory";
            this.lbInventory.Size = new System.Drawing.Size(185, 134);
            this.lbInventory.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // txtMessages
            // 
            this.txtMessages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMessages.Location = new System.Drawing.Point(206, 362);
            this.txtMessages.Multiline = true;
            this.txtMessages.Name = "txtMessages";
            this.txtMessages.Size = new System.Drawing.Size(415, 122);
            this.txtMessages.TabIndex = 2;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 484);
            this.Controls.Add(this.txtMessages);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBox1);
            this.KeyPreview = true;
            this.Name = "FormMain";
            this.Text = "♂ Dungeon ♂";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PlayerKeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox lbEquipment;
        private System.Windows.Forms.ListBox lbInventory;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDispose;
        private System.Windows.Forms.Button btnEquip;
        private System.Windows.Forms.TextBox txtMessages;
    }
}

