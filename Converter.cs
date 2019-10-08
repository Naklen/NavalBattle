using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace NavalBattle
{
    public static class Converter
    {
        public static Bitmap GetResource(CellStates state)
        {
            switch (state)
            {
                case CellStates.Water:
                    return Resource1.Water;
                case CellStates.SmallShip:
                    return Resource1.SmallShip;
                case CellStates.DeadSmallShip:
                    return Resource1.SmallShipDead;
                case CellStates.Empty:
                    return Resource1.Miss;
                case CellStates.ShipHeadUp:
                    return Resource1.ship_head;
                case CellStates.ShipHeadUpDead:
                    return Resource1.ship_head_dead;
                case CellStates.ShipHeadRight:
                    return Resource1.ship_head_r;
                case CellStates.ShipHeadRightDead:
                    return Resource1.ship_head_dead_r;
                case CellStates.ShipMiddleUp:
                    return Resource1.ship_middle;
                case CellStates.ShipMiddleUpDead:
                    return Resource1.ship_middle_dead;
                case CellStates.ShipMiddleRight:
                    return Resource1.ship_middle_r;
                case CellStates.ShipMiddleRightDead:
                    return Resource1.ship_middle_dead_r;
                case CellStates.ShipBackUp:
                    return Resource1.ship_back;
                case CellStates.ShipBackUpDead:
                    return Resource1.ship_back_dead;
                case CellStates.ShipBackRight:
                    return Resource1.ship_back_r;
                case CellStates.ShipBackRightDead:
                    return Resource1.ship_back_dead_r;
                default:
                    throw new ArgumentException();
            }
        }
        
        public static CellStates GetDeadState(CellStates state)
        {
            switch (state)
            {
                case CellStates.SmallShip:
                    return CellStates.DeadSmallShip;
                case CellStates.ShipBackUp:
                    return CellStates.ShipBackUpDead;
                case CellStates.ShipBackRight:
                    return CellStates.ShipBackRightDead;
                case CellStates.ShipHeadUp:
                    return CellStates.ShipHeadUpDead;
                case CellStates.ShipHeadRight:
                    return CellStates.ShipHeadRightDead;
                case CellStates.ShipMiddleUp:
                    return CellStates.ShipMiddleUpDead;
                case CellStates.ShipMiddleRight:
                    return CellStates.ShipMiddleRightDead;                    
                default:
                    throw new ArgumentException();
            }
        }

        public static ShipDirection GetDirection(CellStates cell)
        {
            if (cell == CellStates.Water || cell == CellStates.Empty)
                throw new ArgumentException();
            if (cell == CellStates.SmallShip || cell == CellStates.DeadSmallShip)
                return ShipDirection.Up;
            if (cell == CellStates.ShipBackRight || cell == CellStates.ShipBackRightDead ||
                cell == CellStates.ShipHeadRight || cell == CellStates.ShipHeadRightDead ||
                cell == CellStates.ShipMiddleRight || cell == CellStates.ShipMiddleRightDead)
                return ShipDirection.Right;
            else
                return ShipDirection.Up;
        }
    }
}
