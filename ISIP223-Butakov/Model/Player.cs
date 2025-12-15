using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP223_Butakov.Model
{
    public class Player
    {
        public int HP { get; set; }
        public Weapon Weapon { get; private set; }
        public Armor Armor { get; private set; }
        public int Attack => Weapon.Attack;
        public int Defense => Armor.Defense;
        public bool IsDefending { get; set; }
        public bool IsAlive => HP > 0;

        public Player()
        {
            HP = 100;
            Weapon = new Weapon(10);
            Armor = new Armor(5);
            IsDefending = false;
        }

        public void EquipWeapon(Weapon weapon)
        {
            Weapon = weapon;
        }

        public void EquipArmor(Armor armor)
        {
            Armor = armor;
        }

        public void TakeDamage(int damage)
        {
            HP -= damage;
            if (HP < 0) HP = 0;
        }
    }
}