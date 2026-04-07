using RestfulDemo.Services;

namespace RestfulDemo.Models
{
    public enum TodoEnums
    {
        DRAFT = 0,
        WAITING = 1,
        ONGOING = 2,
        DONE = 3,
    }

    public class TodoModel
    {
        public string? Id { get; set; }
        public string Task {  get; set; }
        public string Content {  get; set; }
        public sbyte? Status { get; set; }
        public sbyte? EstimateTime { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CompleteTime { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }


        /// <summary>
        /// Thay đổi trạng thái của Todo 
        /// </summary>
        /// <param name="newStatus">Trạng thái Todo</param>
        public void SwitchStatus(TodoEnums newStatus)
        {
            this.Status = (sbyte)newStatus;
            this.Save();
        }

        /// <summary>
        /// Đánh dấu là đã hoàn thành và thiết lập thời điểm hoàn thành.
        /// </summary>
        /// <param name="completed">Thời điểm hoàn thành.</param>
        public void Completed(DateTime completed)
        {
            this.CompleteTime = completed;
            this.IsCompleted = true;
            this.Status = (sbyte)TodoEnums.DONE;
            this.Save();
        }

        /// <summary>
        /// Lưu mô hình hiện tại sử dụng <see cref="RestfulDemo.Services.TodoService"/>.
        /// </summary>
        /// <remarks>
        /// Nếu <see cref="Id"/> null hoặc rỗng thì sẽ tạo mới, ngược lại sẽ cập nhật.
        /// </remarks>
        public void Save()
        {
            if (String.IsNullOrEmpty(this.Id))
            {
                // Create new
                TodoService service = new TodoService();
                service.Create(this);
            }
            else
            {
                // Update
                TodoService service = new TodoService();
                service.Update(this);
            }
        }
    }
}
