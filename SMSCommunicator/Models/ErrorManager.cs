using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SMSCommunicator.Models
{
    public sealed class ErrorManager
    {
        private static volatile ErrorManager instance;
        private static object _lock = new object();

        private List<string> Output { get; set; }

        //Event
        public event OutputMessageAdded OutputMessageEvent;
        public delegate void OutputMessageAdded(string output);
        public event EventHandler OutputMessageCleared;

        public ErrorManager()
        {
            Output = new List<string>();

        }

        public static ErrorManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (_lock)
                    {
                        if (instance == null)
                            instance = new ErrorManager();
                    }
                }

                return instance;
            }
        }

        public static void AddOutput(string output)
        {
            if (string.IsNullOrEmpty(output))
                return;

            ErrorManager.Instance.Output.Add(output);

            var handler = ErrorManager.Instance.OutputMessageEvent;
            if(handler != null)
                handler(output);
        }

        public static void AddOutput(Exception ex)
        {
            if (ex == null)
                return;

            ErrorManager.AddOutput(ex.Message);
            ErrorManager.TraverseInnerExceptions(ex);
        }

        public static void Clear()
        {
            ErrorManager.Instance.Output.Clear();

            var handler = ErrorManager.Instance.OutputMessageCleared;
            if(handler != null)
                handler(ErrorManager.Instance, new EventArgs());
        }

        private static void TraverseInnerExceptions(Exception ex)
        {
            if (ex == null)
                return;

            if (ex.InnerException == null)
                return;

            ErrorManager.AddOutput(ex.Message);
            TraverseInnerExceptions(ex.InnerException);
        }

    }
}
