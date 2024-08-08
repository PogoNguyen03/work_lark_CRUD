namespace web_CRUD.Pages.Model
{
    public class Record
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class RecordResponse
    {
        public List<Record> Data { get; set; }
    }

}
