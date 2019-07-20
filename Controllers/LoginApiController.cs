using OdevToplamaProjesi.BusinessLayer.Managers;
using OdevToplamaProjesi.Entities;
using OdevToplamaProjesi.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OdevToplamaProjesi.Web.Controllers
{
    [RoutePrefix("api/LoginApi")]
    public class LoginApiController : ApiController
    {
        KullaniciManager kullaniciManager = new KullaniciManager();
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        [HttpPost]
        [Route("Giris")]
        public IHttpActionResult Giris(LoginModel loginModel)
        {
            if (loginModel.kullaniciadi != null && loginModel.sifre !=null)
            {
                try
                {

                    Kullanicilar kul = kullaniciManager.Find(x => x.KullaniciAdi == loginModel.kullaniciadi && x.Sifre == loginModel.sifre);
                    if (kul!=null)
                    {
                        return Ok(kul);
                    }
                    else
                    {
                        return BadRequest("Hatalı Kullanıcı Adı veya Şifre");
                    }
                   

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
                    return BadRequest("Sunucuyla bağlantı kurulamadı");
                }

            }
            else
            {
                return BadRequest("Eksik Bilgi Girişi");
            }
        }
        [HttpPost]
        [Route("Kayit")]
        public IHttpActionResult Kayit(Kullanicilar kullanici)
        {
            if (kullanici != null)
            {
                try
                {
                    
                    kullaniciManager.Insert(kullanici);
                    return Ok("Kayıt Başarılı");

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
                    return BadRequest("Eklenemedi");
                }

            }
            else
            {
                return BadRequest("Eksik Bilgi Girişi");
            }
        }
        // GET: api/LoginApi/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/LoginApi
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/LoginApi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/LoginApi/5
        public void Delete(int id)
        {
        }
    }
}
