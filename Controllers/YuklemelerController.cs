using Newtonsoft.Json;
using OdevToplamaProjesi.BusinessLayer.Managers;
using OdevToplamaProjesi.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Mvc;

namespace OdevToplamaProjesi.Web.Controllers
{
    public class YuklemelerController : Controller
    {
        OdevManager odevManager = new OdevManager();
        YuklemelerManager yuklemelerManager = new YuklemelerManager();
        Yuklemeler yukleme;
        // GET: Yuklemeler
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult YuklemeleriGetir(int id)
        {

            List<Yuklemeler> yuklemelerListesi;

            try
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:64663/api/YuklemelerApi/GetByOdev");
                    //HTTP GET
                    var responseTask = client.GetAsync("GetByOdev/" + id);
                    responseTask.Wait();

                    var result = responseTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<List<Yuklemeler>>();
                        readTask.Wait();

                        yuklemelerListesi = readTask.Result;
                        var yuklemelerListesiJson = JsonConvert.SerializeObject(yuklemelerListesi);
                        return PartialView("_YuklemelerPartial", yuklemelerListesi);
                    }
                    else //web api sent error response 
                    {
                        var readTask = result.Content.ReadAsStringAsync();
                        readTask.Wait();
                        var mesaj = readTask.Result;
                        //log response status here..



                        return PartialView("_HataPartial");
                    }
                }
            }
            catch (DbEntityValidationException e)
            {

                return PartialView("_HataPartial");
            }
        }
        public PartialViewResult DersleriveOdevleriGetir()
        {
            Kullanicilar kullanici = (Kullanicilar)Session["Kullanici"];

            List<Dersler> dersListesi;
            int id = kullanici.Ogretmenler.ID;
            try
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:64663/api/DersApi/GetByOgretmen");
                    //HTTP GET
                    var responseTask = client.GetAsync("GetByOgretmen/" + id);
                    responseTask.Wait();

                    var result = responseTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<List<Dersler>>();
                        readTask.Wait();

                        dersListesi = readTask.Result;
                        var dersListesiJson = JsonConvert.SerializeObject(dersListesi);
                        return PartialView("_OdevlerPartial", dersListesi);
                    }
                    else //web api sent error response 
                    {
                        var readTask = result.Content.ReadAsStringAsync();
                        readTask.Wait();
                        var mesaj = readTask.Result;
                        //log response status here..



                        return PartialView("_HataPartial");
                    }
                }
            }
            catch (DbEntityValidationException e)
            {

                return PartialView("_HataPartial");
            }
        }
        public ActionResult NotGiris(int yuklemeid, string puan)
        {
            yukleme = yuklemelerManager.Find(x => x.ID == yuklemeid);
            if (yuklemeid > 0 && puan != null)
            {
                try
                {
                    yukleme.OdevNotu = puan;
                    var jsonformatter = new JsonMediaTypeFormatter();
                    jsonformatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:64663/api/YuklemelerApi");



                        //insert........
                        //HTTP POST
                        var postTask = client.PutAsync<Yuklemeler>("YuklemelerApi", yukleme, jsonformatter);
                        postTask.Wait();

                        var result = postTask.Result;

                        if (result.IsSuccessStatusCode)
                        {
                            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var readTask = result.Content.ReadAsStringAsync();
                            readTask.Wait();
                            var mesaj = readTask.Result;
                            return Json(new { result = false, message = mesaj }, JsonRequestBehavior.AllowGet);
                        }

                    }

                }


                catch (DbEntityValidationException e)
                {

                    return Json(new { result = false, message = "Sunucuyla Bağlantı Kurulamadı" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { result = false, message = "Eksik Bilgi Girişi" }, JsonRequestBehavior.AllowGet);
            }

        }
        public FileResult YuklemeIndir(int? id)

        {
            yukleme = yuklemelerManager.Find(x => x.ID == id);
            string dosyaYolu = Path.Combine(Server.MapPath(@"~/Content/Yuklemeler/" + yukleme.YuklenenVeri));
            var ctype = MimeMapping.GetMimeMapping(yukleme.YuklenenVeri);
            return new FilePathResult(dosyaYolu, ctype);
        }
        public ActionResult YuklemeGuncelle(int id)
        {
            Kullanicilar kullanici = (Kullanicilar)Session["Kullanici"];
            Odevler odev = odevManager.Find(x => x.ID==id);
            yukleme = yuklemelerManager.Find(x => x.OdevID == odev.ID && x.OgrenciID == kullanici.OgrenciID);

            return View(odev);

        }
        [HttpPost]
        public ActionResult YuklemeGuncelle(int id,HttpPostedFileBase yuklenenDosya)
        {
            Kullanicilar kullanici = (Kullanicilar)Session["Kullanici"];
            Odevler odev = odevManager.Find(x => x.ID == id);
            
            yukleme = yuklemelerManager.Find(x => x.OdevID == id && x.OgrenciID == kullanici.OgrenciID);
            if (yukleme != null)
            {
                if (yuklenenDosya != null)
                {
                    string path = Server.MapPath("~/Content/Yuklemeler/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    yuklenenDosya.SaveAs(path + Path.GetFileName(yuklenenDosya.FileName));
                    yukleme.YuklenenVeri = Path.GetFileName(yuklenenDosya.FileName);
                    yuklemelerManager.Update(yukleme);
                    TempData["Mesaj"] = "Yükleme Başarıyla Güncellendi";
                }

            }
            else
            {
                TempData["Mesaj"] = "Yükleme Yapılamadı";
            }
            
            return View(odev);
        }
    }
} 
