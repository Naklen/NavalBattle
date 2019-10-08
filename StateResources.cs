using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace NavalBattle
{
    public static class StateResources
    {
        public static Bitmap GetResource(CellStates state)
        {
            switch (state)
            {
                case CellStates.Water:
                    return Resource1.Water;
                case CellStates.Ship:
                    return Resource1.ship1;
                case CellStates.DeadShip:
                    return Resource1.ship1_dead;
                case CellStates.Empty:
                    return Resource1.morj;
                default:
                    throw new ArgumentException();
            }
        }        
    }
}
