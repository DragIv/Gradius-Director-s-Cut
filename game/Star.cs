using System.Drawing;

namespace game
{
    public class Star : MovableObject
    {
        public Star(PointF location, Size size, float speed) : base(location, size, speed)
        {
            direction_buttons[Player.Directions.Left] = true;
            collider = null;
            ChangeDirection();
            moving = true;
        }
        public override void Move()
        {
            base.Move();

            if (location.X < 0)
            {
                Form1.stars.Remove(this);
                moving = false;
            }
        }
    }
}