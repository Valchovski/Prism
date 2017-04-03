﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace Prism
{
    class Prism
    {
        #region
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        #endregion

        private const int WH_KEYBOARD_LL = 13; 
        private const int WM_KEYDOWN = 0x0100; 
        private const int WM_KEYUP = 0x0101; 
        private static LowLevelKeyboardProc _proc = HookCallback; 
        private static IntPtr _hookID = IntPtr.Zero;
        private static bool CONTROL_DOWN = false;
        private static bool SHIFT_DOWN = false;

        public static void Main()
        {
            _hookID = SetHook(_proc);
            Application.Run();
        }

        private static Bitmap CaptureRegion()
        {
            Console.WriteLine("GOSSIP");
            return null;
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);          
                string theKey = ((Keys)vkCode).ToString();        
                Console.Write(theKey);                           
                if (theKey.Contains("ControlKey"))              
                {
                    CONTROL_DOWN = true;                         
                }
                else if (CONTROL_DOWN && theKey.Contains("ShiftKey"))
                {
                    SHIFT_DOWN = true;
                }
                else if (CONTROL_DOWN && SHIFT_DOWN && theKey == "F")        
                {
                    CaptureRegion();
                }
                else if (theKey == "Escape")                     
                {
                    UnhookWindowsHookEx(_hookID);                
                    Environment.Exit(0);                       
                }
            }
            else if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP)
            {
                int vkCode = Marshal.ReadInt32(lParam); 
                string theKey = ((Keys)vkCode).ToString(); 
                if (theKey.Contains("ControlKey"))          
                {
                    CONTROL_DOWN = false;
                }
                if (theKey.Contains("ShiftKey"))
                {
                    SHIFT_DOWN = false;
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
    }
}