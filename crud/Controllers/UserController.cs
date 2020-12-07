using crud.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web;
using System.Threading.Tasks;
using System.Data.Entity.Validation;

namespace crud.Controllers
{
    public class UserController : ApiController
    {

        private CrudOperationsEntities db = new CrudOperationsEntities();

        public IEnumerable<UserInfo> Get()
        {
            //return db.UserInfoes;
            using (CrudOperationsEntities dbcontext = new CrudOperationsEntities())
            {
                return dbcontext.UserInfoes.ToList();
            }
        }

        public UserInfo GetUser(long id)
        {
            var result = db.UserInfoes.FirstOrDefault((p) => p.id == id);
            return result;
        }

        [Route("api/delete/user")]
        [ResponseType(typeof(UserInfo))]
        public async Task<IHttpActionResult> DeleteUser(string email)
        {
            try
            {
                var user_obj = db.UserInfoes.FirstOrDefault((p) => p.email == email);
                user_obj.is_delete = "true";
                db.SaveChanges();
                return Ok(200);
            }catch(Exception e)
            {
                return Ok(400);
            }
        }

        [Route("api/post/user")]
        [ResponseType(typeof(UserInfo))]
        public async Task<IHttpActionResult> PostUser(UserInfo user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                db.UserInfoes.Add(user);
                db.SaveChanges();
                return Ok(200);
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
                return Ok(400);
            }
        }

        [Route("api/update/user")]
        [ResponseType(typeof(UserInfo))]
        public async Task<IHttpActionResult> UpdateUser(UserInfo user)
        {
            try
            {
                var user_obj = db.UserInfoes
                    .Where(s => s.email == user.email).FirstOrDefault<UserInfo>();
                var obj = db.UserInfoes.First<UserInfo>();
                Console.Write(user_obj);

                user_obj.name = user.name;
                db.SaveChanges();
                return Ok(200);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting  
                        // the current instance as InnerException  
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
        }

        //[Route("api/delete/user")]
        //[ResponseType(typeof(UserInfo))]
        //public async Task<IHttpActionResult> delete_user(UserInfo user)
        //{
        //    try
        //    {
        //        var user_obj = db.UserInfoes
        //            .Where(s => s.id == user.id).FirstOrDefault<UserInfo>();
        //        //var obj = db.UserInfoes.First<UserInfo>();
        //        //db.UserInfoes.Remove(user_obj);
        //        //Console.Write(user_obj);
        //        user_obj.is_delete = "true";
        //        db.SaveChanges();
        //        return Ok(200);
        //    }
        //    catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
        //    {
        //        Exception raise = dbEx;
        //        foreach (var validationErrors in dbEx.EntityValidationErrors)
        //        {
        //            foreach (var validationError in validationErrors.ValidationErrors)
        //            {
        //                string message = string.Format("{0}:{1}",
        //                    validationErrors.Entry.Entity.ToString(),
        //                    validationError.ErrorMessage);
        //                // raise a new exception nesting  
        //                // the current instance as InnerException  
        //                raise = new InvalidOperationException(message, raise);
        //            }
        //        }
        //        throw raise;
        //    }
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
