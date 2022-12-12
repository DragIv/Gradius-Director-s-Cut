using System;
using System.Collections.Generic;
using System.Drawing;

namespace game
{
    public class MovableObject : GameObject
    {
        public enum Directions { Up, Down, Right, Left };
        public bool moving = false;
        public Dictionary<Directions, bool> direction_buttons = new Dictionary<Directions, bool>();
        public float speed;
        public PointF vector_speed = new PointF(0, 0);
        public MovableObject(PointF location, Size size, float speed, string name = "") : base(location, size, name)
        {
            this.speed = speed;
            direction_buttons.Add(Directions.Up, false);
            direction_buttons.Add(Directions.Down, false);
            direction_buttons.Add(Directions.Right, false);
            direction_buttons.Add(Directions.Left, false);
        }
        virtual public void Move()
        {
            if (moving)
            {
                PointF loc = new PointF(location.X + vector_speed.X, location.Y - vector_speed.Y);

                if (!(this is Player) || new RectangleF(new PointF(loc.X, loc.Y), size).IntersectsWith(new Rectangle(Form1.field.Location.X + size.Width, Form1.field.Location.Y + size.Height, Form1.field.Size.Width - 2 * size.Width, Form1.field.Size.Height - 2 * size.Height)))
                {
                    location = loc;
                }
            }
        }
        public void ChangeDirection()
        {
            if (direction_buttons[Directions.Up] || direction_buttons[Directions.Down] || direction_buttons[Directions.Left] || direction_buttons[Directions.Right])
            {
                moving = true;
            }
            else
            {
                moving = false;
                animation.changeState("Forward");
                return;
            }
            if (direction_buttons[Directions.Up])
            {
                angle = 90;
                animation.changeState("Upwards");

            }
            if (direction_buttons[Directions.Down])
            {
                angle = 270;
                animation.changeState("Downwards");
            }
            if (direction_buttons[Directions.Left])
            {
                angle = 180;
                animation.changeState("Forward");
            }
            if (direction_buttons[Directions.Right])
            {
                angle = 0;
                animation.changeState("Forward");
            }
            if (direction_buttons[Directions.Up] && direction_buttons[Directions.Right])
            {
                angle = 45;
                animation.changeState("Upwards");
            }
            if (direction_buttons[Directions.Right] && direction_buttons[Directions.Down])
            {
                angle = 315;
                animation.changeState("Downwards");
            }
            if (direction_buttons[Directions.Down] && direction_buttons[Directions.Left])
            {
                angle = 225;
                animation.changeState("Downwards");
            }
            if (direction_buttons[Directions.Left] && direction_buttons[Directions.Up])
            {
                angle = 135;
                animation.changeState("Upwards");
            }
            UpdateVectorSpeed();
        }
        public void UpdateVectorSpeed()
        {
            vector_speed = new PointF((float)Math.Cos(ToRadians(angle)) * speed, (float)Math.Sin(ToRadians(angle)) * speed);
        }
    }
}