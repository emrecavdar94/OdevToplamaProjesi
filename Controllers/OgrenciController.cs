using OdevToplamaProjesi.BusinessLayer.Managers;
using OdevToplamaProjesi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OdevToplamaProjesi.Web.Controllers
{
    public class OgrenciController : Controller
    {
        DersManager dersManager = new DersManager();
        OgretmenManager ogretmenManager = new OgretmenManager();
        KullaniciManager kullaniciManager = new KullaniciManager();
        OgrenciDersManager ogrenciDersManager = new OgrenciDersManager();
        List<OgrenciDersIliskiTablosu> ogrenciDersIliskiTablosu;
        OdevManager odevManager = new OdevManager();
       
        // GET: Ogrenci
        public ActionResult Index()
        {
            Kullanicilar kullanici = (Kullanicilar)Session["Kullanici"];
            ogrenciDersIliskiTablosu = ogrenciDersManager.List(x => x.Ogrenciler.ID == kullanici.Ogrenciler.ID);
            return View(ogrenciDersIliskiTablosu);
        }
        public ActionResult Derslerim()
        {
            Kullanicilar kullanici = (Kullanicilar)Session["Kullanici"];
            ogrenciDersIliskiTablosu= ogrenciDersManager.List(x => x.Ogrenciler.ID == kullanici.Ogrenciler.ID);
            return View(ogrenciDersIliskiTablosu);
        }
        public ActionResult Profilim()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Profilim(int id,string ad,string soyad,string eposta,string ogrencinumarasi,string sifre)
        {
            Kullanicilar kullanici = kullaniciManager.Find(x => x.Ogrenciler.ID == id);
            if (kullanici != null)
            {
                if (ad != "")
                {
                    kullanici.Ogrenciler.OgrenciAdi = ad;
                }
                if (soyad != "")
                {
                    kullanici.Ogrenciler.OgrenciSoyadi = soyad;
                }
                if (eposta != "")
                {
                    kullanici.EPosta = eposta;
                }
                if (sifre != "")
                {
                    kullanici.Sifre = sifre;
                }
                if (ogrencinumarasi!="")
                {
                    kullanici.Ogrenciler.OgrenciNumarasi = ogrencinumarasi;
                }
                kullaniciManager.Update(kullanici);
                TempData["mesaj"] = "Kullanıcı Bilgileri Güncellendi";
                Session["Kullanici"] = kullanici;
                return View();
            }
            TempData["mesaj"] = "Kullanıcı Bilgileri Bulunamadı";
            return View();
        }
    }
}