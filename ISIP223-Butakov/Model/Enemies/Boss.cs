using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP223_Butakov.Model.Enemies
{
    public abstract class Boss : Enemy
    {
        // Базовые характеристики для расчета множителей
        protected const int BASE_HP = 30;
        protected const int BASE_ATTACK = 12;
        protected const int BASE_DEFENSE = 3;
    }
}