using System.Runtime.InteropServices;

namespace Digst.OioIdws.Soap.Cross.Test.Utils
{
    /// <summary> 
    /// The structure contains a table of process IDs (PIDs) and the IPv4 TCP links that  
    /// are context bound to these PIDs. 
    /// </summary> 
    [StructLayout(LayoutKind.Sequential)]
    public struct MIB_TCPTABLE_OWNER_PID
    {
        public uint dwNumEntries;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct,
            SizeConst = 1)]
        public MIB_TCPROW_OWNER_PID[] table;
    }
}