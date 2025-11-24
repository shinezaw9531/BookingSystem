namespace Domain
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }

    public class NotFoundException : DomainException
    {
        public NotFoundException(string name, object key)
            : base($"Entity \"{name}\" ({key}) was not found.") { }
    }

    public class BusinessRuleException : DomainException
    {
        public BusinessRuleException(string message)
            : base(message) { }
    }
}
