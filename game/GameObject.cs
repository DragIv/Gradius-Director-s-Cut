using System;
using System.Drawing;

namespace game
{
    public class GameObject
    {
        public PointF location;
        public Size size;
        public Collider collider;
        public double angle = 0;
        public Animation animation;
        public string name;

        public GameObject(PointF location, Size size, string name = "")
        {
            this.location = location;
            this.size = size;
            this.name = name;
            this.collider = new Collider(location, size, this);
            this.animation = new Animation(new TimeSpan(0, 0, 0, 0, 100), this);
        }
        virtual public void Draw()
        {
            Form1.graphics.DrawImage(animation.GetCurrentSprite(), location);
            //if (collider != null) Form1.graphics.DrawRectangle(Pens.White, new Rectangle(new Point((int)collider.parent.location.X, (int)collider.parent.location.Y), collider.parent.size));
        }
        public double ToRadians(double grads)
        {
            return grads * Math.PI / 180;
        }
        public double ToGrads(double radians)
        {
            return radians * 180 / Math.PI;
        }
        public double getAngleBetweenEntities(PointF pointF)
        {
            float X = pointF.X - location.X;
            float Y = pointF.Y - location.Y;
            double angle = Math.Atan(Y / X);
            double grad_angle = angle * 180 / Math.PI;
            if (pointF.X > location.X && pointF.Y < location.Y)
            {
                grad_angle = -grad_angle;
            }
            if (pointF.X < location.X)
            {
                grad_angle = 180 - grad_angle;
            }
            if (pointF.X > location.X && pointF.Y > location.Y)
            {
                grad_angle = 360 - grad_angle;
            }
            return grad_angle;
        }
        public double getDistance(GameObject obj)
        {
            PointF point1 = new PointF(location.X + size.Width / 2, location.Y + size.Height / 2);
            PointF point2 = new PointF(obj.location.X + obj.size.Width / 2, obj.location.Y + obj.size.Height / 2);
            return Math.Sqrt(Math.Pow(Math.Abs(point1.X - point2.X), 2) + Math.Pow(Math.Abs(point1.Y - point2.Y), 2));
        }
    }
}