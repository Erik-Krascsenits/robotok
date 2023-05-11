namespace ELTE.Robotok.Model
{
    /// <summary>
    /// Kockák eltárolása az összetettebb műveletekhez
    /// </summary>
    public class Cube : CubeToEvaluate
    {
        public bool northAttachment;
        public bool southAttachment;
        public bool eastAttachment;
        public bool westAttachment;
        public string direction;
        public int remainingCleaningOperations;
        public Cube(int x, int y, int value, bool n, bool s, bool e, bool w, string direction, int remainingCleaningOperations) : base(x, y, value)
        {
            northAttachment = n;
            southAttachment = s;
            eastAttachment = e;
            westAttachment = w;
            this.direction = direction;
            this.remainingCleaningOperations = remainingCleaningOperations;
        }
    }
}
