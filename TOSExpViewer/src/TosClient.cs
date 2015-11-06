using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TOSExpViewer
{
    public class TosClient : IDisposable
    {
        private Process tosProcess = null;
        private IntPtr processId = IntPtr.Zero;

        int bytesRead = 0;
        byte[] buffer = new byte[4];

        const int PROCESS_WM_READ = 0x0010;

        IntPtr CURRENT_BASE_EXPERIENCE_ADDRESS = IntPtr.Zero;
        IntPtr REQURED_BASE_EXPERIENCE_ADDRESS = IntPtr.Zero; //0x4;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        public event EventHandler<EventArgs> ClientExitEvent;

        public void Attach()
        {
            Process[] processes = Process.GetProcessesByName("Client_tos");

            if (processes.Length > 0)
            {
                tosProcess = processes[0];
                for (int i = 1; i < processes.Length; i++)
                {
                    processes[i].Dispose();
                }
            }
            else
            {
                throw new SystemException("client_not_found");
            }

            tosProcess.EnableRaisingEvents = true;
            tosProcess.Exited += (obj, args) => {
                this.Dispose();
                if (ClientExitEvent != null)
                    ClientExitEvent(obj, args);
            };

            processId = OpenProcess(PROCESS_WM_READ, false, tosProcess.Id);
            //TODO: var errorCode = Marshal.GetLastWin32Error();

            CURRENT_BASE_EXPERIENCE_ADDRESS = GetCurrentBaseExperiencePtr(tosProcess);
            REQURED_BASE_EXPERIENCE_ADDRESS = CURRENT_BASE_EXPERIENCE_ADDRESS + 0x4;
        }

        private IntPtr GetCurrentBaseExperiencePtr(Process process)
        {
            var offsetList = new int[] { 0x10C };
            var buffer = new byte[4];
            var bytesRead = 0;
            IntPtr currentAddress = new IntPtr(0x01489F10);

            ReadProcessMemory(process.Handle, currentAddress, buffer, buffer.Length, out bytesRead);

            if (bytesRead != 4)
                throw new SystemException("failed_to_read_client_memory");

            Int32 value = BitConverter.ToInt32(buffer, 0);

            currentAddress = (IntPtr)value;

            for (int i = 0; i < offsetList.Length; i++)
            {
                currentAddress = IntPtr.Add(currentAddress, offsetList[i]);
                ReadProcessMemory(process.Handle, currentAddress, buffer, buffer.Length, out bytesRead);

                if (bytesRead != 4)
                    throw new SystemException("failed_to_read_client_memory");

                value = BitConverter.ToInt32(buffer, 0);

                if (i != offsetList.Length - 1)
                {
                    currentAddress = (IntPtr)value;
                }
            }

            return currentAddress;
        }
    
        public int GetBaseExperience()
        {
            ReadProcessMemory(processId, (IntPtr)CURRENT_BASE_EXPERIENCE_ADDRESS, buffer, buffer.Length, out bytesRead);
            if (bytesRead != 4)
                return Int32.MinValue;
            return BitConverter.ToInt32(buffer, 0);
        }

        public int GetRequiredExperience()
        {
            ReadProcessMemory(processId, (IntPtr)REQURED_BASE_EXPERIENCE_ADDRESS, buffer, buffer.Length, out bytesRead);
            if (bytesRead != 4)
                return Int32.MinValue;
            return BitConverter.ToInt32(buffer, 0);
        }

        public void Dispose()
        {
            if (tosProcess != null)
            {
                tosProcess.Dispose();
                tosProcess = null;
            }
        }
    }
}
