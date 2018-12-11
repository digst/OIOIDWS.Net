using System.Runtime.InteropServices;

namespace Digst.OioIdws.Soap.Cross.Test.Utils
{
    /// <summary> 
    /// The structure contains information that describes an IPv4 TCP connection with  
    /// IPv4 addresses, ports used by the TCP connection, and the specific process ID 
    /// (PID) associated with connection. 
    /// </summary> 
    [StructLayout(LayoutKind.Sequential)]
    public struct MIB_TCPROW_OWNER_PID
    {
        public MibTcpState state;
        public uint localAddr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] localPort;
        public uint remoteAddr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] remotePort;
        public int owningPid;
    }
}