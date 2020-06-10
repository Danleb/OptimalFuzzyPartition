using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using OptimalFuzzyPartitionAlgorithm;
using System.Windows;
using OptimalFuzzyPartition.ViewModel;

namespace OptimalFuzzyPartition
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class PartitionCreationWindow : Window
    {
        public PartitionCreationWindow(PartitionSettings partitionSettings)
        {
            InitializeComponent();
            DataContext = new PartitionCreationViewModel(partitionSettings, UnityWindowHost.PipeServerIn, UnityWindowHost.PipeServerOut);
		}

  //      [DllImport("User32.dll")]
		//static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);

		//internal delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);
		//[DllImport("user32.dll")]
		//internal static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);

		//[DllImport("user32.dll")]
		//static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

		//private Process process;
		//private IntPtr unityHWND = IntPtr.Zero;

		//private const int WM_ACTIVATE = 0x0006;
		//private readonly IntPtr WA_ACTIVE = new IntPtr(1);
		//private readonly IntPtr WA_INACTIVE = new IntPtr(0);

		//private void ActivateUnityWindow()
		//{
		//	SendMessage(unityHWND, WM_ACTIVATE, WA_ACTIVE, IntPtr.Zero);
		//}

		//private void DeactivateUnityWindow()
		//{
		//	SendMessage(unityHWND, WM_ACTIVATE, WA_INACTIVE, IntPtr.Zero);
		//}

		//private int WindowEnum(IntPtr hwnd, IntPtr lparam)
		//{
		//	unityHWND = hwnd;
		//	ActivateUnityWindow();
		//	return 0;
		//}

		//private void panel1_Resize(object sender, EventArgs e)
		//{
		//	MoveWindow(unityHWND, 0, 0, panel1.Width, panel1.Height, true);
		//	ActivateUnityWindow();
		//}

		//// Close Unity application
		//private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		//{
		//	try
		//	{
		//		process.CloseMainWindow();

		//		Thread.Sleep(1000);
		//		while (process.HasExited == false)
		//			process.Kill();
		//	}
		//	catch (Exception)
		//	{

		//	}
		//}

		//private void Form1_Activated(object sender, EventArgs e)
		//{
		//	ActivateUnityWindow();
		//}

		//private void Form1_Deactivate(object sender, EventArgs e)
		//{
		//	DeactivateUnityWindow();
		//}
	}
}