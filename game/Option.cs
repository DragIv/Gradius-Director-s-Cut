using System.Drawing;

namespace game
{
    public class Option : MovableObject
    {
        GameObject parent;
        public Option(PointF location, Size size, float speed, GameObject parent) : base(location, size, speed)
        {
            this.location = location;
            this.size = size;
            this.speed = speed;
            this.parent = parent;
            moving = true;
        }
        public override void Move()
        {
            angle = getAngleBetweenEntities(parent.location);
            UpdateVectorSpeed();
            if (getDistance(parent) > 70)
            {
                base.Move();
            }
        }
    }
}