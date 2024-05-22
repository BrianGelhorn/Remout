﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Remout.Customs
{
    public class CustomPopup : Window
    {
        public CustomPopup(
            bool ShowInTaskBar = false,
            double Width = 0,
            double Height = 0,
            WindowStyle windowStyle = WindowStyle.ToolWindow,
            WindowStartupLocation startupLocation = WindowStartupLocation.CenterOwner)
        {
            if (Width < 0) SetWidth(Width);
            if (Height < 0) SetHeight(Height);
            Owner = Application.Current.MainWindow;
            ResizeMode = ResizeMode.NoResize;
            WindowStyle = WindowStyle.ToolWindow;
            ShowInTaskbar = false;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        public void SetWidth(double width)
        {
            Width = width;
        }

        public void SetHeight(double height) 
        { 
            Height = height;
        }
    }
}
