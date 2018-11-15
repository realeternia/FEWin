using System.Collections.Generic;

namespace FEGame.Core
{
    public class IntPair
    {
        [FieldIndex(Index = 1)]
        public int Type;
        [FieldIndex(Index = 2)]
        public int Value;

        public override string ToString()
        {
            return string.Format("{0}:{1}", Type, Value);
        }
    }

    internal class CompareByMid : IComparer<IntPair>
    {
        #region IComparer<IntPair> ��Ա

        public int Compare(IntPair x, IntPair y)
        {
            if (y.Type == 0 && x.Type == 0)
                return 0;
            if (x.Type == 0)
                return 1;
            if (y.Type == 0)
                return -1;
            if (x.Type != y.Type)
            {
                return x.Type.CompareTo(y.Type);
            }
            return y.Value.CompareTo(x.Value);
        }

        #endregion
    }

    internal class CompareBySid : IComparer<IntPair>
    {
        #region IComparer<IntPair> ��Ա

        public int Compare(IntPair x, IntPair y)
        {
            if (x.Type != y.Type)
            {
                if (x.Type == 0)
                    return 1;
                if (y.Type == 0)
                    return -1;
                return x.Type.CompareTo(y.Type);
            }
            return x.Value.CompareTo(y.Value);
        }

        #endregion
    }
}