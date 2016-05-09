﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace ABW_Project
{
    class Log
    {
        static StreamWriter sw;

        public static void Init()
        {
            sw = new StreamWriter("log.txt");
        }

        public static void Add(string txt, bool date = true)
        {
            if (date)
            {
                DateTime date1 = DateTime.Now;
                sw.Write("{0} ",date1);
            }
            sw.WriteLine(txt);
        }

        public static void Close()
        {
            sw.Close();
        }
    }
}
