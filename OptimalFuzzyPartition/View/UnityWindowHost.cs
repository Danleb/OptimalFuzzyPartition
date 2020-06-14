using SimpleTCP;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
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

        public UnityWindowHost()
        {
            port = _nextPortNumber++;
            SimpleTcpServer = new SimpleTcpServer().Start(port);
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
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

        private int WindowEnum(IntPtr hwnd, IntPtr lparam)
        {
            _unityPlayerHwnd = hwnd;
            Debug.Print(hwnd.ToString());
            return 0;
        }

        private bool destroyed = false;

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
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

        ~UnityWindowHost()
        {
            if (destroyed) return;

            Destroy();
        }

        [DllImport("user32.dll", EntryPoint = "DestroyWindow", CharSet = CharSet.Unicode)]
        private static extern bool DestroyWindow(IntPtr hwnd);

        private delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);
        [DllImport("user32.dll")]
        private static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);
    }
}