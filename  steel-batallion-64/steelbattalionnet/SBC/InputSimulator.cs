﻿using System;
using System.Runtime.InteropServices;
using Microsoft.DirectX.DirectInput;
//http://damiproductions.darkbb.com/t504-c-sending-keys-with-the-sendinput-api

namespace SBC
{
    internal class InputSimulator
    {
        #region Imports

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        public static extern short VkKeyScan(char ch);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        #endregion

        public static void SimulateKeyDown(Microsoft.DirectX.DirectInput.Key key)
        {
            INPUT structure = new INPUT();
            structure.type = (int)InputType.INPUT_KEYBOARD;
            structure.ki.wScan = (short)key;
            structure.ki.dwFlags = (int)((int)KEYEVENTF.KEYDOWN | (uint)KEYEVENTF.SCANCODE);
            structure.ki.dwExtraInfo = GetMessageExtraInfo();

            INPUT[] pInputs = new INPUT[] { structure };

            SendInput(1, pInputs, Marshal.SizeOf(structure));
        }

        public static void SimulateKeyUp(Microsoft.DirectX.DirectInput.Key key)
        {
            INPUT structure = new INPUT();
            structure.type = (int)InputType.INPUT_KEYBOARD;
            structure.ki.wScan = (short)key;
            structure.ki.dwFlags = (int)((int)KEYEVENTF.KEYUP | (uint)KEYEVENTF.SCANCODE);
            structure.ki.dwExtraInfo = GetMessageExtraInfo();

            INPUT[] pInputs = new INPUT[] { structure };

            SendInput(1, pInputs, Marshal.SizeOf(structure));
        }

        public static void SimulateKeyPress(Microsoft.DirectX.DirectInput.Key key)
        {
            INPUT structure = new INPUT();
            structure.type = (int)InputType.INPUT_KEYBOARD;
            structure.ki.wScan = (short)key;
            structure.ki.dwFlags = (int)((int)KEYEVENTF.KEYDOWN | (uint)KEYEVENTF.SCANCODE);
            structure.ki.dwExtraInfo = GetMessageExtraInfo();

            INPUT input2 = new INPUT();
            input2.type = (int)InputType.INPUT_KEYBOARD;
            input2.ki.wScan = (short)key;//changed this line
            input2.mi.dwFlags = (int)((int)KEYEVENTF.KEYUP | (uint)KEYEVENTF.SCANCODE);
            input2.ki.dwExtraInfo = GetMessageExtraInfo();

            INPUT[] pInputs = new INPUT[] { structure, input2 };

            SendInput(2, pInputs, Marshal.SizeOf(structure));
        }

        public static void SimulateModifiedKeyStroke(Microsoft.DirectX.DirectInput.Key modifier, Microsoft.DirectX.DirectInput.Key keycode)
        {
            INPUT structure = new INPUT();
            structure.type = (int)InputType.INPUT_KEYBOARD;
            structure.ki.wScan = (short)modifier;
            structure.ki.dwFlags = (int)((int)KEYEVENTF.KEYDOWN | (uint)KEYEVENTF.SCANCODE);
            structure.ki.dwExtraInfo = GetMessageExtraInfo();

            INPUT input2 = new INPUT();
            input2.type = (int)InputType.INPUT_KEYBOARD;
            input2.ki.wScan = (short)keycode;//changed this line,input2 was structure before
            input2.mi.dwFlags = (int)((int)KEYEVENTF.KEYDOWN | (uint)KEYEVENTF.SCANCODE);
            input2.ki.dwExtraInfo = GetMessageExtraInfo();

            INPUT structure2 = new INPUT();
            structure.type = (int)InputType.INPUT_KEYBOARD;
            structure.ki.wScan = (short)keycode;
            structure.ki.dwFlags = (int)((int)KEYEVENTF.KEYUP | (uint)KEYEVENTF.SCANCODE);
            structure.ki.dwExtraInfo = GetMessageExtraInfo();

            INPUT input3 = new INPUT();
            input2.type = (int)InputType.INPUT_KEYBOARD;
            structure.ki.wScan = (short)modifier;
            input2.mi.dwFlags = (int)((int)KEYEVENTF.KEYUP | (uint)KEYEVENTF.SCANCODE);
            input2.ki.dwExtraInfo = GetMessageExtraInfo();

            INPUT[] pInputs = new INPUT[] { structure, input2, structure2, input3 };

            SendInput(4, pInputs, Marshal.SizeOf(structure));
        }

        

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUT
        {
            [FieldOffset(4)]
            public HARDWAREINPUT hi;
            [FieldOffset(4)]
            public KEYBDINPUT ki;
            [FieldOffset(4)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public int type;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public short wVk;
            public short wScan;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        [Flags]
        public enum InputType
        {
            INPUT_MOUSE = 0,
            INPUT_KEYBOARD = 1,
            INPUT_HARDWARE = 2
        }

        [Flags]
        public enum MOUSEEVENTF
        {
            MOVE = 0x0001, /* mouse move */
            LEFTDOWN = 0x0002, /* left button down */
            LEFTUP = 0x0004, /* left button up */
            RIGHTDOWN = 0x0008, /* right button down */
            RIGHTUP = 0x0010, /* right button up */
            MIDDLEDOWN = 0x0020, /* middle button down */
            MIDDLEUP = 0x0040, /* middle button up */
            XDOWN = 0x0080, /* x button down */
            XUP = 0x0100, /* x button down */
            WHEEL = 0x0800, /* wheel button rolled */
            MOVE_NOCOALESCE = 0x2000, /* do not coalesce mouse moves */
            VIRTUALDESK = 0x4000, /* map to entire virtual desktop */
            ABSOLUTE = 0x8000 /* absolute move */
        }

        [Flags]
        public enum KEYEVENTF
        {
            KEYDOWN = 0,
            EXTENDEDKEY = 0x0001,
            KEYUP = 0x0002,
            UNICODE = 0x0004,
            SCANCODE = 0x0008,
        }
    }
}