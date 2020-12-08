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
using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace crud.Controllers
{
    public class UserController : ApiController
    {

        private CrudOperationsEntities db = new CrudOperationsEntities();

        [Route("api/users")]
        public List<DtoResponseModel> Get()
        {
            //try
            //{
                using (CrudOperationsEntities dbcontext = new CrudOperationsEntities())
                {
                   var query = from e in dbcontext.UserInfoes
                                 join d in dbcontext.departments on e.dept_id equals d.id
                                 where e.is_delete == "false"
                                 orderby e.name ascending
                                 select new DtoResponseModel()
                                 {
                                     emp_id = e.id,
                                     emp_name = e.name,
                                     emp_email = e.email,
                                     emp_phone = e.phone,
                                     dname = d.dname
                                 };

                    return query.ToList();
                
                }
            //}
            //catch (Exception e)
            //{
            //    return ;
            //}
        }

        [Route("api/user")]
        public List<UserModel> GetUser(long id)
        {
            using (CrudOperationsEntities dbcontext = new CrudOperationsEntities())
            {
                //var result = dbcontext.UserInfoes.FirstOrDefault((p) => p.id == id);
                var query = dbcontext.UserInfoes.Where((x) => x.id == id)
                    .Select(x => new UserModel()
                    {
                        id = x.id,
                        name = x.name,
                        email = x.email,
                        phone = x.phone
                    });
                return query.ToList();

                //try
                //{
                //    var query = from e in db.UserInfoes
                //                where e.dept_id == id
                //                select e;
                //    return query;
                //}
                //catch(Exception e)
                //{
                //    return Enumerable.Empty<UserInfo>();
                //}
            }
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
                System.Diagnostics.Debug.WriteLine(e.ToString());
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

        [Route("api/numberOfEmployees")]
        public List<DTOGroup> getEmployeeByDept()
        {
            using (CrudOperationsEntities db = new CrudOperationsEntities())
            {
                var query1 = db.UserInfoes.GroupBy((p) => p.dept_id).
                    Select(g => new DTOGroup() { noOfEmployees = g.Key, dept_id = g.Count() });

                var query = from p in db.UserInfoes
                            group p by p.dept_id into g
                            select new 
                            {
                                noOfEmployees = g.Key,
                                count = g.Count()
                            };

                return query1.ToList();
            }
        }

        //[Route("api/getEmpByDept/{id}")]
        //public List<UserInfo> getEmp(int id)
        //{
        //    using (CrudOperationsEntities db = new CrudOperationsEntities())
        //    {
        //        var data = new Dictionary<string, object>();
        //        try
        //        {
        //            //var query = db.UserInfoes
        //            //    .Where((s) => s.dept_id == id)
        //            //   .FirstOrDefault<UserInfo>();

        //            var query = from e in db.UserInfoes
        //                        where e.dept_id == id
        //                        select e;
        //            var emp = query.FirstOrDefault<UserInfo>();

        //            return new List<UserInfo>();
        //        }
        //        catch (Exception e)
        //        {
        //            return new List<UserInfo>();
        //        }
        //    }
        //}

        [Route("api/getEmpByDept/{id}")]
        public List<UserModel> getEmpByDept(long id)
        {
            try
            {
                //var query_syntax = (from e in db.UserInfoes
                //                    where e.dept_id == id
                //                    select new UserModel()
                //                    {
                //                        name = e.name,
                //                        email = e.email,
                //                        phone = e.phone,
                //                        id = e.id,
                //                    }).ToList();

                //if (query_syntax == null || !query_syntax.Any())
                //{
                //    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
                //}


                var query_method_syntax = db.UserInfoes.Where(t => t.dept_id == id).
                    Select(g => new UserModel() { id = g.id, name = g.name, email = g.email, phone = g.phone }).ToList();

                if (query_method_syntax == null || !query_method_syntax.Any())
                {
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
                }

                return query_method_syntax;
            }
            catch(Exception e)
            {
                return new List<UserModel>();
            }
        }

        [Route("api/getEmpByName/{name}")]
        public List<UserModel> getEmpByName(string name)
        {
            try
            {
                //var query_syntax = (from e in db.UserInfoes
                //                    where e.name.Contains(name)
                //                    //where SqlFunctions.PatIndex("%{%}%", name) > 0
                //                    select new UserModel()
                //                    {
                //                        name = e.name,
                //                        email = e.email,
                //                        phone = e.phone,
                //                        id = e.id,
                //                    }).ToList();

                //if (query_syntax == null || !query_syntax.Any())
                //{
                //    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
                //}


                var query_method_syntax = db.UserInfoes.Where(t => t.name.Contains(name)).
                    Select(g => new UserModel() { id = g.id, name = g.name, email = g.email, phone = g.phone }).ToList();

                if (query_method_syntax == null || !query_method_syntax.Any())
                {
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
                }

                return query_method_syntax;
            }
            catch (Exception e)
            {
                return new List<UserModel>();
            }
        }

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
