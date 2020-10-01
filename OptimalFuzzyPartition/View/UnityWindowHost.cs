using SimpleTCP;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace OptimalFuzzyPartition.View
{
    public class UnityWindowHost : HwndHost //TODO dispose
    {
        public StreamWriter StreamWriter => _unityPlayerProcess.StandardInput;
        public StreamReader StreamReader => _unityPlayerProcess.StandardOutput;

        private readonly string UnityPlayerName = "FuzzyPartitionUnityProject.exe";

        private Process _unityPlayerProcess;

        private IntPtr _unityPlayerHwnd;

        public SimpleTcpServer SimpleTcpServer;

        private static int _nextPortNumber = 8910;

        private readonly int port;

        private bool destroyed = false;

        public UnityWindowHost()
        {
            port = _nextPortNumber++;
            SimpleTcpServer = new SimpleTcpServer().Start(port);
        }

        ~UnityWindowHost()
        {
            if (destroyed) return;

            Destroy();
        }

        public void Destroy()
        {
            _unityPlayerProcess.Kill();
            //_unityPlayerProcess.Close();

            //System.Threading.Thread.Sleep(1000);
            //while (_unityPlayerProcess.HasExited == false)
            //    

            //DestroyWindow(handle);
            destroyed = true;
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            if (!File.Exists(UnityPlayerName))
            {
                MessageBox.Show("Unity player file not found.", "Fatal error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw new FileNotFoundException();
            }

            var handle = hwndParent.Handle.ToInt32();

            _unityPlayerProcess = new Process
            {
                StartInfo =
                {
                    FileName = UnityPlayerName,
                    Arguments = "-parentHWND " + handle + " " + port,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            _unityPlayerProcess.Start();

            _unityPlayerProcess.WaitForInputIdle();

            EnumChildWindows(hwndParent.Handle, WindowEnum, IntPtr.Zero);

            return new HandleRef(this, _unityPlayerHwnd);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            Destroy();
        }

        //protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        //{
        //    if ((int)wParam == 0x100) /*WM_KEYDOWN*/
        //    {
        //        Debug.WriteLine("WM_KEYDOWN");
        //        ProcessKeyDown(wParam);
        //    }

        //    return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
        //}

        private void ProcessKeyDown(IntPtr wParam)
        {
            //var item = DockLayoutManager.GetLayoutItem(this);
            //if (item != null && item.IsAutoHidden)
            //{
            //    var window = Window.GetWindow(item);
            //    PresentationSource source = PresentationSource.FromDependencyObject(this);
            //    if (window != null && source != null)
            //    window.RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, source, 0, KeyInterop.KeyFromVirtualKey((int)wParam)) { RoutedEvent = UIElement.KeyDownEvent, Source = this });
            //}
        }

        //public void KeyDown(Key key)
        //{
        //    Trace.WriteLine(key);
        //    var lParam = GetKeyCode(key, false);
        //    PostMessage(_unityPlayerHwnd, WM_KEYDOWN, (IntPtr)key, (IntPtr)lParam);
        //    PostMessage(_unityPlayerHwnd, WM_CHAR, (IntPtr)key, new IntPtr(0));
        //}

        //protected override void OnKeyDown(KeyEventArgs e)
        //{
        //    base.OnKeyDown(e);
        //    var key = e.Key;
        //    Trace.WriteLine(e.Key);
        //    var lParam = GeKeyCode(key, false);
        //    PostMessage(_unityPlayerHwnd, WM_KEYDOWN, (IntPtr)key, (IntPtr)lParam);
        //}

        //protected override void OnKeyUp(KeyEventArgs e)
        //{
        //    base.OnKeyUp(e);
        //    var key = e.Key;
        //    Trace.WriteLine(e.Key);
        //    var lParam = GeKeyCode(key, false);
        //    PostMessage(_unityPlayerHwnd, WM_KEYUP, (IntPtr)key, (IntPtr)lParam);

        //}

        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_CHAR = 0x0102;

        private int WindowEnum(IntPtr hwnd, IntPtr lparam)
        {
            _unityPlayerHwnd = hwnd;
            Debug.Print(hwnd.ToString());
            return 0;
        }

        private static uint GetKeyCode(Key keyCode, bool extended)
        {
            uint scanCode = MapVirtualKey((uint)keyCode, 0);
            var lParam = (0x00000001 | (scanCode << 16));
            if (extended)
            {
                lParam |= 0x01000000;
            }

            return lParam;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool PostMessage(IntPtr hWnd, uint message, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "DestroyWindow", CharSet = CharSet.Unicode)]
        private static extern bool DestroyWindow(IntPtr hwnd);

        private delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);
        [DllImport("user32.dll")]
        private static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);
    }
}