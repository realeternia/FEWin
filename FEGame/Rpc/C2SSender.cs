﻿using JLM.NetSocket;

namespace FEGame.Rpc
{
    public class C2SSender
    {
        private NetClient client;
        public C2SSender(NetClient client)
        {
            this.client = client;
        }

        public void Login(string name)
        {
            client.Send(new PacketC2SLogin(name).Data);
        }

        public void Save(string name, byte[] dats)
        {
            var data = new PacketC2SSave(name, dats).Data;
            client.Send(data);
        }
        public void UpdateLevelExp(int job, int level, int exp)
        {
            var data = new PacketC2SLevelExpChange(job, level, exp).Data;
            client.Send(data);
        }
        public void GetRank()
        {
            var data = new PacketC2SGetRank(0).Data;
            client.Send(data);
        }
        public void UpdatePlayerInfo(string name, int headId)
        {
            var data = new PacketC2SSendPlayerInfo(name, headId).Data;
            client.Send(data);
        }
        public void SendHeartbeat()
        {
            var data = new PacketC2SSendHeartbeat().Data;
            client.Send(data);
        }
    }
}