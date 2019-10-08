using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavalBattle
{
    public class Ship
    {
        public static bool IsDead(CellStates[,] field, Point point)
        {
            var cell = field[point.X, point.Y];
            if (cell == CellStates.DeadSmallShip) return true;
            if (cell == CellStates.SmallShip) return false;
            if (cell == CellStates.Water || cell == CellStates.Empty)
                throw new ArgumentException();
            var direction = Converter.GetDirection(cell);
            if (direction == ShipDirection.Right)
            {
                while (field[point.X, point.Y] != CellStates.ShipBackRight && field[point.X, point.Y] != CellStates.ShipBackRightDead)
                    point = new Point(point.X, point.Y - 1);
                while (field[point.X, point.Y] != CellStates.ShipHeadRight && field[point.X, point.Y] != CellStates.ShipHeadRightDead)
                {
                    cell = field[point.X, point.Y];
                    if (cell != CellStates.ShipBackRightDead && cell != CellStates.ShipHeadRightDead && cell != CellStates.ShipMiddleRightDead)
                        return false;
                    point = new Point(point.X, point.Y + 1);                    
                }
                if (field[point.X, point.Y] != CellStates.ShipHeadRightDead) return false;
                return true;
            }
            else
            {
                while (field[point.X, point.Y] != CellStates.ShipBackUp && field[point.X, point.Y] != CellStates.ShipBackUpDead)
                    point = new Point(point.X + 1, point.Y);
                while (field[point.X, point.Y] != CellStates.ShipHeadUp && field[point.X, point.Y] != CellStates.ShipHeadUpDead)
                {
                    cell = field[point.X, point.Y];
                    if (cell != CellStates.ShipBackUpDead && cell != CellStates.ShipHeadUpDead && cell != CellStates.ShipMiddleUpDead)
                        return false;
                    point = new Point(point.X - 1, point.Y);
                }
                if (field[point.X, point.Y] != CellStates.ShipHeadUpDead) return false;
                return true;
            }
        }

        public static bool IsShip(CellStates cell)
        {
            return !(cell == CellStates.Water || cell == CellStates.Empty);
        }

        public static bool IsMiddlePart(CellStates cell)
        {
            return cell == CellStates.ShipMiddleRight || cell == CellStates.ShipMiddleRightDead ||
                cell == CellStates.ShipMiddleUp || cell == CellStates.ShipMiddleUpDead;
        }
    }
}
