using System;
using NarlonLib.Tools;

namespace FEGame.Core
{
    public class ActionTimely
    {
        public delegate void ActionStep();
        
        public static ActionTimely Register(ActionStep a, double intv)
        {
            ActionTimely action = new ActionTimely();
            action.step = a;
            action.lastActTime = TimeTool.DateTimeToUnixTimeDouble(DateTime.Now);
            action.interval = intv;

            return action;
        }

        private ActionStep step;
        private double lastActTime;
        private double interval;

        private bool isDirty;

        public void Fire()
        {
            isDirty = true;
        }

        public void Update()
        {
            if (isDirty)
            {
                var ntl = TimeTool.DateTimeToUnixTimeDouble(DateTime.Now);
                if (ntl >= lastActTime + interval)
                {
                    step();
                    lastActTime = ntl;
                    isDirty = false;
                }
            }
        }
    }
}