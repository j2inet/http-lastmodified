using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FileSyncExample.Native
{
    public partial class NativeMethods
    {
        [DllImportAttribute("kernel32.dll", EntryPoint = "CreateFileW")]
        public static extern System.IntPtr CreateFileW(
        [InAttribute()][MarshalAsAttribute(UnmanagedType.LPWStr)] string lpFileName,
        uint dwDesiredAccess,
        uint dwShareMode,
        [InAttribute()] System.IntPtr lpSecurityAttributes,
        uint dwCreationDisposition,
        uint dwFlagsAndAttributes,
        [InAttribute()] System.IntPtr hTemplateFile
    );

        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);
    }
}
