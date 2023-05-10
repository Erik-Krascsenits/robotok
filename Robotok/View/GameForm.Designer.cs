namespace ELTE.Robotok.View
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameForm));
            this.playerViewText = new System.Windows.Forms.Label();
            this.noticeBoardText = new System.Windows.Forms.Label();
            this.taskOneDeadlineText = new System.Windows.Forms.Label();
            this.taskOnePointsText = new System.Windows.Forms.Label();
            this.taskTwoDeadlineText = new System.Windows.Forms.Label();
            this.taskTwoPointsText = new System.Windows.Forms.Label();
            this.taskTwoPointsValueText = new System.Windows.Forms.Label();
            this.taskTwoDeadlineValueText = new System.Windows.Forms.Label();
            this.taskOnePointsValueText = new System.Windows.Forms.Label();
            this.taskOneDeadlineValueText = new System.Windows.Forms.Label();
            this.stepsLeftText = new System.Windows.Forms.Label();
            this.stepsLeftValueText = new System.Windows.Forms.Label();
            this.redGroupPointsValueText = new System.Windows.Forms.Label();
            this.greenGroupPointsValueText = new System.Windows.Forms.Label();
            this.redGroupPointsText = new System.Windows.Forms.Label();
            this.greenGroupPointsText = new System.Windows.Forms.Label();
            this.remainingSecondsValueText = new System.Windows.Forms.Label();
            this.remainingSecondsText = new System.Windows.Forms.Label();
            this.communicationWindowText = new System.Windows.Forms.Label();
            this.communicationWindow = new System.Windows.Forms.TextBox();
            this.availableOperationsText = new System.Windows.Forms.Label();
            this.waitButton = new System.Windows.Forms.Button();
            this.moveButton = new System.Windows.Forms.Button();
            this.turnButton = new System.Windows.Forms.Button();
            this.attachButton = new System.Windows.Forms.Button();
            this.detachButton = new System.Windows.Forms.Button();
            this.attachCubesButton = new System.Windows.Forms.Button();
            this.detachCubesButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.operationParametersText = new System.Windows.Forms.Label();
            this.operationParameter = new System.Windows.Forms.ComboBox();
            this.coordinate1 = new System.Windows.Forms.TextBox();
            this.nextRoundValueText = new System.Windows.Forms.Label();
            this.nextRoundText = new System.Windows.Forms.Label();
            this.isSuccessful = new System.Windows.Forms.Label();
            this.successfulText = new System.Windows.Forms.Label();
            this.coordinate2 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // playerViewText
            // 
            this.playerViewText.AutoSize = true;
            this.playerViewText.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.playerViewText.Location = new System.Drawing.Point(127, 43);
            this.playerViewText.Name = "playerViewText";
            this.playerViewText.Size = new System.Drawing.Size(164, 31);
            this.playerViewText.TabIndex = 1;
            this.playerViewText.Text = "Játékos nézet:";
            // 
            // noticeBoardText
            // 
            this.noticeBoardText.AutoSize = true;
            this.noticeBoardText.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.noticeBoardText.Location = new System.Drawing.Point(23, 434);
            this.noticeBoardText.Name = "noticeBoardText";
            this.noticeBoardText.Size = new System.Drawing.Size(157, 31);
            this.noticeBoardText.TabIndex = 3;
            this.noticeBoardText.Text = "Hirdetőtábla:";
            // 
            // taskOneDeadlineText
            // 
            this.taskOneDeadlineText.AutoSize = true;
            this.taskOneDeadlineText.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.taskOneDeadlineText.Location = new System.Drawing.Point(603, 89);
            this.taskOneDeadlineText.Name = "taskOneDeadlineText";
            this.taskOneDeadlineText.Size = new System.Drawing.Size(188, 25);
            this.taskOneDeadlineText.TabIndex = 4;
            this.taskOneDeadlineText.Text = "1. feladat határideje:";
            // 
            // taskOnePointsText
            // 
            this.taskOnePointsText.AutoSize = true;
            this.taskOnePointsText.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.taskOnePointsText.Location = new System.Drawing.Point(603, 118);
            this.taskOnePointsText.Name = "taskOnePointsText";
            this.taskOnePointsText.Size = new System.Drawing.Size(205, 25);
            this.taskOnePointsText.TabIndex = 5;
            this.taskOnePointsText.Text = "1. feladatért járó pont:";
            // 
            // taskTwoDeadlineText
            // 
            this.taskTwoDeadlineText.AutoSize = true;
            this.taskTwoDeadlineText.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.taskTwoDeadlineText.Location = new System.Drawing.Point(603, 148);
            this.taskTwoDeadlineText.Name = "taskTwoDeadlineText";
            this.taskTwoDeadlineText.Size = new System.Drawing.Size(188, 25);
            this.taskTwoDeadlineText.TabIndex = 6;
            this.taskTwoDeadlineText.Text = "2. feladat határideje:";
            // 
            // taskTwoPointsText
            // 
            this.taskTwoPointsText.AutoSize = true;
            this.taskTwoPointsText.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.taskTwoPointsText.Location = new System.Drawing.Point(603, 177);
            this.taskTwoPointsText.Name = "taskTwoPointsText";
            this.taskTwoPointsText.Size = new System.Drawing.Size(205, 25);
            this.taskTwoPointsText.TabIndex = 7;
            this.taskTwoPointsText.Text = "2. feladatért járó pont:";
            // 
            // taskTwoPointsValueText
            // 
            this.taskTwoPointsValueText.AutoSize = true;
            this.taskTwoPointsValueText.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.taskTwoPointsValueText.Location = new System.Drawing.Point(848, 177);
            this.taskTwoPointsValueText.Name = "taskTwoPointsValueText";
            this.taskTwoPointsValueText.Size = new System.Drawing.Size(32, 25);
            this.taskTwoPointsValueText.TabIndex = 11;
            this.taskTwoPointsValueText.Text = GameMenuForm.instance._model.SecondTaskPoints.ToString();
            // 
            // taskTwoDeadlineValueText
            // 
            this.taskTwoDeadlineValueText.AutoSize = true;
            this.taskTwoDeadlineValueText.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.taskTwoDeadlineValueText.Location = new System.Drawing.Point(848, 148);
            this.taskTwoDeadlineValueText.Name = "taskTwoDeadlineValueText";
            this.taskTwoDeadlineValueText.Size = new System.Drawing.Size(92, 25);
            this.taskTwoDeadlineValueText.TabIndex = 10;
            this.taskTwoDeadlineValueText.Text = GameMenuForm.instance._model.SecondTaskDeadline.ToString() + " lépés van hátra";
            // 
            // taskOnePointsValueText
            // 
            this.taskOnePointsValueText.AutoSize = true;
            this.taskOnePointsValueText.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.taskOnePointsValueText.Location = new System.Drawing.Point(848, 118);
            this.taskOnePointsValueText.Name = "taskOnePointsValueText";
            this.taskOnePointsValueText.Size = new System.Drawing.Size(32, 25);
            this.taskOnePointsValueText.TabIndex = 9;
            this.taskOnePointsValueText.Text = GameMenuForm.instance._model.FirstTaskPoints.ToString();
            // 
            // taskOneDeadlineValueText
            // 
            this.taskOneDeadlineValueText.AutoSize = true;
            this.taskOneDeadlineValueText.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.taskOneDeadlineValueText.Location = new System.Drawing.Point(848, 89);
            this.taskOneDeadlineValueText.Name = "taskOneDeadlineValueText";
            this.taskOneDeadlineValueText.Size = new System.Drawing.Size(92, 25);
            this.taskOneDeadlineValueText.TabIndex = 8;
            this.taskOneDeadlineValueText.Text = GameMenuForm.instance._model.FirstTaskDeadline.ToString() + " lépés van hátra";
            // 
            // stepsLeftText
            // 
            this.stepsLeftText.AutoSize = true;
            this.stepsLeftText.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.stepsLeftText.Location = new System.Drawing.Point(599, 43);
            this.stepsLeftText.Name = "stepsLeftText";
            this.stepsLeftText.Size = new System.Drawing.Size(283, 31);
            this.stepsLeftText.TabIndex = 12;
            this.stepsLeftText.Text = "Hátralevő lépések száma:";
            // 
            // stepsLeftValueText
            // 
            this.stepsLeftValueText.AutoSize = true;
            this.stepsLeftValueText.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.stepsLeftValueText.Location = new System.Drawing.Point(881, 43);
            this.stepsLeftValueText.Name = "stepsLeftValueText";
            this.stepsLeftValueText.Size = new System.Drawing.Size(53, 31);
            this.stepsLeftValueText.TabIndex = 13;
            this.stepsLeftValueText.Text = "300";
            // 
            // redGroupPointsValueText
            // 
            this.redGroupPointsValueText.AutoSize = true;
            this.redGroupPointsValueText.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.redGroupPointsValueText.Location = new System.Drawing.Point(848, 244);
            this.redGroupPointsValueText.Name = "redGroupPointsValueText";
            this.redGroupPointsValueText.Size = new System.Drawing.Size(22, 25);
            this.redGroupPointsValueText.TabIndex = 17;

            // 
            // greenGroupPointsValueText
            // 
            this.greenGroupPointsValueText.AutoSize = true;
            this.greenGroupPointsValueText.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.greenGroupPointsValueText.Location = new System.Drawing.Point(848, 215);
            this.greenGroupPointsValueText.Name = "greenGroupPointsValueText";
            this.greenGroupPointsValueText.Size = new System.Drawing.Size(22, 25);
            this.greenGroupPointsValueText.TabIndex = 16;
            this.greenGroupPointsValueText.Text = GameMenuForm.instance._model.GreenTeamPoints.ToString();
            // 
            // redGroupPointsText
            // 
            this.redGroupPointsText.AutoSize = true;
            this.redGroupPointsText.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.redGroupPointsText.Location = new System.Drawing.Point(603, 244);
            this.redGroupPointsText.Name = "redGroupPointsText";
            this.redGroupPointsText.Size = new System.Drawing.Size(217, 25);
            this.redGroupPointsText.TabIndex = 15;
            this.redGroupPointsText.Text = "Piros csapat pontszáma:";
            // 
            // greenGroupPointsText
            // 
            this.greenGroupPointsText.AutoSize = true;
            this.greenGroupPointsText.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.greenGroupPointsText.Location = new System.Drawing.Point(603, 215);
            this.greenGroupPointsText.Name = "greenGroupPointsText";
            this.greenGroupPointsText.Size = new System.Drawing.Size(213, 25);
            this.greenGroupPointsText.TabIndex = 14;
            this.greenGroupPointsText.Text = "Zöld csapat pontszáma:";
            // 
            // remainingSecondsValueText
            // 
            this.remainingSecondsValueText.AutoSize = true;
            this.remainingSecondsValueText.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.remainingSecondsValueText.Location = new System.Drawing.Point(848, 285);
            this.remainingSecondsValueText.Name = "remainingSecondsValueText";
            this.remainingSecondsValueText.Size = new System.Drawing.Size(116, 25);
            this.remainingSecondsValueText.TabIndex = 19;
            this.remainingSecondsValueText.Text = "5 másodperc";
            // 
            // remainingSecondsText
            // 
            this.remainingSecondsText.AutoSize = true;
            this.remainingSecondsText.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.remainingSecondsText.Location = new System.Drawing.Point(603, 285);
            this.remainingSecondsText.Name = "remainingSecondsText";
            this.remainingSecondsText.Size = new System.Drawing.Size(172, 25);
            this.remainingSecondsText.TabIndex = 18;
            this.remainingSecondsText.Text = "Következő lépésig:";
            // 
            // communicationWindowText
            // 
            this.communicationWindowText.AutoSize = true;
            this.communicationWindowText.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.communicationWindowText.Location = new System.Drawing.Point(662, 396);
            this.communicationWindowText.Name = "communicationWindowText";
            this.communicationWindowText.Size = new System.Drawing.Size(258, 31);
            this.communicationWindowText.TabIndex = 20;
            this.communicationWindowText.Text = "Kommunikációs ablak:";
            // 
            // communicationWindow
            // 
            this.communicationWindow.Location = new System.Drawing.Point(595, 439);
            this.communicationWindow.Multiline = true;
            this.communicationWindow.Name = "communicationWindow";
            this.communicationWindow.Size = new System.Drawing.Size(383, 96);
            this.communicationWindow.TabIndex = 21;
            // 
            // availableOperationsText
            // 
            this.availableOperationsText.AutoSize = true;
            this.availableOperationsText.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.availableOperationsText.Location = new System.Drawing.Point(23, 558);
            this.availableOperationsText.Name = "availableOperationsText";
            this.availableOperationsText.Size = new System.Drawing.Size(268, 31);
            this.availableOperationsText.TabIndex = 22;
            this.availableOperationsText.Text = "Elvégezhető műveletek:";
            // 
            // waitButton
            // 
            this.waitButton.BackgroundImage = global::Robotok.WinForms.Properties.Resources.wait;
            this.waitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.waitButton.Location = new System.Drawing.Point(310, 552);
            this.waitButton.Name = "waitButton";
            this.waitButton.Size = new System.Drawing.Size(50, 50);
            this.waitButton.TabIndex = 23;
            this.waitButton.UseVisualStyleBackColor = true;
            this.waitButton.Click += new System.EventHandler(this.waitButton_Click);
            // 
            // moveButton
            // 
            this.moveButton.BackgroundImage = global::Robotok.WinForms.Properties.Resources.move;
            this.moveButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.moveButton.Location = new System.Drawing.Point(366, 552);
            this.moveButton.Name = "moveButton";
            this.moveButton.Size = new System.Drawing.Size(50, 50);
            this.moveButton.TabIndex = 24;
            this.moveButton.UseVisualStyleBackColor = true;
            this.moveButton.Click += new System.EventHandler(this.moveButton_Click);
            // 
            // turnButton
            // 
            this.turnButton.BackgroundImage = global::Robotok.WinForms.Properties.Resources.turn;
            this.turnButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.turnButton.Location = new System.Drawing.Point(422, 552);
            this.turnButton.Name = "turnButton";
            this.turnButton.Size = new System.Drawing.Size(50, 50);
            this.turnButton.TabIndex = 25;
            this.turnButton.UseVisualStyleBackColor = true;
            this.turnButton.Click += new System.EventHandler(this.turnButton_Click);
            // 
            // attachButton
            // 
            this.attachButton.BackgroundImage = global::Robotok.WinForms.Properties.Resources.attach;
            this.attachButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.attachButton.Location = new System.Drawing.Point(478, 552);
            this.attachButton.Name = "attachButton";
            this.attachButton.Size = new System.Drawing.Size(50, 50);
            this.attachButton.TabIndex = 26;
            this.attachButton.UseVisualStyleBackColor = true;
            this.attachButton.Click += new System.EventHandler(this.attachButton_Click);
            // 
            // detachButton
            // 
            this.detachButton.BackgroundImage = global::Robotok.WinForms.Properties.Resources.detach;
            this.detachButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.detachButton.Location = new System.Drawing.Point(534, 552);
            this.detachButton.Name = "detachButton";
            this.detachButton.Size = new System.Drawing.Size(50, 50);
            this.detachButton.TabIndex = 27;
            this.detachButton.UseVisualStyleBackColor = true;
            this.detachButton.Click += new System.EventHandler(this.dettachButton_Click);
            // 
            // attachCubesButton
            // 
            this.attachCubesButton.BackgroundImage = global::Robotok.WinForms.Properties.Resources.attach_cubes;
            this.attachCubesButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.attachCubesButton.Location = new System.Drawing.Point(590, 552);
            this.attachCubesButton.Name = "attachCubesButton";
            this.attachCubesButton.Size = new System.Drawing.Size(50, 50);
            this.attachCubesButton.TabIndex = 28;
            this.attachCubesButton.UseVisualStyleBackColor = true;
            this.attachCubesButton.Click += new System.EventHandler(this.attachCubesButton_Click);
            // 
            // detachCubesButton
            // 
            this.detachCubesButton.BackgroundImage = global::Robotok.WinForms.Properties.Resources.detach_cubes;
            this.detachCubesButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.detachCubesButton.Location = new System.Drawing.Point(646, 552);
            this.detachCubesButton.Name = "detachCubesButton";
            this.detachCubesButton.Size = new System.Drawing.Size(50, 50);
            this.detachCubesButton.TabIndex = 29;
            this.detachCubesButton.UseVisualStyleBackColor = true;
            this.detachCubesButton.Click += new System.EventHandler(this.detachCubesButton_Click);
            // 
            // clearButton
            // 
            this.clearButton.BackgroundImage = global::Robotok.WinForms.Properties.Resources.clear;
            this.clearButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.clearButton.Location = new System.Drawing.Point(702, 552);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(50, 50);
            this.clearButton.TabIndex = 30;
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // operationParametersText
            // 
            this.operationParametersText.AutoSize = true;
            this.operationParametersText.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.operationParametersText.Location = new System.Drawing.Point(23, 628);
            this.operationParametersText.Name = "operationParametersText";
            this.operationParametersText.Size = new System.Drawing.Size(257, 31);
            this.operationParametersText.TabIndex = 31;
            this.operationParametersText.Text = "Műveleti paraméterek:";
            // 
            // operationParameter
            // 
            this.operationParameter.FormattingEnabled = true;
            this.operationParameter.Items.AddRange(new object[] {
            "észak",
            "dél",
            "kelet",
            "nyugat",
            "óramutatóval megegyező",
            "óramutatóval ellenkező"});
            this.operationParameter.Location = new System.Drawing.Point(310, 631);
            this.operationParameter.Name = "operationParameter";
            this.operationParameter.Size = new System.Drawing.Size(218, 28);
            this.operationParameter.TabIndex = 32;
            // 
            // coordinate1
            // 
            this.coordinate1.BackColor = System.Drawing.Color.White;
            this.coordinate1.Enabled = false;
            this.coordinate1.Location = new System.Drawing.Point(556, 632);
            this.coordinate1.Name = "coordinate1";
            this.coordinate1.PlaceholderText = "1. kocka koordinátája";
            this.coordinate1.Size = new System.Drawing.Size(149, 27);
            this.coordinate1.TabIndex = 33;
            // 
            // nextRoundValueText
            // 
            this.nextRoundValueText.AutoSize = true;
            this.nextRoundValueText.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.nextRoundValueText.Location = new System.Drawing.Point(848, 317);
            this.nextRoundValueText.Name = "nextRoundValueText";
            this.nextRoundValueText.Size = new System.Drawing.Size(108, 25);
            this.nextRoundValueText.TabIndex = 36;
            this.nextRoundValueText.Text = "piros csapat";
            // 
            // nextRoundText
            // 
            this.nextRoundText.AutoSize = true;
            this.nextRoundText.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.nextRoundText.Location = new System.Drawing.Point(603, 317);
            this.nextRoundText.Name = "nextRoundText";
            this.nextRoundText.Size = new System.Drawing.Size(141, 25);
            this.nextRoundText.TabIndex = 35;
            this.nextRoundText.Text = "Következő kör:";
            // 
            // isSuccessful
            // 
            this.isSuccessful.AutoSize = true;
            this.isSuccessful.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.isSuccessful.Location = new System.Drawing.Point(603, 356);
            this.isSuccessful.Name = "isSuccessful";
            this.isSuccessful.Size = new System.Drawing.Size(239, 25);
            this.isSuccessful.TabIndex = 37;
            this.isSuccessful.Text = "Előző művelet sikeressége:";
            // 
            // successfulText
            // 
            this.successfulText.AutoSize = true;
            this.successfulText.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.successfulText.Location = new System.Drawing.Point(848, 356);
            this.successfulText.Name = "successfulText";
            this.successfulText.Size = new System.Drawing.Size(65, 25);
            this.successfulText.TabIndex = 38;
            this.successfulText.Text = "sikeres";
            // 
            // coordinate2
            // 
            this.coordinate2.BackColor = System.Drawing.Color.White;
            this.coordinate2.Enabled = false;
            this.coordinate2.Location = new System.Drawing.Point(733, 632);
            this.coordinate2.Name = "coordinate2";
            this.coordinate2.PlaceholderText = "2. kocka koordinátája";
            this.coordinate2.Size = new System.Drawing.Size(149, 27);
            this.coordinate2.TabIndex = 39;
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1046, 671);
            this.Controls.Add(this.coordinate2);
            this.Controls.Add(this.successfulText);
            this.Controls.Add(this.isSuccessful);
            this.Controls.Add(this.nextRoundValueText);
            this.Controls.Add(this.nextRoundText);
            this.Controls.Add(this.coordinate1);
            this.Controls.Add(this.operationParameter);
            this.Controls.Add(this.operationParametersText);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.detachCubesButton);
            this.Controls.Add(this.attachCubesButton);
            this.Controls.Add(this.detachButton);
            this.Controls.Add(this.attachButton);
            this.Controls.Add(this.turnButton);
            this.Controls.Add(this.moveButton);
            this.Controls.Add(this.waitButton);
            this.Controls.Add(this.availableOperationsText);
            this.Controls.Add(this.communicationWindow);
            this.Controls.Add(this.communicationWindowText);
            this.Controls.Add(this.remainingSecondsValueText);
            this.Controls.Add(this.remainingSecondsText);
            this.Controls.Add(this.redGroupPointsValueText);
            this.Controls.Add(this.greenGroupPointsValueText);
            this.Controls.Add(this.redGroupPointsText);
            this.Controls.Add(this.greenGroupPointsText);
            this.Controls.Add(this.stepsLeftValueText);
            this.Controls.Add(this.stepsLeftText);
            this.Controls.Add(this.taskTwoPointsValueText);
            this.Controls.Add(this.taskTwoDeadlineValueText);
            this.Controls.Add(this.taskOnePointsValueText);
            this.Controls.Add(this.taskOneDeadlineValueText);
            this.Controls.Add(this.taskTwoPointsText);
            this.Controls.Add(this.taskTwoDeadlineText);
            this.Controls.Add(this.taskOnePointsText);
            this.Controls.Add(this.taskOneDeadlineText);
            this.Controls.Add(this.noticeBoardText);
            this.Controls.Add(this.playerViewText);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "GameForm";
            this.Text = "Robotok";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public Label playerViewText;
        public Label noticeBoardText;
        public Label taskOneDeadlineText;
        public Label taskOnePointsText;
        public Label taskTwoDeadlineText;
        public Label taskTwoPointsText;
        public Label taskTwoPointsValueText;
        public Label taskTwoDeadlineValueText;
        public Label taskOnePointsValueText;
        public Label taskOneDeadlineValueText;
        public Label stepsLeftText;
        public Label stepsLeftValueText;
        public Label redGroupPointsValueText;
        public Label greenGroupPointsValueText;
        public Label redGroupPointsText;
        public Label greenGroupPointsText;
        public Label remainingSecondsValueText;
        public Label remainingSecondsText;
        public Label communicationWindowText;
        public TextBox communicationWindow;
        public Label availableOperationsText;
        public Button waitButton;
        public Button moveButton;
        public Button turnButton;
        public Button attachButton;
        public Button detachButton;
        public Button attachCubesButton;
        public Button detachCubesButton;
        public Button clearButton;
        public Label operationParametersText;
        public ComboBox operationParameter;
        public TextBox coordinate1;
        public Label nextRoundValueText;
        public Label nextRoundText;
        public Label isSuccessful;
        public Label successfulText;
        public TextBox coordinate2;
    }
}