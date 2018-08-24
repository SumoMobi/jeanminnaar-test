using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi1.ViewModels;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace WebApi1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // GET: api/User
//        [SwaggerOperation("GetAll")]
        [HttpGet]
        public List<User> Get()
        {
            return new List<User>()
            {
                new User{ Id = 1, FirstName = "Joe", LastName = "Blow" },
                new User{ Id = 2, FirstName = "Jill", LastName = "Whatever" },
            };
        }

        // GET: api/User/5
        [HttpGet("{id}", Name = "Get")]
        public User Get(int id)
        {
            return Get()[0];
        }

        // POST: api/User
        [HttpPost]
        public void Post([FromBody] User user)
        {
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] User user)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
