namespace ELTE.Robotok.Model
{
    /// <summary>
    /// Kockák eltárolása az összetettebb műveletekhez
    /// </summary>
    public class Cube : CubeToEvaluate
    {
        public Boolean northAttachment;
        public Boolean southAttachment;
        public Boolean eastAttachment;
        public Boolean westAttachment;
        public string direction;
        public Int32 remainingCleaningOperations;
        public Cube(Int32 x, Int32 y, Int32 value, Boolean north, Boolean south, Boolean east, Boolean west, string direction, Int32 remainingCleaningOperations) : base(x, y, value)
        {
            northAttachment = north;
            southAttachment = south;
            eastAttachment = east;
            westAttachment = west;
            this.direction = direction;
            this.remainingCleaningOperations = remainingCleaningOperations;
        }
    }
}
