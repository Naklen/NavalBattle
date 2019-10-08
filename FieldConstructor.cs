using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NavalBattle
{
    public static class FieldConstructor
    {      
        public static void BuildField(TableLayoutPanel field, Action<Button> onClick)
        {
            field.Size = new Size(510, 510);
            for (var i = 0; i < 10; i++)
            {

                field.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
                field.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50));

            }            

            for (var i = 0; i < 10; i++)
                for (var j = 0; j < 10; j++)
                {
                    var b = new Button() { Image = Resource1.Water, ImageAlign = ContentAlignment.MiddleCenter, Size = Resource1.Water.Size };
                    b.Click += (sender, args) => onClick(b);
                    b.FlatAppearance.BorderSize = 0;
                    b.FlatStyle = FlatStyle.Flat;
                    b.FlatAppearance.MouseDownBackColor = Color.Transparent;
                    b.FlatAppearance.MouseOverBackColor = Color.Transparent;
                    field.Controls.Add(b, i, j);                    
                    //MainForm.
                }

            field.BackColor = Color.Transparent;
        }

        //public static void BuildField(TableLayoutPanel field)
        //{
        //    field.Size = new Size(510, 510);
        //    for (var i = 0; i < 10; i++)
        //    {

        //        field.RowStyles.Add(new RowStyle(SizeType.Absolute, 51));
        //        field.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 51));

        //    }

        //    for (var i = 0; i < 10; i++)
        //        for (var j = 0; j < 10; j++)
        //        {
        //            var b = new Button() { Image = Resource1.Water, ImageAlign = ContentAlignment.MiddleCenter, Size = Resource1.Water.Size };
        //            b.Click += (sender, args) => MessageBox.Show(b.Parent.Controls.GetChildIndex(b).ToString());
        //            b.FlatStyle = FlatStyle.Flat;
        //            b.FlatAppearance.BorderSize = 0;
        //            b.FlatAppearance.MouseDownBackColor = Color.Transparent;
        //            field.Controls.Add(b, i, j);
        //        }

        //    field.BackColor = Color.Transparent;
        //}
    }  
}
