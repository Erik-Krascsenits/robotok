namespace ELTE.Robotok.View;

public partial class GameMenuForm : Form
{
    #region Fields

    Int32 selectedDifficulty;

    #endregion

    #region Constructor

    public GameMenuForm()
    {
        InitializeComponent();
        selectedDifficulty = 2; // alap�rtelmezetten k�zepes neh�zs�get �ll�tunk be
    }

    #endregion

    #region Button events

    private void startButton_Click(object sender, EventArgs e)
    {
        GameForm _gameForm = new GameForm(selectedDifficulty); //p�ld�nyos�tjuk a j�t�k ablak�t
        _gameForm.ShowDialog(); //megjelen�tj�k a j�t�k ablak�t
    }

    private void easyDifficultyButton_Click(object sender, EventArgs e)
    {
        selectedDifficulty = 1;
        easyDifficultyButton.Enabled = false;
        mediumDifficultyButton.Enabled = true;
        hardDifficultyButton.Enabled = true;
    }

    private void mediumDifficultyButton_Click(object sender, EventArgs e)
    {
        selectedDifficulty = 2;
        easyDifficultyButton.Enabled = true;
        mediumDifficultyButton.Enabled = false;
        hardDifficultyButton.Enabled = true;
    }

    private void hardDifficultyButton_Click(object sender, EventArgs e)
    {
        selectedDifficulty = 3;
        easyDifficultyButton.Enabled = true;
        mediumDifficultyButton.Enabled = true;
        hardDifficultyButton.Enabled = false;
    }

    #endregion
}