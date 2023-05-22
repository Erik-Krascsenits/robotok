namespace ELTE.Robotok.Model
{
    /// <summary>
    /// Kockák eltárolása kiértékeléshez
    /// </summary>
    public class CubeToEvaluate
    {
        public Int32 x;
        public Int32 y;
        public Int32 value;

        public CubeToEvaluate(Int32 x, Int32 y, Int32 value)
        {
            this.x = x;
            this.y = y;
            this.value = value;
        }
    }
}
