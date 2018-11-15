﻿using FEGame.Core;

namespace FEGame.Datas.User.Db
{
    public class DbGismoState
    {
        [FieldIndex(Index = 1)] public int BaseId;
        [FieldIndex(Index = 2)] public string ResultName;

        public DbGismoState()
        {
        }

        public DbGismoState(int baseId, string result)
        {
            BaseId = baseId;
            ResultName = result;
        }
    }
}
