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
        selectedDifficulty = 2; // alapértelmezetten közepes nehézséget állítunk be
    }

    #endregion

    #region Button events

    private void startButton_Click(object sender, EventArgs e)
    {
        GameForm _gameForm = new GameForm(selectedDifficulty); //példányosítjuk a játék ablakát
        _gameForm.ShowDialog(); //megjelenítjük a játék ablakát
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