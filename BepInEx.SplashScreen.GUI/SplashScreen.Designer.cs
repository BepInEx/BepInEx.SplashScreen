
namespace BepInEx.SplashScreen
{
    partial class SplashScreen
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashScreen));
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.labelTop = new System.Windows.Forms.Label();
            this.labelBot = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Top;
            this.progressBar1.ForeColor = System.Drawing.Color.ForestGreen;
            this.progressBar1.Location = new System.Drawing.Point(8, 401);
            this.progressBar1.MarqueeAnimationSpeed = 0;
            this.progressBar1.Maximum = 5;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(268, 31);
            this.progressBar1.Step = 1;
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 3;
            this.progressBar1.Value = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pictureBox1.Location = new System.Drawing.Point(8, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(268, 256);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Top;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Location = new System.Drawing.Point(8, 444);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(268, 34);
            this.button1.TabIndex = 4;
            this.button1.TabStop = false;
            this.button1.Text = "Open game folder";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.checkedListBox1.CausesValidation = false;
            this.checkedListBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Items.AddRange(new object[] {
            "Initialize environment and BepInEx",
            "Load and apply patchers",
            "Load and apply plugins",
            "Start the game"});
            this.checkedListBox1.Location = new System.Drawing.Point(8, 276);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.checkedListBox1.Size = new System.Drawing.Size(268, 60);
            this.checkedListBox1.TabIndex = 0;
            this.checkedListBox1.TabStop = false;
            this.checkedListBox1.ThreeDCheckBoxes = true;
            this.checkedListBox1.UseTabStops = false;
            // 
            // labelTop
            // 
            this.labelTop.AutoEllipsis = true;
            this.labelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelTop.Location = new System.Drawing.Point(8, 348);
            this.labelTop.Name = "labelTop";
            this.labelTop.Size = new System.Drawing.Size(268, 20);
            this.labelTop.TabIndex = 1;
            this.labelTop.Text = "BepInEx is initializing...";
            // 
            // labelBot
            // 
            this.labelBot.AutoEllipsis = true;
            this.labelBot.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelBot.Location = new System.Drawing.Point(8, 368);
            this.labelBot.Name = "labelBot";
            this.labelBot.Size = new System.Drawing.Size(268, 33);
            this.labelBot.TabIndex = 2;
            this.labelBot.Text = "The environment is being set up";
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(8, 264);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(268, 12);
            this.panel1.TabIndex = 6;
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(8, 336);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(268, 12);
            this.panel2.TabIndex = 7;
            // 
            // panel5
            // 
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(8, 432);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(268, 12);
            this.panel5.TabIndex = 10;
            // 
            // SplashScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(284, 715);
            this.ControlBox = false;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.labelBot);
            this.Controls.Add(this.labelTop);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(300, 100);
            this.Name = "SplashScreen";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "The game is loading...";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label labelTop;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label labelBot;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel5;
    }
}

