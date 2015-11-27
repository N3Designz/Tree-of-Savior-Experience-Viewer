using EasyHook;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TOSExpViewer.Core;

namespace LuaInjector
{
    public class Main : EasyHook.IEntryPoint
    {

        static string ChannelName;

        private LuaInterface luaInterface;

        public Main(RemoteHooking.IContext InContext, String InChannelName)
        {
            Console.WriteLine("test");
            luaInterface = RemoteHooking.IpcConnectClient<LuaInterface>(InChannelName);
            ChannelName = InChannelName;
            luaInterface.WriteLine("get current process id: " + RemoteHooking.GetCurrentProcessId());
            luaInterface.WriteLine("end main()");
        }

        public void Run(RemoteHooking.IContext InContext, String InChannelName)
        {
            luaInterface.WriteLine("run!");
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        delegate int Direct3D9Device_EndSceneDelegate(IntPtr device);

        protected IntPtr[] GetVTblAddresses(IntPtr pointer, int numberOfMethods)
        {
            return GetVTblAddresses(pointer, 0, numberOfMethods);
        }

        LocalHook Direct3DDevice_EndSceneHook = null;

        protected IntPtr[] GetVTblAddresses(IntPtr pointer, int startIndex, int numberOfMethods)
        {
            List<IntPtr> vtblAddresses = new List<IntPtr>();

            IntPtr vTable = Marshal.ReadIntPtr(pointer);
            for (int i = startIndex; i < startIndex + numberOfMethods; i++)
            {
                vtblAddresses.Add(Marshal.ReadIntPtr(vTable, i * IntPtr.Size)); // using IntPtr.Size allows us to support both 32 and 64-bit processes
            }

            return vtblAddresses.ToArray();
        }
    }
}
