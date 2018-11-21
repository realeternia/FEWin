namespace FEGame.Controller.Battle.Units
{
    public class BaseUnit
    {
        public int CId { get; private set; }
        public byte X { get; set; }
        public byte Y { get; set; }

        public BaseUnit(int id, byte x, byte y)
        {
            CId = id;
            X = x;
            Y = y;
        }
    }
}