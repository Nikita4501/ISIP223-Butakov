using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP223_Butakov.Model
{
    public class Weapon
    {
        public int Attack { get; }

        public Weapon(int attack)
        {
            Attack = attack;
        }
    }
}