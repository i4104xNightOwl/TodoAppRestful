using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestfulDemo.Models;
using RestfulDemo.Services;

namespace RestfulDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly TodoService _todoService;

        public TodoController(TodoService todoService)
        {
            _todoService = todoService;
        }

        [HttpGet("/todo", Name = "getListTodo")]
        public ActionResult<TodoModel[]> Get()
        {
            var todos = _todoService.Get();
            return Ok(todos);
        }

        [HttpGet("/todo/{id}", Name = "getByID")]
        public ActionResult<TodoModel> GetById(string id)
        {
            var todo = _todoService.GetById(id);
            if (todo == null)
                return NotFound();

            return Ok(todo);
        }

        [HttpPost("/todo", Name = "create")]
        public ActionResult<TodoModel> Create([FromBody] TodoModel model)
        {
            if (model == null)
                return BadRequest("Todo model cannot be null");

            var createdTodo = _todoService.Create(model);
            if (createdTodo == null)
                return BadRequest("Failed to create todo");

            return CreatedAtRoute("getByID", new { id = createdTodo.Id }, createdTodo);
        }

        [HttpPut("/todo", Name = "update")]
        public ActionResult<TodoModel> Update([FromBody] TodoModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Id))
                return BadRequest("Todo model and Id are required");

            var updatedTodo = _todoService.Update(model);
            if (updatedTodo == null)
                return BadRequest("Failed to update todo");

            return Ok(updatedTodo);
        }

        [HttpDelete("/todo/{id}", Name = "delete")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Id is required");

            var todo = _todoService.GetById(id);
            if (todo == null)
                return NotFound();

            _todoService.Delete(id);
            return NoContent();
        }
    }
}
