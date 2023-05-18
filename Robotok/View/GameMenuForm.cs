using ELTE.Robotok.Model;
using Robotok.WinForms.View;

namespace ELTE.Robotok.View;

public partial class GameMenuForm : Form
{
    #region Private fields

    private Int32 selectedDifficulty; // j�t�k neh�zs�ge 1 - k�nny�, 2 - k�zepes, 3 - neh�z
    private Int32 selectedGroupCount; // csapatok sz�ma 1 - (1 csoport, 2 j�t�kos), 2 - (2 csoport, 2-2 j�t�kos)
    private System.Windows.Forms.Timer _timer = null!; // id�z�t� a visszasz�ml�l�shoz
    private Int32 actualPlayer; //  a jelenlegi j�t�kos azonos�t�ja 1 - z�ld csapat 1. j�t�kos, 8 - z�ld csapat 2. j�t�kos, 2 - piros csapat 1. j�t�kos, 9 - piros csapat 2. j�t�kos
    private GameForm _gameFormGreenTeamPlayerOne = null!;  // formok a k�l�nb�z� n�zetek megjelen�t�s�hez (a piros csapat n�zetei �s a j�t�kvezet�i n�zet csak opcion�lisan van p�ld�nyos�tva) 
    private GameForm _gameFormGreenTeamPlayerTwo = null!;
    private GameForm _gameFormRedTeamPlayerOne = null!;
    private GameForm _gameFormRedTeamPlayerTwo = null!;
    private RefereeModeForm _refereeModeForm = null!;

    #endregion

    #region Public fields

    public static GameMenuForm instance = null!;  // a f�men� p�ld�nya, a j�t�kosn�zeteken ennek seg�ts�g�vel tudjuk el�rni a modell r�teget
    public RobotokGameModel _model = null!; // j�t�kmodell

    #endregion

    #region Constructor

    /// <summary>
    /// Men�ablak p�ld�nyos�t�sa
    /// </summary>
    public GameMenuForm()
    {
        instance = this; // felvessz�k a jelenlegi formot instancenek, hogy a k�zvetlen �s k�zvetett elemei el�rhet�ek legyenek a n�zetekb�l
        selectedDifficulty = 2; // alap�rtelmezetten k�zepes neh�zs�get �ll�tunk be
        selectedGroupCount = 1; // alap�rtelmezetten egy csoportos j�t�kos �ll�tunk be
        actualPlayer = 1; // be�ll�tjuk a z�ld csapat 1. j�t�kos�t kezd� j�t�kosnak
        InitializeComponent();
    }

    #endregion

    #region Menu button events

    /// <summary>
    /// J�t�k elind�t�sa
    /// </summary>
    private void StartButton_Click(object sender, EventArgs e)
    {
        _model = new RobotokGameModel(selectedDifficulty, selectedGroupCount); // p�ld�nyos�tjuk a model-t
        _gameFormGreenTeamPlayerOne = new GameForm(selectedDifficulty, selectedGroupCount, 1); // p�ld�nyos�tjuk �s megjelen�tj�k a z�ld csapat j�t�kosainak ablakait �s jelezz�k, hogy melyik ablak melyik j�t�kos�
        _gameFormGreenTeamPlayerOne.Text = "Robotok - Z�ld csapat - 1. j�t�kos n�zet";
        _gameFormGreenTeamPlayerOne.playerViewText.Text = "Z�ld csapat - 1. j�t�kos n�zet:";
        _gameFormGreenTeamPlayerOne.Show();
        _gameFormGreenTeamPlayerTwo = new GameForm(selectedDifficulty, selectedGroupCount, 8);
        _gameFormGreenTeamPlayerTwo.Text = "Robotok - Z�ld csapat - 2. j�t�kos n�zet";
        _gameFormGreenTeamPlayerTwo.playerViewText.Text = "Z�ld csapat - 2. j�t�kos n�zet:";
        _gameFormGreenTeamPlayerTwo.Show();

        if (selectedGroupCount == 2) // ha 2 csoport lehet�s�g ker�lt kiv�laszt�sra, akkor p�ld�nyos�tjuk a piros csapat j�t�kosainak ablakait is
        {
            _gameFormRedTeamPlayerOne = new GameForm(selectedDifficulty, selectedGroupCount, 2);
            _gameFormRedTeamPlayerOne.Text = "Robotok - Piros csapat - 1. j�t�kos n�zet";
            _gameFormRedTeamPlayerOne.playerViewText.Text = "Piros csapat - 1. j�t�kos n�zet:";
            _gameFormRedTeamPlayerOne.Show();
            _gameFormRedTeamPlayerTwo = new GameForm(selectedDifficulty, selectedGroupCount, 9);
            _gameFormRedTeamPlayerTwo.Text = "Robotok - Piros csapat - 2. j�t�kos n�zet";
            _gameFormRedTeamPlayerTwo.playerViewText.Text = "Piros csapat - 2. j�t�kos n�zet:";
            _gameFormRedTeamPlayerTwo.Show();
        }

        DisablePlayerButtons(); // letiltjuk a j�t�kosok m�veleti gombjait az egyes j�t�kos kiv�tel�vel

        if (refereeModeCheckbox.Checked)   // p�ld�nyos�tjuk a j�t�kvezet�i m�dot att�l f�gg�en, hogy kiv�laszt�sra ker�lt-e
        {
            _refereeModeForm = new RefereeModeForm();
            _refereeModeForm.Show();
        }

        _gameFormGreenTeamPlayerOne.communicationWindow.TextChanged += new EventHandler(Communication_TextChanged); // l�trehozunk esem�nykezel�ket, amelyek a j�t�kosn�zetek kommunik�ci�s ablak�n t�rt�n� v�ltoz�sokat fogj�k figyelni
        _gameFormGreenTeamPlayerTwo.communicationWindow.TextChanged += new EventHandler(Communication_TextChanged);

        if (selectedGroupCount == 2)
        {
            _gameFormRedTeamPlayerOne.communicationWindow.TextChanged += new EventHandler(Communication_TextChanged);
            _gameFormRedTeamPlayerTwo.communicationWindow.TextChanged += new EventHandler(Communication_TextChanged);
        }

        _gameFormGreenTeamPlayerOne.BringToFront(); // els� l�p�sk�nt a z�ld csapat 1. j�t�kos�nak ablak�t helyezz�k el�t�rbe

        _timer = new System.Windows.Forms.Timer();  // id�z�t� l�trehoz�sa �s elind�t�sa (a f�men�b�l t�rt�nik a Tick esem�ny lekezel�se, a megv�ltozott �llapotokat a n�zetek let�kr�zik)
        _timer.Interval = 1000;
        _timer.Tick += new EventHandler(Timer_Tick);
        _timer.Start();

        startButton.Enabled = false;   // letiltjuk a f�men� gombjait, am�g a j�t�k tart
        difficultyChoice.Enabled = false;
        groupChoice.Enabled = false;
        refereeModeCheckbox.Enabled = false;       
    }

