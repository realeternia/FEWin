﻿using System.Collections.Generic;

namespace FEGame.DataType.Others
{
    public class RankData
    {
        public int Id;
        public int Mark;

        internal class CompareByMark : IComparer<RankData>
        {
            #region IComparer<RankData> 成员

            public int Compare(RankData x, RankData y)
            {
                return y.Mark - x.Mark;
            }

            #endregion
        }
    }
}
