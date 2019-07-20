using OdevToplamaProjesi.BusinessLayer.Managers;
using OdevToplamaProjesi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OdevToplamaProjesi.WebUI.Controllers
{
    public class OgretmenController : Controller
    {
        KullaniciManager kullaniciManager = new KullaniciManager();
        // GET: Ogretmen
        public ActionResult Index()
        {
            Kullanicilar kullanicilar = (Kullanicilar)Session["Kullanici"];
            Ogretmenler ogretmen = kullaniciManager.Find(x => x.ID == kullanicilar.ID).Ogretmenler;
            return View(ogretmen);
        }
        public ActionResult Profilim()
        {
            if (TempData["mesaj"]!=null)
            {
                string mesaj =(string) TempData["mesaj"];
                TempData["mesaj"] = mesaj;
            }
            return View();
        }
        [HttpPost]
        public ActionResult Profilim(int id, string ad,string soyad,string eposta,string sifre)
        {
            Kullanicilar kullanici = kullaniciManager.Find(x => x.Ogretmenler.ID == id);
            if (kullanici!=null)
            {
                if (ad!="")
                {
                    kullanici.Ogretmenler.OgretmenAdi = ad;
                }
                if (soyad!="")
                {
                    kullanici.Ogretmenler.OgretmenSoyadi = soyad;
                }
                if (eposta!="")
                {
                    kullanici.EPosta = eposta;
                }
                if (sifre != "")
                {
                    kullanici.Sifre = sifre;
                }
                kullaniciManager.Update(kullanici);
                TempData["mesaj"] = "Kullanıcı Bilgileri Güncellendi";
                Session["Kullanici"] = kullanici;
                return View();
            }
            TempData["mesaj"] = "Kullanıcı Bilgileri Bulunamadı";
            return View();
        }
        
        public PartialViewResult DersTablosuGetir()
        {
            return PartialView("_DersTablosuPartial");

        }
        
    }
}