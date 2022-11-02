using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_Test.Model
{
    public class InputModel
    {
        public string EffectBlur { get; set; }
        public string EffectGrayscale { get; set; }
        public string Effect3 { get; set; } 
        public string Radius { get; set; }
        public string Size { get; set; }
        public string postFile { get; set; }
    }
}
