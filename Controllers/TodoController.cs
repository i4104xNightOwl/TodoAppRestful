using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestfulDemo.Models;
using RestfulDemo.Services;

namespace RestfulDemo.Controllers
{
    /// <summary>
    /// Controller quản lý các thao tác CRUD cho Todo.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly TodoService _todoService;

        public TodoController(TodoService todoService)
        {
            _todoService = todoService;
        }

        /// <summary>
        /// Lấy danh sách tất cả các Todo.
        /// </summary>
        /// <returns>Danh sách các Todo.</returns>
        [HttpGet("/todo", Name = "getListTodo")]
        public ActionResult<TodoModel[]> Get()
        {
            var todos = _todoService.Get();
            return Ok(todos);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một Todo theo ID.
        /// </summary>
        /// <param name="id">ID của Todo cần lấy.</param>
        /// <returns>Thông tin Todo tương ứng.</returns>
        [HttpGet("/todo/{id}", Name = "getByID")]
        public ActionResult<TodoModel> GetById(string id)
        {
            var todo = _todoService.GetById(id);
            if (todo == null)
                return NotFound();

            return Ok(todo);
        }

        /// <summary>
        /// Tạo mới một Todo.
        /// </summary>
        /// <param name="model">Thông tin Todo cần tạo.</param>
        /// <returns>Todo vừa được tạo.</returns>
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

        /// <summary>
        /// Cập nhật thông tin một Todo.
        /// </summary>
        /// <param name="model">Thông tin Todo cần cập nhật.</param>
        /// <returns>Todo sau khi cập nhật.</returns>
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

        /// <summary>
        /// Xóa một Todo theo ID.
        /// </summary>
        /// <param name="id">ID của Todo cần xóa.</param>
        /// <returns>Không có nội dung trả về.</returns>
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
