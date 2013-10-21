using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ARDrone.Control;
using ARDrone.Capture;
using ARDrone.Hud;
using ARDrone.Input;
using ARDrone.Input.Utils;
using ARDrone.Control.Commands;
using ARDrone.Control.Data;
using ARDrone.Control.Events;
using System.Windows.Threading;

namespace Leap_C
{
    class LeapClass2
    {

        private DroneControl droneControl;
        private delegate void OutputEventHandler(String output);
     

        public void Init_AR_Drone()
        {
  
        }

        public int Roll_AR_Drone()
        {
            return 1;
        }

        public void setPitch(float pitch)
        {

        }

        public void setYaw(float yaw)
        {

        }

        public void set_X_Y(float x, float y)
        {

        }

        public void set_Roll(float roll)
        {

        }

        public void AR_Drone_Takeoff()
        {

        }

        public void AR_Drone_Land()
        {

        }

        public void AR_Drone_FlatTrim()
        {

        }
        public void set_Drone_command(double roll, double pitch, double yaw, double gaz)
        {
            FlightMoveCommand MoveForward = new FlightMoveCommand((int)roll, (int)pitch, (int)yaw, (int)gaz);
            droneControl.SendCommand(MoveForward);
            //Dispatcher.BeginInvoke(new OutputEventHandler(UpdateUISync), message);
        }
    }
}
