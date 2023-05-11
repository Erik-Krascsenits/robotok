namespace ELTE.Robotok.Model
{
    /// <summary>
    /// Kockák eltárolása kiértékeléshez
    /// </summary>
    public class CubeToEvaluate
    {
        public int x;
        public int y;
        public int value;

        public CubeToEvaluate(int x, int y, int value)
        {
            this.x = x;
            this.y = y;
            this.value = value;
        }
    }
}
