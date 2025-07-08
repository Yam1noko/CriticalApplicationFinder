namespace backend.DataTransferObject
{
    public class RuleDto
    {
        public int Id { get; set; } 
        public string Name { get; set; } = string.Empty; 
        public bool UseAnd { get; set; }    
        public bool IsActive { get; set; }  

        public List<string> RuleFullNames { get; set; } = new();     
        public List<string> RuleSubstrings { get; set; } = new();    
    }
}
