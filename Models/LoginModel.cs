using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OdevToplamaProjesi.Web.Models
{
    public class LoginModel
    {
        public string kullaniciadi { get; set; }
        public string sifre { get; set; }
        public LoginModel(String kullaniciadi,String sifre)
        {
            this.kullaniciadi = kullaniciadi;
            this.sifre = sifre;
        }
    }
}