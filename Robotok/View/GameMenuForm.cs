using ELTE.Robotok.Model;
using ELTE.Robotok.Persistence;
using Robotok.WinForms.View;

namespace ELTE.Robotok.View;

public partial class GameMenuForm : Form
{
    #region Fields

    Int32 selectedDifficulty; // játék nehézsége 1 - könnyû, 2 - közepes, 3 - nehéz
    Int32 selectedGroupCount; // csapatok száma 1 - (1 csoport, 2 játékos), 2 - (2 csoport, 2-2 játékos)

    // A fõmenü példánya, a játékosnézeteken ennek segítségével tudjuk elérni a fõmenü segítségével a modell réteget
    public static GameMenuForm instance = null!;

    private IRobotokDataAccess _dataAccess = null!; // adatelérés
    public RobotokGameModel _model = null!; // játékmodell

    private System.Windows.Forms.Timer _timer = null!; // idõzítõ a visszaszámláláshoz

    /* 
    Azt a játékos tárolja el, akié a jelenlegi lépés,
    1 - zöld csapat 1. játékos
    2 - zöld csapat 2. játékos
    3 - piros csapat 1. játékos
    4 - piros csapat 2. játékos
    Így változik az értéke: 1 csapat esetén: 1 -> 2 -> 1 -> ... 2 csapat esetén: 1 -> 2 -> 3 -> 4 -> 1 -> ...
    */
    public int actualPlayer;

    // Formok a különbözõ nézetek megjelenítéségez (a piros csapat nézetei és a játékvezetõi nézet csak opcionálisan van példányosítva) 

    GameForm _gameFormGreenTeamPlayerOne = null!;
    GameForm _gameFormGreenTeamPlayerTwo = null!;

    GameForm _gameFormRedTeamPlayerOne = null!;
    GameForm _gameFormRedTeamPlayerTwo = null!;

    RefereeModeForm _refereeModeForm = null!;

    #endregion

    #region Constructor

    public GameMenuForm()
    {
        /*
        Felvesszük a jelenlegi formot instancenek (azért van rá szükség, hogy a közvetlen
        és közvetett elemei (mint például a modell réteg) elérhetõ legyen a nézetekbõl
        (például le tudják kérdezni a játéktábla tartalmát)
        */
        instance = this;
        selectedDifficulty = 2; // alapértelmezetten közepes nehézséget állítunk be
        selectedGroupCount = 1; // alapértelmezetten egy csoportos játékos állítunk be
        actualPlayer = 1; // beállítjuk a zöld csapat 1. játékosát kezdõ játékosnak
        InitializeComponent();
    }

    #endregion

    #region Menu button events

    private void startButton_Click(object sender, EventArgs e)
    {
        _model = new RobotokGameModel(_dataAccess, selectedDifficulty, selectedGroupCount); // amikor a játékos el akarja indítani a játékot a fõmenübõl, akkor példányosítjuk a model-t
        _model.NewGame(); // új játék kezdete (a modell legenerálja a kezdõ pályát)

        // Példányosítjuk és megjelenítjük a zöld csapat játékosainak ablakait és jelezzük, hogy melyik ablak melyik játékosé

        _gameFormGreenTeamPlayerOne = new GameForm(selectedDifficulty, selectedGroupCount, actualPlayer);
        _gameFormGreenTeamPlayerOne.Text = "Robotok - Zöld csapat - 1. játékos nézet";
        _gameFormGreenTeamPlayerOne.playerViewText.Text = "Zöld csapat - 1. játékos nézet:";
        _gameFormGreenTeamPlayerOne.Show();
        _gameFormGreenTeamPlayerTwo = new GameForm(selectedDifficulty, selectedGroupCount, actualPlayer);
        _gameFormGreenTeamPlayerTwo.Text = "Robotok - Zöld csapat - 2. játékos nézet";
        _gameFormGreenTeamPlayerTwo.playerViewText.Text = "Zöld csapat - 2. játékos nézet:";
        _gameFormGreenTeamPlayerTwo.Show();

        //Ha 2 csoport lehetõség került kiválasztásra, akkor példányosítjuk a piros csapatú játékosok ablakait is

        if (selectedGroupCount == 2)
        {
            _gameFormRedTeamPlayerOne = new GameForm(selectedDifficulty, selectedGroupCount, actualPlayer);
            _gameFormRedTeamPlayerOne.Text = "Robotok - Piros csapat - 1. játékos nézet";
            _gameFormRedTeamPlayerOne.playerViewText.Text = "Piros csapat - 1. játékos nézet:";
            _gameFormRedTeamPlayerOne.Show();
            _gameFormRedTeamPlayerTwo = new GameForm(selectedDifficulty, selectedGroupCount, actualPlayer);
            _gameFormRedTeamPlayerTwo.Text = "Robotok - Piros csapat - 2. játékos nézet";
            _gameFormRedTeamPlayerTwo.playerViewText.Text = "Piros csapat - 2. játékos nézet:";
            _gameFormRedTeamPlayerTwo.Show();
        }

        // Példányosítjuk a játékvezetõi módot attól függõen, hogy kiválasztásra került-e

        if (refereeModeCheckbox.Checked)
        {
            _refereeModeForm = new RefereeModeForm();
            _refereeModeForm.Show();
        }

        // Létrehozunk eseménykezelõket, amelyek a játékosnézetek kommunikációs ablakán történõ változásokat fogják figyelni
        _gameFormGreenTeamPlayerOne.communicationWindow.TextChanged += new EventHandler(communication_TextChanged);
        _gameFormGreenTeamPlayerTwo.communicationWindow.TextChanged += new EventHandler(communication_TextChanged);

        if (selectedGroupCount == 2)
        {
            _gameFormRedTeamPlayerOne.communicationWindow.TextChanged += new EventHandler(communication_TextChanged);
            _gameFormRedTeamPlayerTwo.communicationWindow.TextChanged += new EventHandler(communication_TextChanged);
        }

        // Elsõ lépésként a zöld csapat 1. játékosának ablakát helyezzük elõtérbe

        _gameFormGreenTeamPlayerOne.BringToFront();

        // Idõzítõ létrehozása és elindítása (a fõmenübõl történik a Tick esemény lekezelése, a megváltozott állapotokat a nézetek letükrözik)

        _timer = new System.Windows.Forms.Timer();
        _timer.Interval = 1000;
        _timer.Tick += new EventHandler(Timer_Tick);
        _timer.Start();

        // Letiltjuk a fõmenü gombjait, amíg a játék tart
        startButton.Enabled = false;
        difficultyChoice.Enabled = false;
        groupChoice.Enabled = false;
        refereeModeCheckbox.Enabled = false;
    }

