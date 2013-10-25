using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Leap;
using Leap_C;


namespace Leap_C
{
    public class Leap_Cont : Listener
    {
        //public event Action<FingerList> OnFingersRegistered;
        //public event Action<GestureList> OnGestureMade;
        private long _now;
        private long _previous;
        private long _timeDifference;
        private long _currentTime;
        private long _previousTime;
        private long _timeChange;

        private ARDrone.UI.MainWindow droneWindow;

        private float AscensionRate = 0.0f;
        private float RollRate = 0.0f;
        private float Pitch = 0.0f;
        private float Yaw = 0.0f;

        public Leap_Cont()
        {
        }

        public override void OnInit(Controller controller) { }

        public override void OnConnect(Controller controller)
        {
            //foreach (var gesture in (Gesture.GestureType[])Enum.GetValues(typeof(Gesture.GestureType)))
            //    controller.EnableGesture(gesture);
        }

        public override void OnDisconnect(Controller controller) { }

        public override void OnExit(Controller controller) { }

        public override void OnFrame(Controller controller)
        {/*
            var frame = controller.Frame();
            _now = frame.Timestamp;
            _timeDifference = _now - _previous;

            if (frame.Hands.IsEmpty) return;

            _previous = frame.Timestamp;

            if (_timeDifference < 500) return;
            // Run async
            if (frame.Gestures().Count > 0)
                Task.Factory.StartNew(() => OnGestureMade(frame.Gestures()));
            if (frame.Fingers.Count > 0)
                Task.Factory.StartNew(() => OnFingersRegistered(frame.Fingers));
            */

            ListenerOnFrame(controller);
        }

        private void ListenerOnFrame(Controller controller)
        {
            var fingers = new FingerList();
            var frame = controller.Frame();
           
            
            _currentTime = frame.Timestamp;
            _timeChange = _currentTime - _previousTime;
            if (droneWindow != null)
            {
                #region Read frame data
                if (droneWindow.CurrentDroneState == ARDrone.UI.DroneState.FLYING)
                {
                    if (!frame.Hands.IsEmpty && frame.Fingers.IsEmpty)
                    {
                        // HOVER
                        AscensionRate = 0.0f;
                        RollRate = 0.0f;
                        Pitch = 0.0f;
                        Yaw = 0.0f;
                        droneWindow.set_Drone_command(RollRate, Pitch, Yaw, AscensionRate);
                        droneWindow.UpdateUIAsync("Hovering");
                        _previousTime = _currentTime;
                        frame.Dispose();
                        return;
                    }

                    if (!frame.Hands.IsEmpty)
                    {
                        HandList h = new HandList();
                        var leftMost = h.Leftmost;
                        var rightMost = h.Rightmost;
                        var numberOfHands = h.Count;
                        Hand handy = frame.Hands[0];
                        var pitch = handy.StabilizedPalmPosition.Pitch;
                        var roll = handy.StabilizedPalmPosition.Roll;
                        var xPositionofPalm = handy.PalmPosition.x;
                        var yPositionofPalm = handy.PalmPosition.y;
                        var yaw = handy.StabilizedPalmPosition.Yaw;

                        SetAscensionRate(handy.StabilizedPalmPosition.y);
                        SetRollRate(handy.PalmNormal.Roll);
                        SetPitchRate(handy.PalmNormal.Pitch);
                        SetYawRate(handy.StabilizedPalmPosition.x);
                        droneWindow.UpdateUIAsync(string.Format("AR = {0}, RR = {1}, PR = {2}, Yaw = {3}", AscensionRate, RollRate, Pitch, Yaw));
                        droneWindow.set_Drone_command(RollRate, Pitch, Yaw, AscensionRate);

                        float changeXInHand, changeYInHand;
                        changeXInHand = xPositionofPalm - handy.StabilizedPalmPosition.x;
                        changeYInHand = yPositionofPalm - handy.StabilizedPalmPosition.y;
                    }
                    else
                    {
                        AscensionRate = 0.0f;
                        RollRate = 0.0f;
                        Pitch = 0.0f;
                        Yaw = 0.0f;
                        droneWindow.set_Drone_command(RollRate, Pitch, Yaw, AscensionRate);

                        droneWindow.Land();

                    }
                }
                else if (droneWindow.CurrentDroneState == ARDrone.UI.DroneState.CONNECTED || droneWindow.CurrentDroneState == ARDrone.UI.DroneState.LANDED)
                {
                    if (!frame.Hands.IsEmpty)
                    {
                        Hand h = frame.Hands[0];
                        //SetAscensionRate(h.StabilizedPalmPosition.y);
                        //SetRollRate(h.PalmNormal.Roll);
                        //SetPitchRate(h.PalmNormal.Pitch);
                       // droneWindow.UpdateUIAsync(string.Format("AR = {0}, RR = {1}, PR = {2}", AscensionRate, RollRate, Pitch));
                        droneWindow.UpdateUIAsync(string.Format("yaw = {0} ", h.StabilizedPalmPosition.x));
                        if (h.StabilizedPalmPosition.y > 250.0f)
                        {
                            droneWindow.Takeoff();
                        }
                    }
                }
                #endregion
            }
            

            //if (droneWindow != null)
             //   droneWindow.Navigate(roll, pitch, yaw, 0);

            _previousTime = _currentTime;
            frame.Dispose();
        }

        public void SetAscensionRate(float x)
        {
            if (x > 400.0f)
                AscensionRate = 1.0f;
            else if (x < 100.0f)
                AscensionRate = -1.0f;
            else
            {
                // Scale between 0 and 1
                // (currentValue - minValue) / (maxValue - minValue) * desiredRange - (desiredRange / 2)
                float scaled = (x - 100.0f) / 300.0f * 2 - 1;

                if (scaled < 0.2f && scaled > -0.2f)
                    AscensionRate = 0.0f;
                else
                    AscensionRate = scaled;
            }
        }

        public void SetMainWindow(ARDrone.UI.MainWindow mw)
        {
            droneWindow = mw;
        }

        public void SetRollRate(float rollrate)
        {
            rollrate = -rollrate;
            if (rollrate > 1.0f)
                RollRate = 1.0f;
            else if (rollrate < -1.0f)
                RollRate = -1.0f;
            else
            {
                if (rollrate < 0.2f && rollrate > -0.2f)
                    RollRate = 0.0f;
                else 
                    RollRate = rollrate;
            }
        }

        public void SetPitchRate(float pitchrate)
        {
            if (pitchrate > -0.5f)
                Pitch = -1f;
            else if (pitchrate < -2.5f)
                Pitch = 1f;
            else
            {
                float scaledpitch = (pitchrate + 0.5f) / -2f * 2f - 1f;

                if (scaledpitch < 0.2f && scaledpitch > -0.2f)
                    Pitch = 0;
                else
                    Pitch = scaledpitch;
            }
            Pitch = -Pitch;
        }

        // (currentValue - minValue) / (maxValue - minValue) * desiredRange - (desiredRange / 2)
        public void SetYawRate(float yaw)
        {
            if (yaw < -100.0f)
            {
                Yaw = -0.5f;
            }
            else if (yaw > 100.0f)
                Yaw = 0.5f;
            else
            {
                Yaw = 0.0f;
            }
        }
    }
    
}
    

