using Newtonsoft.Json;
using OdevToplamaProjesi.BusinessLayer.Managers;
using OdevToplamaProjesi.Entities;
using OdevToplamaProjesi.Web.Ayarlar;
using OdevToplamaProjesi.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Mvc;

namespace OdevToplamaProjesi.Web.Controllers
{
    public class LoginController : Controller
    {
        KullaniciManager kullaniciManager = new KullaniciManager();
        Kullanicilar kullanici;
        Ogrenciler ogrenci;
        Ogretmenler ogretmen;
        YetkiManager yetkiManager = new YetkiManager();
        // GET: Login
        public ActionResult Index()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Index(string kullaniciadi, string sifre)
        {


            

            try
            {
                LoginModel loginModel = new LoginModel(kullaniciadi, sifre);
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:64663/api/LoginApi/Giris");
                    //HTTP GET
                    var responseTask = client.PostAsJsonAsync<LoginModel>("Giris",loginModel);
                    responseTask.Wait();

                    var result = responseTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<Kullanicilar>();
                        readTask.Wait();

                        kullanici = readTask.Result;
                        Session["kullanici"] = kullanici;
                        if (kullanici.Yetki.YetkiAdi == "Ogretmen")
                        {

                           
                            return RedirectToAction("Index", "Ogretmen");
                        }
                        else if (kullanici.Yetki.YetkiAdi == "Ogrenci")
                        {
                            
                            return RedirectToAction("Index", "Ogrenci");
                        }
                        else
                        {
                            TempData["mesaj"] = "Hatalı Kullanıcı Adı veya Şifre";
                            return RedirectToAction("Index", "Login");
                        }
                        
                    }
                    else //web api sent error response 
                    {
                        var readTask = result.Content.ReadAsStringAsync();
                        readTask.Wait();
                        var mesaj = readTask.Result;
                        //log response status here..



                        TempData["mesaj"] = mesaj;
                        return RedirectToAction("Index", "Login");
                    }
                }
            }
            catch (DbEntityValidationException e)
            {

                TempData["mesaj"] = "Sunucuyla bağlantı kurulamadı";
                return RedirectToAction("Index", "Login");
            }

           

        }
        public ActionResult CikisYap()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }
        public ActionResult KayitOl()
        {
            return View();
        }
        [HttpPost]
        public ActionResult KayitOl(string ad,string soyad, HttpPostedFileBase profilFotografi,string sifretekrari,string eposta, string sifre, string kullaniciadi,string kayitturu,string ogrenciNumarasi)
        {
            Kullanicilar tempKullanici = kullaniciManager.Find(x => x.EPosta == eposta || x.KullaniciAdi == kullaniciadi || x.Ogrenciler.OgrenciNumarasi == ogrenciNumarasi);
            if (tempKullanici==null)
            {

            
            if (ad != null && soyad != null && sifretekrari != null && eposta != null && sifre != null && kullaniciadi != null)
            {
                if (sifre == sifretekrari)
                {

                    try
                    {
                        string fotograf="";
                        kullanici = new Kullanicilar();
                        kullanici.KullaniciAdi = kullaniciadi;
                        kullanici.Sifre = sifre;
                        kullanici.EPosta = eposta;
                        if (kayitturu=="1")
                        {
                            ogrenci = new Ogrenciler();
                            ogrenci.OgrenciAdi = ad;
                            ogrenci.OgrenciSoyadi = soyad;
                            ogrenci.OgrenciNumarasi = ogrenciNumarasi;
                            kullanici.Yetki=yetkiManager.Find(x=>x.YetkiAdi=="Ogrenci");
                            if (profilFotografi != null)
                            {
                                string yeniResimAdi = "";
                                ResimIslem r = new ResimIslem();
                                yeniResimAdi = r.Ekle(profilFotografi);
                                //yeniResimAdi = new ResimIslem().Ekle(resimGelen);

                                if (yeniResimAdi == "uzanti")
                                {
                                    ViewData["Hata"] = "Lütfen .png veya .jpg uzantılı dosya giriniz.";

                                }
                                else if (yeniResimAdi == "boyut")
                                {

                                    ViewData["Hata"] = "En fazla 1MB boyutunda dosya girebilirsiniz.";

                                }
                                else
                                {
                                    fotograf = yeniResimAdi;
                                }
                                ogrenci.OgrenciFotografi = fotograf;
                                kullanici.Ogrenciler = ogrenci;
                            }
                           
                        }
                        if (kayitturu=="2")
                        {
                            ogretmen = new Ogretmenler();
                            ogretmen.OgretmenAdi = ad;
                            ogretmen.OgretmenSoyadi = soyad;
                            kullanici.Yetki = yetkiManager.Find(x => x.YetkiAdi == "Ogretmen");
                            if (profilFotografi != null)
                            {
                                string yeniResimAdi = "";
                                ResimIslemOgretmen r = new ResimIslemOgretmen();
                                yeniResimAdi = r.Ekle(profilFotografi);
                                //yeniResimAdi = new ResimIslem().Ekle(resimGelen);

                                if (yeniResimAdi == "uzanti")
                                {
                                    ViewData["Hata"] = "Lütfen .png veya .jpg uzantılı dosya giriniz.";

                                }
                                else if (yeniResimAdi == "boyut")
                                {

                                    ViewData["Hata"] = "En fazla 1MB boyutunda dosya girebilirsiniz.";

                                }
                                else
                                {
                                    fotograf = yeniResimAdi;
                                }
                                ogretmen.OgretmenFotografi = fotograf;
                                kullanici.Ogretmenler=ogretmen;
                            }
                            
                        }
                        using (var client = new HttpClient())
                        {
                            var jsonformatter = new JsonMediaTypeFormatter();
                            jsonformatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                            client.BaseAddress = new Uri("http://localhost:64663/api/LoginApi/Kayit");
                            var postTask = client.PostAsync("Kayit", kullanici,jsonformatter);
                            postTask.Wait();

                            var result = postTask.Result;

                            if (result.IsSuccessStatusCode)
                            {
                                TempData["mesaj"] = "Kayıt işlemi başarılı bir şekilde tamamlandı.";
                                return RedirectToAction("Index", "Login");
                            }
                            else
                            {
                                
                                TempData["mesaj"] = "Hatalı Bilgi Girişi";
                                return RedirectToAction("Index", "Login");
                            }

                        }
                    }
                    catch (DbEntityValidationException e)
                    {

                        TempData["mesaj"] = "Sunucuyla bağlantı kurulamadı.";
                    }
                    
                }
               
                return RedirectToAction("Index", "Login");
            }
                TempData["mesaj"] = "Eksik Bilgi Girişi";
            }
            else
            {
                TempData["mesaj"] = "Kullanıcı Sistemle Kayıtlı Tekrar Deneyin";
            }
            
            return RedirectToAction("Index", "Login");
        }

    }
}

