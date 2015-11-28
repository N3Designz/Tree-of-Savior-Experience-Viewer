using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Caliburn.Micro;
using System.Configuration;

namespace TOSExpViewer.Model
{
    public class TosMonitor : PropertyChangedBase
    {
        private static readonly object syncLock = new object();
        
        private readonly int currentBaseExpAddress;

        private Process tosProcess = null;
        private IntPtr processId = IntPtr.Zero;

        private int bytesRead;

        private IntPtr currentBaseExperienceAddress = IntPtr.Zero;
        private IntPtr requredBaseExperienceAddress = IntPtr.Zero; //0x4;

        private IntPtr currentClassExperienceAddress = IntPtr.Zero;

        private IntPtr currentClassTierAddress = IntPtr.Zero;

        private bool attached;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        public TosMonitor(int currentBaseExpAddress, int currentClassExperienceAddress, int currentClassTierAddress)
        {
            if (currentBaseExpAddress <= 0 || currentClassExperienceAddress <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(currentBaseExpAddress) + " " + nameof(currentClassExperienceAddress));
            }

            this.currentBaseExpAddress = currentBaseExpAddress;
            this.currentClassExperienceAddress = new IntPtr(currentClassExperienceAddress);
            this.currentClassTierAddress = new IntPtr(currentClassTierAddress);
        }

        public bool Attached
        {
            get { return attached; }
            set
            {
                if (value == attached)
                {
                    return;
                }
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

        public int getIntAddress(IntPtr address, int bytes)
        {
            byte[] buffer = new byte[4];

            if (!Attached)
            {
                return Int32.MinValue;
            }

            bytesRead = 0;
            ReadProcessMemory(processId, address, buffer, buffer.Length, out bytesRead);

            if (bytesRead != bytes)
            {
                return Int32.MinValue;
            }
            
            return BitConverter.ToInt32(buffer, 0);
        }

        public int getShortAddress(IntPtr address, int bytes)
        {
            byte[] buffer = new byte[4];

            if (!Attached)
            {
                return Int32.MinValue;
            }

            bytesRead = 0;
            ReadProcessMemory(processId, address, buffer, bytes, out bytesRead);

            if (bytesRead != bytes)
            {
                return Int32.MinValue;
            }


            return (Int32)(BitConverter.ToInt16(buffer, 0));
        }

        public int GetCurrentBaseExperience()
        {
            return getIntAddress(currentBaseExperienceAddress, 4);
        }

        public int GetClassTier()
        {
            return getShortAddress(currentClassTierAddress, 2) - 1;
        }

        public int GetRequiredExperience()
        {
            return getIntAddress(requredBaseExperienceAddress, 4);
        }

        public int GetCurrentClassExperience()
        {
            int offset = GetCurrentClassExperienceOffset();

            IntPtr currentClassExperienceBase = (IntPtr)getIntAddress(currentClassExperienceAddress, 4);
            IntPtr newAddress = IntPtr.Add(currentClassExperienceBase, offset);

            int value = getIntAddress(newAddress, 4);

            return value;
        }

        public int GetCurrentClassExperienceOffset()
        {
            int classTier = GetClassTier();

            /*
            esi = 0x1
            0x1 * 0x8 = 0x8
            ecx = 0x8 - 0x1 = 0x7
            0x7 * 4 + 0xC = 0x28
            0x28 + 0x8 = 30
            */

            int offset = classTier;
            offset *= 0x8;
            offset -= classTier;
            offset *= 4;
            offset += 0xC;
            offset += 0x8;

            return offset;
        }

        private IntPtr GetCurrentBaseExperiencePtr(Process process)
        {
            if (!Attached)
            {
                return IntPtr.Zero;
            }

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
