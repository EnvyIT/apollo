using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;

namespace Apollo.Terminal.Common
{
    public static class OnScreenHelper
    {
        private static readonly string OnScreenKeyboardCommand;
        private static readonly Subject<Tuple<UIElement, bool>> FocusSubject = new Subject<Tuple<UIElement, bool>>();
        private static readonly List<Type> UiElements = new List<Type>();
        private static Process _currentProcess;

        static OnScreenHelper()
        {
            OnScreenKeyboardCommand = LoadOnScreenKeyboardCommand();
            AutomateTabTipOpen(FocusSubject.AsObservable());
            AutomateTabTipClose(FocusSubject.AsObservable());
        }

        public static void BindTo<T>() where T : UIElement
        {
            if (UiElements.Contains(typeof(T)))
                return;

            EventManager.RegisterClassHandler(
                typeof(T),
                UIElement.TouchDownEvent,
                new RoutedEventHandler((s, e) =>
                {
                    if (((UIElement) s).IsFocused)
                        FocusSubject.OnNext(new Tuple<UIElement, bool>((UIElement) s, true));
                }),
                true);

            EventManager.RegisterClassHandler(
                typeof(T),
                UIElement.GotFocusEvent,
                new RoutedEventHandler((s, e) => FocusSubject.OnNext(new Tuple<UIElement, bool>((UIElement) s, true))),
                true);

            EventManager.RegisterClassHandler(
                typeof(T),
                UIElement.LostFocusEvent,
                new RoutedEventHandler((s, e) => FocusSubject.OnNext(new Tuple<UIElement, bool>((UIElement) s, false))),
                true);

            UiElements.Add(typeof(T));
        }

        private static void AutomateTabTipClose(IObservable<Tuple<UIElement, bool>> focusObservable)
        {
            focusObservable
                .ObserveOn(Scheduler.Default)
                .Throttle(TimeSpan.FromMilliseconds(100)) // Close only if no other UIElement got focus in 100 ms
                .Where(tuple => tuple.Item2 == false)
                .Do(_ => CloseKeyboard())
                .Subscribe(_ => IsKeyboardClosed());
        }

        private static void AutomateTabTipOpen(IObservable<Tuple<UIElement, bool>> focusObservable)
        {
            focusObservable
                .ObserveOn(Scheduler.Default)
                .Where(tuple => tuple.Item2 == true)
                .Do(_ => OpenKeyboard())
                .Subscribe(tuple => OnKeyboardOpened());
        }

        private static void OpenKeyboard()
        {
            if (_currentProcess != null)
            {
                return;
            }

            try
            {
                _currentProcess = Process.Start(new ProcessStartInfo
                {
                    FileName = OnScreenKeyboardCommand,
                    Verb = "runas", // UAC prompt
                    UseShellExecute = true,
                });

                PoolingTimer.PoolUntilTrue(
                    IsKeyboardClosed,
                    OnKeyboardClosed,
                    TimeSpan.FromMilliseconds(700),
                    TimeSpan.FromMilliseconds(50));
            }
            catch (Exception)
            {
                // Ignore.
            }
        }

        private static void CloseKeyboard()
        {
            try
            {
                _currentProcess?.Kill();
            }
            catch (Exception)
            {
                // Ignore.
            }
            finally
            {
                _currentProcess = null;
            }
        }

        private static void OnKeyboardOpened()
        {
            // nothing to do.
        }

        private static bool IsKeyboardClosed()
        {
            return _currentProcess == null || _currentProcess.HasExited;
        }

        private static void OnKeyboardClosed()
        {
            // nothing to do.
        }

        private static class PoolingTimer
        {
            private static bool _isPooling;

            internal static void PoolUntilTrue(Func<bool> poolingFunc, Action callback, TimeSpan dueTime,
                TimeSpan period)
            {
                if (_isPooling) return;

                _isPooling = true;

                Observable.Timer(dueTime, period)
                    .Select(_ => poolingFunc())
                    .TakeWhile(stop => stop != true)
                    .Where(stop => stop)
                    .Finally(() => _isPooling = false)
                    .Subscribe(
                        _ => { },
                        callback);
            }
        }

        private static string LoadOnScreenKeyboardCommand()
        {
            var windowsDir = Environment.GetEnvironmentVariable("WINDIR");
            if (windowsDir == null)
            {
                throw new ArgumentNullException(nameof(windowsDir));
            }

            var onScreenKeyboard = Path.Combine(Path.Combine(windowsDir, "sysnative"), "osk.exe");
            if (!File.Exists(onScreenKeyboard))
            {
                onScreenKeyboard = null;
            }

            if (onScreenKeyboard == null)
            {
                onScreenKeyboard = Path.Combine(Path.Combine(windowsDir, "system32"), "osk.exe");
                if (!File.Exists(onScreenKeyboard))
                {
                    onScreenKeyboard = null;
                }
            }

            return onScreenKeyboard ?? "osk.exe";
        }
    }
}