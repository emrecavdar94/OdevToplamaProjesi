using OdevToplamaProjesi.BusinessLayer.Managers;
using OdevToplamaProjesi.Entities;
using OdevToplamaProjesi.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace OdevToplamaProjesi.Web.Controllers
{
    public class OdevController : Controller
    {
        // GET: Odev
        OdevManager odevManager = new OdevManager();
        DersManager dersManager = new DersManager();
        List<Odevler> odevListesi;
        OgretmenManager ogretmenManager = new OgretmenManager();
        public PartialViewResult OdevFormuGetir()
        {
            return PartialView("_OdevFormuPartial");
        }
        public ActionResult OdevEkle(int dersID, string odevBasligi, string odevAciklamasi, DateTime baslangicTarihi, DateTime bitisTarihi)
        {

            Dersler ders = dersManager.Find(x => x.ID == dersID);
            Odevler odev;

            try
            {


                if (dersID > 0 && odevBasligi != null && odevAciklamasi != null && baslangicTarihi != null && bitisTarihi != null && ders != null)
                {
                    odev = new Odevler();

                    odev.DersID = ders.ID;
                    odev.OdevBasligi = odevBasligi;
                    odev.OdevAciklamasi = odevAciklamasi;
                    odev.BaslangicTarihi = DateTime.Now;
                    odev.BitisTarihi = baslangicTarihi;





                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:64663/api/OdevApi");



                        //insert........
                        //HTTP POST
                        var postTask = client.PostAsJsonAsync<Odevler>("OdevApi", odev);
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
                else
                {

                    return Json(new { result = false, message = "Eksik Bilgi Girişi" }, JsonRequestBehavior.AllowGet);
                }

                /*context.InUPProduct(Convert.ToInt32(id),pname,Convert.ToDecimal(pprice));

                return Json(1, JsonRequestBehavior.AllowGet);*/
            }
            catch (DbEntityValidationException e)
            {

                return Json(new { result = false, message = "Sunucuyla Bağlantı Kurulamadı" }, JsonRequestBehavior.AllowGet);
            }


        }
        public PartialViewResult DerseAitOdevGetir(int dersID)
        {
            try
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:64663/api/OdevApi/GetByDers");
                    //HTTP GET
                    var responseTask = client.GetAsync("GetByDers/" + dersID);
                    responseTask.Wait();

                    var result = responseTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<List<Odevler>>();
                        readTask.Wait();

                        odevListesi = readTask.Result;

                        return PartialView("_DerseAitOdevPartial", odevListesi);
                    }
                    else //web api sent error response 
                    {
                        var readTask = result.Content.ReadAsStringAsync();
                        readTask.Wait();
                        var mesaj = readTask.Result;
                        //log response status here..



                        return PartialView("~/Views/Yuklemeler/_HataPartial");
                    }
                }
            }
            catch (DbEntityValidationException e)
            {

                return PartialView("~/Views/Yuklemeler/_HataPartial");
            }

        }
        public ActionResult OdevYukleme(int id)
        {
            TempData["Odev"] = odevManager.Find(x => x.ID == id);
            return View(odevManager.Find(x=>x.ID==id));
        }
        [HttpPost]
        public ActionResult OdevYukleme(HttpPostedFileBase yuklenenDosya)
        {
            Odevler odev = (Odevler)TempData["Odev"];
            Kullanicilar kullanici = (Kullanicilar)Session["Kullanici"];
            if (odev.BitisTarihi>=DateTime.Now)
            {

            
            if (yuklenenDosya != null)
            {
                string path = Server.MapPath("~/Content/Yuklemeler/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                yuklenenDosya.SaveAs(path + Path.GetFileName(yuklenenDosya.FileName));
                
                Yuklemeler yukleme = new Yuklemeler();
                yukleme.OdevID = odev.ID;
                yukleme.OgrenciID = kullanici.Ogrenciler.ID;
                yukleme.YuklemeTarihi = DateTime.Now;
                    yukleme.OdevNotu = "Not Girilmedi";
                yukleme.YuklenenVeri = Path.GetFileName(yuklenenDosya.FileName);
                try
                {
                        using (var client = new HttpClient())
                        {
                            client.BaseAddress = new Uri("http://localhost:64663/api/YuklemelerApi/OdevYukle");



                            //insert........
                            //HTTP POST
                            var postTask = client.PostAsJsonAsync<Yuklemeler>("OdevYukle", yukleme);
                            postTask.Wait();

                            var result = postTask.Result;

                            if (result.IsSuccessStatusCode)
                            {
                            TempData["Mesaj"] = "Yükleme Başarıyla Tamamlandı";
                        }
                            else
                            {
                                var readTask = result.Content.ReadAsStringAsync();
                                readTask.Wait();
                                var mesaj = readTask.Result;
                                TempData["Mesaj"] = "Yükleme Yapılamadı";
                        }
                            
                        }

                    }
                   
                
                catch (DbEntityValidationException e)
                {

                    TempData["Mesaj"] = "Sunucuyla Bağlantı Kurulamadı";
                }
                

            }

            TempData["Odev"] = odev;
            }
            else
            {
                TempData["Mesaj"] = "Ödevin Son Teslim Tarihi Geçmiştir";
            }
            return View(odev);
        }
        public PartialViewResult OdevDuzenle(int odevID)
        {
            Odevler odev;
            try
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:64663/api/");
                    //HTTP GET
                    var responseTask = client.GetAsync("OdevApi/" + odevID);
                    responseTask.Wait();

                    var result = responseTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<Odevler>();
                        readTask.Wait();

                        odev = readTask.Result;

                        return PartialView("_OdevDuzenle", odev);
                    }
                    else //web api sent error response 
                    {
                        var readTask = result.Content.ReadAsStringAsync();
                        readTask.Wait();
                        var mesaj = readTask.Result;
                        //log response status here..



                        return PartialView("~/Views/Yuklemeler/_HataPartial");
                    }
                }
            }
            catch (DbEntityValidationException e)
            {

                return PartialView("~/Views/Yuklemeler/_HataPartial");
            }


        }

        public ActionResult OdevGuncelle(int id,string odevbasligi,string odevaciklamasi,string bitistarihi)
        {

            
            OdevGuncellemeModel odevGuncellemeModel = new OdevGuncellemeModel();
            odevGuncellemeModel.odevID = id;
            odevGuncellemeModel.odevBasligi = odevbasligi;
            odevGuncellemeModel.odevAciklama = odevaciklamasi;
            odevGuncellemeModel.odevTarihi = bitistarihi;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:64663/api/");

                //HTTP POST
                var putTask = client.PutAsJsonAsync<OdevGuncellemeModel>("OdevApi", odevGuncellemeModel);
                putTask.Wait();

                var result = putTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { result = false ,message="Güncellenemedi Tekrar Deneyiniz."}, JsonRequestBehavior.AllowGet);
        }
        public ActionResult OdevKaldir(int id)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:64663/api/");
                    //HTTP GET
                    var responseTask = client.DeleteAsync("OdevApi/" + id);
                    responseTask.Wait();

                    var result = responseTask.Result;

                    if (result.IsSuccessStatusCode)
                    {




                        return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                    }
                    else //web api sent error response 
                    {
                        var readTask = result.Content.ReadAsStringAsync();
                        readTask.Wait();
                        var mesaj = readTask.Result;
                        //log response status here..



                        return Json(new { result = true, message = mesaj }, JsonRequestBehavior.AllowGet);
                    }
                }

            }
            catch (DbEntityValidationException e)
            {

                return Json(new { result = false, message = "Sunucuyla Bağlantı Kurulamadı" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}