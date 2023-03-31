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
            this.oneGroupOptionText = new System.Windows.Forms.Label();
            this.startButton = new System.Windows.Forms.Button();
            this.refereeModeText = new System.Windows.Forms.Label();
            this.refereeModeCheckbox = new System.Windows.Forms.CheckBox();
            this.twoGroupsOptionText = new System.Windows.Forms.Label();
            this.easyDifficultyOption = new System.Windows.Forms.RadioButton();
            this.mediumDifficultyOption = new System.Windows.Forms.RadioButton();
            this.hardDifficultyOption = new System.Windows.Forms.RadioButton();
            this.oneGroupOption = new System.Windows.Forms.RadioButton();
            this.twoGroupsOption = new System.Windows.Forms.RadioButton();
            this.groupChoice = new System.Windows.Forms.GroupBox();
            this.difficultyChoice = new System.Windows.Forms.GroupBox();
            this.groupChoice.SuspendLayout();
            this.difficultyChoice.SuspendLayout();
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
            this.difficultySelectionText.Location = new System.Drawing.Point(114, 258);
            this.difficultySelectionText.Name = "difficultySelectionText";
            this.difficultySelectionText.Size = new System.Drawing.Size(320, 45);
            this.difficultySelectionText.TabIndex = 1;
            this.difficultySelectionText.Text = "Nehézség választás";
            // 
            // oneGroupOptionText
            // 
            this.oneGroupOptionText.AutoSize = true;
            this.oneGroupOptionText.Font = new System.Drawing.Font("Segoe UI", 13.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.oneGroupOptionText.Location = new System.Drawing.Point(46, 112);
            this.oneGroupOptionText.Name = "oneGroupOptionText";
            this.oneGroupOptionText.Size = new System.Drawing.Size(246, 30);
            this.oneGroupOptionText.TabIndex = 2;
            this.oneGroupOptionText.Text = "Játék 1 csapatban (2 fő):";
            // 
            // startButton
            // 
            this.startButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.startButton.Location = new System.Drawing.Point(136, 400);
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
            this.refereeModeText.Location = new System.Drawing.Point(46, 207);
            this.refereeModeText.Name = "refereeModeText";
            this.refereeModeText.Size = new System.Drawing.Size(348, 30);
            this.refereeModeText.TabIndex = 8;
            this.refereeModeText.Text = "Játékvezetői nézet külön ablakban:";
            // 
            // refereeModeCheckbox
            // 
            this.refereeModeCheckbox.Checked = true;
            this.refereeModeCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.refereeModeCheckbox.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.refereeModeCheckbox.Location = new System.Drawing.Point(400, 202);
            this.refereeModeCheckbox.Name = "refereeModeCheckbox";
            this.refereeModeCheckbox.Size = new System.Drawing.Size(34, 47);
            this.refereeModeCheckbox.TabIndex = 9;
            this.refereeModeCheckbox.UseVisualStyleBackColor = true;
            // 
            // twoGroupsOptionText
            // 
            this.twoGroupsOptionText.AutoSize = true;
            this.twoGroupsOptionText.Font = new System.Drawing.Font("Segoe UI", 13.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.twoGroupsOptionText.Location = new System.Drawing.Point(46, 157);
            this.twoGroupsOptionText.Name = "twoGroupsOptionText";
            this.twoGroupsOptionText.Size = new System.Drawing.Size(267, 30);
            this.twoGroupsOptionText.TabIndex = 10;
            this.twoGroupsOptionText.Text = "Játék 2 csapatban (2-2 fő):";
            // 
            // easyDifficultyOption
            // 
            this.easyDifficultyOption.AutoSize = true;
            this.easyDifficultyOption.Location = new System.Drawing.Point(23, 16);
            this.easyDifficultyOption.Name = "easyDifficultyOption";
            this.easyDifficultyOption.Size = new System.Drawing.Size(79, 24);
            this.easyDifficultyOption.TabIndex = 11;
            this.easyDifficultyOption.Text = "Könnyű";
            this.easyDifficultyOption.UseVisualStyleBackColor = true;
            this.easyDifficultyOption.CheckedChanged += new System.EventHandler(this.easyDifficultyOption_CheckedChanged);
            // 
            // mediumDifficultyOption
            // 
            this.mediumDifficultyOption.AutoSize = true;
            this.mediumDifficultyOption.Checked = true;
            this.mediumDifficultyOption.Location = new System.Drawing.Point(215, 16);
            this.mediumDifficultyOption.Name = "mediumDifficultyOption";
            this.mediumDifficultyOption.Size = new System.Drawing.Size(86, 24);
            this.mediumDifficultyOption.TabIndex = 12;
            this.mediumDifficultyOption.TabStop = true;
            this.mediumDifficultyOption.Text = "Közepes";
            this.mediumDifficultyOption.UseVisualStyleBackColor = true;
            this.mediumDifficultyOption.CheckedChanged += new System.EventHandler(this.mediumDifficultyOption_CheckedChanged);
            // 
            // hardDifficultyOption
            // 
            this.hardDifficultyOption.AutoSize = true;
            this.hardDifficultyOption.Location = new System.Drawing.Point(415, 16);
            this.hardDifficultyOption.Name = "hardDifficultyOption";
            this.hardDifficultyOption.Size = new System.Drawing.Size(72, 24);
            this.hardDifficultyOption.TabIndex = 13;
            this.hardDifficultyOption.Text = "Nehéz";
            this.hardDifficultyOption.UseVisualStyleBackColor = true;
            this.hardDifficultyOption.CheckedChanged += new System.EventHandler(this.hardDifficultyOption_CheckedChanged);
            // 
            // oneGroupOption
            // 
            this.oneGroupOption.AutoSize = true;
            this.oneGroupOption.Checked = true;
            this.oneGroupOption.Location = new System.Drawing.Point(6, 16);
            this.oneGroupOption.Name = "oneGroupOption";
            this.oneGroupOption.Size = new System.Drawing.Size(17, 16);
            this.oneGroupOption.TabIndex = 14;
            this.oneGroupOption.TabStop = true;
            this.oneGroupOption.UseVisualStyleBackColor = true;
            this.oneGroupOption.CheckedChanged += new System.EventHandler(this.oneGroupOption_CheckedChanged);
            // 
            // twoGroupsOption
            // 
            this.twoGroupsOption.AutoSize = true;
            this.twoGroupsOption.Location = new System.Drawing.Point(6, 62);
            this.twoGroupsOption.Name = "twoGroupsOption";
            this.twoGroupsOption.Size = new System.Drawing.Size(17, 16);
            this.twoGroupsOption.TabIndex = 15;
            this.twoGroupsOption.UseVisualStyleBackColor = true;
            this.twoGroupsOption.CheckedChanged += new System.EventHandler(this.twoGroupsOption_CheckedChanged);
            // 
            // groupChoice
            // 
            this.groupChoice.Controls.Add(this.oneGroupOption);
            this.groupChoice.Controls.Add(this.twoGroupsOption);
            this.groupChoice.Location = new System.Drawing.Point(318, 105);
            this.groupChoice.Name = "groupChoice";
            this.groupChoice.Size = new System.Drawing.Size(31, 84);
            this.groupChoice.TabIndex = 16;
            this.groupChoice.TabStop = false;
            // 
            // difficultyChoice
            // 
            this.difficultyChoice.Controls.Add(this.easyDifficultyOption);
            this.difficultyChoice.Controls.Add(this.mediumDifficultyOption);
            this.difficultyChoice.Controls.Add(this.hardDifficultyOption);
            this.difficultyChoice.Location = new System.Drawing.Point(12, 315);
            this.difficultyChoice.Name = "difficultyChoice";
            this.difficultyChoice.Size = new System.Drawing.Size(506, 46);
            this.difficultyChoice.TabIndex = 17;
            this.difficultyChoice.TabStop = false;
            // 
            // GameMenuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(530, 513);
            this.Controls.Add(this.difficultyChoice);
            this.Controls.Add(this.groupChoice);
            this.Controls.Add(this.twoGroupsOptionText);
            this.Controls.Add(this.refereeModeCheckbox);
            this.Controls.Add(this.refereeModeText);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.oneGroupOptionText);
            this.Controls.Add(this.difficultySelectionText);
            this.Controls.Add(this.groupSelectionText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "GameMenuForm";
            this.Text = "Robotok - Főmenü";
            this.groupChoice.ResumeLayout(false);
            this.groupChoice.PerformLayout();
            this.difficultyChoice.ResumeLayout(false);
            this.difficultyChoice.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private Label groupSelectionText;
    private Label difficultySelectionText;
    private Label oneGroupOptionText;
    private Button startButton;
    private Label refereeModeText;
    private CheckBox refereeModeCheckbox;
    private Label twoGroupsOptionText;
    private RadioButton easyDifficultyOption;
    private RadioButton mediumDifficultyOption;
    private RadioButton hardDifficultyOption;
    private RadioButton oneGroupOption;
    private RadioButton twoGroupsOption;
    private GroupBox groupChoice;
    private GroupBox difficultyChoice;
}