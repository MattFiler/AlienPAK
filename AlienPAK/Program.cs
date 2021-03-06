﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlienPAK
{
    public static class SharedData
    {
        public static string pathToAI = "";
    }
    public enum AlienContentType { TEXTURE, MODEL, UI, SCRIPT, ANIMATION, NONE_SPECIFIED };

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //Need DLLs in directory for image previews to work :(
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DirectXTexNet.dll")) File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + "DirectXTexNet.dll", Properties.Resources.DirectXTexNet);
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "x64");
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "x64/DirectXTexNetImpl.dll")) File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + "x64/DirectXTexNetImpl.dll", Properties.Resources.DirectXTexNetImpl_64);

            //Set paths
            if (args.Length > 0 && args[0] == "-opencage") for (int i = 1; i < args.Length; i++) SharedData.pathToAI += args[i] + " ";
            else SharedData.pathToAI = Environment.CurrentDirectory + " ";
            SharedData.pathToAI = SharedData.pathToAI.Substring(0, SharedData.pathToAI.Length - 1);

            //Verify location
            if (args.Length != 0 && args[0] == "-opencage" && !File.Exists(SharedData.pathToAI + "/AI.exe")) throw new Exception("This tool was launched incorrectly, or was not placed within the Alien: Isolation directory.");

            //Add font resources for use
            FontManager.AddFont(Properties.Resources.Isolation_Isolation);
            FontManager.AddFont(Properties.Resources.JixellationBold_Jixellation);
            FontManager.AddFont(Properties.Resources.NostromoBoldCond_Nostromo_Cond);

            //Launch application
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length != 0 && args[0] == "-opencage") Application.Run(new Landing());
            else Application.Run(new Explorer(args, AlienContentType.NONE_SPECIFIED));
        }
    }
}
