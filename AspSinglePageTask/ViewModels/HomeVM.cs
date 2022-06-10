using AspSinglePageTask.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspSinglePageTask.ViewModels
{
    public class HomeVM
    {
        public List<Slider> sliders{ get; set; }
        public List<Portfolio> portfolios { get; set; }
        public List<About> about { get; set; }
    }
}
