using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace NavalBattle
{
    public partial class MainForm : Form
    {
        TableLayoutPanel PlayerField { get; set; }
        TableLayoutPanel EnemyField { get; set; }
        Button startButton = new Button()
        {
            FlatStyle = FlatStyle.Flat,
            Size = new Size(300, 100),
            BackColor = Color.Red,
            Location = new Point(Resource1.Background.Size.Width / 2 - 150, 700),
            Text = "Start",
            Font = new Font("Century Gothic", 24, FontStyle.Bold, GraphicsUnit.Point, 204)
    };
        Button endTurn = new Button()
        {
            FlatStyle = FlatStyle.Flat,
            Size = new Size(150, 100),
            Location = new Point(800, 700),
            BackColor = Color.Gray,
            Text = "Сделайте ход",
            Font = new Font("Century Gothic", 16, FontStyle.Bold, GraphicsUnit.Point, 204),
            Enabled = false,
            Visible = false
        };

        Tuple<Button, TextBox, int, int>[] selectionShipsElements = new Tuple<Button, TextBox, int, int>[4];
        Tuple<Button, TextBox, int, int> selectedShip;

        ShipDirection direction;
        Model world;
        Enemy enemy;
        //Turns turn;

        public MainForm()
        {
            InitializeComponent();
            Text = "Naval Battle";
            world = new Model();
            enemy = new Enemy();
            BackgroundImage = Resource1.Background;
            Size = Resource1.Background.Size;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            PlayerField = new TableLayoutPanel();
            EnemyField = new TableLayoutPanel();            
            PlayerField.Location = new Point(150, 150);
            EnemyField.Location = new Point(800, 150);
            startButton.FlatAppearance.BorderSize = 0;
            startButton.Click += (sender, args) =>
            {
                if (world.GameState == GameStates.Preparing)
                {
                    if (world.GetPlacedShipCount() != world.playerShipCount)
                        MessageBox.Show("Расставьте оставшиеся корабли");
                    else
                    {
                        world.GameState = GameStates.Game;
                        enemy.ArrangeShips(world);
                        if (new Random().Next(0, 2) > 0)
                        {
                            MessageBox.Show("Противник ходит первым");
                            EnemyTurn();                            
                        }
                        else
                            MessageBox.Show("Вы ходите первым");
                        startButton.Visible = false;
                        panel1.Visible = false;
                        label5.Visible = false;
                        directionButton.Visible = false;
                        endTurn.Visible = true;
                        listBox1.Visible = true;
                    }
                }
            };
            endTurn.FlatAppearance.BorderSize = 0;
            endTurn.Click += (sender, args) =>
            {
                enemy.canShootAgain = true;
                EnemyTurn();                
                Thread.Sleep(750);
                endTurn.BackColor = Color.Gray;
                endTurn.Text = "Сделайте ход";
                endTurn.Enabled = false;
            };
            direction = ShipDirection.Up;

            selectionShipsElements[0] = new Tuple<Button, TextBox, int, int>(button1, textBox1, 4, 1);
            selectionShipsElements[1] = new Tuple<Button, TextBox, int, int>(button2, textBox2, 3, 2);
            selectionShipsElements[2] = new Tuple<Button, TextBox, int, int>(button3, textBox3, 2, 3);
            selectionShipsElements[3] = new Tuple<Button, TextBox, int, int>(button4, textBox4, 1, 4);
            foreach (var el in selectionShipsElements)
                el.Item2.Text = el.Item3.ToString();

            //listBox1.SelectedIndex = listBox1.Items.Count - 1;
           // listBox1.SelectedIndex = -1;

            BuildPlayerField(PlayerField);
            BuildEnemyField(EnemyField);

            Controls.Add(startButton);
            Controls.Add(endTurn);
            Controls.Add(PlayerField);
            Controls.Add(EnemyField);           
        }

        void BuildPlayerField(TableLayoutPanel field)
        {
            Action<Button> onClick = (b) => 
            {                
                if (world.GameState == GameStates.Preparing)
                {
                    if (selectedShip == null)
                        MessageBox.Show("Выберите корабль");
                    else if (selectedShip.Item3 == 0)
                        MessageBox.Show("Все корабли этого типа уже расставлены");
                    else
                    {
                        var shipPlaced = world.TryPlaceShip(field.GetCellPosition(b).Row, field.GetCellPosition(b).Column, selectedShip.Item4, direction);
                        if (!shipPlaced)
                            if (world.GetPlacedShipCount() == world.playerShipCount)
                                MessageBox.Show("Все корабли расставлены");
                            else
                                MessageBox.Show("Здесь нельзя разместить корабль");
                        else
                        {
                            if (selectedShip.Item4 == 1)
                                b.Image = Converter.GetResource(world.playerField[field.GetCellPosition(b).Row, field.GetCellPosition(b).Column]);
                            else
                            {
                                var x = field.GetCellPosition(b).Row;
                                var y = field.GetCellPosition(b).Column;
                                b.Image = Converter.GetResource(world.playerField[x, y]);
                                for (var i = 1; i < selectedShip.Item4; i++)
                                {
                                    if (direction == ShipDirection.Right)
                                    {
                                        var nextButton = PlayerField.Controls[(y + i) * 10 + x] as Button;
                                        nextButton.Image = Converter.GetResource(world.playerField[x, y + i]);
                                    }
                                    else
                                    {
                                        var nextButton = PlayerField.Controls[y * 10 + x - i] as Button;
                                        nextButton.Image = Converter.GetResource(world.playerField[x - i, y]);
                                    }
                                }
                            }
                            UpdateSelectedTuple(selectedShip, selectedShip.Item3 - 1);
                            selectedShip.Item2.Text = selectedShip.Item3.ToString();
                        }
                    }
                }               
                //UpdateCell(field.GetCellPosition(b).Row, field.GetCellPosition(b).Column, field, world.playerField);
            };            
            FieldConstructor.BuildField(field, onClick);
        }

        void BuildEnemyField(TableLayoutPanel field) 
        {
            Action<Button> onClick = (b) =>
            {
                if (world.GameState == GameStates.Game)
                {
                    if (!world.canShoot)
                    {
                        MessageBox.Show("Больше нельзя стрелять");
                        return;
                    }
                    var shotAttempt = world.TryShoot(field.GetCellPosition(b).Row, field.GetCellPosition(b).Column);
                    if (!shotAttempt)
                    {
                        MessageBox.Show("Сюда нельзя стрелять");
                        return;
                    }
                    var cell = world.enemyField[field.GetCellPosition(b).Row, field.GetCellPosition(b).Column];
                    b.Image = Converter.GetResource(cell);
                    listBox1.ForeColor = Color.Black;
                    listBox1.Items.Add($"Вы стреляете по позиции X: {field.GetCellPosition(b).Row} Y: {field.GetCellPosition(b).Column}");
                    if (Ship.IsShip(cell))
                    {
                        listBox1.Items.Add("Вы подбили корабль!");
                        if (Ship.IsDead(world.enemyField, new Point(field.GetCellPosition(b).Row, field.GetCellPosition(b).Column)))
                            listBox1.Items.Add("Вы потопили корабль!");
                    }
                    else
                        listBox1.Items.Add("Промах");
                    UpdateListBox();
                    if (world.canShoot)
                    {
                        endTurn.BackColor = Color.Orange;
                        endTurn.Text = "Передать ход";
                    }
                    else
                    {
                        endTurn.BackColor = Color.GreenYellow;
                        endTurn.Text = "Ход завершен";
                    }
                    endTurn.Enabled = true;
                    if (world.enemyShipCount == 0)
                    {
                        MessageBox.Show("ПОБЕДА!!!");
                        listBox1.Items.Add("Победа!");
                        return;
                    }
                }
                
            };
            FieldConstructor.BuildField(field, onClick);
        }

        void EnemyTurn()
        {
            var isFirstAttempt = true;
            Point target = Point.Empty;            
            while (enemy.canShootAgain)
            {
                if (isFirstAttempt || Ship.IsDead(world.playerField, target))
                {
                    target = enemy.Shoot(world);
                    LogEnemyShot(target);
                    if (Ship.IsMiddlePart(world.playerField[target.X, target.Y]))
                    {
                        var nearestEnd = enemy.ShootToNearestEnd(target, world);
                        var nearestEndButton = PlayerField.Controls[nearestEnd.Y * 10 + nearestEnd.X] as Button;
                        nearestEndButton.Image = Converter.GetResource(world.playerField[nearestEnd.X, nearestEnd.Y]);
                        LogEnemyShot(nearestEnd);
                    }
                    isFirstAttempt = false;
                }
                else
                {
                    target = enemy.ShootAround(target, world);
                    LogEnemyShot(target);
                }
                var playerButton = PlayerField.Controls[target.Y * 10 + target.X] as Button;
                playerButton.Image = Converter.GetResource(world.playerField[target.X, target.Y]);
            }
            world.canShoot = true;
            if (world.playerShipCount == 0)
            {
                MessageBox.Show("Поражение!");
                listBox1.Items.Add("Поражение!");
            }
            UpdateListBox();
        }

        void LogEnemyShot(Point target)
        {
            listBox1.Items.Add($"Противник стреляет в точку Х: {target.X} Y: {target.Y}");
            if (Ship.IsShip(world.playerField[target.X, target.Y]))
            {
                listBox1.Items.Add("Противник подбил ваш корабль!");
                if (Ship.IsDead(world.playerField, target))
                    listBox1.Items.Add("Противник потопил ваш корабль!");
            }
            else
                listBox1.Items.Add("Промах");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void MenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void NewGameButton_Click(object sender, EventArgs e) //Новая игра
        {
            world = new Model();
            enemy = new Enemy();                       
            foreach (var b in PlayerField.Controls)
            {
                var button = b as Button;
                button.Image = Resource1.Water;                
            }           
            foreach (var b in EnemyField.Controls)
            {
                var button = b as Button;
                button.Image = Resource1.Water;                
            }
            startButton.Visible = true;
            endTurn.BackColor = Color.Gray;
            endTurn.Text = "Сделайте ход";
            endTurn.Enabled = false;
            endTurn.Visible = false;
            directionButton.Text = "Up";
            direction = ShipDirection.Up;
            for (var i = 0; i < 4; i++)
            {
                var button = selectionShipsElements[i].Item1;
                var textBox = selectionShipsElements[i].Item2;
                button.BackColor = Color.Coral;
                button.ForeColor = Color.Black;
                textBox.Text = (4 - i).ToString();
                selectionShipsElements[i] = new Tuple<Button, TextBox, int, int>(button, textBox, 4 - i, i + 1);
            }
            selectedShip = null;
            panel1.Visible = true;
            label5.Visible = true;
            directionButton.Visible = true;
            listBox1.Items.Clear();
            listBox1.Visible = false;
        }

        private void directionButton_Click(object sender, EventArgs e) //Кнопка выбора направления установки корабля
        {
            direction = direction == ShipDirection.Up ? ShipDirection.Right : ShipDirection.Up;            
            directionButton.Text = direction == ShipDirection.Up ? "Up" : "Right";
        }

        #region  Кнопки выбора кораблей
        private void Button1_Click(object sender, EventArgs e) //Однопалубный
        {
            SelectShip(0);
        }

        private void Button2_Click(object sender, EventArgs e) //Двухпалубный
        {
            SelectShip(1);
        }

        private void Button3_Click(object sender, EventArgs e) //Трехпалубный
        {
            SelectShip(2);
        }

        private void Button4_Click(object sender, EventArgs e) //Четырехпалубный
        {
            SelectShip(3);
        }

        void SelectShip(int index)
        {
            if (selectedShip != null)
            {
                selectedShip.Item1.BackColor = Color.Coral;
                selectedShip.Item1.ForeColor = Color.Black;
            }
            selectedShip = selectionShipsElements[index];
            selectedShip.Item1.BackColor = Color.Brown;
            selectedShip.Item1.ForeColor = Color.White;

        }
        #endregion

        void UpdateSelectedTuple(Tuple<Button, TextBox, int, int> tuple, int newCount)
        {
            var newTuple = new Tuple<Button, TextBox, int, int>(tuple.Item1, tuple.Item2, newCount, tuple.Item4);
            selectionShipsElements[tuple.Item4 - 1] = newTuple;
            selectedShip = newTuple;
        }

        void UpdateListBox()
        {
            listBox1.ForeColor = Color.Black;
            listBox1.Items.Add("");
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
            listBox1.SelectedIndex = -1;
        }
    }
}
