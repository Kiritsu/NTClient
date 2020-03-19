using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NTLauncher
{
    public static class InteropHelpers
    {
        [DllImport("user32")]
        public static extern bool BringWindowToTop(IntPtr handle);
    }
}
