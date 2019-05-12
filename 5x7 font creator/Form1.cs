using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace font_creator
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        Color selectedColor = Color.Black;
        Color[,,] colorMatrix;
        Color[,] coppiedMatrix;
        Point oldPoint = new Point(100,100);
        int selectedChar = 0;
        byte format = 0;

        Button[] showButtons = new Button[256]; 




        public Form1()
        {
            InitializeComponent();
        colorMatrix = new Color[256, tableLayoutPanel1.RowCount, tableLayoutPanel1.ColumnCount];
        coppiedMatrix = new Color[tableLayoutPanel1.RowCount, tableLayoutPanel1.ColumnCount];
        for (int a = 0; a<256; a++)
            {
                fillArray(85 , a);
            }
            tableLayoutPanel1.Refresh();
            

            for(int a = 0; a < 256; a++)
            {
                int available = panel300.Width / 50;
                Button newButton = new Button();
                newButton.Location = new System.Drawing.Point(((a% available) *50)+3, ((a/ available) *68)+3);
                newButton.Name = "button"+(a+50).ToString();
                newButton.Size = new System.Drawing.Size(44, 62);
                newButton.TabIndex = 58;
                newButton.Text = "button11";
                newButton.UseVisualStyleBackColor = true;
                newButton.MouseDown += new MouseEventHandler(this.button11_Click);
                newButton.MouseHover += new EventHandler(this.button11_Move);
                newButton.Paint += new System.Windows.Forms.PaintEventHandler(this.button11_Paint);
                newButton.Tag = a;
                panel300.Controls.Add(newButton);

                showButtons[a] = newButton;
            }

            for (int a = 0; a < 256; a++)
            {
                metroComboBox1.Items.Add((a + 1).ToString());
            }
            metroComboBox1.SelectedIndex = 0;

            byte tempByte = 0;
            String lineString = "";
            for (byte a = 0; a < tableLayoutPanel1.ColumnCount; a++)
            {
                for (byte b = 0; b < tableLayoutPanel1.RowCount; b++)
                {
                    tempByte = (byte)(tempByte | (colorToNumber(colorMatrix[selectedChar, b, a]) ? 0x01 << b : 0x00));
                }
                if (format == 0)
                    lineString += tempByte.ToString() + " , ";
                else if (format == 1)
                    lineString += "0X" + tempByte.ToString("X2") + " , ";
                else if (format == 2)
                    lineString += "0B" + Convert.ToString(tempByte, 2).PadLeft(8, '0') + " , ";
                tempByte = 0;
            }
            textBox1.Text = lineString;
        }
        public void resizeButtons()
        {
            for (int a = 0; a < 256; a++)
            {
                int available = panel300.Width / 50;
                showButtons[a].Location = new System.Drawing.Point(((a % available) * 50) + 3, ((a / available) * 68) + 3);
            }
        }
        public void fillArray(byte Num , int CharIndex)
        {   
            for (byte a = 0; a < tableLayoutPanel1.RowCount; a++)
            {
                for(byte b = 0; b < tableLayoutPanel1.ColumnCount; b++)
                    colorMatrix[CharIndex, a, b] = (numberToColor(Num) ? Color.Black : Color.White);
            }
        }


        public void fillArray(int CharIndex)
        {
            for (byte a = 0; a < tableLayoutPanel1.RowCount; a++)
            {
                for (byte b = 0; b < tableLayoutPanel1.ColumnCount; b++)
                    colorMatrix[CharIndex, a, b] = selectedColor;
            }
        }

        public Color[,,] stringToColorMatrix(string inputString)
        {
            inputString = inputString.Replace(" ", "");
            inputString=inputString.Replace("\r", "");
            inputString=inputString.Replace("\n", "");
            inputString = inputString.Replace("\t", "");
            Color[,,] returnThis = new Color[256,tableLayoutPanel1.RowCount, tableLayoutPanel1.ColumnCount];

            int index = 0;
            for(int sc = 0; sc < 256; sc++)
            {
                for (byte a = 0; a < tableLayoutPanel1.ColumnCount; a++)
                {
                    int nextStartIndex = 0;
                    if (a != tableLayoutPanel1.ColumnCount-1 || sc != 255)
                    {

                        byte tempByte = 0;
                        nextStartIndex = inputString.IndexOf(",", index);
                        string partString = inputString.Substring(index, nextStartIndex - index);
                        if((partString.IndexOf("0X") != -1) || (partString.IndexOf("0x") != -1))
                        {
                            partString=partString.Replace("0X", "");
                            partString=partString.Replace("0x", "");
                            tempByte = Convert.ToByte(partString, 16);
                        }
                        else if ((partString.IndexOf("0B") != -1) || (partString.IndexOf("0b") != -1) || partString.Length >3)
                        {
                            partString=partString.Replace("0B", "");
                            partString=partString.Replace("0b", "");
                            tempByte = Convert.ToByte(partString, 2);
                        }
                        else
                            tempByte = byte.Parse(partString);


                        for (byte b = 0; b < tableLayoutPanel1.RowCount; b++)
                        {
                            returnThis[sc, b, a] = (numberToColor((byte)(tempByte & (0x01 << b)))?Color.Black:Color.White);
                        }
                        index = nextStartIndex + 1;
                    }
                }
            }
            
            return returnThis;
        }
         


        public string prepareString()
        {
            string returnThis = "";
            byte tempByte = 0;
            for(int sc = 0; sc < 256; sc++)
            {
                for (byte a = 0; a < tableLayoutPanel1.ColumnCount; a++)
                {
                    for (byte b = 0; b < tableLayoutPanel1.RowCount; b++)
                    {
                        tempByte = (byte)(tempByte | (colorToNumber(colorMatrix[sc, b, a])?0x01 << b : 0x00));
                    }
                    if (format == 0)
                        returnThis += tempByte.ToString() + " , ";
                    else if (format == 1)
                        returnThis += "0X" + tempByte.ToString("X2") + " , ";
                    else if (format == 2)
                        returnThis += "0B" + Convert.ToString(tempByte, 2).PadLeft(8, '0') + " , ";
                    tempByte = 0;
                }
                returnThis += "\r\n";
            }
            
            return returnThis;
        }

        public bool numberToColor(byte Num)
        {
            if (Num == 0)
                return false;
            else
                return true;
        }

        public bool colorToNumber(Color color)
        {
            if (color == Color.Black)
                return true;
            else
                return false;
        }

        public Brush colorToBrush(Color color)
        {
            if (color == Color.Black)
                return Brushes.Black;
            else if (color == Color.White)
                return Brushes.White;
            else if (color == Color.Gray)
                return Brushes.Gray;
            else
                return Brushes.Gray;
        }



        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            selectedColor = Color.Gray;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            selectedColor = Color.White;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            selectedColor = Color.Black;
        }

        private void tableLayoutPanel1_Click(object sender, EventArgs e)
        {

        }

        static public Point? GetRowColIndex(TableLayoutPanel tlp, Point point)
        {
            if (point.X > tlp.Width || point.Y > tlp.Height)
                return null;

            int w = tlp.Width;
            int h = tlp.Height;
            int[] widths = tlp.GetColumnWidths();

            int i;
            for (i = widths.Length - 1; i >= 0 && point.X < w; i--)
                w -= widths[i];
            int col = i + 1;

            int[] heights = tlp.GetRowHeights();
            for (i = heights.Length - 1; i >= 0 && point.Y < h; i--)
                h -= heights[i];

            int row = i + 1;


            if (col >= tlp.ColumnCount || row >= tlp.RowCount)
                return null;

            return new Point(col, row);
        }

        private void tableLayoutPanel1_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
                e.Graphics.FillRectangle(new SolidBrush(colorMatrix[selectedChar ,e.Row , e.Column]), e.CellBounds);
        }

        private void tableLayoutPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (Control.MouseButtons == MouseButtons.Left)
            {
                var cellPos = GetRowColIndex(tableLayoutPanel1, tableLayoutPanel1.PointToClient(Cursor.Position));
                if (cellPos != null)
                {
                    Point boxPoint = (Point)cellPos;
                    if (boxPoint != oldPoint)
                    {
                        colorMatrix[selectedChar, boxPoint.Y, boxPoint.X] = selectedColor;
                        oldPoint = boxPoint;
                        tableLayoutPanel1.Refresh();
                        showButtons[selectedChar].Refresh();
                        byte tempByte = 0;
                        String lineString = "";
                        for (byte a = 0; a < tableLayoutPanel1.ColumnCount; a++)
                        {
                            for (byte b = 0; b < tableLayoutPanel1.RowCount; b++)
                            {
                                tempByte = (byte)(tempByte | (colorToNumber(colorMatrix[selectedChar, b, a]) ? 0x01 << b : 0x00));
                            }
                            if (format == 0)
                                lineString += tempByte.ToString() + " , ";
                            else if (format == 1)
                                lineString += "0X" + tempByte.ToString("X2") + " , ";
                            else if (format == 2)
                                lineString += "0B" + Convert.ToString(tempByte, 2).PadLeft(8, '0') + " , ";
                            tempByte = 0;
                        }
                        textBox1.Text = lineString;
                    }
                }
            }
            else if (Control.MouseButtons == MouseButtons.Right)
            {
                var cellPos = GetRowColIndex(tableLayoutPanel1, tableLayoutPanel1.PointToClient(Cursor.Position));
                if (cellPos != null)
                {
                    Point boxPoint = (Point)cellPos;
                    if (boxPoint != oldPoint)
                    {
                        colorMatrix[selectedChar, boxPoint.Y, boxPoint.X] = (numberToColor(1) ? Color.Black : Color.White);
                        oldPoint = boxPoint;
                        tableLayoutPanel1.Refresh();
                        showButtons[selectedChar].Refresh();
                        byte tempByte = 0;
                        String lineString = "";
                        for (byte a = 0; a < tableLayoutPanel1.ColumnCount; a++)
                        {
                            for (byte b = 0; b < tableLayoutPanel1.RowCount; b++)
                            {
                                tempByte = (byte)(tempByte | (colorToNumber(colorMatrix[selectedChar, b, a]) ? 0x01 << b : 0x00));
                            }
                            if (format == 0)
                                lineString += tempByte.ToString() + " , ";
                            else if (format == 1)
                                lineString += "0X" + tempByte.ToString("X2") + " , ";
                            else if (format == 2)
                                lineString += "0B" + Convert.ToString(tempByte, 2).PadLeft(8, '0') + " , ";
                            tempByte = 0;
                        }
                        textBox1.Text = lineString;
                    }
                }
            }
            
        }

        private void tableLayoutPanel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (Control.MouseButtons == MouseButtons.Left)
            {
                var cellPos = GetRowColIndex(tableLayoutPanel1, tableLayoutPanel1.PointToClient(Cursor.Position));
                if (cellPos != null)
                {
                    Point boxPoint = (Point)cellPos;
                    if (boxPoint != oldPoint)
                    {
                        colorMatrix[selectedChar ,boxPoint.Y, boxPoint.X] = selectedColor;
                        oldPoint = boxPoint;
                        tableLayoutPanel1.Refresh();
                        showButtons[selectedChar].Refresh();
                        byte tempByte = 0;
                        String lineString = "";
                        for (byte a = 0; a < tableLayoutPanel1.ColumnCount; a++)
                        {
                            for (byte b = 0; b < tableLayoutPanel1.RowCount; b++)
                            {
                                tempByte = (byte)(tempByte | (colorToNumber(colorMatrix[selectedChar, b, a]) ? 0x01 << b : 0x00));
                            }
                            if (format == 0)
                                lineString += tempByte.ToString() + " , ";
                            else if (format == 1)
                                lineString += "0X" + tempByte.ToString("X2") + " , ";
                            else if (format == 2)
                                lineString += "0B" + Convert.ToString(tempByte, 2).PadLeft(8, '0') + " , ";
                            tempByte = 0;
                        }
                        textBox1.Text = lineString;
                    }
                }
            }  
            else if (Control.MouseButtons == MouseButtons.Right)
            {
                var cellPos = GetRowColIndex(tableLayoutPanel1, tableLayoutPanel1.PointToClient(Cursor.Position));
                if (cellPos != null)
                {
                    Point boxPoint = (Point)cellPos;
                    if (boxPoint != oldPoint)
                    {
                        colorMatrix[selectedChar ,boxPoint.Y, boxPoint.X] = (numberToColor(1)?Color.Black : Color.White);
                        oldPoint = boxPoint;
                        tableLayoutPanel1.Refresh();
                        showButtons[selectedChar].Refresh();
                        byte tempByte = 0;
                        String lineString = "";
                        for (byte a = 0; a < tableLayoutPanel1.ColumnCount; a++)
                        {
                            for (byte b = 0; b < tableLayoutPanel1.RowCount; b++)
                            {
                                tempByte = (byte)(tempByte | (colorToNumber(colorMatrix[selectedChar, b, a]) ? 0x01 << b : 0x00));
                            }
                            if (format == 0)
                                lineString += tempByte.ToString() + " , ";
                            else if (format == 1)
                                lineString += "0X" + tempByte.ToString("X2") + " , ";
                            else if (format == 2)
                                lineString += "0B" + Convert.ToString(tempByte, 2).PadLeft(8, '0') + " , ";
                            tempByte = 0;
                        }
                        textBox1.Text = lineString;
                    }
                }
            }
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            fillArray(selectedChar);
            tableLayoutPanel1.Refresh();
            showButtons[selectedChar].Refresh();
            byte tempByte = 0;
            String lineString = "";
            for (byte a = 0; a < tableLayoutPanel1.ColumnCount; a++)
            {
                for (byte b = 0; b < tableLayoutPanel1.RowCount; b++)
                {
                    tempByte = (byte)(tempByte | (colorToNumber(colorMatrix[selectedChar, b, a]) ? 0x01 << b : 0x00));
                }
                if (format == 0)
                    lineString += tempByte.ToString() + " , ";
                else if (format == 1)
                    lineString += "0X" + tempByte.ToString("X2") + " , ";
                else if (format == 2)
                    lineString += "0B" + Convert.ToString(tempByte, 2).PadLeft(8, '0') + " , ";
                tempByte = 0;
            }
            textBox1.Text = lineString;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = prepareString();
            string fileName = "lastGenerated.af";

            if(File.Exists(fileName))
                File.WriteAllText(fileName, String.Empty);
            using (StreamWriter writer = new StreamWriter(fileName, true))
            {
                
                writer.Write(richTextBox1.Text);
                writer.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                colorMatrix = stringToColorMatrix(richTextBox1.Text);
                tableLayoutPanel1.Refresh();
                panel300.Refresh();
            }
            catch
            {
                if(richTextBox1.Text.Length == 0 )
                {
                    for (int a = 0; a < 256; a++)
                        fillArray(85, a);
                    tableLayoutPanel1.Refresh();
                    panel300.Refresh();
                }
                else
                {
                    MessageBox.Show("Not Proper Font String");
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(metroComboBox1.SelectedIndex < 255)
                metroComboBox1.SelectedIndex++;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (metroComboBox1.SelectedIndex > 0)
                metroComboBox1.SelectedIndex--;
        }

        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedChar = metroComboBox1.SelectedIndex;
            tableLayoutPanel1.Refresh();
            showButtons[selectedChar].Select();
            showButtons[selectedChar].Refresh();
            byte tempByte = 0;
            String lineString = "";
            for (byte a = 0; a < tableLayoutPanel1.ColumnCount; a++)
            {
                for (byte b = 0; b < tableLayoutPanel1.RowCount; b++)
                {
                    tempByte = (byte)(tempByte | (colorToNumber(colorMatrix[selectedChar, b, a]) ? 0x01 << b : 0x00));
                }
                if (format == 0)
                    lineString += tempByte.ToString() + " , ";
                else if (format == 1)
                    lineString += "0X" + tempByte.ToString("X2") + " , ";
                else if (format == 2)
                    lineString += "0B" + Convert.ToString(tempByte, 2).PadLeft(8, '0') + " , ";
                tempByte = 0;
            }
            textBox1.Text = lineString;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            format = 0;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            format = 1;
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            format = 2;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            for(byte a = 0; a < tableLayoutPanel1.RowCount; a++)
            {
                for (byte b = 0; b < tableLayoutPanel1.ColumnCount; b++)
                    coppiedMatrix[a,b] =  colorMatrix[selectedChar, a, b];
                button5.Enabled = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            for (byte a = 0; a < tableLayoutPanel1.RowCount; a++)
            {
                for (byte b = 0; b < tableLayoutPanel1.ColumnCount; b++)
                    colorMatrix[selectedChar, a, b] = coppiedMatrix[a, b];
            }
            tableLayoutPanel1.Refresh();
            showButtons[selectedChar].Refresh();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            for (byte a = 0; a < tableLayoutPanel1.RowCount; a++)
            {
                for (byte b = 0; b < tableLayoutPanel1.ColumnCount; b++)
                {
                    if (colorMatrix[selectedChar, a, b] == Color.Black)
                        colorMatrix[selectedChar, a, b] = Color.White;
                    else if (colorMatrix[selectedChar, a, b] == Color.White)
                        colorMatrix[selectedChar, a, b] = Color.Black;
                }
            }
            tableLayoutPanel1.Refresh();
            showButtons[selectedChar].Refresh();
        }

        private void tableLayoutPanel1_MouseUp(object sender, MouseEventArgs e)
        {
            oldPoint = new Point(100, 100);
        }

        private void tableLayoutPanel1_MouseLeave(object sender, EventArgs e)
        {
            oldPoint = new Point(100, 100);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(richTextBox1.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string fileName = "lastGenerated.af";
            using (StreamReader reader = new StreamReader(fileName))
            {
                richTextBox1.Text = reader.ReadToEnd();
                colorMatrix = stringToColorMatrix(richTextBox1.Text);
            }
            panel300.VerticalScroll.Visible = true;
            panel300.VerticalScroll.Enabled = true;
            panel300.HorizontalScroll.Visible = false;
            panel300.HorizontalScroll.Enabled = false;
            //ScrollBar vScrollBar1 = new VScrollBar();
            //vScrollBar1.Dock = DockStyle.Right;
            //vScrollBar1.Scroll += (sender, e) => { panel1.VerticalScroll.Value = vScrollBar1.Value; };
            //panel1.Controls.Add(vScrollBar1);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            string fileName = "defaultFont.af";
            using (StreamReader reader = new StreamReader(fileName))
            {
                richTextBox1.Text = reader.ReadToEnd();
                colorMatrix = stringToColorMatrix(richTextBox1.Text);
            }
            tableLayoutPanel1.Refresh();
            panel300.Refresh();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // Paint rectangle and ellipse
            
            //for (int a = 0; a < 108; a++)
            //{
            //    int diff = 0;
            //    if ((a / 12) % 2 == 1)
            //    {
            //        diff = 0;
            //    }
            //    else
            //        diff = 1;
            //    Rectangle rc = new Rectangle((a % 12) * (x.Width / 12), (a / 12) * (x.Height / 18), x.Width / 12, x.Height / 18);//panel1.ClientRectangle;
            //    e.Graphics.FillRectangle((a + diff) % 2 == 1 ? Brushes.Black : Brushes.White, rc);

                
            //}
            //e.Graphics.DrawEllipse(Pens.Black, rc);
        }

        private void button11_Paint(object sender, PaintEventArgs e)
        {
            for (int a = 0; a < tableLayoutPanel1.ColumnCount* tableLayoutPanel1.RowCount; a++)
            {
                int diff = 0;
                if ((a / tableLayoutPanel1.ColumnCount) % 2 == 1)
                {
                    diff = 0;
                }
                else
                    diff = 1;
                Rectangle temp = ((Button)sender).ClientRectangle;
                temp.Inflate(-4, -4);

                Rectangle rc = new Rectangle(((a % tableLayoutPanel1.ColumnCount) * (temp.Width / tableLayoutPanel1.ColumnCount))+ temp.X, ((a / tableLayoutPanel1.ColumnCount) * (temp.Height / tableLayoutPanel1.RowCount))+ temp.Y, temp.Width / tableLayoutPanel1.ColumnCount, temp.Height / tableLayoutPanel1.RowCount);//panel1.ClientRectangle;

                e.Graphics.FillRectangle(colorToBrush(colorMatrix[(int)((Button)sender).Tag,a/ tableLayoutPanel1.ColumnCount, a% tableLayoutPanel1.ColumnCount]), rc);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            metroComboBox1.SelectedIndex = (int)((Button)sender).Tag;
        }
        private void button11_Move(object sender, EventArgs e)
        {
            //if (Control.MouseButtons == MouseButtons.Left)
            //{
                //if (selectedChar != (int)((Button)sender).Tag)
                //    metroComboBox1.SelectedIndex = (int)((Button)sender).Tag;
            //}
        }

        private void panel300_SizeChanged(object sender, EventArgs e)
        {
            resizeButtons();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
