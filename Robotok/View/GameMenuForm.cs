using ELTE.Robotok.Model;
using ELTE.Robotok.Persistence;
using Robotok.WinForms.View;

namespace ELTE.Robotok.View;

public partial class GameMenuForm : Form
{
    #region Fields

    Int32 selectedDifficulty; // j�t�k neh�zs�ge 1 - k�nny�, 2 - k�zepes, 3 - neh�z
    Int32 selectedGroupCount; // csapatok sz�ma 1 - (1 csoport, 2 j�t�kos), 2 - (2 csoport, 2-2 j�t�kos)

    // A f�men� p�ld�nya, a j�t�kosn�zeteken ennek seg�ts�g�vel tudjuk el�rni a f�men� seg�ts�g�vel a modell r�teget
    public static GameMenuForm instance = null!;

    private IRobotokDataAccess _dataAccess = null!; // adatel�r�s
    public RobotokGameModel _model = null!; // j�t�kmodell

    private System.Windows.Forms.Timer _timer = null!; // id�z�t� a visszasz�ml�l�shoz

    /* 
    Azt a j�t�kos t�rolja el, aki� a jelenlegi l�p�s,
    1 - z�ld csapat 1. j�t�kos
    2 - z�ld csapat 2. j�t�kos
    3 - piros csapat 1. j�t�kos
    4 - piros csapat 2. j�t�kos
    �gy v�ltozik az �rt�ke: 1 csapat eset�n: 1 -> 2 -> 1 -> ... 2 csapat eset�n: 1 -> 2 -> 3 -> 4 -> 1 -> ...
    */
    public int actualPlayer;

    // Formok a k�l�nb�z� n�zetek megjelen�t�s�gez (a piros csapat n�zetei �s a j�t�kvezet�i n�zet csak opcion�lisan van p�ld�nyos�tva) 

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
        Felvessz�k a jelenlegi formot instancenek (az�rt van r� sz�ks�g, hogy a k�zvetlen
        �s k�zvetett elemei (mint p�ld�ul a modell r�teg) el�rhet� legyen a n�zetekb�l
        (p�ld�ul le tudj�k k�rdezni a j�t�kt�bla tartalm�t)
        */
        instance = this;
        selectedDifficulty = 2; // alap�rtelmezetten k�zepes neh�zs�get �ll�tunk be
        selectedGroupCount = 1; // alap�rtelmezetten egy csoportos j�t�kos �ll�tunk be
        actualPlayer = 1; // be�ll�tjuk a z�ld csapat 1. j�t�kos�t kezd� j�t�kosnak
        InitializeComponent();
    }

    #endregion

    #region Menu button events

