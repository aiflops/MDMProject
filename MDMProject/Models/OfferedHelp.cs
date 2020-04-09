namespace MDMProject.Models
{
    public class OfferedHelp
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string UserId { get; set; }

        public virtual User User { get; set; }

        public int? HelpTypeId { get; set; }

        public virtual HelpType HelpType { get; set; }
    }
}