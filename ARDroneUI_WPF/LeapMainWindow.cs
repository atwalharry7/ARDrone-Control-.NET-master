using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Leap_C;
using Leap;
using ARDrone.Control;
using ARDrone.UI;


namespace Leap_C
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class LeapMainWindow
    {
        private DroneControl droneControl;
        private DroneConfig currentDroneConfig;

        private MainWindow droneWindow;
        private static Controller _controller;

        private bool _cursourEnabled;
        private static Leap_C.Leap_Cont _listener;
        private object sync = new object();

        public LeapMainWindow(MainWindow a)
        {
            _listener = new Leap_Cont();
            _listener.SetMainWindow(a);
            _controller = new Controller();
            droneWindow = a;

            if (_controller.IsConnected)
            {
                droneWindow.UpdateUISync("Leap is connected");
            }

            _controller.AddListener(_listener);
            _cursourEnabled = false;
        }

        public void Disconnect()
        {
            _controller.RemoveListener(_listener);
            _controller.Dispose();
        }
    }
}
