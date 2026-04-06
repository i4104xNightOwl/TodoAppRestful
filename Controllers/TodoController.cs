using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RestfulDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodoController : ControllerBase
    {
        [HttpGet("/todo", Name = "getListTodo")]
        public string get()
        {
            return "test";
        }

        [HttpGet("/todo/{id}", Name = "getByID")]
        public int getById(int id) {
            return id;
        }

        [HttpPost("/todo", Name = "create")]
        public bool create()
        {
            return true;
        }

        [HttpPut("/todo", Name = "update")]
        public bool update() { 
            return true;
        }

        [HttpDelete("/todo/{id}", Name = "delete")]
        public bool delete() { 
            return false;
        }
    }
}
