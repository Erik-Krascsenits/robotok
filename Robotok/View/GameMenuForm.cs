namespace ELTE.Robotok.View;

public partial class GameMenuForm : Form
{
    public GameMenuForm()
    {
        InitializeComponent();
    }

    private void startButton_Click(object sender, EventArgs e)
    {
        GameForm _gameForm = new GameForm(); //példányosítjuk a játék ablakát
        _gameForm.ShowDialog(); //megjelenítjük a játék ablakát
    }
}