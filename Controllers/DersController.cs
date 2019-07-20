using Newtonsoft.Json;
using OdevToplamaProjesi.BusinessLayer.Managers;
using OdevToplamaProjesi.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace OdevToplamaProjesi.Web.Controllers
{
    public class DersController : Controller
    {
        DersManager dersManager = new DersManager();
        OgretmenManager ogretmenManager = new OgretmenManager();
        KullaniciManager kullaniciManager = new KullaniciManager();
        OgrenciDersIliskiTablosu ogrenciDersIliskiTablosu = new OgrenciDersIliskiTablosu();
        OgrenciDersManager ogrenciDersManager = new OgrenciDersManager();


        // GET: Ders
        public ActionResult DersEkle(string dersAdi,string dersKodu)
        {
            Kullanicilar kullanici = (Kullanicilar)Session["Kullanici"];
            Dersler ders = dersManager.Find(x => x.DersAdi == dersAdi);
                       

            try
            {
                var jsonformatter = new JsonMediaTypeFormatter();
                jsonformatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                if (dersAdi != null && dersKodu != null &&ders==null)
                {
                  ders = new Dersler();
                    ders.DersAdi = dersAdi;
                    ders.DersKodu = dersKodu;
                    ders.OgretmenID = kullanici.Ogretmenler.ID;


                }


                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:64663/api/DersApi/");


                    if (ders!=null)
                    {
                        //insert........
                        //HTTP POST
                        var postTask = client.PostAsJsonAsync<Dersler>("DersApi",ders);
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
                            return Json(new { result = false,message=mesaj }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else if(dersAdi != null && dersKodu != null && ders != null)
                    {
                       
                        //update........
                        //HTTP POST
                        var postTask = client.PutAsJsonAsync<Dersler>("DersApi", ders);
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
                    else
                    {
                        
                            return Json(new { result = false,message="Sunucuyla Bağlantı Kurulamadı" }, JsonRequestBehavior.AllowGet);
                      
                    }



                }


                /*context.InUPProduct(Convert.ToInt32(id),pname,Convert.ToDecimal(pprice));

                return Json(1, JsonRequestBehavior.AllowGet);*/
            }
            catch (DbEntityValidationException e)
            {
              
                return Json(new { result=false, message = "Sunucuyla Bağlantı Kurulamadı" }, JsonRequestBehavior.AllowGet);
            }

            
        }
        public PartialViewResult DersFormuGetir()
        {
            return PartialView("_DersFormuPartial");
        }
        public ActionResult DersListesiGetir()
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
                    var responseTask = client.GetAsync("GetByOgretmen/"+id);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<List<Dersler>>();
                        readTask.Wait();

                        dersListesi = readTask.Result;
                        var dersListesiJson=JsonConvert.SerializeObject(dersListesi);
                        return Json(new {result=true, veri = dersListesi },JsonRequestBehavior.AllowGet);
                    }
                    else //web api sent error response 
                    {
                        var readTask = result.Content.ReadAsStringAsync();
                        readTask.Wait();
                        var mesaj = readTask.Result;
                        //log response status here..



                        return Json(new { result = false,hatamesajı=mesaj }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (DbEntityValidationException e)
            {

                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult RandomString()
        {
            Random random = new Random();
            string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            StringBuilder result = new StringBuilder(10);
            for (int i = 0; i < 10; i++)
            {
                result.Append(characters[random.Next(characters.Length)]);
            }
            
            return Json(new { result = result.ToString()
            }, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult DersKayitFormuGetir()
        {
            return PartialView("_DersKayitFormuPartial");
        }
        
        public ActionResult DersKayitOl(string dersKodu)
        {
            if (dersKodu!=null)
            {
                

                try
                {
                    Kullanicilar kullanici = (Kullanicilar)Session["Kullanici"];
                    Dersler dersler = new Dersler();
                    dersler = dersManager.Find(x => x.DersKodu == dersKodu);
                    ogrenciDersIliskiTablosu.DersID = dersler.ID;
                    ogrenciDersIliskiTablosu.OgrencID = kullanici.Ogrenciler.ID;
                    using (var client = new HttpClient())
                        {
                            client.BaseAddress = new Uri("http://localhost:64663/api/OgrenciApi/DersKayit");



                        var jsonformatter = new JsonMediaTypeFormatter();
                        jsonformatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                        var postTask = client.PostAsync<OgrenciDersIliskiTablosu>("DersKayit", ogrenciDersIliskiTablosu,jsonformatter);
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
                return Json(new { result = false, message = "Eksik Bilgi Girişi" });
            }
        }
        public PartialViewResult OgretmenDerslerveOdevler()
        {
            Kullanicilar kullanicilar = (Kullanicilar)Session["Kullanici"];
            Ogretmenler ogretmen = kullaniciManager.Find(x => x.ID == kullanicilar.ID).Ogretmenler;
            return PartialView("~/Views/Ogretmen/_DersTablosuPartial.cshtml",ogretmen);
        }
        public ActionResult DersKaldirOgretmen(int id)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:64663/api/DersApi/");
                    //HTTP GET
                    var responseTask = client.GetAsync("GetDersKaldir/" + id);
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



                        return Json(new { result = true,message=mesaj }, JsonRequestBehavior.AllowGet);
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