    private void GameMenuForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (MessageBox.Show("A fõmenü bezárásával az összes játékablak bezáródik. Folytatja?", "Figyelmeztetés", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
        {
            e.Cancel = true;
        }
    }

    #endregion

    #region Timer event handler

    private void Timer_Tick(Object? sender, EventArgs e)
    {
        GameOver();
        _model.AdvanceTime(actualPlayer); // Játék léptetése

        _gameFormGreenTeamPlayerOne.remainingSecondsValueText.Text = _model.RemainingSeconds.ToString() + " másodperc"; // frissítjük a hátralevõ másodpercek számának kijelzését
        _gameFormGreenTeamPlayerTwo.remainingSecondsValueText.Text = _model.RemainingSeconds.ToString() + " másodperc";

        // Amennyiben 2 csapat játszik a többi játékos Formjának értékeit is kell frisíteni
        if (selectedGroupCount == 2)
        {
            _gameFormRedTeamPlayerOne.remainingSecondsValueText.Text = _model.RemainingSeconds.ToString() + " másodperc";
            _gameFormRedTeamPlayerTwo.remainingSecondsValueText.Text = _model.RemainingSeconds.ToString() + " másodperc";
        }

        if (_model.RemainingSeconds == 0)
        {
            if (selectedGroupCount == 1)
            {
                if (actualPlayer < 2)
                {
                    actualPlayer++;
                }
                else
                {
                    actualPlayer = 1;
                }
            }
            else if (selectedGroupCount == 2)
            {
                if (actualPlayer < 4)
                {
                    actualPlayer++;
                }
                else
                {
                    actualPlayer = 1;
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

            UpdatePlayerButtonStatuses();
            ShowNextPlayerForm();

            if (refereeModeCheckbox.Checked)
            {
                _refereeModeForm.RefreshRefereeView();
            }

        }
        else if(_model.RemainingSeconds == 5)
        {
            UpdatePlayerButtonStatuses();
        }
           
    }

    #endregion

    #region Radio button event handlers

    private void hardDifficultyOption_CheckedChanged(object sender, EventArgs e)
    {
        selectedDifficulty = 3;
    }

    private void mediumDifficultyOption_CheckedChanged(object sender, EventArgs e)
    {
        selectedDifficulty = 2;
    }

    private void easyDifficultyOption_CheckedChanged(object sender, EventArgs e)
    {
        selectedDifficulty = 1;
    }

    private void oneGroupOption_CheckedChanged(object sender, EventArgs e)
    {
        selectedGroupCount = 1;
    }

    private void twoGroupsOption_CheckedChanged(object sender, EventArgs e)
    {
        selectedGroupCount = 2;
    }

    #endregion

    #region Communication window event handler

    /*
    Megnézi melyik szövegdobozban történt változás és a többi tartalmát ennek megfelelõen aktualizálja,
    két csoport esetén a csoportok külön-külön kommunikációs felülettel rendelkeznek, egymásét nem látják
        
    */
    private void communication_TextChanged(object sender, EventArgs e)
    {
        TextBox changedTextBox = (TextBox)sender;

        if (selectedGroupCount == 1)
        {

            if (changedTextBox == _gameFormGreenTeamPlayerOne.communicationWindow)
            {
                _gameFormGreenTeamPlayerTwo.communicationWindow.Text = changedTextBox.Text;
            }
            else
            {
                _gameFormGreenTeamPlayerOne.communicationWindow.Text = changedTextBox.Text;
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

    // Kör kezdetén az aktív játékos gombját engedélyezi, a többit letiltja
    private void UpdatePlayerButtonStatuses()
    {
        if (selectedGroupCount == 1)
        {
            
            if (actualPlayer == 1)
            {
                if (_gameFormGreenTeamPlayerOne.stepsLeftValueText.Text != "0" && _model.RemainingSeconds != 0) 
                {
                    _gameFormGreenTeamPlayerOne.EnableButtons();
                }
                _gameFormGreenTeamPlayerTwo.DisableButtons();

            }
            else
            {
                if (_gameFormGreenTeamPlayerOne.stepsLeftValueText.Text != "0" && _model.RemainingSeconds != 0)
                {
                    _gameFormGreenTeamPlayerOne.DisableButtons();
                }
                _gameFormGreenTeamPlayerTwo.EnableButtons();
            }
        }
        else if (selectedGroupCount == 2)
        {
            if (actualPlayer == 1)
            {
                if (_gameFormGreenTeamPlayerOne.stepsLeftValueText.Text != "0" && _model.RemainingSeconds != 0)
                {
                    _gameFormGreenTeamPlayerOne.EnableButtons();
                }
                _gameFormGreenTeamPlayerTwo.DisableButtons();
                _gameFormRedTeamPlayerOne.DisableButtons();
                _gameFormRedTeamPlayerTwo.DisableButtons();
            }
            if (actualPlayer == 2)
            {
                _gameFormGreenTeamPlayerOne.DisableButtons();
                if (_gameFormGreenTeamPlayerOne.stepsLeftValueText.Text != "0" && _model.RemainingSeconds != 0)
                {
                    _gameFormGreenTeamPlayerTwo.EnableButtons();
                }
                _gameFormRedTeamPlayerOne.DisableButtons();
                _gameFormRedTeamPlayerTwo.DisableButtons();
            }
            if (actualPlayer == 3)
            {
                _gameFormGreenTeamPlayerOne.DisableButtons();
                _gameFormGreenTeamPlayerTwo.DisableButtons();
                if (_gameFormGreenTeamPlayerOne.stepsLeftValueText.Text != "0" && _model.RemainingSeconds != 0)
                {
                    _gameFormRedTeamPlayerOne.EnableButtons();
                }
                _gameFormRedTeamPlayerTwo.DisableButtons();
            }
            if (actualPlayer == 4)
            {
                _gameFormGreenTeamPlayerOne.DisableButtons();
                _gameFormGreenTeamPlayerTwo.DisableButtons();
                _gameFormRedTeamPlayerOne.DisableButtons();
                if (_gameFormGreenTeamPlayerOne.stepsLeftValueText.Text != "0" && _model.RemainingSeconds != 0)
                {
                    _gameFormRedTeamPlayerTwo.EnableButtons();
                }
            }
        }
    }

    private void ShowNextPlayerForm() // egyben, amikor megjelenítjük a nézetet, frissítjük is
    {
        if (actualPlayer == 1)
        {
            _gameFormGreenTeamPlayerOne.RefreshTable(actualPlayer);
            _gameFormGreenTeamPlayerOne.BringToFront();
        }
        else if (actualPlayer == 2)
        {
            _gameFormGreenTeamPlayerTwo.RefreshTable(actualPlayer);
            _gameFormGreenTeamPlayerTwo.BringToFront();
        }
        else if (actualPlayer == 3)
        {
            _gameFormRedTeamPlayerOne.RefreshTable(actualPlayer);
            _gameFormRedTeamPlayerOne.BringToFront();
        }
        else if (actualPlayer == 4)
        {
            _gameFormRedTeamPlayerTwo.RefreshTable(actualPlayer);
            _gameFormRedTeamPlayerTwo.BringToFront();
        }
    }

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

    private void EnableButtons()
    {
        startButton.Enabled = true;
        difficultyChoice.Enabled = true;
        groupChoice.Enabled = true;
        refereeModeCheckbox.Enabled = true;
    }

    private void GameOver()
    {
        if (_model.IsGameOver)
        {
            _timer.Enabled = false;
            string winner = "A zöld csapat pontszáma: " + instance._model.GreenTeamPoints.ToString();
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
}