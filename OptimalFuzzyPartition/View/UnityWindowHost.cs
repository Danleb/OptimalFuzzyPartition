using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace OptimalFuzzyPartition.View
{
    public class UnityWindowHost : HwndHost
    {
        public AnonymousPipeServerStream PipeServerOut { get; private set; }
        public AnonymousPipeServerStream PipeServerIn { get; private set; }

        private readonly string UnityPlayerName = "FuzzyPartitionUnityProject.exe";

        private Process _unityPlayerProcess;

        private IntPtr _unityPlayerHwnd;

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            PipeServerOut = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);
            PipeServerIn = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);

            if (!File.Exists(UnityPlayerName))
            {
                MessageBox.Show("Unity player file not found.", "Fatal error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new FileNotFoundException();
            }

            var handle = hwndParent.Handle.ToInt32();

            _unityPlayerProcess = new Process
            {
                StartInfo =
                {
                    FileName = UnityPlayerName,
                    Arguments = "-parentHWND " + handle + " " + PipeServerIn.GetClientHandleAsString() + " " + PipeServerOut.GetClientHandleAsString(),
                    UseShellExecute = true,
                    CreateNoWindow = true
                }
            };

            _unityPlayerProcess.Start();

            _unityPlayerProcess.WaitForInputIdle();

            EnumChildWindows(hwndParent.Handle, WindowEnum, IntPtr.Zero);

            return new HandleRef(this, _unityPlayerHwnd);
        }

        private int WindowEnum(IntPtr hwnd, IntPtr lparam)
        {
            _unityPlayerHwnd = hwnd;
            Debug.Print(hwnd.ToString());
            return 0;
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            _unityPlayerProcess.CloseMainWindow();

            System.Threading.Thread.Sleep(1000);
            while (_unityPlayerProcess.HasExited == false)
                _unityPlayerProcess.Kill();

            DestroyWindow(hwnd.Handle);
        }

        //protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        //{
        //    handled = false;
        //    return IntPtr.Zero;
        //}

        //[DllImport("user32.dll", EntryPoint = "CreateWindowEx", CharSet = CharSet.Unicode)]
        //private static extern IntPtr CreateWindowEx(int dwExStyle,
        //                                              string lpszClassName,
        //                                              string lpszWindowName,
        //                                              int style,
        //                                              int x, int y,
        //                                              int width, int height,
        //                                              IntPtr hwndParent,
        //                                              IntPtr hMenu,
        //                                              IntPtr hInst,
        //                                              [MarshalAs(UnmanagedType.AsAny)] object pvParam);

        [DllImport("user32.dll", EntryPoint = "DestroyWindow", CharSet = CharSet.Unicode)]
        private static extern bool DestroyWindow(IntPtr hwnd);

        //[DllImport("User32.dll")]
        //private static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);

        private delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);
        [DllImport("user32.dll")]
        private static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);
    }
}