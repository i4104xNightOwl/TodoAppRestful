using Microsoft.Data.Sqlite;
using RestfulDemo.Database;
using RestfulDemo.Models;
using System.Data;

namespace RestfulDemo.Services
{
    public class TodoService
    {
        private readonly SqliteConnection _sql;

        public TodoService()
        {
            _sql = SQLiteManager.getConnection();

            if (_sql.State != ConnectionState.Open)
                _sql.Open();

            CreateTableIfNotExists();
        }

        private void CreateTableIfNotExists()
        {
            using var cmd = _sql.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Todo (
                    Id TEXT PRIMARY KEY,
                    Task TEXT NOT NULL,
                    Content TEXT,
                    Status INTEGER,
                    EstimateTime INTEGER,
                    IsCompleted INTEGER,
                    CompleteTime TEXT,
                    Created TEXT,
                    Updated TEXT
                );";
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Lấy danh sách tất cả Todo
        /// </summary>
        /// <returns>List các <see cref="TodoModel"/></returns>
        public TodoModel[] Get()
        {
            var list = new List<TodoModel>();

            using var cmd = _sql.CreateCommand();
            cmd.CommandText = "SELECT * FROM Todo";

            using var rdr = cmd.ExecuteReader();
            while (rdr.Read()) list.Add(MapTodo(rdr));

            return list.ToArray();
        }

        /// <summary>
        /// Lấy một Todo theo Id.
        /// </summary>
        /// <param name="id">Id của Todo cần lấy.</param>
        /// <returns>Instance của <see cref="TodoModel"/></returns>
        public TodoModel? GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;

            using var cmd = _sql.CreateCommand();
            cmd.CommandText = "SELECT * FROM Todo WHERE Id = $id LIMIT 1";
            cmd.Parameters.AddWithValue("$id", id);

            using var rdr = cmd.ExecuteReader();
            return rdr.Read() ? MapTodo(rdr) : null;
        }

        /// <summary>
        /// Tạo một Todo mới
        /// </summary>
        /// <param name="model">Dữ liệu Todo cần tạo.</param>
        /// <returns>Todo đã được tạo, hoặc <c>null</c></returns>
        public TodoModel? Create(TodoModel model)
        {
            if (model == null) return null;

            model.Id ??= Guid.NewGuid().ToString();
            var now = DateTime.UtcNow;
            model.Created = now;
            model.Updated = now;

            using var cmd = _sql.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Todo
                (Id, Task, Content, Status, EstimateTime, IsCompleted, CompleteTime, Created, Updated)
                VALUES
                ($id, $task, $content, $status, $estimate, $isCompleted, $completeTime, $created, $updated)";

            SetTodoParameters(cmd, model);
            cmd.ExecuteNonQuery();

            return model;
        }

        /// <summary>
        /// Cập nhật một Todo hiện có theo Id.
        /// Nếu bản ghi không tồn tại, phương thức sẽ chuyển sang tạo mới
        /// </summary>
        /// <param name="model">Todo chứa dữ liệu cần cập nhật (phải có Id).</param>
        /// <returns>Todo đã được cập nhật hoặc tạo mới, hoặc <c>null</c> nếu input không hợp lệ.</returns>
        public TodoModel? Update(TodoModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Id))
                return null;

            model.Updated = DateTime.UtcNow;

            using var cmd = _sql.CreateCommand();
            cmd.CommandText = @"
                UPDATE Todo
                SET Task = $task,
                    Content = $content,
                    Status = $status,
                    EstimateTime = $estimate,
                    IsCompleted = $isCompleted,
                    CompleteTime = $completeTime,
                    Updated = $updated
                WHERE Id = $id";

            SetTodoParameters(cmd, model);

