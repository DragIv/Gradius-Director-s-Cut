using System.Drawing;

namespace game
{
    public class Shield : GameObject
    {
        Player parent;
        public int HP = 2;
        public Shield(PointF location, Size size, Player parent, int HP, string name = "Blue") : base(location, size, name)
        {
            this.parent = parent;
            this.HP = HP;
        }
        public void UpdateLocation()
        {
            this.location = new PointF(parent.location.X + parent.size.Width, parent.location.Y);
        }
        public void TakeDamage()
        {
            HP--;
            if (HP == 1)
            {
                parent.shield = new Shield(location, size, parent, 1, "Red");
            }
            if (HP <= 0)
            {
                parent.shield = null;
            }
        }
    }
}