using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCompany.Models
{
    public class Chart
    {
        public string[] labels { get; set; }
        public List<Datasets> datasets { get; set; }
    }

    public class Datasets
    {
        public string label { get; set; }
        public int data { get; set; }
        public string backgroundColor { get; set; }
        public string borderColor { get; set; }
        public int borderWidth { get; set; }
        

    }
}
