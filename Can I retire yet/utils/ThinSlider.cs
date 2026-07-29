using System;
using System.Drawing;
using System.Windows.Forms;

namespace Can_I_retire_yet.utils
{
    class ThinSlider : Control
    {
        public event EventHandler ValueChanged; // Event for value changes

        public int Minimum { get; set; }
        public int Maximum { get; set; } 
        //public int Value { get; set; } 

        private bool dragging = false;

        private int _value = 20000;

        public int Value
        {
            get => _value;
            set
            {
                int newValue = Math.Max(Minimum, Math.Min(Maximum, value));
                if (_value != newValue)
                {
                    _value = newValue;
                    ValueChanged?.Invoke(this, EventArgs.Empty); // Raise event
                    Invalidate(); // Redraw
                }
            }
        }



        public ThinSlider()
        {
            DoubleBuffered = true;
            
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
    }
}
