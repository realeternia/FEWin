using System;
using System.Drawing;
using FEGame.Controller.Battle;
using FEGame.Core;

namespace FEGame.Forms.Items.Battle
{
    public class ChessMoveAnim
    {
        public Action<object> FinishAction;
        private double pathLength;
        private double moveProgress; //0-1
        private double[] pathSegmentLength;
        private Point[] path;
        private int samuraiId;

        private int movePerSecond = 600;

        public void Set(int sid, Point[] p)
        {
            samuraiId = sid;
            pathLength = 0;
            moveProgress = 0;
            path = new Point[p.Length];
            for (int i = 0; i < p.Length; i++)
            {
                path[i] = new Point(p[i].X * TileManager.CellSize, p[i].Y * TileManager.CellSize);
            }

            pathSegmentLength = new double[path.Length - 1];
            for (int i = 0; i < path.Length - 1; i++)
            {
                pathSegmentLength[i] = pathLength;
                pathLength += Distance(path[i], path[i + 1]);
            }
        }

        public bool Update()
        {
            if (pathLength > 0)
            {
                var moveDis = movePerSecond / 20; //20帧

                moveProgress = Math.Min((double) 1, moveProgress + moveDis / pathLength);
                if (moveProgress >= 1)
                {
                    FinishAction(null);
                    pathLength = 0;
                    FinishAction = null;
                }
                return true;
            }
            return false;
        }

        private Point GetNowPosOnPath()
        {
            if (pathLength == 0)
            {
                return path[0];
            }

            var disMoved = moveProgress * pathLength;
            for (int i = pathSegmentLength.Length - 1; i >= 0; i--)
            {
                if (disMoved >= pathSegmentLength[i])
                {
                    var moveOnThisSegement = disMoved - pathSegmentLength[i];
                    return MoveTowards(path[i], path[i + 1], moveOnThisSegement);
                }
            }
            return path[0];
        }

        public void Draw(Graphics g, int baseX, int baseY)
        {
            if (pathLength > 0)
            {
                var nowPos = GetNowPosOnPath();
                var img = HSIcons.GetImage("Samurai", samuraiId);
                g.DrawImage(img, nowPos.X - baseX, nowPos.Y - baseY, TileManager.CellSize, TileManager.CellSize);
            }
        }

        private double Distance(Point x, Point y)
        {
            return Math.Sqrt((x.X - y.X) * (x.X - y.X) + (x.Y - y.Y) * (x.Y - y.Y));
        }
        public static Point MoveTowards(Point current, Point target, double distance)
        {
            Point vector = new Point(target.X - current.X, target.Y - current.Y);
            var magnitude = Magnitude(vector);
            if (magnitude != 0f)
            {
                return new Point((int) (current.X + (vector.X / magnitude) * distance),
                    (int) (current.Y + (vector.Y / magnitude) * distance));
            }
            return target;
        }
        public static double Magnitude(Point current)
        {
            return Math.Sqrt((current.X * current.X) + (current.Y * current.Y));
        }
    }
}