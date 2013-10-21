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


namespace Leap_C
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private static Controller _controller;
        private long _currentTime;
        private long _previousTime;
        private long _timeChange;
        private bool _cursourEnabled;
        private Leap.Listener _listener;
        private object sync = new object();
        LeapClass2 ardrone = new LeapClass2();

        public void Form1()
        {
            InitializeComponent();
            
            _listener = new Leap.Listener();
            _controller = new Controller();
            if (_controller.IsConnected)
            {
                Console.WriteLine("Success");
            }
            _controller.AddListener(_listener);
            
            _cursourEnabled = false;
            Application.Current.MainWindow.Closing += new CancelEventHandler(OnFormClosed);
            
        }

        private void OnFormClosed(object sender, CancelEventArgs e )
        {
            _controller.RemoveListener(_listener);
            _controller.Dispose();
        }

        private void ListenerOnFrame(Controller controller)
        {
            lock (sync)
            {
                var fingers = new FingerList();
                var frame = controller.Frame();
                _currentTime = frame.Timestamp;
                _timeChange = _currentTime - _previousTime;

                #region Read frame data
                if (!frame.Hands.IsEmpty)
                {
                    Hand hand = frame.Hands[0];
                    fingers = hand.Fingers;
                    if (_timeChange > 1000)
                    {
                        if (!fingers.IsEmpty)
                        {
                            if (textBox1.Dispatcher.CheckAccess())
                            {
                                textBox1.Dispatcher.BeginInvoke(new Action<object>(Update), new object[] { fingers });
                            }
                        }
                    }
                }
                #endregion
                #region Gesture
                
                ardrone.Init_AR_Drone();
                int success = 0;

                GestureList gestures = frame.Gestures();
                foreach (Gesture gesture in gestures)
                {
                    switch (gesture.Type)
                    {
                        case Gesture.GestureType.TYPECIRCLE:
                            success = ardrone.Roll_AR_Drone();
                            PrintData("Circle gesture");
                            break;
                        case Gesture.GestureType.TYPESWIPE:
                            PrintData("Swipe gesture");
                            break;
                        case Gesture.GestureType.TYPEKEYTAP:
                            PrintData("Tap gesture");
                            break;
                        case Gesture.GestureType.TYPESCREENTAP:
                            PrintData("Tap ScreenTapGesture");
                            break;
                        default:
                            PrintData("Unknown gesture type.");
                            break;
                    }
                }
                #endregion
                //#region PalmPosition
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
                Console.WriteLine("x: %f y: %f", xPositionofPalm, yPositionofPalm);
               float changeXInHand, changeYInHand;
                changeXInHand = xPositionofPalm - handy.StabilizedPalmPosition.x;
                changeYInHand = yPositionofPalm - handy.StabilizedPalmPosition.y;
                ardrone.set_Drone_command(roll, pitch, yaw, 0);
                //#endregion
                #region Mouse

                if (_cursourEnabled)
                {
                   
                    var screen = controller.LocatedScreens.ClosestScreenHit(fingers[0]);
                   System.Drawing.Point pt = new System.Drawing.Point();
                    if (fingers.Count == 2)
                    {
                        if (_timeChange > 4000)
                        {
                            
                            Leap.Vector leapVec = new Leap.Vector();
                            pt.X = (int)leapVec.x;
                            pt.Y = (int)leapVec.y;
                            System.Windows.Forms.Cursor.Position = pt;
                            
                        }
                    }

                    if (screen != null && screen.IsValid)
                    {
                        if ((int)fingers[0].TipVelocity.Magnitude > 25)
                        {
                            var xScreenIntersect = screen.Intersect(fingers[0], true).x;
                            var yScreenIntersect = screen.Intersect(fingers[0], true).y;

                            if (xScreenIntersect.ToString() != "NaN")
                            {
                                pt.X = (int)(xScreenIntersect * screen.WidthPixels);
                                pt.Y = (int)(screen.HeightPixels - (yScreenIntersect * screen.HeightPixels));
                                
                            }

                        }
                    }
                }

                #endregion
                _previousTime = _currentTime;
                frame.Dispose();
            }
        }

        void PrintData(string str)
        {
            
            
                textBox1.Dispatcher.BeginInvoke(new Action<string>(UpdateGestureBox), str);
            
        }

        private void Update(object obj)
        {
            var fngr = (FingerList)obj;
            Console.WriteLine(fngr.Count.ToString());
            var xRaw = (decimal)fngr[0].TipPosition.x;
            textBox1.Text.Insert(1, fngr.Count.ToString());
            Point pt = new Point(CoordinateConverter.TranslateToLeft(xRaw, 1500),
                700 - (int)fngr[0].TipPosition.y);
            textBox1.PointToScreen(pt);
        }

        private void UpdateGestureBox(string str)
        {
            Console.WriteLine(str);
            textBox2.Text = str;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _cursourEnabled = true;
            ardrone.AR_Drone_Land();
            //code to auto land ardrone
        }

        public void button2_Click(object sender, EventArgs e)
        {
            //code to take off and hover
            ardrone.AR_Drone_Takeoff();
        }

        public void button3_Click(object sender, EventArgs e)
        {
            //code to flat trim
            ardrone.AR_Drone_FlatTrim();
        }
    }
}
