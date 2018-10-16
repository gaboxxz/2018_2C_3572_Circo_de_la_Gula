namespace TGC.Group.Form
{
    partial class GameForm
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
            this.panel3D = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonIsland = new System.Windows.Forms.Button();
            this.buttonCanyon = new System.Windows.Forms.Button();
            this.panel3D.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3D
            // 
            this.panel3D.AutoSize = true;
            this.panel3D.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.panel3D.BackgroundImage = global::TGC.Group.Properties.Resources.index2;
            this.panel3D.Controls.Add(this.button1);
            this.panel3D.Controls.Add(this.buttonIsland);
            this.panel3D.Controls.Add(this.buttonCanyon);
            this.panel3D.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3D.Location = new System.Drawing.Point(0, 0);
            this.panel3D.Name = "panel3D";
            this.panel3D.Size = new System.Drawing.Size(974, 772);
            this.panel3D.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(453, 803);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            // 
            // buttonIsland
            // 
            this.buttonIsland.BackColor = System.Drawing.Color.Transparent;
            this.buttonIsland.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonIsland.Font = new System.Drawing.Font("Constantia", 27.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.buttonIsland.ForeColor = System.Drawing.Color.OrangeRed;
            this.buttonIsland.Location = new System.Drawing.Point(334, 686);
            this.buttonIsland.Name = "buttonIsland";
            this.buttonIsland.Size = new System.Drawing.Size(321, 53);
            this.buttonIsland.TabIndex = 1;
            this.buttonIsland.Text = "Nivel &Isla";
            this.buttonIsland.UseVisualStyleBackColor = false;
            this.buttonIsland.Click += new System.EventHandler(this.ButtonIsland_Click);
            // 
            // buttonCanyon
            // 
            this.buttonCanyon.BackColor = System.Drawing.Color.Transparent;
            this.buttonCanyon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.buttonCanyon.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCanyon.Font = new System.Drawing.Font("Constantia", 27.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCanyon.ForeColor = System.Drawing.Color.OrangeRed;
            this.buttonCanyon.Location = new System.Drawing.Point(334, 617);
            this.buttonCanyon.Name = "buttonCanyon";
            this.buttonCanyon.Size = new System.Drawing.Size(321, 52);
            this.buttonCanyon.TabIndex = 0;
            this.buttonCanyon.Text = "Nivel &Cañon";
            this.buttonCanyon.UseVisualStyleBackColor = false;
            this.buttonCanyon.Click += new System.EventHandler(this.ButtonCanyon_Click);
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(974, 772);
            this.Controls.Add(this.panel3D);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "GameForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Crash Bandicoot";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameForm_FormClosing);
            this.Load += new System.EventHandler(this.GameForm_Load);
            this.panel3D.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel3D;
        private System.Windows.Forms.Button buttonIsland;
        private System.Windows.Forms.Button buttonCanyon;
        private System.Windows.Forms.Button button1;
    }
}

