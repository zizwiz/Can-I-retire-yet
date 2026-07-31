using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Can_I_retire_yet.utils
{
    [ToolboxItem(true)] //This makes it visible in the Toolbox

    //Explicitly make this class public so we can get access to it
    public class ThinSlider : Control
    {
        public Color ThumbColor { get; set; } = Color.DodgerBlue;
        public Color TrackColor { get; set; } = Color.Gray;

        [Category("Behavior")]
        public event EventHandler ValueChanged;

        [Category("Behavior")]
        [Description("Minimum slider value.")]
        public int Minimum { get; set; }

        [Category("Behavior")]
        [Description("Maximum slider value.")]
        public int Maximum { get; set; }

        [Category("Behavior")]
        [Description("Current slider value.")]
        public int Value
        {
            get => _value;
            set
            {
                int newValue = Math.Max(Minimum, Math.Min(Maximum, value));
                if (_value != newValue)
                {
                    _value = newValue;
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                    Invalidate();
                }
            }
        }


        private bool dragging = false;

        private int _value = 20000;


        public ThinSlider()
        {
            DoubleBuffered = true;
            Size = new Size(150, 20);   // default designer size
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Track
            int trackHeight = 4;
            int trackY = (Height - trackHeight) / 2;
            e.Graphics.FillRectangle(Brushes.Gray, 0, trackY, Width, trackHeight);

            // Thumb
            float percent = (float)(Value - Minimum) / (Maximum - Minimum);
            int thumbX = (int)(percent * (Width - 10));
            e.Graphics.FillEllipse(Brushes.DodgerBlue, thumbX, trackY - 4, 10, 10);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            dragging = true;
            UpdateValueFromMouse(e.X);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (dragging)
            {
                UpdateValueFromMouse(e.X);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            dragging = false;
        }

        private void UpdateValueFromMouse(int mouseX)
        {
            float percent = Math.Max(0, Math.Min(1, (float)mouseX / (Width - 10)));
            Value = Minimum + (int)(percent * (Maximum - Minimum));
            Invalidate(); // Redraw


        }

        protected override bool IsInputKey(Keys keyData)
        {
            return keyData == Keys.Left || keyData == Keys.Right || base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
                Value -= 100;
            else if (e.KeyCode == Keys.Right)
                Value += 100;

            base.OnKeyDown(e);
        }

    }
}