    /// <summary>
    /// F�men� bez�r�sa
    /// </summary>
    private void GameMenuForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (MessageBox.Show("A f�men� bez�r�s�val az �sszes j�t�kablak bez�r�dik. Folytatja?", "Figyelmeztet�s", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
        {
            e.Cancel = true;
        }
    }

    #endregion

    #region Timer event handler

    /// <summary>
    /// Id� m�l�s�nak esem�nykezel�je
    /// </summary>
    private void Timer_Tick(Object? sender, EventArgs e)
    {
        GameOver(); // j�t�k v�g�nek ellen�rz�se �s kezel�se
        UpdateActivePlayerButtonStatuses(); 
        _model.AdvanceTime(actualPlayer); // j�t�k l�ptet�se

        _gameFormGreenTeamPlayerOne.remainingSecondsValueText.Text = _model.RemainingSeconds.ToString() + " m�sodperc"; // friss�tj�k a h�tralev� m�sodpercek sz�m�nak kijelz�s�t
        _gameFormGreenTeamPlayerTwo.remainingSecondsValueText.Text = _model.RemainingSeconds.ToString() + " m�sodperc";

       
        if (selectedGroupCount == 2)  // amennyiben 2 csapat j�tszik a t�bbi j�t�kos Formj�nak �rt�keit is kell fris�teni
        {
            _gameFormRedTeamPlayerOne.remainingSecondsValueText.Text = _model.RemainingSeconds.ToString() + " m�sodperc";
            _gameFormRedTeamPlayerTwo.remainingSecondsValueText.Text = _model.RemainingSeconds.ToString() + " m�sodperc";
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
    /// Neh�z neh�zs�g� j�t�km�d kiv�laszt�sa
    /// </summary>
    private void HardDifficultyOption_CheckedChanged(object sender, EventArgs e)
    {
        selectedDifficulty = 3;
    }

    /// <summary>
    /// K�zepes neh�zs�g� j�t�km�d kiv�laszt�sa
    /// </summary>
    private void MediumDifficultyOption_CheckedChanged(object sender, EventArgs e)
    {
        selectedDifficulty = 2;
    }

    /// <summary>
    /// K�nny� neh�zs�g� j�t�km�d kiv�laszt�sa
    /// </summary>
    private void EasyDifficultyOption_CheckedChanged(object sender, EventArgs e)
    {
        selectedDifficulty = 1;
    }

    /// <summary>
    /// Egy csapat v�laszt�sa
    /// </summary>
    private void OneGroupOption_CheckedChanged(object sender, EventArgs e)
    {
        selectedGroupCount = 1;
    }

    /// <summary>
    /// K�t csapat v�laszt�sa
    /// </summary>
    private void TwoGroupsOption_CheckedChanged(object sender, EventArgs e)
    {
        selectedGroupCount = 2;
    }

    #endregion

    #region Communication window event handler

    /// <summary>
    /// Kommunik�ci�s ablakok tartalm�nak aktualiz�l�sa azonos csapaton bel�l
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
    /// Akt�v j�t�kos m�veleti gombjainak enged�lyez�se
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
    /// Az els� j�t�kos kiv�tel�vel az �sszes j�t�kos m�veleti gombjainak letilt�sa
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
    /// J�t�kos ablakok v�lt�sa
    /// </summary>
    private void ShowNextPlayerForm() // egyben, amikor megjelen�tj�k a n�zetet, friss�tj�k is
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
    /// J�t�k be�ll�t�sainak enged�lyez�se
    /// </summary>
    private void EnableButtons()
    {
        startButton.Enabled = true;
        difficultyChoice.Enabled = true;
        groupChoice.Enabled = true;
        refereeModeCheckbox.Enabled = true;
    }

    /// <summary>
    /// J�t�k v�g�nek kezel�se
    /// </summary>
    private void GameOver()
    {
        if (_model.IsGameOver)
        {
            _timer.Enabled = false;
            String winner = "A z�ld csapat pontsz�ma: " + instance._model.GreenTeamPoints.ToString();
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

    #region Public methods

    /// <summary>
    /// J�t�kosok ablakainak bez�r�sa
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