    private void startButton_Click(object sender, EventArgs e)
    {
        _model = new RobotokGameModel(_dataAccess, selectedDifficulty, selectedGroupCount); // amikor a j�t�kos el akarja ind�tani a j�t�kot a f�men�b�l, akkor p�ld�nyos�tjuk a model-t
        _model.NewGame(); // �j j�t�k kezdete (a modell legener�lja a kezd� p�ly�t)

        // P�ld�nyos�tjuk �s megjelen�tj�k a z�ld csapat j�t�kosainak ablakait �s jelezz�k, hogy melyik ablak melyik j�t�kos�

        _gameFormGreenTeamPlayerOne = new GameForm(selectedDifficulty, selectedGroupCount, actualPlayer);
        _gameFormGreenTeamPlayerOne.Text = "Robotok - Z�ld csapat - 1. j�t�kos n�zet";
        _gameFormGreenTeamPlayerOne.playerViewText.Text = "Z�ld csapat - 1. j�t�kos n�zet:";
        _gameFormGreenTeamPlayerOne.Show();
        _gameFormGreenTeamPlayerTwo = new GameForm(selectedDifficulty, selectedGroupCount, actualPlayer);
        _gameFormGreenTeamPlayerTwo.Text = "Robotok - Z�ld csapat - 2. j�t�kos n�zet";
        _gameFormGreenTeamPlayerTwo.playerViewText.Text = "Z�ld csapat - 2. j�t�kos n�zet:";
        _gameFormGreenTeamPlayerTwo.Show();

        //Ha 2 csoport lehet�s�g ker�lt kiv�laszt�sra, akkor p�ld�nyos�tjuk a piros csapat� j�t�kosok ablakait is

        if (selectedGroupCount == 2)
        {
            _gameFormRedTeamPlayerOne = new GameForm(selectedDifficulty, selectedGroupCount, actualPlayer);
            _gameFormRedTeamPlayerOne.Text = "Robotok - Piros csapat - 1. j�t�kos n�zet";
            _gameFormRedTeamPlayerOne.playerViewText.Text = "Piros csapat - 1. j�t�kos n�zet:";
            _gameFormRedTeamPlayerOne.Show();
            _gameFormRedTeamPlayerTwo = new GameForm(selectedDifficulty, selectedGroupCount, actualPlayer);
            _gameFormRedTeamPlayerTwo.Text = "Robotok - Piros csapat - 2. j�t�kos n�zet";
            _gameFormRedTeamPlayerTwo.playerViewText.Text = "Piros csapat - 2. j�t�kos n�zet:";
            _gameFormRedTeamPlayerTwo.Show();
        }

        // P�ld�nyos�tjuk a j�t�kvezet�i m�dot att�l f�gg�en, hogy kiv�laszt�sra ker�lt-e

        if (refereeModeCheckbox.Checked)
        {
            _refereeModeForm = new RefereeModeForm();
            _refereeModeForm.Show();
        }

        // L�trehozunk esem�nykezel�ket, amelyek a j�t�kosn�zetek kommunik�ci�s ablak�n t�rt�n� v�ltoz�sokat fogj�k figyelni
        _gameFormGreenTeamPlayerOne.communicationWindow.TextChanged += new EventHandler(communication_TextChanged);
        _gameFormGreenTeamPlayerTwo.communicationWindow.TextChanged += new EventHandler(communication_TextChanged);

        if (selectedGroupCount == 2)
        {
            _gameFormRedTeamPlayerOne.communicationWindow.TextChanged += new EventHandler(communication_TextChanged);
            _gameFormRedTeamPlayerTwo.communicationWindow.TextChanged += new EventHandler(communication_TextChanged);
        }

        // Els� l�p�sk�nt a z�ld csapat 1. j�t�kos�nak ablak�t helyezz�k el�t�rbe

        _gameFormGreenTeamPlayerOne.BringToFront();

        // Id�z�t� l�trehoz�sa �s elind�t�sa (a f�men�b�l t�rt�nik a Tick esem�ny lekezel�se, a megv�ltozott �llapotokat a n�zetek let�kr�zik)

        _timer = new System.Windows.Forms.Timer();
        _timer.Interval = 1000;
        _timer.Tick += new EventHandler(Timer_Tick);
        _timer.Start();

        // Letiltjuk a f�men� gombjait, am�g a j�t�k tart
        startButton.Enabled = false;
        difficultyChoice.Enabled = false;
        groupChoice.Enabled = false;
        refereeModeCheckbox.Enabled = false;
    }

