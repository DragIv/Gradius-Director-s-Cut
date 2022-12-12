using System.Drawing;

namespace game
{
    public class PowerUp : MovableObject
    {
        public PowerUp(PointF location, Size size, float speed) : base(location, size, speed)
        {
            direction_buttons[Player.Directions.Left] = true;
            ChangeDirection();
            moving = true;
        }
        public override void Move()
        {
            base.Move();

            if (location.X < 0)
            {
                Form1.gameObjects.Remove(this);
                moving = false;
            }
        }
    }
}