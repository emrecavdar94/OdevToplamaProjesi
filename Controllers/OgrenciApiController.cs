using OdevToplamaProjesi.BusinessLayer.Managers;
using OdevToplamaProjesi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OdevToplamaProjesi.Web.Controllers
{
    [RoutePrefix("api/OgrenciApi")]
    public class OgrenciApiController : ApiController
    {
        DersManager dersManager = new DersManager();
        OgrenciDersIliskiTablosu ogrenciDersIliskiTablosu = new OgrenciDersIliskiTablosu();
        OgrenciDersManager ogrenciDersManager = new OgrenciDersManager();
        // GET: api/OgrenciApi
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [Route("DersKayit/")]
        public IHttpActionResult DersKayit(OgrenciDersIliskiTablosu ogrenciDersIliskiTablosu)
        {
            if (ogrenciDersIliskiTablosu !=null)
            {
                try
                {
                    ogrenciDersManager.Insert(ogrenciDersIliskiTablosu);
                    return Ok(ogrenciDersIliskiTablosu);
              

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
        [Route("DersKayitFromAndroid/{derskodu}/{ogrenciid}")]
        public IHttpActionResult DersKayitFromAndroid(string derskodu,int ogrenciid)
        {
            OgrenciDersIliskiTablosu ogrDers = new OgrenciDersIliskiTablosu();
                try
                {
                    Dersler dersler = new Dersler();
                    dersler = dersManager.Find(x => x.DersKodu.Contains(derskodu));
                    ogrDers.DersID = dersler.ID;
                    ogrDers.OgrencID = ogrenciid;
                    ogrenciDersManager.Insert(ogrDers);
                    return Ok("Derse Kayıt Başarılı ! ");


                }
                catch (Exception)
                {

                    return BadRequest("Sunucuyla Bağlantı Kurulamadı");
                }

            }
          

        // POST: api/OgrenciApi
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/OgrenciApi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/OgrenciApi/5
        public void Delete(int id)
        {
        }
    }
}
