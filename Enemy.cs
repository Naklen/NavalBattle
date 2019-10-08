using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NavalBattle
{
    public class Enemy
    {
        private HashSet<Point> avalibleTargets;
        public bool canShootAgain;
        public ShipDirection knownDirection;

        public Enemy()
        {
            canShootAgain = true;
            avalibleTargets = new HashSet<Point>();
            for (var i = 0; i < 10; i++)
                for (var j = 0; j < 10; j++)
                    avalibleTargets.Add(new Point(i, j));
        }

        public void ArrangeShips(Model world)
        {
            ArrangeNeededShips(4, 1, world.enemyField);
            ArrangeNeededShips(3, 2, world.enemyField);
            ArrangeNeededShips(2, 3, world.enemyField);
            ArrangeNeededShips(1, 4, world.enemyField);
        }

        void ArrangeNeededShips(int size, int count, CellStates[,] field)
        {
            var rnd = new Random();
            while (count > 0)
            {
                var x = rnd.Next(0, 10);
                var y = rnd.Next(0, 10);
                var direction = (ShipDirection)rnd.Next(0, 2);
                if (TryPlaceShip(x, y, field, size, direction))
                    count--;
            }
        }

        public Point Shoot(Model world)
        {         
            var rnd = new Random();
            var targetIndex = rnd.Next(0, avalibleTargets.Count);
            var iterator = avalibleTargets.GetEnumerator();
            iterator.MoveNext();
            int x, y;            
            for(var i = 0; i < targetIndex; i++)
                iterator.MoveNext();
            x = iterator.Current.X;
            y = iterator.Current.Y;
            ProcessPoint(x, y, world);
            return new Point(x, y);
        }

        public Point ShootAround(Point previous, Model world)
        {
            var rnd = new Random();
            var result = new Point(previous.X + rnd.Next(-1, 2), previous.Y + rnd.Next(-1, 2));
            while(!avalibleTargets.Contains(result))
                result = new Point(previous.X + rnd.Next(-1, 2), previous.Y + rnd.Next(-1, 2));
            ProcessPoint(result.X, result.Y, world);
            return result;
        }

        public Point ShootToNearestEnd(Point previous, Model world)
        {
            var prevCell = world.playerField[previous.X, previous.Y];
            for (var i = -1; i < 2; i++)
            {

                if (Converter.GetDirection(prevCell) == ShipDirection.Right)
                {
                    var newCell = world.playerField[previous.X, previous.Y + i];
                    if (Ship.IsShip(newCell) && !Ship.IsMiddlePart(newCell))
                    {
                        ProcessPoint(previous.X, previous.Y + i, world);
                        return new Point(previous.X, previous.Y + i);
                    }
                }
                else
                {
                    var newCell = world.playerField[previous.X + i, previous.Y];
                    if (Ship.IsShip(newCell) && !Ship.IsMiddlePart(newCell))
                    {
                        ProcessPoint(previous.X + i, previous.Y, world);
                        return new Point(previous.X + i, previous.Y);
                    }
                }                    
            }
            throw new ArgumentException();
        }

        void ProcessPoint(int x, int y, Model world)
        {
            if (world.playerField[x, y] == CellStates.Water)
            {
                avalibleTargets.Remove(new Point(x, y));
                world.playerField[x, y] = CellStates.Empty;
                canShootAgain = false;
            }
            else
            {
                for (var dx = -1; dx <= 1; dx++)
                    for (var dy = -1; dy <= 1; dy++)
                    {
                        if (x + dx < 0 || y + dy < 0 || x + dx > 9 || y + dy > 9) continue;
                        //if (Ship.IsShip(world.playerField[x + dx, y + dy])) continue;
                        if (world.playerField[x, y] == CellStates.ShipBackRight && dx == 0 && dy == 1) continue;
                        else if (world.playerField[x, y] == CellStates.ShipMiddleRight && dx == 0 && dy != 0) continue;
                        else if (world.playerField[x, y] == CellStates.ShipHeadRight && dx == 0 && dy == -1) continue;
                        else if (world.playerField[x, y] == CellStates.ShipBackUp && dx == -1 && dy == 0) continue;
                        else if (world.playerField[x, y] == CellStates.ShipMiddleUp && dx != 0 && dy == 0) continue;
                        else if (world.playerField[x, y] == CellStates.ShipHeadUp && dx == 1 && dy == 0) continue;
                        avalibleTargets.Remove(new Point(x + dx, y + dy));
                    }
                world.playerField[x, y] = Converter.GetDeadState(world.playerField[x, y]);
                knownDirection = Converter.GetDirection(world.playerField[x, y]);
                if (Ship.IsDead(world.playerField, new Point(x, y)))
                    world.playerShipCount--;
                canShootAgain = true;
            }
        }

        bool TryPlaceShip(int x, int y, CellStates[,] field, int size = 1, ShipDirection direction = ShipDirection.Up)
        {
            if (x < 0 || y < 0 || x > 9 || y > 9) return false;
            if (size == 1)
            {
                var result = Model.CheckCellAmbit(x, y, field);
                if (result)
                {
                    field[x, y] = CellStates.SmallShip;
                }
                return result;
            }
            else
            {
                if (direction == ShipDirection.Right)
                {
                    var result = Model.CheckCellAmbit(x, y, field) && Model.CheckCellAmbit(x , y + size - 1, field);
                    if (result)
                    {
                        field[x, y] = CellStates.ShipBackRight;
                        field[x, y + size - 1] = CellStates.ShipHeadRight;
                        for (var i = y + 1; i < y + size - 1; i++)
                            field[x, i] = CellStates.ShipMiddleRight;
                    }
                    return result;
                }
                else
                {
                    var result = Model.CheckCellAmbit(x, y, field) && Model.CheckCellAmbit(x - size + 1, y, field);
                    if (result)
                    {
                        field[x, y] = CellStates.ShipBackUp;
                        field[x - size + 1, y] = CellStates.ShipHeadUp;
                        for (var i = x - 1; i > x - size + 1; i--)
                            field[i, y] = CellStates.ShipMiddleUp;
                    }
                    return result;
                }
            }
        }
    }
}
