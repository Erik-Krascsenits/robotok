namespace ELTE.Robotok.View;

partial class GameMenuForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameMenuForm));
            this.groupSelectionText = new System.Windows.Forms.Label();
            this.difficultySelectionText = new System.Windows.Forms.Label();
            this.playerOneText = new System.Windows.Forms.Label();
            this.playerTwoText = new System.Windows.Forms.Label();
            this.easyDifficultyButton = new System.Windows.Forms.Button();
            this.mediumDifficultyButton = new System.Windows.Forms.Button();
            this.hardDifficultyButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.refereeModeText = new System.Windows.Forms.Label();
            this.refereeModeCheckbox = new System.Windows.Forms.CheckBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fájlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.játékBetöltéseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupSelectionText
            // 
            this.groupSelectionText.AutoSize = true;
            this.groupSelectionText.Font = new System.Drawing.Font("Segoe UI", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.groupSelectionText.Location = new System.Drawing.Point(135, 36);
            this.groupSelectionText.Name = "groupSelectionText";
            this.groupSelectionText.Size = new System.Drawing.Size(265, 45);
            this.groupSelectionText.TabIndex = 0;
            this.groupSelectionText.Text = "Csapatválasztás";
            // 
            // difficultySelectionText
            // 
            this.difficultySelectionText.AutoSize = true;
            this.difficultySelectionText.Font = new System.Drawing.Font("Segoe UI", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.difficultySelectionText.Location = new System.Drawing.Point(101, 260);
            this.difficultySelectionText.Name = "difficultySelectionText";
            this.difficultySelectionText.Size = new System.Drawing.Size(320, 45);
            this.difficultySelectionText.TabIndex = 1;
            this.difficultySelectionText.Text = "Nehézség választás";
            // 
            // playerOneText
            // 
            this.playerOneText.AutoSize = true;
            this.playerOneText.Font = new System.Drawing.Font("Segoe UI", 13.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.playerOneText.Location = new System.Drawing.Point(46, 118);
            this.playerOneText.Name = "playerOneText";
            this.playerOneText.Size = new System.Drawing.Size(109, 30);
            this.playerOneText.TabIndex = 2;
            this.playerOneText.Text = "1. játékos:";
            // 
            // playerTwoText
            // 
            this.playerTwoText.AutoSize = true;
            this.playerTwoText.Font = new System.Drawing.Font("Segoe UI", 13.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.playerTwoText.Location = new System.Drawing.Point(46, 167);
            this.playerTwoText.Name = "playerTwoText";
            this.playerTwoText.Size = new System.Drawing.Size(109, 30);
            this.playerTwoText.TabIndex = 3;
            this.playerTwoText.Text = "2. játékos:";
            // 
            // easyDifficultyButton
            // 
            this.easyDifficultyButton.Location = new System.Drawing.Point(86, 348);
            this.easyDifficultyButton.Name = "easyDifficultyButton";
            this.easyDifficultyButton.Size = new System.Drawing.Size(94, 29);
            this.easyDifficultyButton.TabIndex = 4;
            this.easyDifficultyButton.Text = "Könnyű";
            this.easyDifficultyButton.UseVisualStyleBackColor = true;
            this.easyDifficultyButton.Click += new System.EventHandler(this.easyDifficultyButton_Click);
            // 
            // mediumDifficultyButton
            // 
            this.mediumDifficultyButton.Enabled = false;
            this.mediumDifficultyButton.Location = new System.Drawing.Point(218, 348);
            this.mediumDifficultyButton.Name = "mediumDifficultyButton";
            this.mediumDifficultyButton.Size = new System.Drawing.Size(94, 29);
            this.mediumDifficultyButton.TabIndex = 5;
            this.mediumDifficultyButton.Text = "Közepes";
            this.mediumDifficultyButton.UseVisualStyleBackColor = true;
            this.mediumDifficultyButton.Click += new System.EventHandler(this.mediumDifficultyButton_Click);
            // 
            // hardDifficultyButton
            // 
            this.hardDifficultyButton.Location = new System.Drawing.Point(353, 348);
            this.hardDifficultyButton.Name = "hardDifficultyButton";
            this.hardDifficultyButton.Size = new System.Drawing.Size(94, 29);
            this.hardDifficultyButton.TabIndex = 6;
            this.hardDifficultyButton.Text = "Nehéz";
            this.hardDifficultyButton.UseVisualStyleBackColor = true;
            this.hardDifficultyButton.Click += new System.EventHandler(this.hardDifficultyButton_Click);
            // 
            // startButton
            // 
            this.startButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.startButton.Location = new System.Drawing.Point(135, 413);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(264, 85);
            this.startButton.TabIndex = 7;
            this.startButton.Text = "Játék indítása";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // refereeModeText
            // 
            this.refereeModeText.AutoSize = true;
            this.refereeModeText.Font = new System.Drawing.Font("Segoe UI", 13.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.refereeModeText.Location = new System.Drawing.Point(46, 212);
            this.refereeModeText.Name = "refereeModeText";
            this.refereeModeText.Size = new System.Drawing.Size(348, 30);
            this.refereeModeText.TabIndex = 8;
            this.refereeModeText.Text = "Játékvezetői nézet külön ablakban:";
            // 
            // refereeModeCheckbox
            // 
            this.refereeModeCheckbox.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.refereeModeCheckbox.Location = new System.Drawing.Point(400, 206);
            this.refereeModeCheckbox.Name = "refereeModeCheckbox";
            this.refereeModeCheckbox.Size = new System.Drawing.Size(34, 47);
            this.refereeModeCheckbox.TabIndex = 9;
            this.refereeModeCheckbox.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fájlToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(530, 28);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fájlToolStripMenuItem
            // 
            this.fájlToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.játékBetöltéseToolStripMenuItem});
            this.fájlToolStripMenuItem.Name = "fájlToolStripMenuItem";
            this.fájlToolStripMenuItem.Size = new System.Drawing.Size(45, 24);
            this.fájlToolStripMenuItem.Text = "Fájl";
            // 
            // játékBetöltéseToolStripMenuItem
            // 
            this.játékBetöltéseToolStripMenuItem.Name = "játékBetöltéseToolStripMenuItem";
            this.játékBetöltéseToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.játékBetöltéseToolStripMenuItem.Text = "Játék betöltése...";
            // 
            // GameMenuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(530, 513);
            this.Controls.Add(this.refereeModeCheckbox);
            this.Controls.Add(this.refereeModeText);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.hardDifficultyButton);
            this.Controls.Add(this.mediumDifficultyButton);
            this.Controls.Add(this.easyDifficultyButton);
            this.Controls.Add(this.playerTwoText);
            this.Controls.Add(this.playerOneText);
            this.Controls.Add(this.difficultySelectionText);
            this.Controls.Add(this.groupSelectionText);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "GameMenuForm";
            this.Text = "Robotok - Főmenü (alpha 2.0)";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private Label groupSelectionText;
    private Label difficultySelectionText;
    private Label playerOneText;
    private Label playerTwoText;
    private Button easyDifficultyButton;
    private Button mediumDifficultyButton;
    private Button hardDifficultyButton;
    private Button startButton;
    private Label refereeModeText;
    private CheckBox refereeModeCheckbox;
    private MenuStrip menuStrip1;
    private ToolStripMenuItem fájlToolStripMenuItem;
    private ToolStripMenuItem játékBetöltéseToolStripMenuItem;
}