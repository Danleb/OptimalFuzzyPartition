using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace OptimalFuzzyPartition.View
{
    public class UnityWindowHost : HwndHost
    {
        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            return new HandleRef();
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            
        }
    }
}