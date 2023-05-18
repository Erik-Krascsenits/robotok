using ELTE.Robotok.Model;
using Robotok.WinForms.View;

namespace ELTE.Robotok.View;

public partial class GameMenuForm : Form
{
    #region Private fields

    private Int32 selectedDifficulty; // játék nehézsége 1 - könnyû, 2 - közepes, 3 - nehéz
    private Int32 selectedGroupCount; // csapatok száma 1 - (1 csoport, 2 játékos), 2 - (2 csoport, 2-2 játékos)
    private System.Windows.Forms.Timer _timer = null!; // idõzítõ a visszaszámláláshoz
    private Int32 actualPlayer; //  a jelenlegi játékos azonosítója 1 - zöld csapat 1. játékos, 8 - zöld csapat 2. játékos, 2 - piros csapat 1. játékos, 9 - piros csapat 2. játékos
    private GameForm _gameFormGreenTeamPlayerOne = null!;  // formok a különbözõ nézetek megjelenítéséhez (a piros csapat nézetei és a játékvezetõi nézet csak opcionálisan van példányosítva) 
    private GameForm _gameFormGreenTeamPlayerTwo = null!;
    private GameForm _gameFormRedTeamPlayerOne = null!;
    private GameForm _gameFormRedTeamPlayerTwo = null!;
    private RefereeModeForm _refereeModeForm = null!;

    #endregion

    #region Public fields

    public static GameMenuForm instance = null!;  // a fõmenü példánya, a játékosnézeteken ennek segítségével tudjuk elérni a modell réteget
    public RobotokGameModel _model = null!; // játékmodell

    #endregion

    #region Constructor

    /// <summary>
    /// Menüablak példányosítása
    /// </summary>
    public GameMenuForm()
    {
        instance = this; // felvesszük a jelenlegi formot instancenek, hogy a közvetlen és közvetett elemei elérhetõek legyenek a nézetekbõl
        selectedDifficulty = 2; // alapértelmezetten közepes nehézséget állítunk be
        selectedGroupCount = 1; // alapértelmezetten egy csoportos játékos állítunk be
        actualPlayer = 1; // beállítjuk a zöld csapat 1. játékosát kezdõ játékosnak
        InitializeComponent();
    }

    #endregion

    #region Menu button events

