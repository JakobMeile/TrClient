using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Net.Http;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using System.ComponentModel;
using System.Windows.Media;
using System.Xml.Serialization;
using System.IO;
using TrClient;

namespace TrClient
{
    public class clsTrPairOrderID : IComparable
    {
        public int Order { get; set; }
        public string ID { get; set; }

        // constructor ved skabelse af ny region
        public clsTrPairOrderID(int InputOrder, string InputID)
        {
            Order = InputOrder;
            ID = InputID;
            
            // Debug.WriteLine($"New pair! Order = {Order}, ID = {ID}");
        }

        public int CompareTo(object obj)
        {
            var pair = obj as clsTrPairOrderID;
            return Order.CompareTo(pair.Order);
        }


    }
}
