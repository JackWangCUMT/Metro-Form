﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Metro
{
    public class MetroForm : Form
    {
        public static class ColorSchema
        {
            public static Color
            cPrimaryLightLight = Color.FromArgb(0x80, 0xCB, 0xEB),
            cPrimaryLight = Color.FromArgb(0x4F, 0xC8, 0xFC),
            cPrimary = Color.FromArgb(0x41, 0xB1, 0xE1),
            cPrimaryDark = Color.FromArgb(0x27, 0x6B, 0x87),
            cPrimaryDarkDark = Color.FromArgb(0x08, 0x6F, 0x9E),

            cSecondary = Color.FromArgb(0xFF, 0xFF, 0xFF);

            public static SolidBrush
            bCaption = new SolidBrush(cPrimary),
            bWindow = new SolidBrush(cSecondary),
            bCaptionTitle = new SolidBrush(cSecondary),
            bCaptionControls = new SolidBrush(cPrimaryDark),
            bCaptionControlsHover = new SolidBrush(cPrimaryLightLight),
            bCaptionControlsActive = new SolidBrush(cPrimaryDarkDark),
            bCaptionControlsShadow = new SolidBrush(cPrimaryLight);

            public static Pen
            pBorder = new Pen(cPrimary),
            pCaptionControls = new Pen(cPrimaryDark, 2f),
            pCaptionControlsActive = new Pen(cSecondary, 2f),
            pCaptionControlsShadow = new Pen(cPrimaryLight, 2f);

            public static void Update()
            {

            }
        }

        MetroFormGlow glow = null;
        public MetroForm()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.EnableNotifyMessage, true);

            UpdateBounds();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;

            glow = new MetroFormGlow(this);
            //glow.Show();
            //glow.Render();
            //this.Focus();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            glow.Show();
            glow.Render();
            
            //glow.BringToFront();
            this.BringToFront();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            glow.Hide();
        }

        Rectangle CaptionBounds, CaptionDragBounds, WindowBounds, btnClose, btnMaximize;
        const int buttonsize = 8, buttonwidth = 34, buttonxmid = (buttonwidth / 2 + buttonsize / 2);
        void UpdateBounds()
        {
            CaptionBounds = new Rectangle(0, 0, this.Width, 30);
            CaptionDragBounds = new Rectangle(0, 0, CaptionBounds.Width - (buttonwidth * capcontrols.Length), CaptionBounds.Height);
            WindowBounds = new Rectangle(0, 0, this.Width - 1, this.Height - 1);

            btnClose = new Rectangle(CaptionBounds.Width - buttonwidth, 0, buttonwidth, CaptionBounds.Height);
            btnMaximize = new Rectangle(CaptionBounds.Width - buttonwidth * 2, 0, buttonwidth, CaptionBounds.Height);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            UpdateBounds();
            if (glow != null) glow.Render();
        }
        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);

            if (glow != null) glow.UpdatePosition();
        }

        Font fCaption = new Font("Microsoft Sans Serif", 10.25f, FontStyle.Regular);
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(ColorSchema.bCaption, CaptionBounds);
            e.Graphics.DrawRectangle(ColorSchema.pBorder, WindowBounds);

            TextRenderer.DrawText(e.Graphics, this.Text, fCaption, new Rectangle(9, 0, CaptionDragBounds.Width - 9, CaptionBounds.Height), ColorSchema.cSecondary, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            int midy = (int)Math.Round(CaptionBounds.Height / 2f - 5f);
            int midys = midy + 1;

            /* Close button */
            int closex = this.Width - buttonxmid - 1;
            if (capcontrols[0] == 2)
            {
                e.Graphics.FillRectangle(ColorSchema.bCaptionControlsActive, btnClose);

                e.Graphics.DrawLine(ColorSchema.pCaptionControlsActive, closex, midy, closex + buttonsize, midy + buttonsize);
                e.Graphics.DrawLine(ColorSchema.pCaptionControlsActive, closex, midy + buttonsize, closex + buttonsize, midy);
            }
            else
            {
                if (capcontrols[0] == 1)
                    e.Graphics.FillRectangle(ColorSchema.bCaptionControlsHover, btnClose);
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.DrawLine(ColorSchema.pCaptionControlsShadow, closex, midys, closex + buttonsize, midys + buttonsize);
                e.Graphics.DrawLine(ColorSchema.pCaptionControlsShadow, closex, midys + buttonsize, closex + buttonsize, midys);

                e.Graphics.DrawLine(ColorSchema.pCaptionControls, closex, midy, closex + buttonsize, midy + buttonsize);
                e.Graphics.DrawLine(ColorSchema.pCaptionControls, closex, midy + buttonsize, closex + buttonsize, midy);
            }

            /* Maximize button */
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            int maximizex = this.Width - buttonwidth - buttonxmid;
            if (capcontrols[1] == 2)
            {
                e.Graphics.FillRectangle(ColorSchema.bCaptionControlsActive, btnMaximize);

                e.Graphics.DrawRectangle(ColorSchema.pCaptionControlsActive, maximizex, midys, 8, 8);
            }
            else
            {
                if (capcontrols[1] == 1)
                    e.Graphics.FillRectangle(ColorSchema.bCaptionControlsHover, btnMaximize);
                e.Graphics.DrawRectangle(ColorSchema.pCaptionControlsShadow, maximizex, midys + 1, 8, 8);

                e.Graphics.DrawRectangle(ColorSchema.pCaptionControls, maximizex, midys, 8, 8);
            }
        }

        int[] capcontrols = new int[3];
        void UpdateCaptionControl(int index, int state)
        {
            if (state <= capcontrols[index] && state != 0) return;
            
            this.Invalidate();
            capcontrols[index] = state;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            e = new MouseEventArgs(e.Button, e.Clicks, e.X - 9, e.Y - 30, e.Delta);

            base.OnMouseMove(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(ColorSchema.bWindow, e.Graphics.ClipBounds);
            //base.OnPaintBackground(e);
        }

        const int bordersize = 8;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WinAPI.WM_NCHITTEST)
            {
                Point mouse = this.PointToClient(Cursor.Position);

                //System.Diagnostics.Debug.WriteLine(mouse.X + ":" + mouse.Y);

                UpdateCaptionControl(0, (btnClose.Contains(mouse) ? 1 : 0));
                UpdateCaptionControl(1, (btnMaximize.Contains(mouse) ? 1 : 0));

                foreach (int b in capcontrols)
                    if (b != 0)
                    {
                        m.Result = (IntPtr)WinAPI.HTBORDER;
                        return;
                    }

                bool
                left = false,
                right = false,
                top = false,
                bottom = false;

                if (mouse.X <= bordersize) left = true;
                else if (mouse.X >= this.Width - bordersize) right = true;
                if (mouse.Y <= bordersize) top = true;
                else if (mouse.Y >= this.Height - bordersize) bottom = true;

                if (top && left) m.Result = (IntPtr)WinAPI.HTTOPLEFT;
                else if (top && right) m.Result = (IntPtr)WinAPI.HTTOPRIGHT;
                else if (bottom && left) m.Result = (IntPtr)WinAPI.HTBOTTOMLEFT;
                else if (bottom && right) m.Result = (IntPtr)WinAPI.HTBOTTOMRIGHT;
                else if (top) m.Result = (IntPtr)WinAPI.HTTOP;
                else if (bottom) m.Result = (IntPtr)WinAPI.HTBOTTOM;
                else if (left) m.Result = (IntPtr)WinAPI.HTLEFT;
                else if (right) m.Result = (IntPtr)WinAPI.HTRIGHT;
                else if (CaptionDragBounds.Contains(mouse)) m.Result = (IntPtr)WinAPI.HTCAPTION;
                else if (CaptionBounds.Contains(mouse)) m.Result = (IntPtr)WinAPI.HTBORDER;
                else m.Result = (IntPtr)WinAPI.HTCLIENT;
                return;
            }
            else if (m.Msg == WinAPI.WM_NCMOUSELEAVE)
            {
                for (int i = 0; i < capcontrols.Length; i++) capcontrols[i] = 0;
                this.Invalidate();
            }
            else if (m.Msg == WinAPI.WM_NCLBUTTONUP)
            {
                Point mouse = this.PointToClient(Cursor.Position);
                if (btnClose.Contains(mouse) && capcontrols[0] == 2) { this.Close(); return; }
                else if (btnMaximize.Contains(mouse) && capcontrols[1] == 2)
                {
                    this.WindowState = (this.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized);
                    if (this.WindowState == FormWindowState.Maximized) glow.Hide();
                    else glow.Show();

                    return;
                }

                for (int i = 0; i < capcontrols.Length; i++)
                    if (capcontrols[i] == 2) capcontrols[i] = 1;
                this.Invalidate();
            }
            else if (m.Msg == WinAPI.WM_NCLBUTTONDOWN)
            {
                Point mouse = this.PointToClient(Cursor.Position);
                UpdateCaptionControl(0, (btnClose.Contains(mouse) ? 2 : 0));
                UpdateCaptionControl(1, (btnMaximize.Contains(mouse) ? 2 : 0));
            }
            else if (m.Msg == WinAPI.WM_NCCALCSIZE && m.WParam.ToInt32() == 1)
            {
                m.Result = new IntPtr(0xF0);
                return;
            }
            else if (m.Msg == WinAPI.WM_SYSCOMMAND)
            {
                int command = m.WParam.ToInt32() & 0xFFF0;
                if (command == WinAPI.SC_MINIMIZE) glow.WindowState = FormWindowState.Minimized;
                else if (command == WinAPI.SC_RESTORE) glow.WindowState = FormWindowState.Normal;
            }

            base.WndProc(ref m);
        }
    }
}
