namespace CsvConverter
{
    public class WorkItem
    {
        public string InputFile { get; set; } = string.Empty;
        public string OuputFile { get; set; } = string.Empty;
        public WorkItemStatus Status { get; set; } = WorkItemStatus.Todo;
    }
}
