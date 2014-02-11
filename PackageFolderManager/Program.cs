using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SMSCommunicator.Models;
using SMSCommunicator.Controllers;

namespace PackageFolderManager
{
    class Program
    {
        static void Main(string[] args)
        {
            //arg1 = path where mapping file is located (string)
            //arg2 = output logging path

            string mappingPath = String.Empty;
            string loggingPath = String.Empty;

            if (args.Length > 0)
            {
                mappingPath = args[0];
                if (args.Length > 1)
                    loggingPath = args[1];
            }
            else
            {
                Console.WriteLine("Missing mapping file location.");
                return;
            }

            PackageFolderManager pfm = new PackageFolderManager(mappingPath, loggingPath);

            try
            {
                pfm.Run();
            }
            catch (Exception)
            {
                pfm.Close();
                return;
            }

            pfm.Close();
        }
    }
}
