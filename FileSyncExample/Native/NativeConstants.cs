using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSyncExample.Native
{
    public  class NativeConstants
    {
        public const uint GENERIC_WRITE = 1073741824;
        public const uint GENERIC_READ = 0x80000000;
        public const int FILE_SHARE_DELETE = 4;
        public const int FILE_SHARE_WRITE = 2;
        public const int FILE_SHARE_READ = 1;

        public const int OPEN_ALWAYS = 4;
        public const int CREATE_ALWAYS = 2;
        public const int CREATE_NEW = 1;
    }
}
