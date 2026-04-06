using RestfulDemo.Services;

namespace RestfulDemo.Models
{
    public class TodoModel
    {
        public string Id { get; set; }
        public string Task {  get; set; }
        public string Content {  get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CompleteTime { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public void Completed(DateTime completed)
        {
            this.CompleteTime = completed;
            this.IsCompleted = true;
        }

        public void Save()
        {
            if (String.IsNullOrEmpty(this.Id))
            {
                // TODO: Create new
                TodoService service = new TodoService();
                service.create(this);
            } 
            else
            {
                // TODO: Update
                TodoService service = new TodoService();
                service.update(this);
            }
            
        }
    }
}
