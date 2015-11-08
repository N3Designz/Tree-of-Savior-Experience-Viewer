using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Caliburn.Micro;

namespace TOSExpViewer.Model
{
    /// <summary> Original code created by Excrulon @ https://bitbucket.org/Excrulon/tree-of-savior-experience-viewer/src. Minor modifications only. </summary>
    public class TosMonitor : PropertyChangedBase
    {
        private static readonly object syncLock = new object();
        
        private readonly int currentBaseExpAddress;

        private Process tosProcess = null;
        private IntPtr processId = IntPtr.Zero;

        private int bytesRead;
        private byte[] buffer = new byte[4];

        private IntPtr currentBaseExperienceAddress = IntPtr.Zero;
        private IntPtr requredBaseExperienceAddress = IntPtr.Zero; //0x4;
        private bool attached;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        public TosMonitor(int currentBaseExpAddress)
        {
            if (currentBaseExpAddress <= 0) throw new ArgumentOutOfRangeException(nameof(currentBaseExpAddress));
            this.currentBaseExpAddress = currentBaseExpAddress;
        }

        public bool Attached
        {
            get { return attached; }
            set
            {
                if (value == attached) return;
                attached = value;
                NotifyOfPropertyChange(() => Attached);
            }
        }
        
        /// <summary> Must attach to the process before making other calls </summary>
        public void Attach()
        {
            lock (syncLock)
            {
                if (Attached) return;

                const int processWmRead = 0x0010;

                Process[] processes = Process.GetProcessesByName("Client_tos");
                if (processes.Length <= 0) return;

                Attached = true;
                tosProcess = processes[0];
                for (int i = 1; i < processes.Length; i++)
                {
                    processes[i].Dispose();
                }

                tosProcess.EnableRaisingEvents = true;
                tosProcess.Exited += (obj, args) =>
                {
                    Attached = false;
                    Cleanup();
                };

                try
                {
                    processId = OpenProcess(processWmRead, false, tosProcess.Id);
                    //TODO: var errorCode = Marshal.GetLastWin32Error();

                    currentBaseExperienceAddress = GetCurrentBaseExperiencePtr(tosProcess);
                    requredBaseExperienceAddress = currentBaseExperienceAddress + 0x4;
                }
                catch
                {
                    Cleanup();
                    throw;
                }
            }
        }

        public int GetBaseExperience()
        {
            if (!Attached) return Int32.MinValue;

            ReadProcessMemory(processId, currentBaseExperienceAddress, buffer, buffer.Length, out bytesRead);
            if (bytesRead != 4)
                return Int32.MinValue;
            return BitConverter.ToInt32(buffer, 0);
        }

        public int GetRequiredExperience()
        {
            if (!Attached) return Int32.MinValue;

            ReadProcessMemory(processId, requredBaseExperienceAddress, buffer, buffer.Length, out bytesRead);
            if (bytesRead != 4)
                return Int32.MinValue;
            return BitConverter.ToInt32(buffer, 0);
        }

        private IntPtr GetCurrentBaseExperiencePtr(Process process)
        {
            if (!Attached) return IntPtr.Zero;

            var offsetList = new int[] { 0x10C };
            var buffer = new byte[4];
            var outBytes = 0;
            IntPtr currentAddress = new IntPtr(currentBaseExpAddress);

            ReadProcessMemory(process.Handle, currentAddress, buffer, buffer.Length, out outBytes);

            if (outBytes != 4)
            {
                Attached = false;
                Cleanup();
                return IntPtr.Zero;
            }

            Int32 value = BitConverter.ToInt32(buffer, 0);

            currentAddress = (IntPtr)value;

            for (int i = 0; i < offsetList.Length; i++)
            {
                currentAddress = IntPtr.Add(currentAddress, offsetList[i]);
                ReadProcessMemory(process.Handle, currentAddress, buffer, buffer.Length, out outBytes);

                if (outBytes != 4)
                {
                    Attached = false;
                    Cleanup();
                    return IntPtr.Zero;
                }

                value = BitConverter.ToInt32(buffer, 0);

                if (i != offsetList.Length - 1)
                {
                    currentAddress = (IntPtr)value;
                }
            }

            return currentAddress;
        }

        private void Cleanup()
        {
            tosProcess?.Dispose();
            tosProcess = null;
        }
    }
}
