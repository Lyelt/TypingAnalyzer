using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Diagnostics;
using TypingAnalyzer.Interfaces;
using System.Windows.Forms;
using System.Linq;

namespace TypingAnalyzer.Keyboard
{
    public class GlobalKeyboardHook : IGlobalKeyboardHook, IDisposable
    {
        public event EventHandler<GlobalKeyboardHookEventArgs> KeyboardPressed;

        public const int WH_KEYBOARD_LL = 13;
        public const int VkSnapshot = 0x2c;
        public const int KfAltdown = 0x2000;
        public const int LlkhfAltdown = (KfAltdown >> 8);
        private IntPtr _windowsHookHandle;
        private IntPtr _user32LibraryHandle;
        private HookProc _hookProc;

        public GlobalKeyboardHook()
        {
            _windowsHookHandle = IntPtr.Zero;
            _user32LibraryHandle = IntPtr.Zero;
            _hookProc = LowLevelKeyboardProc; // we must keep alive _hookProc, because GC is not aware about SetWindowsHookEx behaviour.
            _user32LibraryHandle = LoadLibrary("User32");

            if (_user32LibraryHandle == IntPtr.Zero)
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode, $"Failed to load library 'User32.dll'. Error {errorCode}: {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
            }

            _windowsHookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, _hookProc, _user32LibraryHandle, 0);

            if (_windowsHookHandle == IntPtr.Zero)
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode, $"Failed to adjust keyboard hooks for '{Process.GetCurrentProcess().ProcessName}'. Error {errorCode}: {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
            }
        }

        public IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            bool fEatKeyStroke = false;

            var wparamTyped = wParam.ToInt32();
            if (Enum.IsDefined(typeof(KeyboardState), wparamTyped))
            {
                object o = Marshal.PtrToStructure(lParam, typeof(LowLevelKeyboardInputEvent));
                LowLevelKeyboardInputEvent p = (LowLevelKeyboardInputEvent)o;
                KeyboardState state = (KeyboardState)wparamTyped;

                string keyData = GetCharsFromKeys((uint)p.VirtualCode, (Control.ModifierKeys & Keys.Shift) == Keys.Shift);
                var eventArguments = new GlobalKeyboardHookEventArgs(p.VirtualCode, keyData, state);
                KeyboardPressed?.Invoke(this, eventArguments);
                fEatKeyStroke = eventArguments.Handled;
            }

            return fEatKeyStroke ? (IntPtr)1 : CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        private string GetCharsFromKeys(uint vKeyCode, bool shift)
        {
            var buf = new StringBuilder(256);
            var keyboardState = new byte[256];

            if (shift)
                keyboardState[(int)Keys.ShiftKey] = 0xff;

            ToUnicode(vKeyCode, 0, keyboardState, buf, 256, 0);
            return buf.ToString();
        }

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // because we can unhook only in the same thread, not in garbage collector thread
                if (_windowsHookHandle != IntPtr.Zero)
                {
                    if (!UnhookWindowsHookEx(_windowsHookHandle))
                    {
                        int errorCode = Marshal.GetLastWin32Error();
                        throw new Win32Exception(errorCode, $"Failed to remove keyboard hooks for '{Process.GetCurrentProcess().ProcessName}'. Error {errorCode}: {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
                    }
                    _windowsHookHandle = IntPtr.Zero;

                    // ReSharper disable once DelegateSubtraction
                    _hookProc -= LowLevelKeyboardProc;
                }
            }

            if (_user32LibraryHandle != IntPtr.Zero)
            {
                if (!FreeLibrary(_user32LibraryHandle)) // reduces reference to library by 1.
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new Win32Exception(errorCode, $"Failed to unload library 'User32.dll'. Error {errorCode}: {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
                }
                _user32LibraryHandle = IntPtr.Zero;
            }
        }

        ~GlobalKeyboardHook()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Win32
        delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// The SetWindowsHookEx function installs an application-defined hook procedure into a hook chain.
        /// You would install a hook procedure to monitor the system for certain types of events. These events are
        /// associated either with a specific thread or with all threads in the same desktop as the calling thread.
        /// </summary>
        /// <param name="idHook">hook type</param>
        /// <param name="lpfn">hook procedure</param>
        /// <param name="hMod">handle to application instance</param>
        /// <param name="dwThreadId">thread identifier</param>
        /// <returns>If the function succeeds, the return value is the handle to the hook procedure.</returns>
        [DllImport("USER32", SetLastError = true)]
        static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);

        /// <summary>
        /// The UnhookWindowsHookEx function removes a hook procedure installed in a hook chain by the SetWindowsHookEx function.
        /// </summary>
        /// <param name="hhk">handle to hook procedure</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("USER32", SetLastError = true)]
        public static extern bool UnhookWindowsHookEx(IntPtr hHook);

        /// <summary>
        /// The CallNextHookEx function passes the hook information to the next hook procedure in the current hook chain.
        /// A hook procedure can call this function either before or after processing the hook information.
        /// </summary>
        /// <param name="hHook">handle to current hook</param>
        /// <param name="code">hook code passed to hook procedure</param>
        /// <param name="wParam">value passed to hook procedure</param>
        /// <param name="lParam">value passed to hook procedure</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("USER32", SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hHook, int code, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("user32.dll")]
        private static extern int MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        public static extern int ToUnicode(uint virtualKeyCode, uint scanCode, byte[] keyboardState, [Out, MarshalAs(UnmanagedType.LPWStr, SizeConst = 64)]StringBuilder receivingBuffer, int bufferSize, uint flags);
        #endregion
    }
}
