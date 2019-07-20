using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OdevToplamaProjesi.Web.Models
{
    public class OdevGuncellemeModel
    {
        public int odevID { get; set; }
        public string odevBasligi { get; set; }
        public string odevAciklama { get; set; }
        public string  odevTarihi { get; set; }
    }
}