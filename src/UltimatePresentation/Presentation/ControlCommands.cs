using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace UltimatePresentation.Presentation
{
        public static class ControlCommands
        {
                //static ControlCommands()
                //{
                //        DispatcherTimer dt = new DispatcherTimer
                //        {
                //                Interval = TimeSpan.FromMilliseconds(500)
                //        };
                //        dt.Tick += (sender, e) =>
                //        {
                //                CommandManager.InvalidateRequerySuggested();
                //        };
                //        dt.Start();
                //}
                private enum CommandId : byte
                {
                        GoBack,
                        GoForWard,
                        CloseDialog,
                        CloseFlyout,
                        CloseMenuContent,
                        ShowSoftKeyBoardWindow,
                        CloseSoftKeyBoardWindow,
                        /// <summary>
                        /// 计数标志
                        /// </summary>
                        Last
                }

                private static RoutedUICommand[] _internalCommands = new RoutedUICommand[(int)ControlCommands.CommandId.Last];

                #region CommandPropertys
                public static RoutedUICommand ShowSoftKeyBoardWindow
                {
                        get
                        {
                                return ControlCommands._EnsureCommand(ControlCommands.CommandId.ShowSoftKeyBoardWindow);
                        }
                }
                public static RoutedUICommand CloseSoftKeyBoardWindow
                {
                        get
                        {
                                return ControlCommands._EnsureCommand(ControlCommands.CommandId.CloseSoftKeyBoardWindow);
                        }
                }


                public static RoutedUICommand CloseMenuContent
                {
                        get
                        {
                                return ControlCommands._EnsureCommand(ControlCommands.CommandId.CloseMenuContent);
                        }
                }
                public static RoutedUICommand GoBack
                {
                        get
                        {
                                return ControlCommands._EnsureCommand(ControlCommands.CommandId.GoBack);
                        }
                }
                public static RoutedUICommand GoForWard
                {
                        get
                        {
                                return ControlCommands._EnsureCommand(ControlCommands.CommandId.GoForWard);
                        }
                }
                public static RoutedUICommand CloseDialog
                {
                        get
                        {
                                return ControlCommands._EnsureCommand(ControlCommands.CommandId.CloseDialog);
                        }
                }
                public static RoutedUICommand CloseFlyout
                {
                        get
                        {
                                return ControlCommands._EnsureCommand(ControlCommands.CommandId.CloseFlyout);
                        }
                }
                #endregion

                internal static string GetUIText(byte commandId)
                {
                        string result = string.Empty;
                        return result;
                }
                internal static InputGestureCollection LoadDefaultGestureFromResource(byte commandId)
                {
                        InputGestureCollection inputGestureCollection = new InputGestureCollection();
                        return inputGestureCollection;
                }
                private static RoutedUICommand _EnsureCommand(ControlCommands.CommandId idCommand)
                {
                        if (idCommand >= (ControlCommands.CommandId)0 &&
                                idCommand < ControlCommands.CommandId.Last)
                        {
                                lock (ControlCommands._internalCommands.SyncRoot)
                                {
                                        if (ControlCommands._internalCommands[(int)idCommand] == null)
                                        {
                                                RoutedUICommand routedUICommand = new RoutedUICommand(
                                                        ControlCommands.GetPropertyName(idCommand),
                                                        ControlCommands.GetPropertyName(idCommand),
                                                        typeof(ControlCommands));

                                                ControlCommands._internalCommands[(int)idCommand] = routedUICommand;
                                        }
                                }
                                return ControlCommands._internalCommands[(int)idCommand];
                        }
                        return null;
                }
                private static string GetPropertyName(ControlCommands.CommandId commandId)
                {
                        string result = string.Empty;
                        result = commandId.ToString();
                        return result;
                }
        }

}
