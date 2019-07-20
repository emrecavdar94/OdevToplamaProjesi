using OdevToplamaProjesi.BusinessLayer.Managers;
using OdevToplamaProjesi.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Routing;

namespace OdevToplamaProjesi.Web.Controllers
{
    [RoutePrefix("api/DersApi")]
    public class DersApiController : ApiController
    {
        DersManager dersManager = new DersManager();
        OgrenciDersManager ogrenciDersManager = new OgrenciDersManager();
        OgretmenManager ogretmenManager = new OgretmenManager();
  
        [Route("GetDersKaldir/{id}")]
       public IHttpActionResult GetDersKaldir(int id)
        {
           
            if (id>0)
            {
                Dersler ders = dersManager.Find(x => x.ID == id);
                try
                {
                    dersManager.Delete(ders);
                    return Ok("Ders Silindi.");
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
            }
            return BadRequest("Eksik Bilgi Girişi");
        }
        
       [Route("GetByOgretmen/{id}")]
        public IHttpActionResult GetByOgretmen(int id) {
            List<Dersler> dersListesi = new List<Dersler>();
            if (id > 0)
            {
                try
                {
                    dersListesi = dersManager.List(x => x.OgretmenID == id);
                    if (dersListesi.Count>0)
                    {
                        return Ok(dersListesi);
                    }
                    else
                    {
                        return BadRequest("Öğretmene Ait Ders Bulunamadı");
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
        // GET: api/Ders/5
        [Route("GetByOgrenci/{id}")]
        public IHttpActionResult GetByOgrenci(int id)
        {
            List<OgrenciDersIliskiTablosu> dersListesi = new List<OgrenciDersIliskiTablosu>();
            if (id > 0)
            {
                try
                {
                    dersListesi = ogrenciDersManager.List(x => x.OgrencID == id);
                    if (dersListesi.Count>0)
                    {
                        return Ok(dersListesi);
                    }
                    else
                    {
                        return BadRequest("Öğrenciye Ait Ders Bulunamadı");
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
        // GET: api/Ders/5
        public void Get(int id)
        {
            
        }

        // POST: api/Ders
        public IHttpActionResult Post(Dersler ders)
        {
            Dersler dersTemp = dersManager.Find(x => x.DersAdi == ders.DersAdi && x.OgretmenID == ders.OgretmenID);
            if (ders != null)
            {
                try
                {
                    if (dersTemp==null)
                    {
                        dersManager.Insert(ders);
                        return Ok(ders);
                    }
                    else
                    {
                        dersTemp.DersKodu = ders.DersKodu;
                        dersManager.Update(dersTemp);
                        return Ok(dersTemp);
                    }
                   

                }
                catch (Exception e)
                {

                    return BadRequest("Eklenemedi");
                }

            }
            else
            {
                return BadRequest("Eksik Bilgi Girişi");
            }
        }

        // PUT: api/Home/5
        public IHttpActionResult Put(Dersler ders)
        {
            if (ders != null)
            {
                try
                {
                    dersManager.Update(ders);
                    return Ok();

                }
                catch (Exception)
                {

                    return BadRequest("Güncellenemedi");
                }

            }
            else
            {
                return BadRequest("Eksik Bilgi Girişi");
            }
        }

        // DELETE: api/Home/5
        public IHttpActionResult Delete(Dersler ders)
        {
            if (ders != null)
            {
                try
                {
                    dersManager.Delete(ders);
                    return Ok();

                }
                catch (Exception)
                {

                    return BadRequest("Silinemedi");
                }

            }
            else
            {
                return BadRequest("Eksik Bilgi Girişi");
            }
        }
    }
}

