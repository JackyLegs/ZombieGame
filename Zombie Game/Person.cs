using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zombie_Game
{
    class Person
    {
        protected int health;
        protected int speed;
        public string direction;
        public int attackLeft;
        public int attackTop;
        protected int damage;


        public virtual int getHealth()
        {
            return health;
        }
        public virtual void setHealth(int x)
        {
            health -= x;
        }
        public virtual int getSpeed()
        {
            return speed;
        }
    }
}