            var rows = cmd.ExecuteNonQuery();
            return rows == 0 ? Create(model) : model;
        }

        /// <summary>
        /// Xóa một Todo theo Id.
        /// </summary>
        /// <param name="id">Id của Todo cần xóa.</param>
        public void Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return;

            using var cmd = _sql.CreateCommand();
            cmd.CommandText = "DELETE FROM Todo WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }


        /// <summary>
        /// Chuyển dữ liệu từ <see cref="SqliteDataReader"/> thành <see cref="TodoModel"/>.
        /// </summary>
        /// <param name="rdr">SqliteDataReader đang trỏ tới một hàng dữ liệu hợp lệ.</param>
        /// <returns>Đối tượng <see cref="TodoModel"/> tương ứng.</returns>
        private TodoModel MapTodo(SqliteDataReader rdr)
        {
            return new TodoModel
            {
                Id = rdr["Id"]?.ToString(),
                Task = rdr["Task"]?.ToString() ?? string.Empty,
                Content = rdr["Content"]?.ToString() ?? string.Empty,
                Status = rdr["Status"] == DBNull.Value ? null : Convert.ToSByte(rdr["Status"]),
                EstimateTime = rdr["EstimateTime"] == DBNull.Value ? null : Convert.ToSByte(rdr["EstimateTime"]),
                IsCompleted = rdr["IsCompleted"] != DBNull.Value && Convert.ToInt32(rdr["IsCompleted"]) == 1,
                CompleteTime = (DateTime)ParseDateTime(rdr["CompleteTime"]),
                Created = (DateTime)ParseDateTime(rdr["Created"]),
                Updated = (DateTime)ParseDateTime(rdr["Updated"])
            };
        }

        /// <summary>
        /// Thêm các tham số vào <see cref="SqliteCommand"/> dựa trên giá trị trong <see cref="TodoModel"/>.
        /// Sử dụng tham số hóa để tránh SQL injection và chuẩn hóa dữ liệu trước khi thực thi câu lệnh.
        /// </summary>
        /// <param name="cmd">Đối tượng <see cref="SqliteCommand"/> sẽ nhận các tham số.</param>
        /// <param name="model">Đối tượng <see cref="TodoModel"/> chứa dữ liệu cần lưu.</param>
        private void SetTodoParameters(SqliteCommand cmd, TodoModel model)
        {
            cmd.Parameters.AddWithValue("$id", model.Id);
            cmd.Parameters.AddWithValue("$task", model.Task ?? string.Empty);
            cmd.Parameters.AddWithValue("$content", model.Content ?? string.Empty);
            cmd.Parameters.AddWithValue("$status", model.Status ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$estimate", model.EstimateTime ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$isCompleted", model.IsCompleted ? 1 : 0);
            cmd.Parameters.AddWithValue("$completeTime", ToDbDateTime(model.CompleteTime));
            cmd.Parameters.AddWithValue("$created", ToDbDateTime(model.Created));
            cmd.Parameters.AddWithValue("$updated", ToDbDateTime(model.Updated));
        }

        /// <summary>
        /// Chuyển đổi một giá trị <see cref="DateTime?"/> sang giá trị phù hợp để lưu vào SQLite.
        /// Nếu có giá trị, trả về chuỗi ngày giờ theo định dạng ISO 8601; nếu không, trả về <see cref="DBNull.Value"/>.
        /// </summary>
        /// <param name="dateTime">Giá trị ngày giờ có thể là <c>null</c>.</param>
        /// <returns>Chuỗi ISO 8601 hoặc <see cref="DBNull.Value"/> nếu không có giá trị.</returns>
        private object ToDbDateTime(DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.ToString("o") : DBNull.Value;
        }

        /// <summary>
        /// Phân tích giá trị lấy từ SQL và chuyển về <see cref="DateTime?"/>.
        /// Trả về <c>null</c> nếu giá trị là <see cref="DBNull.Value"/> hoặc không thể phân tích thành DateTime.
        /// </summary>
        /// <param name="value">Giá trị thu được từ database.</param>
        /// <returns><see cref="DateTime?"/> nếu phân tích thành công, ngược lại <c>null</c>.</returns>
        private DateTime? ParseDateTime(object value)
        {
            if (value == DBNull.Value || string.IsNullOrWhiteSpace(value?.ToString()))
                return null;

            return DateTime.TryParse(value.ToString(), out var dt) ? dt : null;
        }
    }
}