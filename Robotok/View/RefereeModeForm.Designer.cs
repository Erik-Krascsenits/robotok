namespace Robotok.WinForms.View
{
    partial class RefereeModeForm
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
            this.stepsLeftText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // stepsLeftText
            // 
            this.stepsLeftText.AutoSize = true;
            this.stepsLeftText.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.stepsLeftText.Location = new System.Drawing.Point(308, 27);
            this.stepsLeftText.Name = "stepsLeftText";
            this.stepsLeftText.Size = new System.Drawing.Size(217, 31);
            this.stepsLeftText.TabIndex = 13;
            this.stepsLeftText.Text = "Játékvezetői nézet:";
            // 
            // RefereeModeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(859, 505);
            this.Controls.Add(this.stepsLeftText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "RefereeModeForm";
            this.Text = "Játékvezetői nézet";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RefereeModeForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public Label stepsLeftText;
    }
}