    /// <summary>
    /// Játék elindítása
    /// </summary>
    private void StartButton_Click(object sender, EventArgs e)
    {
        _model = new RobotokGameModel(selectedDifficulty, selectedGroupCount); // példányosítjuk a model-t
        _gameFormGreenTeamPlayerOne = new GameForm(selectedDifficulty, selectedGroupCount, 1); // példányosítjuk és megjelenítjük a zöld csapat játékosainak ablakait és jelezzük, hogy melyik ablak melyik játékosé
        _gameFormGreenTeamPlayerOne.Text = "Robotok - Zöld csapat - 1. játékos nézet";
        _gameFormGreenTeamPlayerOne.playerViewText.Text = "Zöld csapat - 1. játékos nézet:";
        _gameFormGreenTeamPlayerOne.Show();
        _gameFormGreenTeamPlayerTwo = new GameForm(selectedDifficulty, selectedGroupCount, 8);
        _gameFormGreenTeamPlayerTwo.Text = "Robotok - Zöld csapat - 2. játékos nézet";
        _gameFormGreenTeamPlayerTwo.playerViewText.Text = "Zöld csapat - 2. játékos nézet:";
        _gameFormGreenTeamPlayerTwo.Show();

        if (selectedGroupCount == 2) // ha 2 csoport lehetõség került kiválasztásra, akkor példányosítjuk a piros csapat játékosainak ablakait is
        {
            _gameFormRedTeamPlayerOne = new GameForm(selectedDifficulty, selectedGroupCount, 2);
            _gameFormRedTeamPlayerOne.Text = "Robotok - Piros csapat - 1. játékos nézet";
            _gameFormRedTeamPlayerOne.playerViewText.Text = "Piros csapat - 1. játékos nézet:";
            _gameFormRedTeamPlayerOne.Show();
            _gameFormRedTeamPlayerTwo = new GameForm(selectedDifficulty, selectedGroupCount, 9);
            _gameFormRedTeamPlayerTwo.Text = "Robotok - Piros csapat - 2. játékos nézet";
            _gameFormRedTeamPlayerTwo.playerViewText.Text = "Piros csapat - 2. játékos nézet:";
            _gameFormRedTeamPlayerTwo.Show();
        }

        DisablePlayerButtons(); // letiltjuk a játékosok mûveleti gombjait az egyes játékos kivételével

        if (refereeModeCheckbox.Checked)   // példányosítjuk a játékvezetõi módot attól függõen, hogy kiválasztásra került-e
        {
            _refereeModeForm = new RefereeModeForm();
            _refereeModeForm.Show();
        }

        _gameFormGreenTeamPlayerOne.communicationWindow.TextChanged += new EventHandler(Communication_TextChanged); // létrehozunk eseménykezelõket, amelyek a játékosnézetek kommunikációs ablakán történõ változásokat fogják figyelni
        _gameFormGreenTeamPlayerTwo.communicationWindow.TextChanged += new EventHandler(Communication_TextChanged);

        if (selectedGroupCount == 2)
        {
            _gameFormRedTeamPlayerOne.communicationWindow.TextChanged += new EventHandler(Communication_TextChanged);
            _gameFormRedTeamPlayerTwo.communicationWindow.TextChanged += new EventHandler(Communication_TextChanged);
        }

        _gameFormGreenTeamPlayerOne.BringToFront(); // elsõ lépésként a zöld csapat 1. játékosának ablakát helyezzük elõtérbe

        _timer = new System.Windows.Forms.Timer();  // idõzítõ létrehozása és elindítása (a fõmenübõl történik a Tick esemény lekezelése, a megváltozott állapotokat a nézetek letükrözik)
        _timer.Interval = 1000;
        _timer.Tick += new EventHandler(Timer_Tick);
        _timer.Start();

        startButton.Enabled = false;   // letiltjuk a fõmenü gombjait, amíg a játék tart
        difficultyChoice.Enabled = false;
        groupChoice.Enabled = false;
        refereeModeCheckbox.Enabled = false;       
    }

