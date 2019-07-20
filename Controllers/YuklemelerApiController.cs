using OdevToplamaProjesi.BusinessLayer.Managers;
using OdevToplamaProjesi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace OdevToplamaProjesi.Web.Controllers
{
    [RoutePrefix("api/YuklemelerApi")]
    public class YuklemelerApiController : ApiController
    {
        Yuklemeler yuklemeler;
        YuklemelerManager yuklemelerManager = new YuklemelerManager();
        // GET: api/YuklemelerApi
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/YuklemelerApi/5
        public string Get(int id)
        {
            return "value";
        }
        [Route("GetByOdev/{id}")]
        public IHttpActionResult GetByOdev(int id)
        {
            List<Yuklemeler> yuklemelerListesi = new List<Yuklemeler>();
            if (id > 0)
            {
                try
                {
                    yuklemelerListesi = yuklemelerManager.List(x => x.OdevID == id);
                    if (yuklemelerListesi.Count > 0)
                    {
                        return Ok(yuklemelerListesi);
                    }
                    else
                    {
                        return BadRequest("Derse Ait Yükleme Bulunamadı");
                    }

                }
                catch (Exception)
                {

                    return BadRequest("Sunucuyla Bağlantı Kurulamadı");
                }

            }
            else
            {
                return BadRequest("Eksik Bilgi Girişi");
            }
        }
        [HttpPost]
        [Route("OdevYukle")]
        public IHttpActionResult OdevYukle(Yuklemeler yukleme)
        {

            if (yukleme != null)
            {
                try
                {
                    yuklemelerManager.Insert(yukleme);
                    
                        return Ok();
                  
                }
                catch (Exception)
                {

                    return BadRequest("Sunucuyla Bağlantı Kurulamadı");
                }

            }
            else
            {
                return BadRequest("Eksik Bilgi Girişi");
            }
        }
        [HttpPost]
        [Route("OdevYukleFromAndroid/{odevID}/{kullaniciID}")]
        public HttpResponseMessage OdevYukleFromAndroid(int odevID,int kullaniciID)
        {
            Yuklemeler yukleme = new Yuklemeler();
            yukleme.OdevID = odevID;
            yukleme.OgrenciID = kullaniciID;
            yukleme.YuklemeTarihi = DateTime.Now;
            
            var request = HttpContext.Current.Request;
            var description = request.Form["derscription"];
            var photo = request.Files["photo"];
            photo.SaveAs(HttpContext.Current.Server.MapPath("~/Content/Yuklemeler/"+photo.FileName));
            yukleme.YuklenenVeri = photo.FileName;
            yukleme.OdevNotu = "Not Girilmedi";
            yuklemelerManager.Insert(yukleme);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
        public IHttpActionResult Put(Yuklemeler yukleme)
        {
            try
            {
               

                if (yukleme!=null)
                {
                    yuklemelerManager.Update(yukleme);
                    return Ok();
                }
                else
                {
                    return BadRequest("Yükleme Bulunamadı");
                }

            }
            catch (Exception)
            {

                return BadRequest("Sunucuyla Bağlantı Kurulamadı");
            }
        }
        [Route("GetYuklemeByOgrenciID/{ogrenciID}")]
        public IHttpActionResult GetYuklemeByOgrenciID(int ogrenciID)
        {
            try
            {
                List<Yuklemeler> yuklemeList = yuklemelerManager.List(x => x.OgrenciID == ogrenciID);
                if (yuklemeList.Count>0)
                {
                    return Ok(yuklemeList);
                }
                else
                {
                    return BadRequest("Yukleme Bulunamadı");
                }
                
            }
            catch (Exception e)
            {

                return BadRequest("Bağlantı Kurulamadı");
            }
            
        }

        // DELETE: api/YuklemelerApi/5
        public void Delete(int id)
        {
        }
    }
}
