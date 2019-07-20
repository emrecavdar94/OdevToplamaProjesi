using OdevToplamaProjesi.BusinessLayer.Managers;
using OdevToplamaProjesi.Entities;
using OdevToplamaProjesi.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OdevToplamaProjesi.Web.Controllers
{
    [RoutePrefix("api/OdevApi")]
    public class OdevApiController : ApiController
    {
        OdevManager odevManager = new OdevManager();
        
        // GET: api/OdevApi
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/OdevApi/5
        public IHttpActionResult Get(int id)
        {
            return Ok(odevManager.Find(x=>x.ID==id));
        }
        [Route("GetByOgretmen/{id}")]
        public IHttpActionResult GetByOgretmen(int id)
        {
            List<Odevler> odevListesi = new List<Odevler>();
            if (id > 0)
            {
                try
                {
                    odevListesi = odevManager.List(x => x.Dersler.OgretmenID == id);
                    if (odevListesi.Count > 0)
                    {
                        return Ok(odevListesi);
                    }
                    else
                    {
                        return BadRequest("Öğretmene Ait Ödev Bulunamadı");
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
        [Route("GetByDers/{id}")]
        public IHttpActionResult GetByDers(int id)
        {
          
            List<Odevler> odevListesi = new List<Odevler>();
            if (id > 0)
            {
                try
                {
                    odevListesi = odevManager.List(x => x.Dersler.ID == id);
                    if (odevListesi.Count > 0)
                    {
                        return Ok(odevListesi);
                    }
                    else
                    {
                        return BadRequest("Derse Ait Ödev Bulunamadı");
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
        // POST: api/OdevApi
        public IHttpActionResult Post(Odevler odev)
        {

            if (odev != null)
            {
                try
                {
                    odevManager.Insert(odev);
                    return Ok("Eklendi");

                }
                catch (Exception e )
                {

                    return BadRequest("Eklenemedi");
                }

            }
            else
            {
                return BadRequest("Eksik Bilgi Girişi");
            }
        }
       
        // PUT: api/OdevApi/5
        public IHttpActionResult Put(OdevGuncellemeModel odevGuncellemeModel)
        {
            Odevler odev = odevManager.Find(x => x.ID == odevGuncellemeModel.odevID);
      
            odev.OdevBasligi = odevGuncellemeModel.odevBasligi;
            odev.OdevAciklamasi = odevGuncellemeModel.odevAciklama;
            odev.BitisTarihi = Convert.ToDateTime(odevGuncellemeModel.odevTarihi);

            try
            {
                odevManager.Update(odev);
                return Ok();
            }
            catch (Exception)
            {

                return BadRequest("Hata");
            }
        }
        [HttpPut]
        [Route("PutAndroid")]
        public IHttpActionResult PutAndroid(OdevGuncellemeModel odevGuncellemeModel)
        {
            Odevler odev = odevManager.Find(x => x.ID == odevGuncellemeModel.odevID);
            if (odevGuncellemeModel.odevBasligi!=null || odevGuncellemeModel.odevBasligi!="")
            {
                odev.OdevBasligi = odevGuncellemeModel.odevBasligi;
            }
            if (odevGuncellemeModel.odevAciklama != null || odevGuncellemeModel.odevAciklama != "")
            {
                odev.OdevAciklamasi = odevGuncellemeModel.odevAciklama;
            }
            if (odevGuncellemeModel.odevTarihi != null || odevGuncellemeModel.odevTarihi != "")
            {
                odev.BitisTarihi = DateTime.ParseExact(odevGuncellemeModel.odevTarihi, "dd/MM/yyyy", null);
            }
            

            try
            {
                odevManager.Update(odev);
                return Ok();
            }
            catch (Exception)
            {

                return BadRequest("Hata");
            }
        }
        public IHttpActionResult Delete(int id)
        {
            
            
                Odevler odev = odevManager.Find(x => x.ID == id);
                if (odev != null){

               
               
                    try
                     {
                            odevManager.Delete(odev);
                            return Ok("Silindi");

                    }
                    catch (Exception e)
                     {

                        return BadRequest("Silinemedi");
                     }

                }
                else {
                    return BadRequest("Eksik Bilgi Girişi");
                }

            }
            
        }


    

    }

