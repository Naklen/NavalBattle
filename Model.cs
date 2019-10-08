using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavalBattle
{
    public class Model
    {
        public CellStates[,] playerField;
        public CellStates[,] enemyField;
        public GameStates GameState { get; set; }
        public int playerShipCount;
        public int smallShipsPlaced;
        public int mediumShipsPlaced;
        public int bigShipsPlaced;
        public int hugeShipsPlaced;
        public int enemyShipCount;
        public bool canShoot;

        public Model(int shipCount = 10)
        {
            playerField = new CellStates[10, 10];
            enemyField = new CellStates[10, 10];

            for (var i = 0; i < 10; i++)
                for (var j = 0; j < 10; j++)
                {
                    playerField[i, j] = CellStates.Water;
                    enemyField[i, j] = CellStates.Water;
                }
            GameState = GameStates.Preparing;
            playerShipCount = shipCount;
            enemyShipCount = shipCount;
            smallShipsPlaced = 0;
            mediumShipsPlaced = 0;
            bigShipsPlaced = 0;
            hugeShipsPlaced = 0;
            canShoot = true;
        }

        public bool TryPlaceShip(int x, int y, int size = 1, ShipDirection direction = ShipDirection.Up)
        {
            if (GetPlacedShipCount() == playerShipCount) return false;
            if (x < 0 || y < 0 || x > 9 || y > 9) return false;
            if (size == 1)
            {
                var result = CheckCellAmbit(x, y, playerField);
                if (result)
                {
                    playerField[x, y] = CellStates.SmallShip;
                    smallShipsPlaced++;
                }
                return result;
            }
            else
            {
                if (direction == ShipDirection.Right)
                {
                    var result = CheckCellAmbit(x, y, playerField) && CheckCellAmbit(x, y + size - 1, playerField);
                    if (result)
                    {
                        playerField[x, y] = CellStates.ShipBackRight;
                        playerField[x, y + size - 1] = CellStates.ShipHeadRight;
                        for (var i = y + 1; i < y + size - 1; i++)
                            playerField[x, i] = CellStates.ShipMiddleRight;
                        if (size == 2) mediumShipsPlaced++;
                        else if (size == 3) bigShipsPlaced++;
                        else hugeShipsPlaced++;
                    }
                    return result;
                }
                else
                {
                    var result = CheckCellAmbit(x, y, playerField) && CheckCellAmbit(x - size + 1, y, playerField);
                    if (result)
                    {
                        playerField[x, y] = CellStates.ShipBackUp;
                        playerField[x - size + 1, y] = CellStates.ShipHeadUp;
                        for (var i = x - 1; i > x - size + 1; i--)
                            playerField[i, y] = CellStates.ShipMiddleUp;
                        if (size == 2) mediumShipsPlaced++;
                        else if (size == 3) bigShipsPlaced++;
                        else hugeShipsPlaced++;
                    }
                    return result;
                }
            }
        }

        public static bool CheckCellAmbit(int x, int y, CellStates[,] field)
        {
            if (x < 0 || x > 9 || y < 0 || y > 9) return false;
            for (var dx = -1; dx < 2; dx++)
                for (var dy = -1; dy < 2; dy++)
                {
                    if (x + dx < 0 || y + dy < 0 || x + dx > 9 || y + dy > 9) continue;
                    if (field[x + dx, y + dy] != CellStates.Water) return false;
                }
            return true;
        }

        public bool TryShoot(int x, int y)
        {
            if (x < 0 || y < 0 || x > 9 || y > 9) return false;            
            if (enemyField[x, y] == CellStates.Empty || enemyField[x, y] == CellStates.DeadSmallShip) return false;
            if (enemyField[x, y] == CellStates.Water)
            {
                enemyField[x, y] = CellStates.Empty;
                canShoot = false;
            }
            else
            {
                enemyField[x, y] = Converter.GetDeadState(enemyField[x,y]);
                if (Ship.IsDead(enemyField, new System.Drawing.Point(x,y)))
                    enemyShipCount--;
            }
            return true;
        }

        public int GetPlacedShipCount()
        {
            return smallShipsPlaced + mediumShipsPlaced + bigShipsPlaced + hugeShipsPlaced;
        }
    }
}