    private void GameMenuForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (MessageBox.Show("A f�men� bez�r�s�val az �sszes j�t�kablak bez�r�dik. Folytatja?", "Figyelmeztet�s", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
        {
            e.Cancel = true;
        }
    }

    #endregion

    #region Timer event handler

    private void Timer_Tick(Object? sender, EventArgs e)
    {
        GameOver();
        _model.AdvanceTime(actualPlayer); // J�t�k l�ptet�se

        _gameFormGreenTeamPlayerOne.remainingSecondsValueText.Text = _model.RemainingSeconds.ToString() + " m�sodperc"; // friss�tj�k a h�tralev� m�sodpercek sz�m�nak kijelz�s�t
        _gameFormGreenTeamPlayerTwo.remainingSecondsValueText.Text = _model.RemainingSeconds.ToString() + " m�sodperc";

        // Amennyiben 2 csapat j�tszik a t�bbi j�t�kos Formj�nak �rt�keit is kell fris�teni
        if (selectedGroupCount == 2)
        {
            _gameFormRedTeamPlayerOne.remainingSecondsValueText.Text = _model.RemainingSeconds.ToString() + " m�sodperc";
            _gameFormRedTeamPlayerTwo.remainingSecondsValueText.Text = _model.RemainingSeconds.ToString() + " m�sodperc";
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
                _gameFormGreenTeamPlayerOne.stepsLeftValueText.Text = (_model.GameStepCount - 1).ToString(); // ha elfogyott a gondolkod�si id�, cs�kkenti a h�tralev� l�p�sek sz�m�t
                _gameFormGreenTeamPlayerOne.taskOneDeadlineValueText.Text = (_model.FirstTaskDeadline - 1).ToString() + " l�p�s van h�tra";
                _gameFormGreenTeamPlayerOne.taskTwoDeadlineValueText.Text = (_model.SecondTaskDeadline - 1).ToString() + " l�p�s van h�tra";
                _gameFormGreenTeamPlayerTwo.stepsLeftValueText.Text = (_model.GameStepCount - 1).ToString();
                _gameFormGreenTeamPlayerTwo.taskOneDeadlineValueText.Text = (_model.FirstTaskDeadline - 1).ToString() + " l�p�s van h�tra";
                _gameFormGreenTeamPlayerTwo.taskTwoDeadlineValueText.Text = (_model.SecondTaskDeadline - 1).ToString() + " l�p�s van h�tra";

                if (selectedGroupCount == 2) // 2 csapat eset�n a pirosak�t is friss�teni kell
                {
                    _gameFormRedTeamPlayerOne.stepsLeftValueText.Text = (_model.GameStepCount - 1).ToString();
                    _gameFormRedTeamPlayerOne.taskOneDeadlineValueText.Text = (_model.FirstTaskDeadline - 1).ToString() + " l�p�s van h�tra";
                    _gameFormRedTeamPlayerOne.taskTwoDeadlineValueText.Text = (_model.SecondTaskDeadline - 1).ToString() + " l�p�s van h�tra";
                    _gameFormRedTeamPlayerTwo.stepsLeftValueText.Text = (_model.GameStepCount - 1).ToString();
                    _gameFormRedTeamPlayerTwo.taskOneDeadlineValueText.Text = (_model.FirstTaskDeadline - 1).ToString() + " l�p�s van h�tra";
                    _gameFormRedTeamPlayerTwo.taskTwoDeadlineValueText.Text = (_model.SecondTaskDeadline - 1).ToString() + " l�p�s van h�tra";
                }  
            }
            else
            {
                _gameFormGreenTeamPlayerOne.stepsLeftValueText.Text = _model.GameStepCount.ToString(); // ha elfogyott a gondolkod�si id�, cs�kkenti a h�tralev� l�p�sek sz�m�t
                _gameFormGreenTeamPlayerOne.taskOneDeadlineValueText.Text = _model.FirstTaskDeadline.ToString() + " l�p�s van h�tra";
                _gameFormGreenTeamPlayerOne.taskTwoDeadlineValueText.Text = _model.SecondTaskDeadline.ToString() + " l�p�s van h�tra";
                _gameFormGreenTeamPlayerTwo.stepsLeftValueText.Text = _model.GameStepCount.ToString();
                _gameFormGreenTeamPlayerTwo.taskOneDeadlineValueText.Text = _model.FirstTaskDeadline.ToString() + " l�p�s van h�tra";
                _gameFormGreenTeamPlayerTwo.taskTwoDeadlineValueText.Text = _model.SecondTaskDeadline.ToString() + " l�p�s van h�tra";

                if (selectedGroupCount == 2) // 2 csapat eset�n a pirosak�t is friss�teni kell
                {
                    _gameFormRedTeamPlayerOne.stepsLeftValueText.Text = _model.GameStepCount.ToString();
                    _gameFormRedTeamPlayerOne.taskOneDeadlineValueText.Text = _model.FirstTaskDeadline.ToString() + " l�p�s van h�tra";
                    _gameFormRedTeamPlayerOne.taskTwoDeadlineValueText.Text = _model.SecondTaskDeadline.ToString() + " l�p�s van h�tra";
                    _gameFormRedTeamPlayerTwo.stepsLeftValueText.Text = _model.GameStepCount.ToString();
                    _gameFormRedTeamPlayerTwo.taskOneDeadlineValueText.Text = _model.FirstTaskDeadline.ToString() + " l�p�s van h�tra";
                    _gameFormRedTeamPlayerTwo.taskTwoDeadlineValueText.Text = _model.SecondTaskDeadline.ToString() + " l�p�s van h�tra";
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
    Megn�zi melyik sz�vegdobozban t�rt�nt v�ltoz�s �s a t�bbi tartalm�t ennek megfelel�en aktualiz�lja,
    k�t csoport eset�n a csoportok k�l�n-k�l�n kommunik�ci�s fel�lettel rendelkeznek, egym�s�t nem l�tj�k
        
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

    // K�r kezdet�n az akt�v j�t�kos gombj�t enged�lyezi, a t�bbit letiltja
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

    private void ShowNextPlayerForm() // egyben, amikor megjelen�tj�k a n�zetet, friss�tj�k is
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
            string winner = "A z�ld csapat pontsz�ma: " + instance._model.GreenTeamPoints.ToString();
            if (instance._model.Teams == 2)
            {
                if (instance._model.GreenTeamPoints > instance._model.RedTeamPoints)
                {
                    winner = "A z�ld csapat nyert! \nZ�ld csapat pontsz�mja: " + instance._model.GreenTeamPoints.ToString() + "\nPiros csapat pontsz�ma: " + instance._model.RedTeamPoints.ToString() + "\n";
                }
                else if (instance._model.GreenTeamPoints < instance._model.RedTeamPoints)
                {
                    winner = "A piros csapat nyert! \nPiros csapat pontsz�mja: " + instance._model.RedTeamPoints.ToString() + "\nZ�ld csapat pontsz�ma: " + instance._model.GreenTeamPoints.ToString() + "\n";
                } else
                {
                    winner = "D�ntetlen! \nZ�ld csapat pontsz�mja: " + instance._model.GreenTeamPoints.ToString() + "\nPiros csapat pontsz�ma: " + instance._model.RedTeamPoints.ToString() + "\n";
                }
            }

            MessageBox.Show(winner, "Game over!" , MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            DisposeAllForms();
        }
    }

    #endregion
}