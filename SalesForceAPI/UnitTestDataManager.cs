﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog;

namespace SalesForceAPI
{
    public class UnitTestDataManager
    {
        private static bool _unitTestDataManagerEnabled;

        private static string TempPath => ConnectionUtil.Session?.CatchLocation?.FullName ?? Path.GetTempPath();

        private static string IdFileName => Path.Combine(TempPath, "ApexSharp.UnitTestDataManager.id.txt");

        public static void UnitTestDataManagerOn()
        {
            _unitTestDataManagerEnabled = true;
            RemoveAllIds();
        }

        public static void UnitTestDataManagerOff()
        {
            RemoveAllIds();
            _unitTestDataManagerEnabled = false;
        }

        public static void AddId(string id)
        {
            Console.WriteLine(id);
            if (_unitTestDataManagerEnabled)
            {
                List<string> idList = new List<string>();

                FileInfo dataFile = new FileInfo(IdFileName);

                if (dataFile.Exists)
                {
                    idList = File.ReadAllLines(dataFile.FullName).ToList();
                }
                idList.Add(id);

                File.WriteAllLines(dataFile.FullName, idList);
            }
        }

        public static void RemoveId(string id)
        {
            if (_unitTestDataManagerEnabled)
            {
                List<string> idList = new List<string>();

                FileInfo dataFile = new FileInfo(IdFileName);
                if (dataFile.Exists)
                {
                    idList = File.ReadAllLines(dataFile.FullName).ToList();
                    idList.Remove(id);
                    File.WriteAllLines(dataFile.FullName, idList);
                }
            }
        }

        public static void RemoveAllIds()
        {
            Log.Logger.Information("Cleaning All Values");

            FileInfo dataFile = new FileInfo(IdFileName);
            if (dataFile.Exists)
            {
                var idList = File.ReadAllLines(dataFile.FullName).ToList();
                idList.Clear();
                File.WriteAllLines(dataFile.FullName, idList);
            }

        }

        public static int IdCount()
        {
            List<string> idList = new List<string>();
            FileInfo dataFile = new FileInfo(IdFileName);
            if (dataFile.Exists)
            {
                idList = File.ReadAllLines(dataFile.FullName).ToList();
            }
            return idList.Count;
        }

    }
}