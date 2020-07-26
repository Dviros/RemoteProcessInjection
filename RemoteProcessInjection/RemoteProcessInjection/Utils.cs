using RemoteProcessInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace RemoteProcessInjection
{
    class Utils
    {
        public static Int32 ErrorMsg()
        {
            Win32Exception errorMessage = new Win32Exception(Marshal.GetLastWin32Error());
            Logger.Print(Logger.STATUS.ERROR, String.Format("{0} (Error Code: {1})", errorMessage.Message, errorMessage.NativeErrorCode.ToString()));
            return (Int32)errorMessage.NativeErrorCode;
        }
        public static Dictionary<String, String> GetArgs(string[] args)
        {
            Dictionary<String, String> Arguments = new Dictionary<String, String>();
            if (args.Length == 2)
            {
                Arguments.Add("Host", args[0]);
                Arguments.Add("Username", null);
                Arguments.Add("Password", null);
                Arguments.Add("FilePath", args[1]);
                Arguments.Add("Share", null);

            }
            else if (args.Length == 3)
            {
                Arguments.Add("Host", args[0]);
                Arguments.Add("Username", null);
                Arguments.Add("Password", null);
                Arguments.Add("FilePath", args[1]);
                Arguments.Add("Share", args[2]);

            }
            else if (args.Length == 4)
            {
                Arguments.Add("Host", args[0]);
                Arguments.Add("Username", args[1]);
                Arguments.Add("Password", args[2]);
                Arguments.Add("FilePath", args[3]);
                Arguments.Add("Share", null);
            }
            else if (args.Length == 5)
            {
                Arguments.Add("Host", args[0]);
                Arguments.Add("Username", args[1]);
                Arguments.Add("Password", args[2]);
                Arguments.Add("FilePath", args[3]);
                Arguments.Add("Share", args[4]);
            }
            return Arguments;
        }
        public static String GetCurrentUser()
        {
            string Username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            return Username;
        }
        public static void Help()
        {
            Console.WriteLine(@"[*] Usage: .\CSharpExec.exe <Host> [<Username>] [<Password>] <Path2Exe>");
            Console.WriteLine(@"[*] Example 1: .\CSharpExec.exe 192.168.0.1 Administrator Password123! c:\beacon-svc.exe");
            Console.WriteLine(@"[*] Example 2: .\CSharpExec.exe 192.168.0.1 Administrator Password123! c:\beacon-svc.exe C$");
            Console.WriteLine(@"[*] Example 3: .\CSharpExec.exe 192.168.0.1 c:\beacon-svc.exe");
            Console.WriteLine(@"[*] Example 4: .\CSharpExec.exe 192.168.0.1 c:\beacon-svc.exe C$");
            Environment.Exit(1);
        }
    }
}