    /// <summary>
    /// Fõmenü bezárása
    /// </summary>
    private void GameMenuForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (MessageBox.Show("A fõmenü bezárásával az összes játékablak bezáródik. Folytatja?", "Figyelmeztetés", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
        {
            e.Cancel = true;
        }
    }

    #endregion

    #region Timer event handler

    /// <summary>
    /// Idõ múlásának eseménykezelõje
    /// </summary>
    private void Timer_Tick(Object? sender, EventArgs e)
    {
        GameOver(); // játék végének ellenõrzése és kezelése
        UpdateActivePlayerButtonStatuses(); 
        _model.AdvanceTime(actualPlayer); // játék léptetése

        _gameFormGreenTeamPlayerOne.remainingSecondsValueText.Text = _model.RemainingSeconds.ToString() + " másodperc"; // frissítjük a hátralevõ másodpercek számának kijelzését
        _gameFormGreenTeamPlayerTwo.remainingSecondsValueText.Text = _model.RemainingSeconds.ToString() + " másodperc";

       
        if (selectedGroupCount == 2)  // amennyiben 2 csapat játszik a többi játékos Formjának értékeit is kell frisíteni
        {
            _gameFormRedTeamPlayerOne.remainingSecondsValueText.Text = _model.RemainingSeconds.ToString() + " másodperc";
            _gameFormRedTeamPlayerTwo.remainingSecondsValueText.Text = _model.RemainingSeconds.ToString() + " másodperc";
        }
       
        if (_model.RemainingSeconds == 0)
        {
            if (selectedGroupCount == 1)
            {
                if (actualPlayer == 1)
                {
                    actualPlayer = 8;
                    _gameFormGreenTeamPlayerOne.DisableButtons();
                }
                else
                {
                    actualPlayer = 1;
                    _gameFormGreenTeamPlayerTwo.DisableButtons();
                }
            }
            else if (selectedGroupCount == 2)
            {
                if (actualPlayer == 1)
                {
                    actualPlayer = 8;
                    _gameFormGreenTeamPlayerOne.DisableButtons();
                }
                else if (actualPlayer == 8)
                {
                    actualPlayer = 2;
                    _gameFormGreenTeamPlayerTwo.DisableButtons();
                }
                else if (actualPlayer == 2)
                {
                    actualPlayer = 9;
                    _gameFormRedTeamPlayerOne.DisableButtons();
                }
                else if (actualPlayer == 9)
                {
                    actualPlayer = 1;
                    _gameFormRedTeamPlayerTwo.DisableButtons();
                }
            }

            if (actualPlayer == 1)
            {
                _gameFormGreenTeamPlayerOne.stepsLeftValueText.Text = (_model.GameStepCount - 1).ToString(); // ha elfogyott a gondolkodási idõ, csökkenti a hátralevõ lépések számát
                _gameFormGreenTeamPlayerOne.taskOneDeadlineValueText.Text = (_model.FirstTaskDeadline - 1).ToString() + " lépés van hátra";
                _gameFormGreenTeamPlayerOne.taskTwoDeadlineValueText.Text = (_model.SecondTaskDeadline - 1).ToString() + " lépés van hátra";
                _gameFormGreenTeamPlayerTwo.stepsLeftValueText.Text = (_model.GameStepCount - 1).ToString();
                _gameFormGreenTeamPlayerTwo.taskOneDeadlineValueText.Text = (_model.FirstTaskDeadline - 1).ToString() + " lépés van hátra";
                _gameFormGreenTeamPlayerTwo.taskTwoDeadlineValueText.Text = (_model.SecondTaskDeadline - 1).ToString() + " lépés van hátra";

                if (selectedGroupCount == 2) // 2 csapat esetén a pirosakét is frissíteni kell
                {
                    _gameFormRedTeamPlayerOne.stepsLeftValueText.Text = (_model.GameStepCount - 1).ToString();
                    _gameFormRedTeamPlayerOne.taskOneDeadlineValueText.Text = (_model.FirstTaskDeadline - 1).ToString() + " lépés van hátra";
                    _gameFormRedTeamPlayerOne.taskTwoDeadlineValueText.Text = (_model.SecondTaskDeadline - 1).ToString() + " lépés van hátra";
                    _gameFormRedTeamPlayerTwo.stepsLeftValueText.Text = (_model.GameStepCount - 1).ToString();
                    _gameFormRedTeamPlayerTwo.taskOneDeadlineValueText.Text = (_model.FirstTaskDeadline - 1).ToString() + " lépés van hátra";
                    _gameFormRedTeamPlayerTwo.taskTwoDeadlineValueText.Text = (_model.SecondTaskDeadline - 1).ToString() + " lépés van hátra";
                }  
            }
            else
            {
                _gameFormGreenTeamPlayerOne.stepsLeftValueText.Text = _model.GameStepCount.ToString(); // ha elfogyott a gondolkodási idõ, csökkenti a hátralevõ lépések számát
                _gameFormGreenTeamPlayerOne.taskOneDeadlineValueText.Text = _model.FirstTaskDeadline.ToString() + " lépés van hátra";
                _gameFormGreenTeamPlayerOne.taskTwoDeadlineValueText.Text = _model.SecondTaskDeadline.ToString() + " lépés van hátra";
                _gameFormGreenTeamPlayerTwo.stepsLeftValueText.Text = _model.GameStepCount.ToString();
                _gameFormGreenTeamPlayerTwo.taskOneDeadlineValueText.Text = _model.FirstTaskDeadline.ToString() + " lépés van hátra";
                _gameFormGreenTeamPlayerTwo.taskTwoDeadlineValueText.Text = _model.SecondTaskDeadline.ToString() + " lépés van hátra";

                if (selectedGroupCount == 2) // 2 csapat esetén a pirosakét is frissíteni kell
                {
                    _gameFormRedTeamPlayerOne.stepsLeftValueText.Text = _model.GameStepCount.ToString();
                    _gameFormRedTeamPlayerOne.taskOneDeadlineValueText.Text = _model.FirstTaskDeadline.ToString() + " lépés van hátra";
                    _gameFormRedTeamPlayerOne.taskTwoDeadlineValueText.Text = _model.SecondTaskDeadline.ToString() + " lépés van hátra";
                    _gameFormRedTeamPlayerTwo.stepsLeftValueText.Text = _model.GameStepCount.ToString();
                    _gameFormRedTeamPlayerTwo.taskOneDeadlineValueText.Text = _model.FirstTaskDeadline.ToString() + " lépés van hátra";
                    _gameFormRedTeamPlayerTwo.taskTwoDeadlineValueText.Text = _model.SecondTaskDeadline.ToString() + " lépés van hátra";
                }
            }
   
            ShowNextPlayerForm();

            if (refereeModeCheckbox.Checked)
            {
                _refereeModeForm.RefreshRefereeView();
            }
        }   
    }

    #endregion

    #region Radio button event handlers

    /// <summary>
    /// Nehéz nehézségû játékmód kiválasztása
    /// </summary>
    private void HardDifficultyOption_CheckedChanged(object sender, EventArgs e)
    {
        selectedDifficulty = 3;
    }

    /// <summary>
    /// Közepes nehézségû játékmód kiválasztása
    /// </summary>
    private void MediumDifficultyOption_CheckedChanged(object sender, EventArgs e)
    {
        selectedDifficulty = 2;
    }

    /// <summary>
    /// Könnyû nehézségû játékmód kiválasztása
    /// </summary>
    private void EasyDifficultyOption_CheckedChanged(object sender, EventArgs e)
    {
        selectedDifficulty = 1;
    }

    /// <summary>
    /// Egy csapat választása
    /// </summary>
    private void OneGroupOption_CheckedChanged(object sender, EventArgs e)
    {
        selectedGroupCount = 1;
    }

    /// <summary>
    /// Két csapat választása
    /// </summary>
    private void TwoGroupsOption_CheckedChanged(object sender, EventArgs e)
    {
        selectedGroupCount = 2;
    }

    #endregion

    #region Communication window event handler

    /// <summary>
    /// Kommunikációs ablakok tartalmának aktualizálása azonos csapaton belül
    /// </summary>
    private void Communication_TextChanged(object ?sender, EventArgs e)
    {
        TextBox? changedTextBox = null;
        if (sender != null)
        {
            changedTextBox = (TextBox)sender;
        }

        if (selectedGroupCount == 1)
        {

            if (changedTextBox == _gameFormGreenTeamPlayerOne.communicationWindow)
            {
                if (changedTextBox != null)
                {
                    _gameFormGreenTeamPlayerTwo.communicationWindow.Text = changedTextBox.Text;
                }
            }
            else
            {
                if (changedTextBox != null)
                {
                    _gameFormGreenTeamPlayerOne.communicationWindow.Text = changedTextBox.Text;
                }
            }
        }
        else if (selectedGroupCount == 2)
        {
            if (changedTextBox == _gameFormGreenTeamPlayerOne.communicationWindow)
            {
                _gameFormGreenTeamPlayerTwo.communicationWindow.Text = changedTextBox.Text;
            }
            else if (changedTextBox == _gameFormGreenTeamPlayerTwo.communicationWindow)
            {
                _gameFormGreenTeamPlayerOne.communicationWindow.Text = changedTextBox.Text;
            }
            else if (changedTextBox == _gameFormRedTeamPlayerOne.communicationWindow)
            {
                _gameFormRedTeamPlayerTwo.communicationWindow.Text = changedTextBox.Text;
            }
            else if (changedTextBox == _gameFormRedTeamPlayerTwo.communicationWindow)
            {
                _gameFormRedTeamPlayerOne.communicationWindow.Text = changedTextBox.Text;
            }
        }
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Aktív játékos mûveleti gombjainak engedélyezése
    /// </summary>
    private void UpdateActivePlayerButtonStatuses() 
    {
        if (actualPlayer == 1)
        {
            _gameFormGreenTeamPlayerOne.EnableButtons();

        }
        else if(actualPlayer == 8)
        { 
            _gameFormGreenTeamPlayerTwo.EnableButtons();
        }
        
        if (selectedGroupCount == 2)
        {
            if (actualPlayer == 2)
            {
                    _gameFormRedTeamPlayerOne.EnableButtons();
            }
            else if (actualPlayer == 9)
            {
                    _gameFormRedTeamPlayerTwo.EnableButtons();
            }
        }
    }

    /// <summary>
    /// Az elsõ játékos kivételével az összes játékos mûveleti gombjainak letiltása
    /// </summary>
    private void DisablePlayerButtons()
    {
        _gameFormGreenTeamPlayerTwo.DisableButtons();
        if (selectedGroupCount == 2)
        {
            _gameFormRedTeamPlayerOne.DisableButtons();
            _gameFormRedTeamPlayerTwo.DisableButtons();
        }
    }

    /// <summary>
    /// Játékos ablakok váltása
    /// </summary>
    private void ShowNextPlayerForm() // egyben, amikor megjelenítjük a nézetet, frissítjük is
    {
        if (actualPlayer == 1)
        {
            _gameFormGreenTeamPlayerOne.RefreshTable(actualPlayer);
            _gameFormGreenTeamPlayerOne.BringToFront();
        }
        else if (actualPlayer == 8)
        {
            _gameFormGreenTeamPlayerTwo.RefreshTable(actualPlayer);
            _gameFormGreenTeamPlayerTwo.BringToFront();
        }
        else if (actualPlayer == 2)
        {
            _gameFormRedTeamPlayerOne.RefreshTable(actualPlayer);
            _gameFormRedTeamPlayerOne.BringToFront();
        }
        else if (actualPlayer == 9)
        {
            _gameFormRedTeamPlayerTwo.RefreshTable(actualPlayer);
            _gameFormRedTeamPlayerTwo.BringToFront();
        }
    }

    /// <summary>
    /// Játék beállításainak engedélyezése
    /// </summary>
    private void EnableButtons()
    {
        startButton.Enabled = true;
        difficultyChoice.Enabled = true;
        groupChoice.Enabled = true;
        refereeModeCheckbox.Enabled = true;
    }

    /// <summary>
    /// Játék végének kezelése
    /// </summary>
    private void GameOver()
    {
        if (_model.IsGameOver)
        {
            _timer.Enabled = false;
            String winner = "A zöld csapat pontszáma: " + instance._model.GreenTeamPoints.ToString();
            if (instance._model.Teams == 2)
            {
                if (instance._model.GreenTeamPoints > instance._model.RedTeamPoints)
                {
                    winner = "A zöld csapat nyert! \nZöld csapat pontszámja: " + instance._model.GreenTeamPoints.ToString() + "\nPiros csapat pontszáma: " + instance._model.RedTeamPoints.ToString() + "\n";
                }
                else if (instance._model.GreenTeamPoints < instance._model.RedTeamPoints)
                {
                    winner = "A piros csapat nyert! \nPiros csapat pontszámja: " + instance._model.RedTeamPoints.ToString() + "\nZöld csapat pontszáma: " + instance._model.GreenTeamPoints.ToString() + "\n";
                } else
                {
                    winner = "Döntetlen! \nZöld csapat pontszámja: " + instance._model.GreenTeamPoints.ToString() + "\nPiros csapat pontszáma: " + instance._model.RedTeamPoints.ToString() + "\n";
                }
            }

            MessageBox.Show(winner, "Game over!" , MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            DisposeAllForms();
        }
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Játékosok ablakainak bezárása
    /// </summary>
    public void DisposeAllForms()
    {
        _gameFormGreenTeamPlayerOne.Dispose();
        _gameFormGreenTeamPlayerTwo.Dispose();
        if (selectedGroupCount == 2)
        {
            _gameFormRedTeamPlayerOne.Dispose();
            _gameFormRedTeamPlayerTwo.Dispose();
        }
        if (refereeModeCheckbox.Checked)
        {
            _refereeModeForm.Dispose();
        }
        _timer.Enabled = false;
        EnableButtons();
    }

    #endregion
}