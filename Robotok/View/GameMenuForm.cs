namespace ELTE.Robotok.View;

public partial class GameMenuForm : Form
{
    public GameMenuForm()
    {
        InitializeComponent();
    }

    private void startButton_Click(object sender, EventArgs e)
    {
        GameForm _gameForm = new GameForm(); //p�ld�nyos�tjuk a j�t�k ablak�t
        _gameForm.ShowDialog(); //megjelen�tj�k a j�t�k ablak�t
    }
}